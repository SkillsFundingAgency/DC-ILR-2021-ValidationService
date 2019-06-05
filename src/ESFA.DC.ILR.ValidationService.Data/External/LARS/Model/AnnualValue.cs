using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    /// <summary>
    /// the lars annual value
    /// </summary>
    /// <seealso cref="ILARSAnnualValue" />
    public class AnnualValue :
        ILARSAnnualValue
    {
        /// <summary>
        /// Gets or sets the learn aim reference.
        /// </summary>
        public string LearnAimRef { get; set; }

        /// <summary>
        /// Gets or sets the effective from date.
        /// </summary>
        public DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the effective to date.
        /// </summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>
        /// Gets the effective from (date).
        /// </summary>
        public DateTime StartDate => EffectiveFrom;

        /// <summary>
        /// Gets the effective to (date).
        /// </summary>
        public DateTime? EndDate => EffectiveTo;

        /// <summary>
        /// Gets or sets the basic skills.
        /// </summary>
        public int? BasicSkills { get; set; }

        /// <summary>
        /// Gets or sets the type of the basic skills.
        /// </summary>
        public int? BasicSkillsType { get; set; }

        /// <summary>
        /// Gets or sets the full level 2 entitlement category.
        /// </summary>
        public int? FullLevel2EntitlementCategory { get; set; }

        /// <summary>
        /// Gets or sets the full level 2 percentage.
        /// </summary>
        public decimal? FullLevel2Percent { get; set; }

        /// <summary>
        /// Gets or sets the full level 3 entitlement category.
        /// </summary>
        public int? FullLevel3EntitlementCategory { get; set; }

        /// <summary>
        /// Gets or sets the full level 3 percentage.
        /// </summary>
        public decimal? FullLevel3Percent { get; set; }
    }
}
