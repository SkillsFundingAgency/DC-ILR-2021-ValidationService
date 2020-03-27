namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model
{
    public class Organisation
    {
        // Org Details
        public long? UKPRN { get; set; }

        // Org Details
        public string LegalOrgType { get; set; }

        // Org Partner UKPRN
        public bool? PartnerUKPRN { get; set; }

        // Org Long Term Residence Indicator
        public bool? LongTermResid { get; set; }
    }
}
