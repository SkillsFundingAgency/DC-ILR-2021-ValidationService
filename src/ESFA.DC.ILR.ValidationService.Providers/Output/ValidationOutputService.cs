using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.IO.Model.Validation;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.IO.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers.Output
{
    public class ValidationOutputService : IValidationOutputService
    {
        private const string Error = "E";
        private const string Warning = "W";
        private const string Fail = "F";

        private readonly IFileService _fileService;
        private readonly IJsonSerializationService _serializationService;
        private readonly IValidationErrorsDataService _validationErrorsDataService;
        private readonly ILogger _logger;

        public ValidationOutputService(
            IFileService fileService,
            IJsonSerializationService serializationService,
            IValidationErrorsDataService validationErrorsDataService,
            ILogger logger)
        {
            _fileService = fileService;
            _serializationService = serializationService;
            _validationErrorsDataService = validationErrorsDataService;
            _logger = logger;
        }

        public async Task ProcessAsync(IValidationContext validationContext, IMessage message, IEnumerable<IValidationError> validationErrors, CancellationToken cancellationToken)
        {
            var existingValidationErrors = await GetExistingValidationErrors(validationContext, cancellationToken);

            var outputValidationErrors = validationErrors
                .Select(ve => new ValidationError
                {
                    LearnerReferenceNumber = ve.LearnerReferenceNumber,
                    AimSequenceNumber = ve.AimSequenceNumber,
                    RuleName = ve.RuleName,
                    Severity = SeverityToString(ve.Severity),
                    ValidationErrorParameters = ve.ErrorMessageParameters?
                    .Select(emp => new ValidationErrorParameter
                    {
                        PropertyName = emp.PropertyName,
                        Value = emp.Value
                    }).ToList()
                }).ToList();

            outputValidationErrors.AddRange(existingValidationErrors);

            var invalidLearnerRefNumbers = BuildInvalidLearnRefNumbers(outputValidationErrors).ToList();
            var validLearnerRefNumbers = BuildValidLearnRefNumbers(message, invalidLearnerRefNumbers).ToList();
            _logger.LogDebug($"ValidationOutputService invalid:{invalidLearnerRefNumbers.Count} valid:{validLearnerRefNumbers.Count}");

            var validationErrorMessageLookups = outputValidationErrors
                .Select(ve => ve.RuleName)
                .Distinct()
                .Select(rn => new ValidationErrorMessageLookup
                {
                    RuleName = rn,
                    Message = _validationErrorsDataService.MessageforRuleName(rn)
                }).ToList();

            await SaveAsync(
                validationContext,
                validLearnerRefNumbers,
                invalidLearnerRefNumbers,
                outputValidationErrors,
                validationErrorMessageLookups,
                cancellationToken);
        }

        public IEnumerable<string> BuildInvalidLearnRefNumbers(IEnumerable<ValidationError> validationErrors)
        {
            return validationErrors
                .Where(x => !string.IsNullOrEmpty(x.LearnerReferenceNumber)
                    && x.Severity == Error)
                .Select(ve => ve.LearnerReferenceNumber)
                .Distinct();
        }

        public IEnumerable<string> BuildValidLearnRefNumbers(IMessage message, IEnumerable<string> invalidLearnRefNumbers)
        {
            var invalidLearnRefNumbersHashSet = invalidLearnRefNumbers != null ? new HashSet<string>(invalidLearnRefNumbers) : new HashSet<string>();

            return message?
                       .Learners?
                       .Select(l => l.LearnRefNumber)
                       .Where(lrn => !invalidLearnRefNumbersHashSet.Contains(lrn))
                       .Distinct()
                   ?? new List<string>();
        }

        public async Task SaveAsync(
            IValidationContext validationContext,
            IEnumerable<string> validLearnerRefNumbers,
            IEnumerable<string> invalidLearnerRefNumbers,
            IEnumerable<ValidationError> validationErrors,
            IEnumerable<ValidationErrorMessageLookup> validationErrorMessageLookups,
            CancellationToken cancellationToken)
        {
            validationContext.InvalidLearnRefNumbersCount = invalidLearnerRefNumbers.Count();
            validationContext.ValidLearnRefNumbersCount = validLearnerRefNumbers.Count();
            validationContext.ValidationTotalErrorCount = validationErrors.Count(x => x.Severity == Error);
            validationContext.ValidationTotalWarningCount = validationErrors.Count(x => x.Severity == Warning);

            await Task.WhenAll(
                OutputAsync(validationContext.ValidLearnRefNumbersKey, validationContext.Container, validLearnerRefNumbers, cancellationToken),
                OutputAsync(validationContext.InvalidLearnRefNumbersKey, validationContext.Container, invalidLearnerRefNumbers, cancellationToken),
                OutputAsync(validationContext.ValidationErrorsKey, validationContext.Container, validationErrors, cancellationToken),
                OutputAsync(validationContext.ValidationErrorMessageLookupKey, validationContext.Container, validationErrorMessageLookups, cancellationToken));
        }

        public string SeverityToString(Severity? severity)
        {
            switch (severity)
            {
                case Severity.Error:
                    return Error;
                case Severity.Warning:
                    return Warning;
                case Severity.Fail:
                    return Fail;
                case null:
                    return null;
                default:
                    return null;
            }
        }

        private async Task OutputAsync<T>(string key, string container, IEnumerable<T> output, CancellationToken cancellationToken)
        {
            using (var fileStream = await _fileService.OpenWriteStreamAsync(key, container, cancellationToken))
            {
                _serializationService.Serialize(output, fileStream);
            }
        }

        private async Task<IEnumerable<ValidationError>> GetExistingValidationErrors(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            IEnumerable<ValidationError> validationErrors = new List<ValidationError>();

            try
            {
                using (var stream = await _fileService.OpenReadStreamAsync(validationContext.ValidationErrorsKey, validationContext.Container, cancellationToken))
                {
                    stream.Position = 0;

                    validationErrors = _serializationService.Deserialize<List<ValidationError>>(stream);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed To get Existing Validation Errors, assume none available and carry on.", e);
            }

            return validationErrors;
        }
    }
}
