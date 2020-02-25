using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearningDeliveryWorkPlacementQueryService : ILearningDeliveryWorkPlacementQueryService
    {
        private readonly List<int> _validWorkPlaceModes = new List<int>
        {
            WorkPlaceModeConstants.InternalWorkPlacement,
            WorkPlaceModeConstants.ExternalWorkPlacement
        };

        public bool HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(IEnumerable<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements, DateTime? learnActEndDate)
        {
            return learningDeliveryWorkPlacements != null
                   && learnActEndDate != null
                   && learningDeliveryWorkPlacements.Any(ldwp => ldwp.WorkPlaceEndDateNullable > learnActEndDate);
        }

        public bool HasAnyEmpIdNullAndStartDateNotNull(IEnumerable<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements)
        {
            return learningDeliveryWorkPlacements != null
                   && learningDeliveryWorkPlacements
                       .Any(ldwp => ldwp.WorkPlaceEmpIdNullable == null && ldwp.WorkPlaceStartDate != null);
        }

        public bool IsValidWorkPlaceMode(int workPlaceMode)
        {
            return _validWorkPlaceModes.Contains(workPlaceMode);
        }
    }
}
