using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R100Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<string> _assessmentPriceFinancialRecordKeys =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ApprenticeshipFinancialRecord.ResidualAssessmentPrice,
                ApprenticeshipFinancialRecord.TotalAssessmentPrice,
            };

        public R100Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R100)
        {
        }

        public R100Rule()
            : base(null, RuleNameConstants.R100)
        {
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries.Where(ConditionMet))
            {
                HandleValidationError(
                    learner.LearnRefNumber,
                    learningDelivery.AimSeqNumber);
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return !IsNonFundedApprenticeshipStandard(learningDelivery)
                   && IsCompletedApprenticeshipStandardAim(learningDelivery)
                   && !HasAssessmentPrice(learningDelivery);
        }

        public virtual bool IsNonFundedApprenticeshipStandard(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == FundModels.NotFundedByESFA
                   && learningDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard;
        }

        public virtual bool IsCompletedApprenticeshipStandardAim(ILearningDelivery learningDelivery)
        {
            return learningDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
                   && learningDelivery.AimType == TypeOfAim.ProgrammeAim
                   && learningDelivery.CompStatus == CompletionState.HasCompleted;
        }

        public bool IsAssessmentPrice(IAppFinRecord appFinRecord)
        {
            var compoundAppFinRecordKey = appFinRecord.AFinType + appFinRecord.AFinCode;

            return _assessmentPriceFinancialRecordKeys.Contains(compoundAppFinRecordKey);
        }

        public virtual bool HasAssessmentPrice(ILearningDelivery learningDelivery)
        {
            return learningDelivery
                       .AppFinRecords?
                       .Any(IsAssessmentPrice)
                ?? false;
        }
    }
}
