namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.ValidationService.Data.Extensions;
    using ESFA.DC.ILR.ValidationService.Interface;
    using ESFA.DC.ILR.ValidationService.Rules.Abstract;
    using ESFA.DC.ILR.ValidationService.Rules.Constants;
    using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

    public class LearnDelFAMType_40Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;

        public LearnDelFAMType_40Rule(
            IDerivedData_07Rule dd07,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_40)
        {
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                foreach (var learnDelFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (ConditionMet(learningDelivery.FundModel, learningDelivery.ProgTypeNullable, learnDelFam.LearnDelFAMType))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learnDelFam.LearnDelFAMType));
                    }
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, string learnDelFamType)
        {
            return FAMTypeConditionMet(learnDelFamType)
                   && FundModelConditionMet(fundModel)
                   && DD07ConditionMet(progType);
        }

        public bool FAMTypeConditionMet(string learnDelFamType)
        {
            return learnDelFamType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ADL);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModels.NotFundedByESFA;
        }

        public bool DD07ConditionMet(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learnDelFAMType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType)
            };
        }
    }
}
