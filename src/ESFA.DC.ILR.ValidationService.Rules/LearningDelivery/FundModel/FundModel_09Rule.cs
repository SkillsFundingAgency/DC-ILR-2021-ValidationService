using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel
{
    public class FundModel_09Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;

        private readonly DateTime _learnStartDate = new DateTime(2017, 5, 1);

        public FundModel_09Rule(IDerivedData_07Rule dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FundModel_09)
        {
            _dd07 = dd07;
        }

        public FundModel_09Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.AimType,
                    learningDelivery.FundModel,
                    learningDelivery.LearnStartDate,
                    learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int aimType, int fundModel, DateTime learnStartDate, int? progType)
        {
            return AimTypeConditionMet(aimType)
                   && FundModelConditionMet(fundModel)
                   && LearnStartDateConditionMet(learnStartDate)
                   && ProgTypeConditionMet(progType)
                   && ApprenticeshipConditionMet(fundModel, progType);
        }

        public virtual bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return fundModel != FundModels.OtherAdult;
        }

        public virtual bool ProgTypeConditionMet(int? progType)
        {
            return progType == 25;
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate < _learnStartDate;
        }

        public virtual bool ApprenticeshipConditionMet(int fundModel, int? progType)
        {
            return !(fundModel == FundModels.NotFundedByESFA && _dd07.IsApprenticeship(progType));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
            };
        }
    }
}
