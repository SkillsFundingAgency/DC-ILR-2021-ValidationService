namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// the lars learning category
    /// </summary>
    public interface ILARSLearningCategory :
        ISupportFundingWithdrawal
    {
        /// <summary>
        /// Gets the learn aim reference.
        /// </summary>
        string LearnAimRef { get; }

        /// <summary>
        /// Gets the category reference.
        /// </summary>
        int CategoryRef { get; }
    }
}
