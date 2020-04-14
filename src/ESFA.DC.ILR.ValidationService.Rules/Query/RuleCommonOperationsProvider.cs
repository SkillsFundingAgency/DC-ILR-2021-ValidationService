using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class RuleCommonOperationsProvider :
        IProvideRuleCommonOperations
    {
        private readonly IDerivedData_07Rule _derivedData07;

        public RuleCommonOperationsProvider(
            IDerivedData_07Rule derivedData07)
        {
            It.IsNull(derivedData07)
                .AsGuard<ArgumentNullException>(nameof(derivedData07));

            _derivedData07 = derivedData07;
        }

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.NullSafeAny(matchCondition);

        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.Restart);

        public bool IsRestart(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsRestart);

        public bool IsAdvancedLearnerLoan(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.AdvancedLearnerLoan);

        public bool IsAdvancedLearnerLoan(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsAdvancedLearnerLoan);

        public bool IsLoansBursary(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding);

        public bool IsLoansBursary(ILearningDelivery thisDelivery) =>
            CheckDeliveryFAMs(thisDelivery, IsLoansBursary);

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.OLASSOffendersInCustody);

        public bool IsLearnerInCustody(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsLearnerInCustody);

        public bool IsSteelWorkerRedundancyTraining(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.SteelIndustriesRedundancyTraining);

        public bool IsSteelWorkerRedundancyTraining(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsSteelWorkerRedundancyTraining);

        public bool IsReleasedOnTemporaryLicence(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.ReleasedOnTemporaryLicence);

        public bool IsReleasedOnTemporaryLicence(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsReleasedOnTemporaryLicence);

        public bool InApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool InAProgramme(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ProgrammeAim);

        public bool IsComponentOfAProgram(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ComponentAimInAProgramme);

        public bool IsTraineeship(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship);

        public bool IsStandardApprenticeship(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.ApprenticeshipStandard);

        public bool HasQualifyingFunding(ILearningDelivery delivery, params int[] desiredFundings) =>
            It.IsInRange(delivery.FundModel, desiredFundings);

        public bool HasQualifyingStart(ILearningDelivery delivery, DateTime minStart, DateTime? maxStart = null) =>
            It.Has(delivery)
            && It.IsBetween(delivery.LearnStartDate, minStart, maxStart ?? DateTime.MaxValue);

        public bool HasQualifyingStart(ILearnerEmploymentStatus employment, DateTime minStart, DateTime? maxStart = null) =>
            It.Has(employment)
            && It.IsBetween(employment.DateEmpStatApp, minStart, maxStart ?? DateTime.MaxValue);

        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime? thisStartDate, IReadOnlyCollection<ILearnerEmploymentStatus> usingSources) =>
            usingSources
                .NullSafeWhere(x => x.DateEmpStatApp <= thisStartDate)
                .OrderByDescending(x => x.DateEmpStatApp)
                .FirstOrDefault();
    }
}
