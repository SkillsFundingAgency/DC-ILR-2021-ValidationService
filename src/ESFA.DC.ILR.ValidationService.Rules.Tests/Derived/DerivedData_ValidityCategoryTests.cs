using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_ValidityCategoryTests
    {
        [Fact]
        public void CommunityLearningMatch_True()
        {
            NewDDRule().CommunityLearningMatch(new TestLearningDelivery { FundModel = 10 }).Should().BeTrue();
        }

        [Fact]
        public void CommunityLearningMatch_False()
        {
            NewDDRule().CommunityLearningMatch(new TestLearningDelivery { FundModel = 99 }).Should().BeFalse();
        }

        [Fact]
        public void ESFMatch_True()
        {
            NewDDRule().ESFMatch(new TestLearningDelivery { FundModel = 70 }).Should().BeTrue();
        }

        [Fact]
        public void ESFMatch_False()
        {
            NewDDRule().CommunityLearningMatch(new TestLearningDelivery { FundModel = 99 }).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        public void EFA16To19Match_True(int fundModel)
        {
            NewDDRule().EFA16To19Match(new TestLearningDelivery { FundModel = fundModel }).Should().BeTrue();
        }

        [Fact]
        public void EFA16To19Match_False()
        {
            NewDDRule().EFA16To19Match(new TestLearningDelivery { FundModel = 99 }).Should().BeFalse();
        }

        [Fact]
        public void AdvancedLearnerLoanMatch_True()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ADL",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "1"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(true);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object).AdvancedLearnerLoanMatch(new TestLearningDelivery { FundModel = 99, LearningDeliveryFAMs = learningDeliveryFAMs }).Should().BeTrue();
        }

        [Theory]
        [InlineData(99, false)]
        [InlineData(70, true)]
        [InlineData(70, false)]
        public void AdvancedLearnerLoanMatch_False(int fundModel, bool mockReturn)
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ADL",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "1"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(mockReturn);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object).AdvancedLearnerLoanMatch(new TestLearningDelivery { FundModel = fundModel, LearningDeliveryFAMs = learningDeliveryFAMs }).Should().BeFalse();
        }

        [Theory]
        [InlineData(99)]
        [InlineData(81)]
        [InlineData(36)]
        public void AnyMatch_True(int fundModel)
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "1"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(false);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object).AnyMatch(new TestLearningDelivery { FundModel = fundModel, ProgTypeNullable = 25, LearningDeliveryFAMs = learningDeliveryFAMs }).Should().BeTrue();
        }

        [Theory]
        [InlineData(99, true)]
        [InlineData(70, false)]
        [InlineData(36, false)]
        public void AnyMatch_False(int fundModel, bool mockReturn)
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "1"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(mockReturn);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object).AnyMatch(new TestLearningDelivery { FundModel = fundModel, LearningDeliveryFAMs = learningDeliveryFAMs }).Should().BeFalse();
        }

        [Fact]
        public void OlassAdultMatch_True()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(true);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object).OlassAdultMatch(new TestLearningDelivery { FundModel = 35, LearningDeliveryFAMs = learningDeliveryFAMs }).Should().BeTrue();
        }

        [Theory]
        [InlineData(99, true)]
        [InlineData(35, false)]
        [InlineData(99, false)]
        public void OlassAdult_False(int fundModel, bool mockReturn)
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(mockReturn);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object).OlassAdultMatch(new TestLearningDelivery { FundModel = fundModel, LearningDeliveryFAMs = learningDeliveryFAMs }).Should().BeFalse();
        }

        [Fact]
        public void AdultSkillsMatch_True()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "035"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);
            dd07Mock.Setup(qs => qs.IsApprenticeship(null)).Returns(false);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object, dd07Mock.Object).AdultSkillsMatch(new TestLearningDelivery { FundModel = 35, LearningDeliveryFAMs = learningDeliveryFAMs }).Should().BeTrue();
        }

        [Theory]
        [InlineData(99, null, false, false)]
        [InlineData(35, 25, false, true)]
        [InlineData(35, 25, true, true)]
        [InlineData(35, null, true, false)]
        public void AdultSkillsMatch_False(int fundModel, int? progType, bool mockReturnLDM, bool mockReturnDD07)
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "035"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(mockReturnLDM);
            dd07Mock.Setup(qs => qs.IsApprenticeship(progType)).Returns(mockReturnDD07);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object, dd07Mock.Object).AdultSkillsMatch(new TestLearningDelivery { FundModel = fundModel, ProgTypeNullable = progType, LearningDeliveryFAMs = learningDeliveryFAMs }).Should().BeFalse();
        }

        [Theory]
        [InlineData(35, 24, 3, 2012)]
        [InlineData(36, 24, 3, 2012)]
        public void ApprenticeshipsMatch_True(int fundModel, int progType, int aimType, int year)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(qs => qs.IsApprenticeship(progType)).Returns(true);

            NewDDRule(dd07: dd07Mock.Object).ApprenticeshipsMatch(
                new TestLearningDelivery
                {
                    FundModel = fundModel,
                    ProgTypeNullable = progType,
                    AimType = aimType,
                    LearnStartDate = new DateTime(year, 09, 01)
                }).Should().BeTrue();
        }

        [Theory]
        [InlineData(70, 24, 3, 2012, true)]
        [InlineData(35, 25, 3, 2012, true)]
        [InlineData(35, 24, 2, 2012, true)]
        [InlineData(35, 24, 3, 2010, true)]
        [InlineData(35, 24, 3, 2010, false)]
        public void ApprenticeshipsMatch_False(int fundModel, int progType, int aimType, int year, bool mockReturn)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(qs => qs.IsApprenticeship(progType)).Returns(mockReturn);

            NewDDRule(dd07: dd07Mock.Object).ApprenticeshipsMatch(
                new TestLearningDelivery
                {
                    FundModel = fundModel,
                    ProgTypeNullable = progType,
                    AimType = aimType,
                    LearnStartDate = new DateTime(year, 09, 01)
                }).Should().BeFalse();
        }

        [Fact]
        public void UnemployedMatch_True()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "1"
                }
            };

            var delivery = new TestLearningDelivery
            {
                FundModel = 35,
                ProgTypeNullable = null,
                LearnStartDate = new DateTime(2012, 09, 01),
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var employmentStatuses = new List<ILearnerEmploymentStatus>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(qs => qs.IsApprenticeship(null)).Returns(false);

            var dd11Mock = new Mock<IDerivedData_11Rule>();
            dd11Mock.Setup(qs => qs.IsAdultFundedOnBenefitsAtStartOfAim(delivery, employmentStatuses)).Returns(true);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object, dd07Mock.Object, dd11Mock.Object).UnemployedMatch(delivery, employmentStatuses).Should().BeTrue();
        }

        [Theory]
        [InlineData(36, 2012, null, true, false, false)]
        [InlineData(35, 2016, null, true, false, false)]
        [InlineData(35, 2012, 24, true, true, false)]
        [InlineData(35, 2012, null, false, false, false)]
        [InlineData(35, 2012, null, true, false, true)]
        public void UnemployedMatch_False(int fundModel, int year, int? progType, bool dd11MockReturn, bool dd07MockReturn, bool ldmMockReturn)
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "1"
                }
            };

            var delivery = new TestLearningDelivery
            {
                FundModel = fundModel,
                ProgTypeNullable = progType,
                LearnStartDate = new DateTime(year, 09, 01),
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var employmentStatuses = new List<ILearnerEmploymentStatus>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(ldmMockReturn);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(qs => qs.IsApprenticeship(progType)).Returns(dd07MockReturn);

            var dd11Mock = new Mock<IDerivedData_11Rule>();
            dd11Mock.Setup(qs => qs.IsAdultFundedOnBenefitsAtStartOfAim(delivery, employmentStatuses)).Returns(dd11MockReturn);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object, dd07Mock.Object, dd11Mock.Object).UnemployedMatch(delivery, employmentStatuses).Should().BeFalse();
        }

        [Theory]
        [InlineData(LARSValidities.CommunityLearning, 10, false, false, false, false)]
        [InlineData(LARSValidities.EuropeanSocialFund, 70, false, false, false, false)]
        [InlineData(LARSValidities.EFA16To19, 25, false, false, false, false)]
        [InlineData(LARSValidities.AdvancedLearnerLoan, 99, true, false, false, false)]
        [InlineData(LARSValidities.Any, 99, false, false, false, false)]
        [InlineData(LARSValidities.OLASSAdult, 35, false, true, false, false)]
        [InlineData(LARSValidities.AdultSkills, 35, false, false, false, false)]
        [InlineData(LARSValidities.Apprenticeships, 36, false, false, true, false)]
        [InlineData(LARSValidities.Unemployed, 35, false, false, false, true)]
        [InlineData(null, 900, false, false, false, false)]
        public void GetValidityCategory(string expectedCategory, int fundModel, bool famTypeMock, bool ldmFamMock, bool dd07Mock, bool dd11Mock)
        {
            var delivery = new TestLearningDelivery
            {
                FundModel = fundModel,
                AimType = 3,
                ProgTypeNullable = 24,
                LearnStartDate = new DateTime(2015, 09, 01),
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            };

            var employmentStatuses = new List<ILearnerEmploymentStatus>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ADL")).Returns(famTypeMock);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(ldmFamMock);

            var dd07RuleMock = new Mock<IDerivedData_07Rule>();
            dd07RuleMock.Setup(qs => qs.IsApprenticeship(It.IsAny<int?>())).Returns(dd07Mock);

            var dd11RuleMock = new Mock<IDerivedData_11Rule>();
            dd11RuleMock.Setup(qs => qs.IsAdultFundedOnBenefitsAtStartOfAim(delivery, employmentStatuses)).Returns(dd11Mock);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object, dd07RuleMock.Object, dd11RuleMock.Object).GetValidityCategory(delivery, employmentStatuses).Should().Be(expectedCategory);
        }

        [Theory]
        [InlineData(LARSValidities.CommunityLearning, 10, false, false, false, false, false)]
        [InlineData(LARSValidities.EuropeanSocialFund, 70, false, false, false, false, false)]
        [InlineData(LARSValidities.EFA16To19, 25, false, false, false, false, false)]
        [InlineData(LARSValidities.AdvancedLearnerLoan, 99, true, false, false, false, false)]
        [InlineData(LARSValidities.Any, 99, false, false, false, false, false)]
        [InlineData(LARSValidities.OLASSAdult, 35, false, true, false, false, false)]
        [InlineData(LARSValidities.AdultSkills, 35, false, false, false, false, false)]
        [InlineData(LARSValidities.Apprenticeships, 36, false, false, true, false, false)]
        [InlineData(LARSValidities.Unemployed, 35, false, false, false, true, false)]
        [InlineData(null, 10, false, false, false, false, true)]
        public void Derive(string expectedCategory, int fundModel, bool famTypeMock, bool ldmFamMock, bool dd07Mock, bool dd11Mock, bool resMock)
        {
            var delivery = new TestLearningDelivery
            {
                FundModel = fundModel,
                AimType = 3,
                ProgTypeNullable = 24,
                LearnStartDate = new DateTime(2015, 09, 01),
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            };

            var employmentStatuses = new List<ILearnerEmploymentStatus>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ADL")).Returns(famTypeMock);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES")).Returns(resMock);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(ldmFamMock);

            var dd07RuleMock = new Mock<IDerivedData_07Rule>();
            dd07RuleMock.Setup(qs => qs.IsApprenticeship(It.IsAny<int?>())).Returns(dd07Mock);

            var dd11RuleMock = new Mock<IDerivedData_11Rule>();
            dd11RuleMock.Setup(qs => qs.IsAdultFundedOnBenefitsAtStartOfAim(delivery, employmentStatuses)).Returns(dd11Mock);

            NewDDRule(learningDeliveryFAMQueryServiceMock.Object, dd07RuleMock.Object, dd11RuleMock.Object).Derive(delivery, employmentStatuses).Should().Be(expectedCategory);
        }

        private DerivedData_ValidityCategory NewDDRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IDerivedData_07Rule dd07 = null,
            IDerivedData_11Rule dd11 = null)
        {
            return new DerivedData_ValidityCategory(learningDeliveryFAMQueryService, dd07, dd11);
        }
    }
}
