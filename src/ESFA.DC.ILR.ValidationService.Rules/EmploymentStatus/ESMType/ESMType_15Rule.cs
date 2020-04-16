using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_15Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerEmploymentStatusMonitoringQueryService _learnerEmploymentStatusMonitoringQueryService;
        private readonly string[] _esmTypes = { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };
        private string[] _duplicateESMTypes;

    public ESMType_15Rule(
            ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ESMType_15)
        {
            _learnerEmploymentStatusMonitoringQueryService = learnerEmploymentStatusMonitoringQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearnerEmploymentStatuses != null)
            {
                foreach (var learnerEmploymentStatus in objectToValidate.LearnerEmploymentStatuses)
                {
                    if (ConditionMet(learnerEmploymentStatus.EmploymentStatusMonitorings))
                    {
                        var duplicateESMTypesForError = string.Join(", ", _duplicateESMTypes);

                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            errorMessageParameters: BuildErrorMessageParameters(duplicateESMTypesForError));
                    }
                }
            }
        }

        public bool ConditionMet(IEnumerable<IEmploymentStatusMonitoring> employmentStatusMonitorings)
        {
            if (_learnerEmploymentStatusMonitoringQueryService.HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(employmentStatusMonitorings, _esmTypes))
            {
                _duplicateESMTypes =
                    _learnerEmploymentStatusMonitoringQueryService.GetDuplicatedEmploymentStatusMonitoringTypesForTypes(
                        employmentStatusMonitorings, _esmTypes).ToArray();

                return true;
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string duplicateESMTypes)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ESMType, duplicateESMTypes),
            };
        }
    }
}
