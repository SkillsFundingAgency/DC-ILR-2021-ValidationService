using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_35Rule : IDerivedData_35Rule
    {
        private readonly ILearningDeliveryFAMQueryService _learnDelFAMQueryService;

        private readonly List<string> _famCodesSOF = new List<string>()
        {
            LearningDeliveryFAMCodeConstants.SOF_Unassigned_110,
            LearningDeliveryFAMCodeConstants.SOF_Unassigned_111,
            LearningDeliveryFAMCodeConstants.SOF_Unassigned_112,
            LearningDeliveryFAMCodeConstants.SOF_Unassigned_113,
            LearningDeliveryFAMCodeConstants.SOF_Unassigned_114,
            LearningDeliveryFAMCodeConstants.SOF_Unassigned_115,
            LearningDeliveryFAMCodeConstants.SOF_Unassigned_116
        };

        public DerivedData_35Rule(ILearningDeliveryFAMQueryService learnDelFAMQueryService)
        {
            _learnDelFAMQueryService = learnDelFAMQueryService;
        }

        public bool IsCombinedAuthorities(ILearningDelivery learningDelivery)
        {
            return LearningDeliveryFAMConditionMet(learningDelivery.LearningDeliveryFAMs);
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learnDelFAMQueryService.
                            HasAnyLearningDeliveryFAMCodesForType(
                                                                  learningDeliveryFAMs, 
                                                                  LearningDeliveryFAMTypeConstants.SOF, 
                                                                  _famCodesSOF);
        }
    }
}
