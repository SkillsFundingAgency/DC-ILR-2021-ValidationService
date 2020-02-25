using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_11Rule : AbstractRule, IRule<ILearner>
    {
        private ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;

        public AFinType_11Rule(
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinType_11)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.AppFinRecords,
                    learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(int fundModel, IEnumerable<IAppFinRecord> appFinRecords, int? progType)
        {
            return FundModelConditionMet(fundModel)
                && AFinCodeConditionMet(appFinRecords)
                && !Excluded(progType);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 36;
        }

        public bool AFinCodeConditionMet(IEnumerable<IAppFinRecord> appFinRecords)
        {
            var codes = new int[] { 2, 4 };

            return _learningDeliveryAppFinRecordQueryService.HasAnyLearningDeliveryAFinCodes(appFinRecords, codes);
        }

        public bool Excluded(int? progType)
        {
            return progType == 25;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
            };
        }
    }
}
