using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF eligibility rule definition
    /// </summary>
    public interface IEsfEligibilityRule
    {
        string TenderSpecReference { get; }

        string LotReference { get; }

        int? MinAge { get; }

        int? MaxAge { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IEsfEligibilityRule"/> has benefits support.
        /// Null - information is not provided
        /// 0 - No Benefit
        /// 1 - Benefit
        /// </summary>
        bool? Benefits { get; }

        /// <summary>
        /// Gets the minimum length of unemployment.
        /// </summary>
        int? MinLengthOfUnemployment { get; }

        /// <summary>
        /// Gets the maximum length of unemployment.
        /// </summary>
        int? MaxLengthOfUnemployment { get; }

        string MinPriorAttainment { get; set; }

        string MaxPriorAttainment { get; set; }

        IEnumerable<IEsfEligibilityRuleEmploymentStatus> EmploymentStatuses { get; }

        IEnumerable<IEsfEligibilityRuleLocalAuthority> LocalAuthorities { get; }

        IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership> LocalEnterprisePartnerships { get; }

        IEnumerable<IEsfEligibilityRuleSectorSubjectAreaLevel> SectorSubjectAreaLevels { get; }
    }
}
