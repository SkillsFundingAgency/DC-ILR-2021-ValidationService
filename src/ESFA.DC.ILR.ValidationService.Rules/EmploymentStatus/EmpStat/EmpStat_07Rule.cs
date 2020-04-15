using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpStat_07Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _check;

        public EmpStat_07Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_07)
        {
            
                
            
                

            _check = commonOperations;
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
            _check.GetEmploymentStatusOn(thisDate, fromEmployments);

        public bool IsNotValid(ILearningDelivery thisDelivery, ILearnerEmploymentStatus thisEmployment) =>
            HasQualifyingFunding(thisDelivery)
            && HasQualifyingStart(thisDelivery)
            && !HasQualifyingEmployment(thisEmployment);

        public bool HasQualifyingFunding(ILearningDelivery thisDelivery) =>
            _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.Other16To19);

        public bool HasQualifyingStart(ILearningDelivery thisDelivery) =>
            _check.HasQualifyingStart(thisDelivery, DateTime.MinValue, LastViableDate);

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
