using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
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

            var academicStartDate = new DateTime(2019, 8, 1);
            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            var result = NewRule(academicYearDataService: mockAcademicYearDataService.Object).LearnStartDateConditionMet(startDate);
            result.Should().BeTrue();

            mockAcademicYearDataService.Verify(x => x.Start(), Times.AtLeastOnce);
        }

        [Fact]
        public void LearnStartDate_True_AsStartIsGreater()
        {
            var startDate = new DateTime(2020, 02, 01);

            var academicStartDate = new DateTime(2019, 8, 1);
            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            var result = NewRule(academicYearDataService: mockAcademicYearDataService.Object).LearnStartDateConditionMet(startDate);

            result.Should().BeTrue();
            mockAcademicYearDataService.Verify(x => x.Start(), Times.AtLeastOnce);
        }

        [Fact]
        public void LearnStartDate_Fails_AsStartisLower()
        {
            var startDate = new DateTime(2019, 07, 01);

            var academicStartDate = new DateTime(2019, 8, 1);
            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);
            var result = NewRule(academicYearDataService: mockAcademicYearDataService.Object).LearnStartDateConditionMet(startDate);

            result.Should().BeFalse();
            mockAcademicYearDataService.Verify(x => x.Start(), Times.AtLeastOnce);
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
        public void ExclusionConditionMet_Pass()
        {
            var famCode = "001";
            var famType = "DAM";

            var learningDelFams = new List<ILearningDeliveryFAM>();

            var mockLearningDelFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDelFAMQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDelFams, famType, famCode)).Returns(false);

            var rule = NewRule(learningDeliveryFAMQueryService: mockLearningDelFAMQueryService.Object).ExclusionConditionMet(learningDelFams);
            rule.Should().BeTrue();

            mockLearningDelFAMQueryService.Verify(x => x.HasLearningDeliveryFAMCodeForType(learningDelFams, famType, famCode), Times.AtLeastOnce);
        }

        [Fact]
        public void ExclusionConditionMet_Fails()
        {
            var famCode = "001";
            var famType = "DAM";

            var learningDelFams = new List<ILearningDeliveryFAM>();

            var mockLearningDelFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDelFAMQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDelFams, famType, famCode)).Returns(true);

            var rule = NewRule(learningDeliveryFAMQueryService: mockLearningDelFAMQueryService.Object).ExclusionConditionMet(learningDelFams);
            rule.Should().BeFalse();

            mockLearningDelFAMQueryService.Verify(x => x.HasLearningDeliveryFAMCodeForType(learningDelFams, famType, famCode), Times.AtLeastOnce);
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

            var famTypeSOF = "SOF";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famTypeSOF)).Returns(true);

            var famCodeDAM = "001";
            var famTypeDAM = "DAM";
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famTypeDAM, famCodeDAM)).Returns(false);

            var academicStartDate = new DateTime(2019, 8, 1);
            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            var rule = NewRule(
                            learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object,
                            organisationDataService: mockOrganisationDataService.Object,
                            academicYearDataService: mockAcademicYearDataService.Object);

            var result = rule.ConditionMet(ukprn, progType, fundModel, startDate, mcaglaPostcodeList, learningDeliveryFams);
            result.Should().BeTrue();

            mockOrganisationDataService.Verify(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType), Times.AtLeastOnce);
            mockLearningDeliveryFAMQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, famTypeSOF), Times.AtLeastOnce);
            mockLearningDeliveryFAMQueryService.Verify(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famTypeDAM, famCodeDAM), Times.AtLeastOnce);
            mockAcademicYearDataService.Verify(x => x.Start(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true, false, false, false, false, false, false)]
        [InlineData(false, true, false, false, false, false, false)]
        [InlineData(false, false, true, false, false, false, false)]
        [InlineData(false, false, false, true, false, false, false)]
        [InlineData(false, false, false, false, true, false, false)]
        [InlineData(false, false, false, false, false, true, false)]
        [InlineData(false, false, false, false, false, false, true)]
        public void ConditionMet_Fails(bool progCondition, bool fundCondition, bool startDateCondition, bool qualifyingCondition, bool organisationCondition, bool delFAMSCondition, bool excludingCondition)
        {
            var ukprn = 123;
            int progType = 24;
            int fundModel = 25;
            DateTime startDate = new DateTime(2019, 6, 18);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF
                }
            };

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

            var lsdPostcode02Rule = NewRuleMock();
            lsdPostcode02Rule.Setup(x => x.ProgTypeConditionMet(progType)).Returns(progCondition);
            lsdPostcode02Rule.Setup(x => x.FundModelConditionMet(fundModel)).Returns(fundCondition);
            lsdPostcode02Rule.Setup(x => x.LearnStartDateConditionMet(startDate)).Returns(startDateCondition);
            lsdPostcode02Rule.Setup(x => x.CheckQualifyingPeriod(startDate, learningDeliveryFAMs, mcaglaPostcodeList)).Returns(qualifyingCondition);
            lsdPostcode02Rule.Setup(x => x.OrganisationConditionMet(ukprn)).Returns(organisationCondition);
            lsdPostcode02Rule.Setup(x => x.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(delFAMSCondition);
            lsdPostcode02Rule.Setup(x => x.ExclusionConditionMet(learningDeliveryFAMs)).Returns(excludingCondition);

            lsdPostcode02Rule.Object
                .ConditionMet(ukprn, progType, fundModel, startDate, mcaglaPostcodeList, learningDeliveryFAMs)
                .Should()
                .BeFalse();
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

            var famCodeDAM = "001";
            var famTypeDAM = "DAM";
            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, "SOF")).Returns(true);
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famTypeDAM, famCodeDAM)).Returns(false);

            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(false);

            var academicStartDate = new DateTime(2019, 8, 1);
            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object,
                    postcodesDataService: mockPostcodeService.Object,
                    organisationDataService: mockOrganisationDataService.Object,
                    academicYearDataService: mockAcademicYearDataService.Object,
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

            var famCodeDAM = "001";
            var famTypeDAM = "DAM";
            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFams, "SOF")).Returns(true);
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famTypeDAM, famCodeDAM)).Returns(true);

            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(true);

            var academicStartDate = new DateTime(2019, 8, 1);
            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object,
                    postcodesDataService: mockPostcodeService.Object,
                    organisationDataService: mockOrganisationDataService.Object,
                    academicYearDataService: mockAcademicYearDataService.Object,
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
            IAcademicYearDataService academicYearDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LSDPostcode_02Rule(learningDeliveryFAMQueryService, organisationDataService, postcodesDataService, academicYearDataService, validationErrorHandler);
        }
    }
}
