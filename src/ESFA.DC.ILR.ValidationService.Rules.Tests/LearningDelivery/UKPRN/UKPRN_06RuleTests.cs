using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    public class UKPRN_06RuleTests : AbstractRuleTests<UKPRN_06Rule>
    {
        private readonly int _fundModel = TypeOfFunding.AdultSkills;
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.AEBC_19TRN1920,
            FundingStreamPeriodCodeConstants.AEBC_ASCL1920,
            FundingStreamPeriodCodeConstants.AEB_19TRLS1920,
            FundingStreamPeriodCodeConstants.AEB_ASLS1920,
            FundingStreamPeriodCodeConstants.AEB_19TRN1920,
            FundingStreamPeriodCodeConstants.AEB_AS1920
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_06");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(_fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(36)]
        [InlineData(99)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, "035")]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL, "034")]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL, "357")]
        public void LearningDeliveryFAMsConditionMet_True(string learnDelFamType, string learnDelFamCode)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = learnDelFamType,
                    LearnDelFAMCode = learnDelFamCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, learnDelFamType, learnDelFamCode)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False_Null()
        {
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(null, LearningDeliveryFAMTypeConstants.LDM, "034")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(null, LearningDeliveryFAMTypeConstants.LDM, "357")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(null).Should().BeTrue();
        }

        [Theory]
        [InlineData("357")]
        [InlineData("034")]
        public void LearningDeliveryFAMsConditionMet_False(string famCode)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, famCode)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 24;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var progType = 25;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void DD35ConditionMet_True()
        {
            var learnDelivery = new TestLearningDelivery();

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learnDelivery)).Returns(false);

            NewRule(dd35: dd35Mock.Object).DD35ConditionMet(learnDelivery).Should().BeTrue();
        }

        [Fact]
        public void DD35ConditionMet_False()
        {
            var learnDelivery = new TestLearningDelivery();

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learnDelivery)).Returns(true);

            NewRule(dd35: dd35Mock.Object).DD35ConditionMet(learnDelivery).Should().BeFalse();
        }

        [Fact]
        public void FCTFundingConditionMet_True()
        {
            IEnumerable<string> fundingStreamPeriodCodes = new HashSet<string>() { "ABC" };

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.FundingRelationshipFCTExists(fundingStreamPeriodCodes)).Returns(false);

            NewRule(fCSDataService: fcsDataServiceMock.Object).FCTFundingConditionMet().Should().BeTrue();
        }

        [Fact]
        public void FCTFundingConditionMet_False()
        {
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(true);

            NewRule(fCSDataService: fcsDataServiceMock.Object).FCTFundingConditionMet().Should().BeFalse();
        }

        [Fact]
        public void LearnActEndDateConditionMet_True()
        {
            DateTime learnActEndDate = new DateTime(2018, 08, 01);

            DateTime academicYear = new DateTime(2018, 8, 1);

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate, academicYear)).Returns(false);

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object)
                .LearnActEndDateConditionMet(learnActEndDate, academicYear).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateConditionMet_False()
        {
            DateTime learnActEndDate = new DateTime(2018, 5, 1);
            DateTime academicYear = new DateTime(2018, 8, 1);

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate, academicYear)).Returns(true);

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).LearnActEndDateConditionMet(learnActEndDate, academicYear).Should().BeFalse();
        }

        [Theory]
        [InlineData(24)]
        [InlineData(null)]
        public void ConditionMet_True(int? progType)
        {
            var fundModel = _fundModel;
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime learnActEndDate = new DateTime(2018, 8, 1);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "034"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "357"
                }
            };

            var learnDelivery = new TestLearningDelivery
            {
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var rule = NewRuleMock();

            rule.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            rule.Setup(r => r.LearnActEndDateConditionMet(learnActEndDate, academicYear)).Returns(true);
            rule.Setup(r => r.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(true);
            rule.Setup(r => r.FCTFundingConditionMet()).Returns(true);
            rule.Setup(r => r.DD07ConditionMet(24)).Returns(true);
            rule.Setup(r => r.DD35ConditionMet(learnDelivery)).Returns(true);

            rule.Object.ConditionMet(fundModel, progType, academicYear, learnActEndDate, learnDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true, true, true, true)]
        [InlineData(true, false, true, true, true)]
        [InlineData(true, true, false, true, true)]
        [InlineData(true, true, true, false, true)]
        [InlineData(true, true, true, true, false)]
        [InlineData(false, false, false, false, true)]
        [InlineData(false, true, true, false, false)]
        [InlineData(false, false, false, true, true)]
        [InlineData(false, true, false, false, true)]
        [InlineData(true, true, true, false, false)]
        [InlineData(true, false, false, true, true)]
        [InlineData(true, true, false, false, true)]
        [InlineData(true, true, false, false, false)]
        [InlineData(false, false, false, false, false)]
        public void ConditionMet_False(bool condition1, bool condition2, bool condition3, bool condition4, bool condition5)
        {
            var fundModel = _fundModel;
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime learnActEndDate = new DateTime(2018, 8, 1);
            int progType = 24;
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "034"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "357"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                    LearnDelFAMCode = "110"
                }
            };

            var learnDelivery = new TestLearningDelivery
            {
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var rule = NewRuleMock();

            rule.Setup(r => r.FundModelConditionMet(fundModel)).Returns(condition1);
            rule.Setup(r => r.LearnActEndDateConditionMet(learnActEndDate, academicYear)).Returns(condition2);
            rule.Setup(r => r.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(condition3);
            rule.Setup(r => r.FCTFundingConditionMet()).Returns(condition4);
            rule.Setup(r => r.DD07ConditionMet(progType)).Returns(condition5);
            rule.Setup(r => r.DD35ConditionMet(learnDelivery)).Returns(false);

            rule.Object.ConditionMet(fundModel, progType, academicYear, learnActEndDate, learnDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(25, null, null, true)]
        [InlineData(35, null, null, false)]
        [InlineData(35, 24, null, false)]
        public void ConditionMet_False_NullValuesCheck(int fundModel, int? progType, string learnActEndDateString, bool excludeLearningDeliveryFAMs)
        {
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            var learningDeliveryFAMs = excludeLearningDeliveryFAMs ? null : new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "034"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "357"
                }
            };

            var learnDelivery = new TestLearningDelivery
            {
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dd35Mock = new Mock<IDerivedData_35Rule>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learnDelivery)).Returns(false);
            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(new DateTime(2018, 07, 01), academicYear)).Returns(false);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(true);

            NewRule(
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object,
                dd07: dd07Mock.Object,
                dd35: dd35Mock.Object,
                academicYearQueryService: academicYearQueryServiceMock.Object,
                fCSDataService: fcsDataServiceMock.Object)
                .ConditionMet(fundModel, progType, academicYear, learnActEndDate, learnDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var fundModel = _fundModel;
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = new DateTime(2018, 8, 1);
            int? progType = 24;

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = "034"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = "357"
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dd35Mock = new Mock<IDerivedData_35Rule>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(startDate);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    learningDeliveryFAMQueryServiceMock.Object,
                    fcsDataServiceMock.Object,
                    dd07Mock.Object,
                    dd35Mock.Object,
                    academicYearDataServiceMock.Object,
                    academicYearQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            DateTime academicYear = new DateTime(2018, 8, 1);
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime? learnActEndDate = new DateTime(2018, 8, 1);
            int? progType = 24;

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = _fundModel,
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = "034"
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = "357"
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dd35Mock = new Mock<IDerivedData_35Rule>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(startDate);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    learningDeliveryFAMQueryServiceMock.Object,
                    fcsDataServiceMock.Object,
                    dd07Mock.Object,
                    dd35Mock.Object,
                    academicYearDataServiceMock.Object,
                    academicYearQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, _fundModel)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(_fundModel);

            validationErrorHandlerMock.Verify();
        }

        private UKPRN_06Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IFCSDataService fCSDataService = null,
            IDerivedData_07Rule dd07 = null,
            IDerivedData_35Rule dd35 = null,
            IAcademicYearDataService academicYearDataService = null,
            IAcademicYearQueryService academicYearQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_06Rule(learningDeliveryFAMQueryService, fCSDataService, dd07, dd35, academicYearDataService, academicYearQueryService, validationErrorHandler);
        }
    }
}
