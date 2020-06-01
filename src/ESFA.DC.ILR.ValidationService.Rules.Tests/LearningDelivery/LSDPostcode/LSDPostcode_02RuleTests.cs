using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
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

        [Fact]
        public void LearnSartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2019, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnSartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2018, 8, 1)).Should().BeFalse();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(35)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(70).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionOne_True()
        {
            var learnStartDate = new DateTime(2019, 9, 1);
            var devolvedPostcodes = new List<IDevolvedPostcode>();

            NewRule().PostcodeConditionOne(devolvedPostcodes, true).Should().BeTrue();
        }

        [Fact]
        public void PostcodeConditionOne_False()
        {
            var devolvedPostcodes = new List<IDevolvedPostcode>();

            NewRule().PostcodeConditionOne(devolvedPostcodes, false).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionTwo_True()
        {
            var learnStartDate = new DateTime(2019, 9, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "Postcode",
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = "112"
                }
            };

            NewRule().PostcodeConditionTwo(devolvedPostcodes, learnStartDate, sofCode).Should().BeTrue();
        }

        [Fact]
        public void PostcodeConditionTwo_False()
        {
            var learnStartDate = new DateTime(2019, 9, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "Postcode",
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = sofCode
                }
            };

            NewRule().PostcodeConditionTwo(devolvedPostcodes, learnStartDate, sofCode).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionTwo_FalseWhenEmpty()
        {
            var learnStartDate = new DateTime(2019, 9, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>();

            NewRule().PostcodeConditionTwo(devolvedPostcodes, learnStartDate, sofCode).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionThree_False_SOF()
        {
            var learnStartDate = new DateTime(2019, 9, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "Postcode",
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = "112"
                }
            };

            NewRule().PostcodeConditionThree(devolvedPostcodes, learnStartDate, sofCode).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionThree_False_DateWithinRange()
        {
            var learnStartDate = new DateTime(2019, 10, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "Postcode",
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = sofCode
                }
            };

            NewRule().PostcodeConditionThree(devolvedPostcodes, learnStartDate, sofCode).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionThree_True_DateOutOfRange_Lower()
        {
            var learnStartDate = new DateTime(2019, 8, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "Postcode",
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = sofCode
                }
            };

            NewRule().PostcodeConditionThree(devolvedPostcodes, learnStartDate, sofCode).Should().BeTrue();
        }

        [Fact]
        public void PostcodeConditionThree_True_DateOutOfRange_Upper()
        {
            var learnStartDate = new DateTime(2019, 12, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "Postcode",
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    EffectiveTo = new DateTime(2019, 10, 1),
                    SourceOfFunding = sofCode
                }
            };

            NewRule().PostcodeConditionThree(devolvedPostcodes, learnStartDate, sofCode).Should().BeTrue();
        }

        [Fact]
        public void PostcodeConditionThree_FalseWhenEmpty()
        {
            var learnStartDate = new DateTime(2019, 12, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>();

            NewRule().PostcodeConditionThree(devolvedPostcodes, learnStartDate, sofCode).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionMet_False_StartDate()
        {
            var learnStartDate = new DateTime(2019, 9, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "Postcode",
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = sofCode
                }
            };

            NewRule().PostcodeConditionMet(devolvedPostcodes, learnStartDate, sofCode, false).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionMet_False_EmptyDevolvedPostcodes()
        {
            var learnStartDate = new DateTime(2019, 9, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "Postcode",
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = sofCode
                }
            };

            NewRule().PostcodeConditionMet(Array.Empty<IDevolvedPostcode>(), learnStartDate, sofCode, false).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionMet_True_SofCode()
        {
            var learnStartDate = new DateTime(2019, 8, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "Postcode",
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = "106"
                }
            };

            NewRule().PostcodeConditionMet(devolvedPostcodes, learnStartDate, sofCode, false).Should().BeTrue();
        }

        [Fact]
        public void PostcodeConditionMet_True()
        {
            var learnStartDate = new DateTime(2019, 8, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = "Postcode",
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = sofCode
                }
            };

            NewRule().PostcodeConditionMet(devolvedPostcodes, learnStartDate, sofCode, false).Should().BeTrue();
        }

        [Fact]
        public void PostcodeConditionMet_True_EmtpyDevolvedPostcodesforDD35()
        {
            var learnStartDate = new DateTime(2019, 8, 1);
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>();

            NewRule().PostcodeConditionMet(devolvedPostcodes, learnStartDate, sofCode, true).Should().BeTrue();
        }

        [Theory]
        [InlineData(true, "LDM", null, null, false, false, false)]
        [InlineData(false, "LDM", null, null, true, false, false)]
        [InlineData(false, "RES", null, null, false, false, true)]
        [InlineData(false, "DAM", null, null, false, false, true)]
        [InlineData(false, "ACT", "ZZ99 9ZZ", null, false, false, false)]
        [InlineData(false, "ACT", null, 1, false, false, false)]
        public void IsExcluded_True(bool longTermResUkprn, string famType, string postcode, int? progType, bool mockResultLDM, bool mockResultDAM, bool mockResultRES)
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = "034"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(mockResultLDM);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_Code_001)).Returns(mockResultDAM);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(mockResultRES);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).IsExcluded(progType, postcode, learningDeliveryFAMs, longTermResUkprn).Should().BeTrue();
        }

        [Fact]
        public void IsExcluded_False()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "034"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_Code_001)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).IsExcluded(null, null, learningDeliveryFAMs, false).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2019, 8, 1);
            var fundModel = 35;
            var lsdPostcode = "Postcode";
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = lsdPostcode,
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = sofCode
                }
            };

            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            postcodeDataServiceMock.Setup(p => p.GetDevolvedPostcodes(lsdPostcode)).Returns(devolvedPostcodes);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_Code_001)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            NewRule(postcodesDataService: postcodeDataServiceMock.Object, learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, fundModel, null, lsdPostcode, devolvedPostcodes, false, sofCode, It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), false).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_dd35()
        {
            var learnStartDate = new DateTime(2019, 8, 1);
            var fundModel = 35;
            var lsdPostcode = "Postcode";
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>();

            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            postcodeDataServiceMock.Setup(p => p.GetDevolvedPostcodes(lsdPostcode)).Returns(devolvedPostcodes);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_Code_001)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            NewRule(postcodesDataService: postcodeDataServiceMock.Object, learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, fundModel, null, lsdPostcode, devolvedPostcodes, true, sofCode, It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), false).Should().BeTrue();
        }

        [Theory]
        [InlineData(2018, 8, 35, "USDC", false, false, false)]
        [InlineData(2019, 9, 35, "USDC", false, false, false)]
        [InlineData(2019, 8, 70, "USDC", false, false, false)]
        [InlineData(2019, 8, 35, "LTR", true, false, false)]
        [InlineData(2019, 8, 35, "USDC", true, false, false)]
        [InlineData(2019, 8, 35, "USDC", false, true, false)]
        [InlineData(2019, 8, 35, "USDC", false, false, true)]
        public void ConditionMet_False(int year, int month, int fundModel, string legalOrgType, bool mockResultLDM, bool mockResultDAM, bool mockResultRES)
        {
            var learnStartDate = new DateTime(year, month, 1);
            var lsdPostcode = "Postcode";
            var sofCode = "105";
            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = lsdPostcode,
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = sofCode
                }
            };

            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            postcodeDataServiceMock.Setup(p => p.GetDevolvedPostcodes(lsdPostcode)).Returns(devolvedPostcodes);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(mockResultLDM);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_Code_001)).Returns(mockResultDAM);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES)).Returns(mockResultRES);
            NewRule(postcodesDataService: postcodeDataServiceMock.Object, learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object)
                  .ConditionMet(learnStartDate, fundModel, null, lsdPostcode, devolvedPostcodes, false, sofCode, It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), false).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var ukprn = 1;
            var learnStartDate = new DateTime(2019, 8, 1);
            var fundModel = 35;
            var lsdPostcode = "Postcode";
            var legalOrgType = "USDC";
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "105"
                },
            };
            var learningDelivery = new TestLearningDelivery
            {
                LearnStartDate = learnStartDate,
                FundModel = fundModel,
                LSDPostcode = lsdPostcode,
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery,
                    learningDelivery,
                }
            };

            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = lsdPostcode,
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = "105"
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var organisationsDataServiceMock = new Mock<IOrganisationDataService>();
            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd35Mock = new Mock<IDerivedData_35Rule>();

            fileDataServiceMock.Setup(fm => fm.UKPRN()).Returns(ukprn);
            organisationsDataServiceMock.Setup(o => o.GetLegalOrgTypeForUkprn(ukprn)).Returns(legalOrgType);
            postcodeDataServiceMock.Setup(p => p.GetDevolvedPostcodes(lsdPostcode)).Returns(devolvedPostcodes);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.GetLearningDeliveryFAMsForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.SOF)).Returns(learningDeliveryFams);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_Code_001)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES)).Returns(false);
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(It.IsAny<ILearningDelivery>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandlerMock.Object,
                    fileDataServiceMock.Object,
                    postcodeDataServiceMock.Object,
                    organisationsDataServiceMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    dd35Mock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(10));
            }
        }

        [Fact]
        public void ValidateError_EmptyDevolvedPostcodes()
        {
            var ukprn = 1;
            var learnStartDate = new DateTime(2019, 8, 1);
            var fundModel = 35;
            var lsdPostcode = "Postcode";
            var legalOrgType = "USDC";
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "110"
                },
            };
            var learningDelivery = new TestLearningDelivery
            {
                LearnStartDate = learnStartDate,
                FundModel = fundModel,
                LSDPostcode = lsdPostcode,
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery,
                    learningDelivery,
                }
            };

            var devolvedPostcodes = new List<IDevolvedPostcode>();

            var fileDataServiceMock = new Mock<IFileDataService>();
            var organisationsDataServiceMock = new Mock<IOrganisationDataService>();
            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd35Mock = new Mock<IDerivedData_35Rule>();

            fileDataServiceMock.Setup(fm => fm.UKPRN()).Returns(ukprn);
            organisationsDataServiceMock.Setup(o => o.GetLegalOrgTypeForUkprn(ukprn)).Returns(legalOrgType);
            postcodeDataServiceMock.Setup(p => p.GetDevolvedPostcodes(lsdPostcode)).Returns(devolvedPostcodes);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.GetLearningDeliveryFAMsForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.SOF)).Returns(learningDeliveryFams);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_Code_001)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES)).Returns(false);
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(It.IsAny<ILearningDelivery>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandlerMock.Object,
                    fileDataServiceMock.Object,
                    postcodeDataServiceMock.Object,
                    organisationsDataServiceMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    dd35Mock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(10));
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var ukprn = 1;
            var learnStartDate = new DateTime(2019, 8, 1);
            var fundModel = 35;
            var lsdPostcode = "Postcode";
            var legalOrgType = "USDC";
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "105"
                },
            };
            var learningDelivery = new TestLearningDelivery
            {
                LearnStartDate = learnStartDate,
                FundModel = fundModel,
                LSDPostcode = lsdPostcode,
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery,
                    learningDelivery,
                }
            };

            var devolvedPostcodes = new List<IDevolvedPostcode>
            {
                new DevolvedPostcode
                {
                    Postcode = lsdPostcode,
                    EffectiveFrom = new DateTime(2019, 9, 1),
                    SourceOfFunding = "105"
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var organisationsDataServiceMock = new Mock<IOrganisationDataService>();
            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd35Mock = new Mock<IDerivedData_35Rule>();

            fileDataServiceMock.Setup(fm => fm.UKPRN()).Returns(ukprn);
            organisationsDataServiceMock.Setup(o => o.GetLegalOrgTypeForUkprn(ukprn)).Returns(legalOrgType);
            postcodeDataServiceMock.Setup(p => p.GetDevolvedPostcodes(lsdPostcode)).Returns(devolvedPostcodes);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.GetLearningDeliveryFAMsForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.SOF)).Returns(learningDeliveryFams);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_Code_001)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES)).Returns(true);
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(It.IsAny<ILearningDelivery>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandlerMock.Object,
                    fileDataServiceMock.Object,
                    postcodeDataServiceMock.Object,
                    organisationsDataServiceMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    dd35Mock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(0));
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learsStartDate = new DateTime(2019, 8, 1);
            var fundModel = 36;
            var lsdPostcode = "Postcode";
            var learnDelFamType = "SOF";
            var learnDelFamCode = "105";

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/08/2019")).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 36)).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, "Postcode")).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, "SOF")).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "105")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learsStartDate, fundModel, lsdPostcode, learnDelFamType, learnDelFamCode);

            validationErrorHandlerMock.Verify();
        }

        private LSDPostcode_02Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFileDataService fileDataService = null,
            IPostcodesDataService postcodesDataService = null,
            IOrganisationDataService organisationDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IDerivedData_35Rule dd35 = null)
        {
            return new LSDPostcode_02Rule(
                validationErrorHandler,
                fileDataService,
                postcodesDataService,
                organisationDataService,
                learningDeliveryFAMQueryService,
                dd35);
        }
    }
}
