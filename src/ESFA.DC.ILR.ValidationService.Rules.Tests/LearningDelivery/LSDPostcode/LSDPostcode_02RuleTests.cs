using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LSDPostcode
{
    public class LSDPostcode_02RuleTests : AbstractRuleTests<LSDPostcode_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LSDPostcode_02");
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(25, true)]
        [InlineData(TypeOfLearningProgramme.Traineeship, false)]
        public void ProgtypeConditionMet(int? progType, bool asExpected)
        {
            NewRule().ProgTypeConditionMet(progType).Should().Be(asExpected);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        [InlineData(15, false)]
        public void FundModelConditionMet(int fundModel, bool asExpected)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(asExpected);
        }

        [Fact]
        public void LearnStartDate_Passes_AsStartDateisEqual()
        {
            var startDate = new DateTime(2019, 08, 01);
            NewRule().LearnStartDateConditionMet(startDate).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDate_True_AsStartIsGreater()
        {
            var startDate = new DateTime(2020, 02, 01);
            NewRule().LearnStartDateConditionMet(startDate).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDate_Fails_AsStartisLower()
        {
            var startDate = new DateTime(2019, 07, 01);
            NewRule().LearnStartDateConditionMet(startDate).Should().BeFalse();
        }

        [Fact]
        public void CheckQualifyingPeriod_Passes()
        {
            var startDate = new DateTime(2019, 08, 01);
            var mcaglaPostcodeList = new List<McaglaSOFPostcode>()
            {
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2019, 10, 03),
                    SofCode = "SofCode"
                },
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2020, 01, 01)
                },
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var famType = "SOF";

            var mockFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famType)).Returns(true);

            var ruleLSDPostcode02 = NewRule(learningDeliveryFAMQueryService: mockFAMQueryService.Object).CheckQualifyingPeriod(startDate, learningDeliveryFams, mcaglaPostcodeList);

            ruleLSDPostcode02.Should().BeTrue();
        }

        [Fact]
        public void CheckQualifyingPeriod_Fails_AsFamTypeIsSame()
        {
            var startDate = new DateTime(2019, 08, 01);
            var mcaglaPostcodeList = new List<McaglaSOFPostcode>()
            {
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2019, 10, 03),
                    SofCode = "SOF"
                }
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var famType = "SOF";

            var mockFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famType)).Returns(false);

            var ruleLSDPostcode02 = NewRule(learningDeliveryFAMQueryService: mockFAMQueryService.Object).CheckQualifyingPeriod(startDate, learningDeliveryFams, mcaglaPostcodeList);

            ruleLSDPostcode02.Should().BeFalse();
        }

        [Fact]
        public void CheckQualifyingPeriod_Fails_AsStartDateisGreater()
        {
            var startDate = new DateTime(2019, 08, 01);
            var mcaglaPostcodeList = new List<McaglaSOFPostcode>()
            {
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2019, 07, 03),
                    SofCode = "SOFCode"
                }
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var famType = "SOF";

            var mockFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famType)).Returns(true);

            var ruleLSDPostcode02 = NewRule(learningDeliveryFAMQueryService: mockFAMQueryService.Object).CheckQualifyingPeriod(startDate, learningDeliveryFams, mcaglaPostcodeList);

            ruleLSDPostcode02.Should().BeFalse();
        }

        [Fact]
        public void OraginisationConditionMet_True()
        {
            var ukprn = 123;
            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(false);

            var lsdPostcodeRule02 = NewRule(organisationDataService: mockOrganisationDataService.Object).OrganisationConditionMet(ukprn);

            lsdPostcodeRule02.Should().BeTrue();
            mockOrganisationDataService.Verify(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType), Times.AtLeastOnce);
        }

        [Fact]
        public void OraginisationConditionMet_Fails()
        {
            var ukprn = 123;
            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(true);

            var lsdPostcodeRule02 = NewRule(organisationDataService: mockOrganisationDataService.Object).OrganisationConditionMet(ukprn);

            lsdPostcodeRule02.Should().BeFalse();
            mockOrganisationDataService.Verify(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType), Times.AtLeastOnce);
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var famType = "RES";

            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famType)).Returns(false);

            var rule = NewRule(learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFams);

            rule.Should().BeTrue();
            mockLearningDeliveryFAMQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famType), Times.AtLeastOnce);
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_Fails()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var famType = "RES";

            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famType)).Returns(true);

            var rule = NewRule(learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFams);

            rule.Should().BeFalse();
            mockLearningDeliveryFAMQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famType), Times.AtLeastOnce);
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 25;
            var fundModel = 35;
            var startDate = new DateTime(2019, 08, 01);

            var mcaglaPostcodeList = new List<McaglaSOFPostcode>()
            {
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2019, 10, 03),
                    SofCode = "SofCode"
                },
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2020, 01, 01)
                },
            };

            var ukprn = 123;
            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(false);

            var famType = "SOF";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famType)).Returns(true);

            var rule = NewRule(learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object, organisationDataService: mockOrganisationDataService.Object);

            var result = rule.ConditionMet(ukprn, progType, fundModel, startDate, mcaglaPostcodeList, learningDeliveryFams);
            result.Should().BeTrue();

            mockOrganisationDataService.Verify(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType), Times.AtLeastOnce);
            mockLearningDeliveryFAMQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famType), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(TypeOfLearningProgramme.Traineeship, 35, "01-08-2019", "SOF", false, true)]
        public void ConditionMet_Fails_DueToProgType(int progType, int fundModel, string learnStartDate, string learnDelFamType, bool mockOrganisationCondition, bool mockFamsCondition)
        {
            var startDate = DateTime.Parse(learnStartDate);
            var mcaglaPostcodeList = new List<McaglaSOFPostcode>()
            {
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2019, 10, 03),
                    SofCode = "SofCode"
                },
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2020, 01, 01)
                },
            };

            var ukprn = 123;
            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(mockOrganisationCondition);

            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, learnDelFamType)).Returns(mockFamsCondition);

            var ruleToTest = NewRule(learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object, organisationDataService: mockOrganisationDataService.Object);
            var result = ruleToTest.ConditionMet(ukprn, progType, fundModel, startDate, mcaglaPostcodeList, learningDeliveryFams);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(21, 35, "01-08-2018", "SOF", false, true)]
        public void ConditionMet_Fails_DueToFundModel(int progType, int fundModel, string learnStartDate, string learnDelFamType, bool mockOrganisationCondition, bool mockFamsCondition)
        {
            var startDate = DateTime.Parse(learnStartDate);
            var mcaglaPostcodeList = new List<McaglaSOFPostcode>()
            {
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2019, 10, 03),
                    SofCode = "SofCode"
                },
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2020, 01, 01)
                },
            };

            var ukprn = 123;
            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(mockOrganisationCondition);

            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, learnDelFamType)).Returns(mockFamsCondition);

            var ruleToTest = NewRule(learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object, organisationDataService: mockOrganisationDataService.Object);
            var result = ruleToTest.ConditionMet(ukprn, progType, fundModel, startDate, mcaglaPostcodeList, learningDeliveryFams);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(21, 35, "01-08-2019", "SOF", false, false)]
        public void ConditionMet_Fails_DueToQualifyingPeriod(int progType, int fundModel, string learnStartDate, string learnDelFamType, bool mockOrganisationCondition, bool mockFamsCondition)
        {
            var startDate = DateTime.Parse(learnStartDate);
            var mcaglaPostcodeList = new List<McaglaSOFPostcode>()
            {
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2019, 10, 03),
                    SofCode = "SofCode"
                },
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2020, 01, 01)
                },
            };

            var ukprn = 123;
            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(mockOrganisationCondition);

            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, learnDelFamType)).Returns(mockFamsCondition);

            var ruleToTest = NewRule(learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object, organisationDataService: mockOrganisationDataService.Object);
            var result = ruleToTest.ConditionMet(ukprn, progType, fundModel, startDate, mcaglaPostcodeList, learningDeliveryFams);

            result.Should().BeFalse();
            mockLearningDeliveryFAMQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, learnDelFamType), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(21, 35, "01-08-2019", "SOF", true, true)]
        public void ConditionMet_Fails_DueToOrganisationCondition(int progType, int fundModel, string learnStartDate, string learnDelFamType, bool mockOrganisationCondition, bool mockFamsCondition)
        {
            var startDate = DateTime.Parse(learnStartDate);
            var mcaglaPostcodeList = new List<McaglaSOFPostcode>()
            {
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2019, 10, 03),
                    SofCode = "SofCode"
                },
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2020, 01, 01)
                },
            };

            var ukprn = 123;
            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(mockOrganisationCondition);

            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, learnDelFamType)).Returns(mockFamsCondition);

            var ruleToTest = NewRule(learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object, organisationDataService: mockOrganisationDataService.Object);
            var result = ruleToTest.ConditionMet(ukprn, progType, fundModel, startDate, mcaglaPostcodeList, learningDeliveryFams);

            result.Should().BeFalse();
            mockOrganisationDataService.Verify(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(21, 35, "01-08-2019", "SOF", false, false)]
        public void ConditionMet_Fails_DueToLearningFAMsCondition(int progType, int fundModel, string learnStartDate, string learnDelFamType, bool mockOrganisationCondition, bool mockFamsCondition)
        {
            var startDate = DateTime.Parse(learnStartDate);
            var mcaglaPostcodeList = new List<McaglaSOFPostcode>()
            {
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2019, 10, 03),
                    SofCode = "SofCode"
                },
                new McaglaSOFPostcode()
                {
                    EffectiveFrom = new DateTime(2020, 01, 01)
                },
            };

            var ukprn = 123;
            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(mockOrganisationCondition);

            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, learnDelFamType)).Returns(mockFamsCondition);

            var ruleToTest = NewRule(learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object, organisationDataService: mockOrganisationDataService.Object);
            var result = ruleToTest.ConditionMet(ukprn, progType, fundModel, startDate, mcaglaPostcodeList, learningDeliveryFams);

            result.Should().BeFalse();
            mockLearningDeliveryFAMQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, learnDelFamType), Times.AtLeastOnce);
        }

        [Fact]
        public void Validate_Error()
        {
            var ukprn = 123;
            var lsdPostcode = "LSDPostcode";
            var learnStartDate = new DateTime(2019, 09, 01);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2019, 09, 1),
                    LearnDelFAMType = "RES"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                 new TestLearningDelivery
                 {
                     PartnerUKPRNNullable = ukprn,
                     FundModel = 35,
                     ProgTypeNullable = 25,
                     AimType = 1,
                     LearnStartDate = learnStartDate,
                     LSDPostcode = lsdPostcode,
                     LearningDeliveryFAMs = learningDeliveryFams
                 }
            };

            var learner = new TestLearner()
            {
                Postcode = lsdPostcode,
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = learningDeliveries
            };

            var mcaglaSOFPostcodeList = new List<IMcaglaSOFPostcode>()
            {
                 new McaglaSOFPostcode()
                    {
                        SofCode = "SofCode",
                        EffectiveFrom = learnStartDate.AddYears(1),
                        EffectiveTo = learnStartDate.AddMonths(2)
                    }
            };

            var mockPostcodeService = new Mock<IPostcodesDataService>();
            mockPostcodeService.Setup(x => x.GetMcaglaSOFPostcodes(lsdPostcode))
                                .Returns(mcaglaSOFPostcodeList);

            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, "SOF")).Returns(true);

            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object,
                    postcodesDataService: mockPostcodeService.Object,
                    organisationDataService: mockOrganisationDataService.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var ukprn = 123;
            var lsdPostcode = "LSDPostcode";
            var learnStartDate = new DateTime(2019, 09, 01);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2019, 09, 1),
                    LearnDelFAMType = "RES"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                 new TestLearningDelivery
                 {
                     PartnerUKPRNNullable = ukprn,
                     FundModel = 35,
                     ProgTypeNullable = 24,
                     AimType = 1,
                     LearnStartDate = learnStartDate,
                     LSDPostcode = lsdPostcode,
                     LearningDeliveryFAMs = learningDeliveryFams
                 }
            };

            var learner = new TestLearner()
            {
                Postcode = lsdPostcode,
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = learningDeliveries
            };

            var mcaglaSOFPostcodeList = new List<IMcaglaSOFPostcode>()
            {
                 new McaglaSOFPostcode()
                    {
                        SofCode = "SofCode",
                        EffectiveFrom = learnStartDate.AddYears(1),
                        EffectiveTo = learnStartDate.AddMonths(2)
                    }
            };

            var mockPostcodeService = new Mock<IPostcodesDataService>();
            mockPostcodeService.Setup(x => x.GetMcaglaSOFPostcodes(lsdPostcode))
                                .Returns(mcaglaSOFPostcodeList);

            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, "SOF")).Returns(true);

            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object,
                    postcodesDataService: mockPostcodeService.Object,
                    organisationDataService: mockOrganisationDataService.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learnStartDate = new DateTime(2018, 09, 01);
            var fundModel = TypeOfFunding.AdultSkills;
            var lsdPostcode = "lsdPostCode";
            var learnDelFAMType = "ABC";
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/09/2018")).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, lsdPostcode)).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnStartDate, fundModel, lsdPostcode, learnDelFAMType);

            validationErrorHandlerMock.Verify();
        }

        private LSDPostcode_02Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IOrganisationDataService organisationDataService = null,
            IPostcodesDataService postcodesDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LSDPostcode_02Rule(learningDeliveryFAMQueryService, organisationDataService, postcodesDataService,  validationErrorHandler);
        }
    }
}
