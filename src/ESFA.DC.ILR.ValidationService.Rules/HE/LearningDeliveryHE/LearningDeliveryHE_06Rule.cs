using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.LearningDeliveryHE
{
    public class LearningDeliveryHE_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string[] _notionalNVQLevels =
        {
                LARSConstants.NotionalNVQLevelV2Strings.Level4,
                LARSConstants.NotionalNVQLevelV2Strings.Level5,
                LARSConstants.NotionalNVQLevelV2Strings.Level6,
                LARSConstants.NotionalNVQLevelV2Strings.Level7,
                LARSConstants.NotionalNVQLevelV2Strings.Level8,
                LARSConstants.NotionalNVQLevelV2Strings.HigherLevel
        };

        private readonly ILARSDataService _lARSDataService;

        public LearningDeliveryHE_06Rule(IValidationErrorHandler validationErrorHandler, ILARSDataService lARSDataService)
            : base(validationErrorHandler, RuleNameConstants.LearningDeliveryHE_06)
        {
            _lARSDataService = lARSDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery)
                    && !LARSNotionalNVQLevelV2Exclusion(learningDelivery.LearnAimRef))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return FundModelConditionMet(learningDelivery.FundModel) &&
                LearningDeliveryHEConditionMet(learningDelivery.LearningDeliveryHEEntity);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 10 || fundModel == 70;
        }

       public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHEEntity) => learningDeliveryHEEntity != null;

        public bool LARSNotionalNVQLevelV2Exclusion(string learnAimRef) => _lARSDataService.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
