using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_29Rule :
        IDerivedData_29Rule
    {
        private ILARSDataService _larsData;

        public DerivedData_29Rule(ILARSDataService larsData)
        {
            _larsData = larsData;
        }

        public bool IsTraineeship(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == TypeOfLearningProgramme.Traineeship;

        public bool IsWorkExperience(ILearningDelivery delivery)
        {
            var categories = _larsData.GetCategoriesFor(delivery.LearnAimRef);

            return categories.Any(IsWorkExperience);
        }

        public bool IsWorkExperience(ILARSLearningCategory category) =>
            category.CategoryRef == TypeOfLARSCategory.WorkPlacementSFAFunded
            || category.CategoryRef == TypeOfLARSCategory.WorkPreparationSFATraineeships;

        public bool IsInflexibleElementOfTrainingAimLearningDelivery(ILearningDelivery candidate)
        {
            /*
               if
                   LearningDelivery.ProgType = 24
                   where LearningDelivery.LearnAimRef = LARS_LearnAimRef
                   and LARS_CategoryRef = 2 or 4
                       set to Y,
                       otherwise set to N
            */

            return IsTraineeship(candidate) && IsWorkExperience(candidate);
        }
    }
}
