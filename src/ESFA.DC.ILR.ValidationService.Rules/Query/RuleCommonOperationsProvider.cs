using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class RuleCommonOperationsProvider :
        IProvideRuleCommonOperations
    {
        private readonly IDerivedData_07Rule _derivedData07;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public RuleCommonOperationsProvider(
            IDerivedData_07Rule derivedData07,
            IDateTimeQueryService dateTimeQueryService)
        {
            _derivedData07 = derivedData07;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.NullSafeAny(matchCondition);

        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            monitor.LearnDelFAMType == Monitoring.Delivery.Types.Restart;

        public bool IsRestart(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsRestart);

        public bool IsAdvancedLearnerLoan(ILearningDeliveryFAM monitor) =>
            monitor.LearnDelFAMType.CaseInsensitiveEquals(Monitoring.Delivery.Types.AdvancedLearnerLoan);

        public bool IsAdvancedLearnerLoan(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsAdvancedLearnerLoan);

        public bool IsLoansBursary(ILearningDeliveryFAM monitor) =>
            monitor.LearnDelFAMType.CaseInsensitiveEquals(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding);

        public bool IsLoansBursary(ILearningDelivery thisDelivery) =>
            CheckDeliveryFAMs(thisDelivery, IsLoansBursary);

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
           Monitoring.Delivery.OLASSOffendersInCustody.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsLearnerInCustody(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsLearnerInCustody);

        public bool IsSteelWorkerRedundancyTraining(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.SteelIndustriesRedundancyTraining.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsSteelWorkerRedundancyTraining(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsSteelWorkerRedundancyTraining);

        public bool IsReleasedOnTemporaryLicence(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.ReleasedOnTemporaryLicence.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsReleasedOnTemporaryLicence(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsReleasedOnTemporaryLicence);

        public bool InApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool InAProgramme(ILearningDelivery delivery) =>
            delivery.AimType == TypeOfAim.ProgrammeAim;

        public bool IsComponentOfAProgram(ILearningDelivery delivery) =>
            delivery.AimType == TypeOfAim.ComponentAimInAProgramme;

        public bool IsTraineeship(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == TypeOfLearningProgramme.Traineeship;

        public bool IsStandardApprenticeship(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard;

        public bool HasQualifyingFunding(ILearningDelivery delivery, params int[] desiredFundings) =>
           desiredFundings.Contains(delivery.FundModel);

        public bool HasQualifyingStart(ILearningDelivery delivery, DateTime minStart, DateTime? maxStart = null) =>
            delivery != null
            && _dateTimeQueryService.IsDateBetween(delivery.LearnStartDate, minStart, maxStart ?? DateTime.MaxValue);

        public bool HasQualifyingStart(ILearnerEmploymentStatus employment, DateTime minStart, DateTime? maxStart = null) =>
            employment != null
            && _dateTimeQueryService.IsDateBetween(employment.DateEmpStatApp, minStart, maxStart ?? DateTime.MaxValue);

        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime? thisStartDate, IReadOnlyCollection<ILearnerEmploymentStatus> usingSources) =>
            usingSources
                .NullSafeWhere(x => x.DateEmpStatApp <= thisStartDate)
                .OrderByDescending(x => x.DateEmpStatApp)
                .FirstOrDefault();
    }
}
