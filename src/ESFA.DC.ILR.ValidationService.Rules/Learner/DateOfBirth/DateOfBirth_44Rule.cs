using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_44Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fcsDataService;
        private readonly IDerivedData_23Rule _derivedDataRule23;

        public DateOfBirth_44Rule(
            IFCSDataService fcsDataService,
            IDerivedData_23Rule derivedDataRule23,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_44)
        {
            _fcsDataService = fcsDataService;
            _derivedDataRule23 = derivedDataRule23;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
        public void Validate(ILearner learner)
        {
            if (learner?.DateOfBirthNullable == null || learner.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.FundModel != FundModels.EuropeanSocialFund)
                {
                    continue;
                }

                var contract = _fcsDataService.GetContractAllocationFor(learningDelivery.ConRefNumber);

                if (contract?.EsfEligibilityRule == null)
                {
                    continue;
                }

                int? age = _derivedDataRule23.GetLearnersAgeAtStartOfESFContract(learner, learningDelivery.ConRefNumber);
                if (age == null)
                {
                    continue;
                }

                if (age.Value < (contract.EsfEligibilityRule.MinAge ?? int.MinValue)
                    || age.Value > (contract.EsfEligibilityRule.MaxAge ?? int.MaxValue))
                {
                    RaiseValidationMessage(learner, learningDelivery);
                }
            }
        }

        private void RaiseValidationMessage(ILearner learner, ILearningDelivery learningDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable)
            };

            HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, parameters);
        }
    }
}
