using System.Collections.Generic;
using System.Threading;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationRules.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class ExternalDataCachePopulationServiceTests
    {
        [Fact]
        public async void PopulateAsync()
        {
            var validationContextMock = new Mock<IValidationContext>();

            var externalDataCache = new ExternalDataCache();
            var employersDataMapperMock = new Mock<IEmployersDataMapper>();
            var epaOrgDataMapperMock = new Mock<IEpaOrgDataMapper>();
            var fcsDataMapperMock = new Mock<IFcsDataMapper>();
            var larsDataMapperMock = new Mock<ILarsDataMapper>();
            var organisationsDataMapperMock = new Mock<IOrganisationsDataMapper>();
            var postcodesDataMapperMock = new Mock<IPostcodesDataMapper>();
            var ulnDataMapperMock = new Mock<IUlnDataMapper>();
            var validationErrorsDataMapperMock = new Mock<IValidationErrorsDataMapper>();
            var validationRulesDataMapperMock = new Mock<IValidationRulesDataMapper>();

            var referenceDataRoot = new ReferenceDataRoot();

            employersDataMapperMock.Setup(m => m.MapEmployers(It.IsAny<List<ReferenceDataService.Model.Employers.Employer>>())).Returns(new List<int>()).Verifiable();
            epaOrgDataMapperMock.Setup(m => m.MapEpaOrganisations(It.IsAny<List<ReferenceDataService.Model.EPAOrganisations.EPAOrganisation>>())).Returns(new Dictionary<string, List<EPAOrganisations>>()).Verifiable();
            fcsDataMapperMock.Setup(m => m.MapFcsContractAllocations(It.IsAny<List<ReferenceDataService.Model.FCS.FcsContractAllocation>>())).Returns(new Dictionary<string, IFcsContractAllocation>()).Verifiable();
            larsDataMapperMock.Setup(m => m.MapLarsLearningDeliveries(It.IsAny<List<ReferenceDataService.Model.LARS.LARSLearningDelivery>>())).Returns(new Dictionary<string, LearningDelivery>()).Verifiable();
            larsDataMapperMock.Setup(m => m.MapLarsStandards(It.IsAny<List<ReferenceDataService.Model.LARS.LARSStandard>>())).Returns(new List<ILARSStandard>()).Verifiable();
            larsDataMapperMock.Setup(m => m.MapLarsStandardValidities(It.IsAny<List<ReferenceDataService.Model.LARS.LARSStandard>>())).Returns(new List<ILARSStandardValidity>()).Verifiable();
            organisationsDataMapperMock.Setup(m => m.MapOrganisations(It.IsAny<List<ReferenceDataService.Model.Organisations.Organisation>>())).Returns(new Dictionary<long, Organisation>()).Verifiable();
            organisationsDataMapperMock.Setup(m => m.MapCampusIdentifiers(It.IsAny<List<ReferenceDataService.Model.Organisations.Organisation>>())).Returns(new List<ICampusIdentifier>()).Verifiable();
            postcodesDataMapperMock.Setup(m => m.MapONSPostcodes(It.IsAny<List<ReferenceDataService.Model.Postcodes.Postcode>>())).Returns(new List<ONSPostcode>()).Verifiable();
            postcodesDataMapperMock.Setup(m => m.MapPostcodes(It.IsAny<List<ReferenceDataService.Model.Postcodes.Postcode>>())).Returns(new List<string>()).Verifiable();
            postcodesDataMapperMock.Setup(m => m.MapMcaglaSOFPostcodes(It.IsAny<List<ReferenceDataService.Model.Postcodes.Postcode>>())).Returns(new List<McaglaSOFPostcode>()).Verifiable();

            ulnDataMapperMock.Setup(m => m.MapUlns(It.IsAny<List<long>>())).Returns(new List<long>()).Verifiable();
            validationErrorsDataMapperMock.Setup(m => m.MapValidationErrors(It.IsAny<List<ReferenceDataService.Model.MetaData.ValidationError>>())).Returns(new Dictionary<string, ValidationError>()).Verifiable();
            validationRulesDataMapperMock.Setup(m => m.MapValidationRules(It.IsAny<List<ReferenceDataService.Model.MetaData.ValidationRule>>())).Returns(new List<ValidationRule>()).Verifiable();

            NewService(
                externalDataCache,
                employersDataMapperMock.Object,
                epaOrgDataMapperMock.Object,
                fcsDataMapperMock.Object,
                larsDataMapperMock.Object,
                organisationsDataMapperMock.Object,
                postcodesDataMapperMock.Object,
                ulnDataMapperMock.Object,
                validationErrorsDataMapperMock.Object,
                validationRulesDataMapperMock.Object).Populate(referenceDataRoot);

            employersDataMapperMock.VerifyAll();
            epaOrgDataMapperMock.VerifyAll();
            fcsDataMapperMock.VerifyAll();
            larsDataMapperMock.VerifyAll();
            organisationsDataMapperMock.VerifyAll();
            postcodesDataMapperMock.VerifyAll();
            ulnDataMapperMock.VerifyAll();
            validationErrorsDataMapperMock.VerifyAll();
            validationRulesDataMapperMock.VerifyAll();
        }

        private ExternalDataCachePopulationService NewService(
            IExternalDataCache externalDataCache = null,
            IEmployersDataMapper employersDataMapper = null,
            IEpaOrgDataMapper epaOrgDataMapper = null,
            IFcsDataMapper fcsDataMapper = null,
            ILarsDataMapper larsDataMapper = null,
            IOrganisationsDataMapper organisationsDataMapper = null,
            IPostcodesDataMapper postcodesDataMapper = null,
            IUlnDataMapper ulnDataMapper = null,
            IValidationErrorsDataMapper validationErrorsDataMapper = null,
            IValidationRulesDataMapper validationRulesDataMapper = null)
        {
            return new ExternalDataCachePopulationService(
                externalDataCache,
                employersDataMapper,
                epaOrgDataMapper,
                fcsDataMapper,
                larsDataMapper,
                organisationsDataMapper,
                postcodesDataMapper,
                ulnDataMapper,
                validationErrorsDataMapper,
                validationRulesDataMapper);
        }
    }
}
