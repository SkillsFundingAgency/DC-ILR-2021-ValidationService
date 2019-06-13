using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    /// <summary>
    /// derived data rule 29
    /// Traineeship learning aims that are not the flexible element of LARS_BasicSkillsType
    /// </summary>
    public class DerivedData_29Rule :
        IDerivedData_29Rule
    {
        /// <summary>
        /// The lars data (service)
        /// </summary>
        private ILARSDataService _larsData;

        /// <summary>
        /// Initializes a new instance of the <see cref="DerivedData_29Rule"/> class.
        /// </summary>
        /// <param name="larsData">The lars data.</param>
        public DerivedData_29Rule(ILARSDataService larsData)
        {
            _larsData = larsData;
        }

        /// <summary>
        /// Determines whether the specified delivery is traineeship.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is traineeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTraineeship(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship);

        /// <summary>
        /// Determines whether [is work experience] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is work experience] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsWorkExperience(ILearningDelivery delivery)
        {
            var categories = _larsData.GetCategoriesFor(delivery.LearnAimRef);

            return categories.SafeAny(IsWorkExperience);
        }

        /// <summary>
        /// Determines whether [is work experience] [the specified category].
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>
        ///   <c>true</c> if [is work experience] [the specified category]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsWorkExperience(ILARSLearningCategory category) =>
            It.IsInRange(category.CategoryRef, TypeOfLARSCategory.WorkPlacementSFAFunded, TypeOfLARSCategory.WorkPreparationSFATraineeships);

        /// <summary>
        /// Determines whether [is inflexible element of training aim] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is inflexible element of training aim] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInflexibleElementOfTrainingAim(ILearner candidate)
        {
            It.IsNull(candidate)
                .AsGuard<ArgumentNullException>(nameof(candidate));

            var lds = candidate.LearningDeliveries.AsSafeReadOnlyList();

            /*
                if
                    LearningDelivery.ProgType = 24
                    where LearningDelivery.LearnAimRef = LARS_LearnAimRef
                    and LARS_CategoryRef = 2 or 4
                        set to Y,
                        otherwise set to N
             */

            return lds.Any(d => IsTraineeship(d) && IsWorkExperience(d));
        }

        public bool IsInflexibleElementOfTrainingAimLearningDelivery(ILearningDelivery candidate)
        {
            It.IsNull(candidate)
                .AsGuard<ArgumentNullException>(nameof(candidate));

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
