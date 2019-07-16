namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    /// <summary>
    /// the learn aim reference action providers branching result
    /// this isn't the right place for this; but namespaces are shonky...
    /// </summary>
    public interface IBranchResult
    {
        /// <summary>
        /// Gets a value indicating whether [out of scope].
        /// </summary>
        bool OutOfScope { get; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        string Category { get; }
    }
}
