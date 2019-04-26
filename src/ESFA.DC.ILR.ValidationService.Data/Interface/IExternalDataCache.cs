using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    /// <summary>
    /// the external data cache definition
    /// </summary>
    public interface IExternalDataCache
    {
        IReadOnlyCollection<long> ULNs { get; }

        /// <summary>
        /// Gets the employer reference numbers.
        /// </summary>
        IReadOnlyCollection<int> ERNs { get; }

        IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; }

        /// <summary>
        /// Gets the LARS standards.
        /// </summary>
        IReadOnlyCollection<ILARSStandard> Standards { get; }

        /// <summary>
        /// Gets the LARS standard validities.
        /// </summary>
        IReadOnlyCollection<ILARSStandardValidity> StandardValidities { get; }

        IReadOnlyDictionary<long, Organisation> Organisations { get; }

        /// <summary>
        /// Gets the epa organisations.
        /// </summary>
        IReadOnlyDictionary<string, List<EPAOrganisations>> EPAOrganisations { get; }

        IReadOnlyCollection<string> Postcodes { get; }

        /// <summary>
        /// Gets the ons postcodes.
        /// </summary>
        IReadOnlyCollection<IONSPostcode> ONSPostcodes { get; }

        IReadOnlyDictionary<string, ValidationError> ValidationErrors { get; }

        /// <summary>
        /// Gets the FCS contract allocations.
        /// </summary>
        IReadOnlyDictionary<string, IFcsContractAllocation> FCSContractAllocations { get; }

        IReadOnlyCollection<ICampusIdentifier> CampusIdentifiers { get; set; }
    }
}
