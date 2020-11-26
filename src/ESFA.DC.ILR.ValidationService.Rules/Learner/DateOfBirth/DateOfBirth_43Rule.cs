using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_43Rule : AbstractRule, IRule<ILearner>
    {
        private const int MinAge = 15;

        private readonly IDerivedData_23Rule _derivedData23;

        public DateOfBirth_43Rule(
            IDerivedData_23Rule derivedData23,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_43)
        {
            _derivedData23 = derivedData23;
        }

        public void Validate(ILearner learner)
        {
            var learningDelivery =
                learner?.LearningDeliveries?.FirstOrDefault(ld => ld.FundModel == FundModels.EuropeanSocialFund);

            if (learningDelivery == null)
            {
                return;
            }

            var age = _derivedData23.GetLearnersAgeAtStartOfESFContract(learner, learningDelivery.ConRefNumber);
            if (age != null && age < MinAge)
            {
                RaiseValidationMessage(learner, learningDelivery);
            }
        }

        private void RaiseValidationMessage(ILearner learner, ILearningDelivery learningDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.EuropeanSocialFund),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable)
            };

            HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, parameters);
        }
    }
}