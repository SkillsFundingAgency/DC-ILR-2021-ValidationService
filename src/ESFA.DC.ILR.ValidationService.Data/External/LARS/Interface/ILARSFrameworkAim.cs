namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// the lars framework aim
    /// </summary>
    public interface ILARSFrameworkAim :
        ISupportFundingWithdrawal
    {
        /// <summary>
        /// Gets the framework code.
        /// </summary>
        int FworkCode { get; }

        /// <summary>
        /// Gets the type of the programme.
        /// </summary>
        int ProgType { get; }

        /// <summary>
        /// Gets the pathway code.
        /// </summary>
        int PwayCode { get; }

        /// <summary>
        /// Gets the learning aim reference.
        /// </summary>
        string LearnAimRef { get; }

        /// <summary>
        /// Gets the type of the framework component.
        /// </summary>
        int? FrameworkComponentType { get; }
    }
}
