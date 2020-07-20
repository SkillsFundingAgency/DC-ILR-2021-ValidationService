using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model
{
    public class Organisation : IOrganisation
    {
        public long? UKPRN { get; set; }

        public string LegalOrgType { get; set; }

        public bool? PartnerUKPRN { get; set; }

        public bool? LongTermResid { get; set; }

        public IEnumerable<IShortTermFundingInitiative> ShortTermFundingInitiatives { get; set; }
    }
}
