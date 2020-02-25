using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R110Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;

        public R110Rule(ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R110)
        {
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries.Where(ld => ConditionMet(ld, learner.LearnerEmploymentStatuses)))
            {
                HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber);
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            if (IsApprenticeshipProgramme(learningDelivery) && It.Has(learningDelivery.LearningDeliveryFAMs))
            {
                return GetLearningDeliveryFAMsWhereApprenticeshipProgrammeFundedThroughContract(learningDelivery.LearningDeliveryFAMs)
                    .Any(fam => LearnerNotEmployedOnDate(learnerEmploymentStatuses, fam.LearnDelFAMDateFromNullable));
            }

            return false;
        }

        public bool IsApprenticeshipProgramme(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;
        }

        public IEnumerable<ILearningDeliveryFAM> GetLearningDeliveryFAMsWhereApprenticeshipProgrammeFundedThroughContract(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return learningDeliveryFAMs
                .Where(fam =>
                    fam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ACT)
                    && fam.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMCodeConstants.ACT_ContractEmployer));
        }

        public bool LearnerNotEmployedOnDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime? learningDeliveryFamDateFrom)
        {
            return learningDeliveryFamDateFrom.HasValue
                && _learnerEmploymentStatusQueryService.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learningDeliveryFamDateFrom.Value)?.EmpStat != TypeOfEmploymentStatus.InPaidEmployment;
        }
    }
}
