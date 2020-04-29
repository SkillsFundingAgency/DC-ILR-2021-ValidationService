using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpStat_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int>
        {
            TypeOfFunding.Age16To19ExcludingApprenticeships,
            TypeOfFunding.Other16To19
        };

        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;

        public EmpStat_07Rule(
            IValidationErrorHandler validationErrorHandler,
            IDateTimeQueryService dateTimeQueryService,
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_07)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
        }

        public static DateTime LastViableDate => new DateTime(2013, 07, 31);

        public static int PlannedTotalQualifyingHours => 540;

        public void Validate(ILearner thisLearner)
        {
            if (!HasQualifyingLearningHours(GetLearningHoursTotal(thisLearner)))
            {
                return;
            }

            var employments = thisLearner.LearnerEmploymentStatuses.ToReadOnlyCollection();

            thisLearner.LearningDeliveries
                .ForAny(
                    x => IsNotValid(x, GetEmploymentStatusOn(x.LearnStartDate, employments)),
                    x => RaiseValidationMessage(thisLearner, x));
        }

        public int GetUsableValue(int? candidate) =>
            candidate ?? 0;

        public int GetLearningHoursTotal(ILearner thisLearner) =>
            GetUsableValue(thisLearner.PlanLearnHoursNullable)
            + GetUsableValue(thisLearner.PlanEEPHoursNullable);

        public bool HasQualifyingLearningHours(int candidate) =>
            candidate < PlannedTotalQualifyingHours;

        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime thisDate, IReadOnlyCollection<ILearnerEmploymentStatus> fromEmployments) =>
            _learnerEmploymentStatusQueryService.LearnerEmploymentStatusForDate(fromEmployments, thisDate);

        public bool IsNotValid(ILearningDelivery thisDelivery, ILearnerEmploymentStatus thisEmployment) =>
            HasQualifyingFunding(thisDelivery)
            && HasQualifyingStart(thisDelivery)
            && !HasQualifyingEmployment(thisEmployment);

        public bool HasQualifyingFunding(ILearningDelivery thisDelivery) =>
            _fundModels.Contains(thisDelivery.FundModel);

        public bool HasQualifyingStart(ILearningDelivery thisDelivery) =>
            _dateTimeQueryService.IsDateBetween(thisDelivery.LearnStartDate, DateTime.MinValue, LastViableDate);

        public bool HasQualifyingEmployment(ILearnerEmploymentStatus thisEmployment) =>
            thisEmployment != null;

        public void RaiseValidationMessage(ILearner thisLearner, ILearningDelivery thisDelivery)
        {
            HandleValidationError(thisLearner.LearnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisLearner, thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearner thisLearner, ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, thisLearner.PlanLearnHoursNullable),
                BuildErrorMessageParameter(PropertyNameConstants.PlanEEPHours, thisLearner.PlanEEPHoursNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };
        }
    }
}
