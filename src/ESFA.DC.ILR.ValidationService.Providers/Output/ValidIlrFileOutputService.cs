using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers.Output
{
    public class ValidIlrFileOutputService : IValidIlrFileOutputService
    {
        private readonly IFileService _fileService;
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly ILogger _logger;

        public ValidIlrFileOutputService(
            IFileService fileService, 
            IXmlSerializationService xmlSerializationService, 
            IJsonSerializationService jsonSerializationService,
            ILogger logger)
        {
            _fileService = fileService;
            _xmlSerializationService = xmlSerializationService;
            _jsonSerializationService = jsonSerializationService;
            _logger = logger;
        }

        public async Task ProcessAsync(IValidationContext validationContext, IMessage message, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Starting ILR Valid File Output Service");
            var validLearnerRefNumbers = await GetValidLearners(validationContext, cancellationToken);
            var validIlrFile = BuildValidMessage(message, validLearnerRefNumbers);

            _logger.LogInfo("Updating Context File Name");
            validationContext.Filename = BuildValidFileReference(validationContext.Filename);

            _logger.LogInfo("Persisting Valid ILR File");
            using (var fileStream = await _fileService.OpenWriteStreamAsync(validationContext.Filename, validationContext.Container, cancellationToken))
            {
                _xmlSerializationService.Serialize(validIlrFile, fileStream);
            }

            _logger.LogInfo("Finished ILR Valid File Output Service");
        }

        private async Task<IEnumerable<string>> GetValidLearners(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            IEnumerable<string> learners;

            using (var fileStream = await _fileService.OpenReadStreamAsync(validationContext.ValidLearnRefNumbersKey, validationContext.Container, cancellationToken))
            {
                learners = _jsonSerializationService.Deserialize<IEnumerable<string>>(fileStream);
            }

            return learners;
        }

        private Message BuildValidMessage(IMessage message, IEnumerable<string> validLearners)
        {
            var inputMessage = message as Message;

            inputMessage.Learner = inputMessage.Learner?.Where(l => validLearners.Contains(l.LearnRefNumber)).ToArray();
            inputMessage.LearnerDestinationandProgression = inputMessage.LearnerDestinationandProgression?.Where(l => validLearners.Contains(l.LearnRefNumber)).ToArray();

            return inputMessage;
        }

        private string BuildValidFileReference(string fileReference)
        {
            return fileReference.Replace("Tight", "Valid");
        }
    }
}
