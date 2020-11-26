using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_07Rule : AbstractRule, IRule<ILearner>
    {
        private const int MaxNumberOfHours = 10;
        private const int MaxRuleAge = 25;
        private readonly int applicableLLDDCode = LLDDHealthProblemConstants.LearningDifficulty;

        private readonly int[] _applicableFundModels = { FundModels.NotFundedByESFA, FundModels.CommunityLearning };

        private readonly IDerivedData_06Rule _derivedData06;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LLDDHealthProb_07Rule(
            IDerivedData_06Rule derivedData06,
            IDateTimeQueryService dateTimeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LLDDHealthProb_07)
        {
            _derivedData06 = derivedData06;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner learner)
        {
            var learningDeliveries = learner?.LearningDeliveries;

            if (learningDeliveries == null)
            {
                return;
            }

            if ((learner.PlanLearnHoursNullable ?? -1) <= MaxNumberOfHours ||
                learner.LLDDHealthProb != applicableLLDDCode ||
                (learner.LLDDAndHealthProblems?.Any() ?? false))
            {
                return;
            }

            if (learningDeliveries.Any(ld => !_applicableFundModels.Contains(ld.FundModel)))
            {
                return;
            }

            if (ExceptionApplies(learner.DateOfBirthNullable, learningDeliveries))
            {
                return;
            }

            if (!learner.LearningDeliveries.Any(ld =>
                    ld.FundModel == FundModels.NotFundedByESFA &&
                    (ld.LearningDeliveryFAMs
                        ?.Any(ldf => !ldf.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.SOF) ||
                                    !ldf.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMCodeConstants.SOF_LA)) ?? false)))
            {
                RaiseValidationMessage(learner);
            }
        }

        private bool ExceptionApplies(DateTime? dateOfBirth, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            var startDate = _derivedData06.Derive(learningDeliveries);

            return _dateTimeQueryService.YearsBetween(dateOfBirth ?? DateTime.MaxValue, startDate) >= MaxRuleAge;
        }

        private void RaiseValidationMessage(ILearner learner)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LLDDHealthProb, learner.LLDDHealthProb)
            };

            HandleValidationError(learner.LearnRefNumber, null, parameters);
        }
    }
}