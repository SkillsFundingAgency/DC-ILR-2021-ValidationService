using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    public class EsfEligibilityRule : IEsfEligibilityRule
    {
        public string TenderSpecReference { get; set; }

        public string LotReference { get; set; }

        public bool? Benefits { get; set; }

        public int? MinAge { get; set; }

        public int? MaxAge { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of unemployment.
        /// </summary>
        public int? MinLengthOfUnemployment { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of unemployment.
        /// </summary>
        public int? MaxLengthOfUnemployment { get; set; }

        public string MinPriorAttainment { get; set; }

        public string MaxPriorAttainment { get; set; }

        public IEnumerable<IEsfEligibilityRuleEmploymentStatus> EmploymentStatuses { get; set; }

        public IEnumerable<IEsfEligibilityRuleLocalAuthority> LocalAuthorities { get; set; }

        public IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership> LocalEnterprisePartnerships { get; set; }

        public IEnumerable<IEsfEligibilityRuleSectorSubjectAreaLevel> SectorSubjectAreaLevels { get; set; }
    }
}
