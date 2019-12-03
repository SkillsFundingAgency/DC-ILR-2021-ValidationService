using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationRules.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External
{
    public class ExternalDataCache :
        IExternalDataCache
    {
        public IReadOnlyCollection<long> ULNs { get; set; }

        public IReadOnlyCollection<int> ERNs { get; set; }

        public IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; set; }

        public IReadOnlyDictionary<long, Organisation.Model.Organisation> Organisations { get; set; }

        public IReadOnlyCollection<ILARSStandard> Standards { get; set; }

        public IReadOnlyCollection<ILARSStandardValidity> StandardValidities { get; set; }

        public IReadOnlyDictionary<string, List<EPAOrganisations>> EPAOrganisations { get; set; }

        public IReadOnlyCollection<string> Postcodes { get; set; }

        public IReadOnlyCollection<IONSPostcode> ONSPostcodes { get; set; }

        public IReadOnlyDictionary<string, IReadOnlyCollection<IDevolvedPostcode>> DevolvedPostcodes { get; set; }

        public IReadOnlyDictionary<string, ValidationError> ValidationErrors { get; set; }

        public IReadOnlyDictionary<string, IFcsContractAllocation> FCSContractAllocations { get; set; }

        public IReadOnlyCollection<ICampusIdentifier> CampusIdentifiers { get; set; }

        public IReadOnlyCollection<ValidationRule> ValidationRules { get; set; }

        public int ReturnPeriod { get; set; }
    }
}
