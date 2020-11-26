using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EPAOrgID
{
    public class EPAOrgID_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;

        public EPAOrgID_03Rule(
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EPAOrgID_03)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.EPAOrgID,
                    learningDelivery.AppFinRecords))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.EPAOrgID));
                }
            }
        }

        public bool ConditionMet(string epaOrgID, IEnumerable<IAppFinRecord> appFinRecords)
        {
            var appFinCodes = new[] { 2, 4 };

            return !string.IsNullOrWhiteSpace(epaOrgID)
                && !_learningDeliveryAppFinRecordQueryService
                       .HasAnyLearningDeliveryAFinCodesForType(appFinRecords, ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, appFinCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string epaOrgID)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EPAOrgID, epaOrgID)
            };
        }
    }
}
