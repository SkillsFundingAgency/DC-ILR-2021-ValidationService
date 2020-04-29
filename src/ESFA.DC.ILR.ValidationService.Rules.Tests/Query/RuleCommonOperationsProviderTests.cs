using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class RuleCommonOperationsProviderTests
    {
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfFunding.Other16To19, true)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfFunding.EuropeanSocialFund, true)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfFunding.OtherAdult, true)]
        public void HasQualifyingFundingMeetsExpectation(int funding, int candidate, bool expectation)
        {
            var sut = NewService();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);

            var result = sut.HasQualifyingFunding(mockDelivery.Object, candidate);

            Assert.Equal(expectation, result);
        }

        private RuleCommonOperationsProvider NewService()
        {
            return new RuleCommonOperationsProvider();
        }
    }
}
