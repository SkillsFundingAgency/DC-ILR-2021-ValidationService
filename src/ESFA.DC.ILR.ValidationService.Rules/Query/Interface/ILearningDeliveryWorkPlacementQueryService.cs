using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearningDeliveryWorkPlacementQueryService : IQueryService
    {
        bool HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(IEnumerable<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements, DateTime? learnActEndDate);

        bool HasAnyEmpIdNullAndStartDateNotNull(IEnumerable<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements);

        bool IsValidWorkPlaceMode(int workPlaceMode);
    }
}
