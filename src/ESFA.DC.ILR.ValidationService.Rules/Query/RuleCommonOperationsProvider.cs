using System;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class RuleCommonOperationsProvider : IProvideRuleCommonOperations
    {
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public RuleCommonOperationsProvider(IDateTimeQueryService dateTimeQueryService)
        {
            _dateTimeQueryService = dateTimeQueryService;
        }

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.NullSafeAny(matchCondition);

        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            monitor.LearnDelFAMType == Monitoring.Delivery.Types.Restart;

        public bool IsRestart(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsRestart);

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

        public bool HasQualifyingFunding(ILearningDelivery delivery, params int[] desiredFundings) =>
           desiredFundings.Contains(delivery.FundModel);

        public bool HasQualifyingStart(ILearningDelivery delivery, DateTime minStart, DateTime? maxStart = null) =>
            delivery != null
            && _dateTimeQueryService.IsDateBetween(delivery.LearnStartDate, minStart, maxStart ?? DateTime.MaxValue);
    }
}
