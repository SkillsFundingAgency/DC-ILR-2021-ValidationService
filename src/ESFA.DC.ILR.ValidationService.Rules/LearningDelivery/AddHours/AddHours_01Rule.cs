using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours
{
    public class AddHours_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { FundModels.AdultSkills, FundModels.ApprenticeshipsFrom1May2017, FundModels.OtherAdult };
        private readonly DateTime _learnStartDate = new DateTime(2015, 8, 1);

        public AddHours_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AddHours_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.AddHoursNullable,
                    learningDelivery.LearnStartDate))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.FundModel, learningDelivery.AddHoursNullable));
                }
            }
        }

        public bool ConditionMet(int fundModel, int? addHours, DateTime learnStartDate)
        {
            return FundModelConditionMet(fundModel)
                   && AddHoursConditionMet(addHours)
                   && LearnStartDateConditionMet(learnStartDate);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool AddHoursConditionMet(int? addHours)
        {
            return addHours.HasValue;
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate < _learnStartDate;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int fundModel, int? addHoursNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.AddHours, addHoursNullable)
            };
        }
    }
}
