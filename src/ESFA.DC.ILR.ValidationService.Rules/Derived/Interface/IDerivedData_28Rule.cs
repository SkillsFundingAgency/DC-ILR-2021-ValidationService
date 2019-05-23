using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 28
    /// Adult skills funded learner unemployed with benefits on learning aim start date
    /// </summary>
    public interface IDerivedData_28Rule
    {
        /// <summary>
        /// Determines whether [is adult skills unemployed with benefits] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">This delivery.</param>
        /// <param name="forThisCandidate">For this candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is adult skills unemployed with benefits] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool IsAdultFundedUnemployedWithBenefits(ILearningDelivery thisDelivery, ILearner forThisCandidate);
    }
}
