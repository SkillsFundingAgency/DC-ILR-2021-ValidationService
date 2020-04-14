using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_01Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        public const string Name = RuleNameConstants.EmpStat_01;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly IDerivedData_07Rule _derivedData07;

        private readonly IAcademicYearDataService _yearData;

        public EmpStat_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_07Rule derivedData07,
            IAcademicYearDataService yearData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData07)
                .AsGuard<ArgumentNullException>(nameof(derivedData07));
            It.IsNull(yearData)
                .AsGuard<ArgumentNullException>(nameof(yearData));

            _messageHandler = validationErrorHandler;
            _derivedData07 = derivedData07;
            _yearData = yearData;
        }

        public string RuleName => Name;

        public DateTime FirstViableDate => new DateTime(2012, 08, 01);

        public DateTime LastViableDate => new DateTime(2014, 07, 31);

        public TimeSpan LastInviableAge => new TimeSpan(6939, 0, 0, 0);       

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.NullSafeAny(matchCondition);

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.OLASSOffendersInCustody);

        public bool IsLearnerInCustody(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsLearnerInCustody);

        public bool IsComunityLearningFund(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.LocalAuthorityCommunityLearningFunds);

        public bool IsComunityLearningFund(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsComunityLearningFund);

        public bool IsNotFundedByESFA(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.NotFundedByESFA);

        public bool IsNotQualifiedFunding(ILearningDelivery delivery) =>
            IsNotFundedByESFA(delivery) && IsComunityLearningFund(delivery);

        public bool IsApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool InTraining(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship);

        public bool IsExcluded(ILearningDelivery delivery) =>
            IsLearnerInCustody(delivery)
            || IsNotQualifiedFunding(delivery)
            || IsApprenticeship(delivery)
            || InTraining(delivery);

        public bool IsQualifyingFunding(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.AdultSkills, TypeOfFunding.OtherAdult, TypeOfFunding.NotFundedByESFA);

        public bool IsQualifyingAim(ILearningDelivery delivery) =>
            It.IsBetween(delivery.LearnStartDate, FirstViableDate, LastViableDate);

        public DateTime GetYearOfLearningCommencementDate(DateTime candidate) =>
            _yearData.GetAcademicYearOfLearningDate(candidate, AcademicYearDates.PreviousYearEnd);

        public bool IsQualifyingAge(ILearner learner, ILearningDelivery delivery) =>
            It.Has(learner.DateOfBirthNullable) && ((GetYearOfLearningCommencementDate(delivery.LearnStartDate) - learner.DateOfBirthNullable.Value) > LastInviableAge);

        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus eStatus, DateTime learningStartDate) =>
            eStatus.DateEmpStatApp <= learningStartDate;

        public bool HasQualifyingEmploymentStatus(ILearner learner, ILearningDelivery delivery) =>
            learner.LearnerEmploymentStatuses.NullSafeAny(x => HasQualifyingEmploymentStatus(x, delivery.LearnStartDate));

        public bool IsNotValid(ILearner learner, ILearningDelivery delivery) =>
            !IsExcluded(delivery)
                && IsQualifyingAge(learner, delivery)
                && IsQualifyingFunding(delivery)
                && IsQualifyingAim(delivery)
                && !HasQualifyingEmploymentStatus(learner, delivery);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => IsNotValid(objectToValidate, x))
                .ForEach(x => RaiseValidationMessage(objectToValidate, x));
        }

        public void RaiseValidationMessage(ILearner learner, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, "(missing)"),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable)
            };

            _messageHandler.Handle(RuleName, learner.LearnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
