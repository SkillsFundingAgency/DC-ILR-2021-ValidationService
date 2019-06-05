using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// the lars annual value
    /// </summary>
    public interface ILARSAnnualValue :
        ISupportFundingWithdrawal
    {
        /// <summary>
        /// Gets the learn aim reference.
        /// </summary>
        string LearnAimRef { get; }

        /// <summary>
        /// Gets the basic skills.
        /// </summary>
        int? BasicSkills { get; }

        /// <summary>
        /// Gets the type of the basic skills.
        /// </summary>
        int? BasicSkillsType { get; }

        /// <summary>
        /// Gets the full level 2 entitlement category.
        /// </summary>
        int? FullLevel2EntitlementCategory { get; }

        /// <summary>
        /// Gets the full level 2 percentage.
        /// </summary>
        decimal? FullLevel2Percent { get; }

        /// <summary>
        /// Gets the full level 3 entitlement category.
        /// </summary>
        int? FullLevel3EntitlementCategory { get; }

        /// <summary>
        /// Gets the full level 3 percentage.
        /// </summary>
        decimal? FullLevel3Percent { get; }
    }
}
