using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 21
    /// Adult skills funded unemployed learner on other state benefits on learning aim start date
    /// </summary>
    public interface IDerivedData_21Rule
    {
        /// <summary>
        /// Determines whether [is adult funded unemployed with other state benefits] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">This delivery.</param>
        /// <param name="forThisCandidate">For this candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is adult funded unemployed with other state benefits] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool IsAdultFundedUnemployedWithOtherStateBenefits(ILearningDelivery thisDelivery, ILearner forThisCandidate);
    }
}
