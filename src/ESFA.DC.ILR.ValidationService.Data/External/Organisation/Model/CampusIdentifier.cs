using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model
{
    public class CampusIdentifier : ICampusIdentifier
    {
        public long MasterUKPRN { get; set; }

        public string CampusIdentifer { get; set; }
    }
}
