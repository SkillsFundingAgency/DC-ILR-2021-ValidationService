using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome
{
    public class Outcome_04Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _common;

        public Outcome_04Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOps)
            : base(validationErrorHandler, RuleNameConstants.Outcome_04)
        {
            _common = commonOps;
        }

        public bool HasAchievementDate(ILearningDelivery delivery) =>
            It.Has(delivery.AchDateNullable);

        public bool HasQualifyingOutcome(ILearningDelivery delivery) =>
            It.IsInRange(delivery.OutcomeNullable, OutcomeConstants.Achieved);

        public bool IsExcluded(ILearningDelivery delivery) =>
            _common.IsStandardApprenticeship(delivery)
            && _common.HasQualifyingFunding(delivery, TypeOfFunding.ApprenticeshipsFrom1May2017);

        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsExcluded(delivery)
            && HasAchievementDate(delivery)
            && !HasQualifyingOutcome(delivery);

        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

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
