using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_35Rule : IDerivedData_35Rule
    {
        private readonly ILearningDeliveryFAMQueryService _learnDelFAMQueryService;

        private readonly List<string> _famCodesSOF = new List<string>()
        {
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_GMCA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_LCRCA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_WMCA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_WECA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_TVCA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_CPCA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_London,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_NTCA
        };

        public DerivedData_35Rule(ILearningDeliveryFAMQueryService learnDelFAMQueryService)
        {
            _learnDelFAMQueryService = learnDelFAMQueryService;
        }

        public bool IsCombinedAuthorities(ILearningDelivery learningDelivery)
        {
            return LearningDeliveryFAMConditionMet(learningDelivery?.LearningDeliveryFAMs);
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return learningDeliveryFAMs != null &&
                   _learnDelFAMQueryService.
                       HasAnyLearningDeliveryFAMCodesForType(
                           learningDeliveryFAMs,
                           LearningDeliveryFAMTypeConstants.SOF,
                           _famCodesSOF);
        }
    }
}
