using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface
{
    public interface IOrganisation
    {
        long? UKPRN { get; set; }

        string LegalOrgType { get; set; }

        bool? PartnerUKPRN { get; set; }

        bool? LongTermResid { get; set; }

        IEnumerable<IShortTermFundingInitiative> ShortTermFundingInitiatives { get; set; }
    }
}
