using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    /// <summary>
    /// i provide learn aim reference rule branching action results
    /// this isn't the right place for this; but namespaces are shonky...
    /// </summary>
    public interface IProvideLearnAimRefRuleActions : IQueryService
    {
        /// <summary>
        /// Gets the branching result for (this delivery and learner)
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="andLearner">and learner.</param>
        /// <returns>a branching result</returns>
        IBranchResult GetBranchingResultFor(ILearningDelivery thisDelivery, ILearner andLearner);
    }
}
