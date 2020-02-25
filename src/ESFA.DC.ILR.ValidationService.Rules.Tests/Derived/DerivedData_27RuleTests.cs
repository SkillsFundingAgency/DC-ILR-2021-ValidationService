using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_27RuleTests
    {
        [Fact]
        public void IsUKPRNCollegeOrGrantFundedProvider_False()
        {
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            organisationDataServiceMock.Setup(o => o.GetLegalOrgTypeForUkprn(123456789)).Returns("ABCD");

            NewRule(organisationDataService: organisationDataServiceMock.Object).IsUKPRNCollegeOrGrantFundedProvider(123456789).Should().BeFalse();
        }

        [Fact]
        public void IsUKPRNCollegeOrGrantFundedProvider_EmptyCheck()
        {
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            organisationDataServiceMock.Setup(o => o.GetLegalOrgTypeForUkprn(123456789)).Returns(string.Empty);

            NewRule(organisationDataService: organisationDataServiceMock.Object).IsUKPRNCollegeOrGrantFundedProvider(123456789).Should().BeFalse();
        }

        [Fact]
        public void IsUKPRNCollegeOrGrantFundedProvider_True()
        {
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            organisationDataServiceMock.Setup(o => o.GetLegalOrgTypeForUkprn(987654321)).Returns("USFC");

            NewRule(organisationDataService: organisationDataServiceMock.Object).IsUKPRNCollegeOrGrantFundedProvider(987654321).Should().BeTrue();
        }

        public DerivedData_27Rule NewRule(IOrganisationDataService organisationDataService = null)
        {
            return new DerivedData_27Rule(organisationDataService: organisationDataService);
        }
    }
}
