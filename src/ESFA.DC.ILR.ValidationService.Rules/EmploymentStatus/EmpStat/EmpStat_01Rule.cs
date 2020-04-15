using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_01Rule : AbstractRule, IRule<ILearner>
    {
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;
        public HashSet<int> _fundModels = new HashSet<int>
        {
            TypeOfFunding.AdultSkills,
            TypeOfFunding.OtherAdult,
            TypeOfFunding.NotFundedByESFA
        };
       
        private readonly IDerivedData_07Rule _derivedData07;
        private readonly IAcademicYearDataService _yearData;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public EmpStat_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_07Rule derivedData07,
            IAcademicYearDataService yearData,
            IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_01)
        {
            _derivedData07 = derivedData07;
            _yearData = yearData;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public DateTime FirstViableDate => new DateTime(2012, 08, 01);

        public DateTime LastViableDate => new DateTime(2014, 07, 31);

        public TimeSpan LastInviableAge => new TimeSpan(6939, 0, 0, 0);       

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.NullSafeAny(matchCondition);

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.OLASSOffendersInCustody.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsLearnerInCustody(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsLearnerInCustody);

        public bool IsComunityLearningFund(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.LocalAuthorityCommunityLearningFunds.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsComunityLearningFund(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsComunityLearningFund);

        public bool IsNotFundedByESFA(ILearningDelivery delivery) =>
            delivery.FundModel == TypeOfFunding.NotFundedByESFA;

        public bool IsNotQualifiedFunding(ILearningDelivery delivery) =>
            IsNotFundedByESFA(delivery) && IsComunityLearningFund(delivery);

        public bool IsApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool InTraining(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == TypeOfLearningProgramme.Traineeship;

        public bool IsExcluded(ILearningDelivery delivery) =>
            IsLearnerInCustody(delivery)
            || IsNotQualifiedFunding(delivery)
            || IsApprenticeship(delivery)
            || InTraining(delivery);

        public bool IsQualifyingFunding(ILearningDelivery delivery) =>
            _fundModels.Contains(delivery.FundModel);

        public bool IsQualifyingAim(ILearningDelivery delivery) =>
            _dateTimeQueryService.IsDateBetween(delivery.LearnStartDate, FirstViableDate, LastViableDate);

        public DateTime GetYearOfLearningCommencementDate(DateTime candidate) =>
            _yearData.GetAcademicYearOfLearningDate(candidate, AcademicYearDates.PreviousYearEnd);

        public bool IsQualifyingAge(ILearner learner, ILearningDelivery delivery) =>
            learner.DateOfBirthNullable.HasValue && ((GetYearOfLearningCommencementDate(delivery.LearnStartDate) - learner.DateOfBirthNullable.Value) > LastInviableAge);

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

        public void RaiseValidationMessage(ILearner learner, ILearningDelivery thisDelivery) =>
          HandleValidationError(learner.LearnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(learner, thisDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearner learner, ILearningDelivery thisDelivery) => new[]
        {
            BuildErrorMessageParameter(MessagePropertyName, "(missing)"),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
            BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable)
        };
    }
}
