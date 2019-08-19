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
using System.Linq;
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
        public void IsInvalidSofCodeOnLearnStartDate_True()
        {
            var learnStartDate = new DateTime(2019, 08, 01);
            var famCode = "001";
            var famType = "SOF";

            var learningDelFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = famCode,
                    LearnDelFAMType = famType
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "111",
                    LearnDelFAMType = "LDM"
                }
            };

            var learningDelFamsMockResult = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = famCode,
                    LearnDelFAMType = famType
                },
            };

            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    SourceOfFunding = "001",
                    EffectiveFrom = new DateTime(2019, 08, 02)
                },
                new DevolvedPostcode
                {
                    SourceOfFunding = "111",
                    EffectiveFrom = new DateTime(2019, 08, 01)
                }
            };

            var mockLearningDelFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDelFAMQueryService.Setup(x => x.GetLearningDeliveryFAMsForType(learningDelFams, LearningDeliveryFAMTypeConstants.SOF)).Returns(learningDelFamsMockResult);

            NewRule(mockLearningDelFAMQueryService.Object).IsInvalidSofCodeOnLearnStartDate(learnStartDate, learningDelFams, devolvedPostcodes).Should().BeTrue();
        }

        [Theory]
        [InlineData(2020, "001", "SOF", "001", true)]
        [InlineData(2019, "002", "SOF", "001", false)]
        [InlineData(2019, "001", "LDM", "001", false)]
        [InlineData(2019, "001", "SOF", "002", true)]
        public void IsInvalidSofCodeOnLearnStartDate_False(int yearStart, string sofCode, string learnDelFamType, string postcodeSofCode, bool ldFamMock)
        {
            var learnStartDate = new DateTime(yearStart, 08, 01);
            var famCode = sofCode;
            var famType = learnDelFamType;

            var learningDelFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = famCode,
                    LearnDelFAMType = famType
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "111",
                    LearnDelFAMType = "LDM"
                }
            };

            var learningDelFamsMockResult = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = famCode,
                    LearnDelFAMType = famType
                },
            };

            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    SourceOfFunding = postcodeSofCode,
                    EffectiveFrom = new DateTime(2019, 08, 02)
                },
                new DevolvedPostcode
                {
                    SourceOfFunding = "111",
                    EffectiveFrom = new DateTime(2019, 08, 01)
                }
            };

            var ldFamMockOutput = ldFamMock == true ? learningDelFamsMockResult : new List<ILearningDeliveryFAM>();

            var mockLearningDelFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDelFAMQueryService.Setup(x => x.GetLearningDeliveryFAMsForType(learningDelFams, LearningDeliveryFAMTypeConstants.SOF)).Returns(ldFamMockOutput);

            NewRule(mockLearningDelFAMQueryService.Object).IsInvalidSofCodeOnLearnStartDate(learnStartDate, learningDelFams, devolvedPostcodes).Should().BeFalse();
        }

        //[Fact]
        //public void ConditionMet_True()
        //{
        //    var famCode = "001";
        //    var famType = "SOF";

        //    var learningDeliveryFams = new List<ILearningDeliveryFAM>
        //    {
        //        new TestLearningDeliveryFAM
        //        {
        //            LearnDelFAMCode = famCode,
        //            LearnDelFAMType = famType
        //        },
        //        new TestLearningDeliveryFAM
        //        {
        //            LearnDelFAMCode = "111",
        //            LearnDelFAMType = "LDM"
        //        }
        //    };

        //    var learningDelFamsMockResult = new List<ILearningDeliveryFAM>
        //    {
        //        new TestLearningDeliveryFAM
        //        {
        //            LearnDelFAMCode = famCode,
        //            LearnDelFAMType = famType
        //        },
        //    };

        //    var progType = 25;
        //    var fundModel = 35;
        //    var startDate = new DateTime(2019, 08, 01);

        //    var devolvedPostcodes = new List<IDevolvedPostcode>
        //    {
        //        new DevolvedPostcode
        //        {
        //            SourceOfFunding = "001",
        //            EffectiveFrom = new DateTime(2019, 08, 02)
        //        },
        //        new DevolvedPostcode
        //        {
        //            SourceOfFunding = "111",
        //            EffectiveFrom = new DateTime(2019, 08, 01)
        //        }
        //    };

        //    var ukprn = 123;
        //    var legalOrgType = "USDC";
        //    var mockOrganisationDataService = new Mock<IOrganisationDataService>();
        //    mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(false);

        //    var famTypeSOF = "SOF";

        //    var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
        //    mockLearningDeliveryFAMQueryService.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.SOF)).Returns(learningDelFamsMockResult);

        //    var famCodeDAM = "001";
        //    var famTypeDAM = "DAM";
        //    mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famTypeDAM, famCodeDAM)).Returns(false);

        //    var rule = NewRule(
        //                    learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object,
        //                    organisationDataService: mockOrganisationDataService.Object);

        //    rule.ConditionMet(ukprn, progType, fundModel, startDate, devolvedPostcodes, learningDeliveryFams).Should().BeTrue();
        //}

        [Fact]
        public void ConditionMet_True()
        {
            var ukprn = 123;
            var progType = 25;
            var fundModel = 35;
            var learnStartDate = new DateTime(2019, 08, 01);
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var devolvedPostcodes = new List<IDevolvedPostcode>();

            var rule = NewRuleMock();

            rule.Setup(rm => rm.ProgTypeConditionMet(progType)).Returns(true);
            rule.Setup(rm => rm.OrganisationConditionMet(ukprn)).Returns(true);
            rule.Setup(rm => rm.FundModelConditionMet(fundModel)).Returns(true);
            rule.Setup(rm => rm.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            rule.Setup(rm => rm.IsInvalidSofCodeOnLearnStartDate(learnStartDate, learningDeliveryFams, devolvedPostcodes)).Returns(true);
            rule.Setup(rm => rm.LearningDeliveryFAMsConditionMet(learningDeliveryFams)).Returns(true);
            rule.Setup(rm => rm.ExclusionConditionMet(learningDeliveryFams)).Returns(true);

            rule.Object.ConditionMet(ukprn, progType, fundModel, learnStartDate, devolvedPostcodes, learningDeliveryFams).Should().BeTrue();
        }

        [Theory]
        [InlineData(true, false, false, false, false, false, false)]
        [InlineData(false, true, false, false, false, false, false)]
        [InlineData(false, false, true, false, false, false, false)]
        [InlineData(false, false, false, true, false, false, false)]
        [InlineData(false, false, false, false, true, false, false)]
        [InlineData(false, false, false, false, false, true, false)]
        [InlineData(false, false, false, false, false, false, true)]
        [InlineData(false, false, false, false, false, false, false)]
        public void ConditionMet_False(bool condition1, bool condition2, bool condition3, bool condition4, bool condition5, bool condition6, bool condition7)
        {
            var ukprn = 123;
            var progType = 25;
            var fundModel = 35;
            var learnStartDate = new DateTime(2019, 08, 01);
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var devolvedPostcodes = new List<IDevolvedPostcode>();

            var rule = NewRuleMock();

            rule.Setup(rm => rm.ProgTypeConditionMet(progType)).Returns(condition1);
            rule.Setup(rm => rm.OrganisationConditionMet(ukprn)).Returns(condition2);
            rule.Setup(rm => rm.FundModelConditionMet(fundModel)).Returns(condition3);
            rule.Setup(rm => rm.LearnStartDateConditionMet(learnStartDate)).Returns(condition4);
            rule.Setup(rm => rm.IsInvalidSofCodeOnLearnStartDate(learnStartDate, learningDeliveryFams, devolvedPostcodes)).Returns(condition5);
            rule.Setup(rm => rm.LearningDeliveryFAMsConditionMet(learningDeliveryFams)).Returns(condition6);
            rule.Setup(rm => rm.ExclusionConditionMet(learningDeliveryFams)).Returns(condition7);

            rule.Object.ConditionMet(ukprn, progType, fundModel, learnStartDate, devolvedPostcodes, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var famCode = "001";
            var famType = "SOF";

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = famCode,
                        LearnDelFAMType = famType
                    },
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = "111",
                        LearnDelFAMType = "LDM"
                    }
                };

            var learningDelFamsMockResult = new List<ILearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = famCode,
                        LearnDelFAMType = famType
                    },
                };

            var ukprn = 123;
            var progType = 25;
            var fundModel = 35;
            var learnStartDate = new DateTime(2019, 08, 01);
            var lsdPostcode = "LSDPostcode";

            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "LSDPostcode",
                    SourceOfFunding = "001",
                    EffectiveFrom = new DateTime(2019, 08, 02)
                },
                new DevolvedPostcode
                {
                    Postcode = "LSDPostcode",
                    SourceOfFunding = "111",
                    EffectiveFrom = new DateTime(2019, 08, 01)
                }
             };

            var learningDeliveries = new List<TestLearningDelivery>()
                {
                     new TestLearningDelivery
                     {
                         PartnerUKPRNNullable = ukprn,
                         FundModel = fundModel,
                         ProgTypeNullable = progType,
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

            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(false);

            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.SOF)).Returns(learningDelFamsMockResult);

            var famCodeDAM = "001";
            var famTypeDAM = "DAM";
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famTypeDAM, famCodeDAM)).Returns(false);

            var mockPostcodesDataService = new Mock<IPostcodesDataService>();
            mockPostcodesDataService.Setup(ds => ds.GetDevolvedPostcodes(lsdPostcode)).Returns(devolvedPostcodes);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object,
                    postcodesDataService: mockPostcodesDataService.Object,
                    organisationDataService: mockOrganisationDataService.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
            }
        }

        [Fact]
        public void Validate_Error_MultipleTriggers()
        {
            var famCode = "001";
            var famType = "SOF";

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = famCode,
                        LearnDelFAMType = famType
                    },
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = "111",
                        LearnDelFAMType = "LDM"
                    }
                };

            var learningDelFamsMockResult = new List<ILearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = famCode,
                        LearnDelFAMType = famType
                    },
                };

            var ukprn = 123;
            var progType = 25;
            var fundModel = 35;
            var learnStartDate = new DateTime(2019, 08, 01);
            var lsdPostcode = "LSDPostcode";

            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "LSDPostcode",
                    SourceOfFunding = "001",
                    EffectiveFrom = new DateTime(2019, 08, 02)
                },
                new DevolvedPostcode
                {
                    Postcode = "LSDPostcode",
                    SourceOfFunding = "111",
                    EffectiveFrom = new DateTime(2019, 08, 01)
                }
             };

            var learningDeliveries = new List<TestLearningDelivery>()
                {
                     new TestLearningDelivery
                     {
                         PartnerUKPRNNullable = ukprn,
                         FundModel = fundModel,
                         ProgTypeNullable = progType,
                         AimType = 1,
                         LearnStartDate = learnStartDate,
                         LSDPostcode = lsdPostcode,
                         LearningDeliveryFAMs = learningDeliveryFams
                     },
                     new TestLearningDelivery
                     {
                         PartnerUKPRNNullable = ukprn,
                         FundModel = fundModel,
                         ProgTypeNullable = progType,
                         AimType = 1,
                         LearnStartDate = learnStartDate,
                         LSDPostcode = lsdPostcode,
                         LearningDeliveryFAMs = learningDeliveryFams
                     },
                     new TestLearningDelivery
                     {
                         PartnerUKPRNNullable = ukprn,
                         FundModel = fundModel,
                         ProgTypeNullable = progType,
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

            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(false);

            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.SOF)).Returns(learningDelFamsMockResult);

            var famCodeDAM = "001";
            var famTypeDAM = "DAM";
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famTypeDAM, famCodeDAM)).Returns(false);

            var mockPostcodesDataService = new Mock<IPostcodesDataService>();
            mockPostcodesDataService.Setup(ds => ds.GetDevolvedPostcodes(lsdPostcode)).Returns(devolvedPostcodes);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object,
                    postcodesDataService: mockPostcodesDataService.Object,
                    organisationDataService: mockOrganisationDataService.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 3);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var famCode = "001";
            var famType = "SOF";

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = famCode,
                        LearnDelFAMType = famType
                    },
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = "111",
                        LearnDelFAMType = "LDM"
                    }
                };

            var learningDelFamsMockResult = new List<ILearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = famCode,
                        LearnDelFAMType = famType
                    },
                };

            var ukprn = 123;
            var progType = 25;
            var fundModel = 35;
            var learnStartDate = new DateTime(2019, 08, 01);
            var lsdPostcode = "LSDPostcode";

            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "LSDPostcode",
                    SourceOfFunding = "001",
                    EffectiveFrom = new DateTime(2019, 08, 02)
                },
                new DevolvedPostcode
                {
                    Postcode = "LSDPostcode",
                    SourceOfFunding = "111",
                    EffectiveFrom = new DateTime(2019, 08, 01)
                }
             };

            var learningDeliveries = new List<TestLearningDelivery>()
                {
                     new TestLearningDelivery
                     {
                         PartnerUKPRNNullable = ukprn,
                         FundModel = fundModel,
                         ProgTypeNullable = progType,
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

            var legalOrgType = "USDC";
            var mockOrganisationDataService = new Mock<IOrganisationDataService>();
            mockOrganisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(false);

            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockLearningDeliveryFAMQueryService.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.SOF)).Returns(learningDelFamsMockResult);

            var famCodeDAM = "001";
            var famTypeDAM = "DAM";
            mockLearningDeliveryFAMQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famTypeDAM, famCodeDAM)).Returns(true);

            var mockPostcodesDataService = new Mock<IPostcodesDataService>();
            mockPostcodesDataService.Setup(ds => ds.GetDevolvedPostcodes(lsdPostcode)).Returns(devolvedPostcodes);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object,
                    postcodesDataService: mockPostcodesDataService.Object,
                    organisationDataService: mockOrganisationDataService.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 0);
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
            return new LSDPostcode_02Rule(learningDeliveryFAMQueryService, organisationDataService, postcodesDataService, validationErrorHandler);
        }
    }
}
