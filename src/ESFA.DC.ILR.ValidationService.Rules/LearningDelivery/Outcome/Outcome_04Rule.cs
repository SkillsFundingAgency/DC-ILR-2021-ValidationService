using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome
{
    public class Outcome_04Rule : AbstractRule, IRule<ILearner>
    {
        public Outcome_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Outcome_04)
        {
        }

        public bool HasAchievementDate(ILearningDelivery delivery) =>
            delivery.AchDateNullable.HasValue;

        public bool HasQualifyingOutcome(ILearningDelivery delivery) =>
            delivery.OutcomeNullable == OutcomeConstants.Achieved;

        public bool IsExcluded(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
            && delivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;

        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsExcluded(delivery)
            && HasAchievementDate(delivery)
            && !HasQualifyingOutcome(delivery);

        public void Validate(ILearner theLearner)
        {
            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery delivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, delivery.AchDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, delivery.OutcomeNullable)
            };
        }
    }
}
