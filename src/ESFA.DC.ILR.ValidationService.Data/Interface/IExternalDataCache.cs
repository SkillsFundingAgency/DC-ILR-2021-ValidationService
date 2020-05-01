using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationRules.Model;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IExternalDataCache
    {
        IReadOnlyCollection<long> ULNs { get; }

        IReadOnlyCollection<int> ERNs { get; }

        IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; }

        IReadOnlyCollection<ILARSStandard> Standards { get; }

        IReadOnlyCollection<ILARSStandardValidity> StandardValidities { get; }

        IReadOnlyDictionary<long, Organisation> Organisations { get; }

        IReadOnlyDictionary<string, List<EPAOrganisations>> EPAOrganisations { get; }

        IReadOnlyCollection<string> Postcodes { get; }

        IReadOnlyCollection<IONSPostcode> ONSPostcodes { get; }

        IReadOnlyDictionary<string, IReadOnlyCollection<IDevolvedPostcode>> DevolvedPostcodes { get; }

        IReadOnlyDictionary<string, ValidationError> ValidationErrors { get; }

        IReadOnlyDictionary<string, IFcsContractAllocation> FCSContractAllocations { get; }

        IReadOnlyCollection<ICampusIdentifier> CampusIdentifiers { get; set; }

        IReadOnlyCollection<ValidationRule> ValidationRules { get; }

        int ReturnPeriod { get; }
    }
}
