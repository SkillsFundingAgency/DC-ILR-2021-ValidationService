using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
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
    public class UKPRN_09RuleTests : AbstractRuleTests<UKPRN_09Rule>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int> { 36, 99 };
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.APPS2021
        };

        private readonly HashSet<string> _learnDelFamCodes = new HashSet<string> { "034", "353", "354", "355" };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_09");
        }

        [Fact]
        public void DD07ConditionMet_False_Null()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(null)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var progType = 24;
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "357"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 24;
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "357"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
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

        [Theory]
        [InlineData(null)]
        [InlineData("2020-08-01")]
        public void LearnActEndDateConditionMet_True(string learnActEndDateString)
        {
            DateTime? learnActEndDate = learnActEndDateString != null ? DateTime.Parse(learnActEndDateString) : (DateTime?)null;

            DateTime academicYear = new DateTime(2018, 8, 1);

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            if (learnActEndDate != null)
            {
                academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);
            }

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).LearnActEndDateConditionMet(learnActEndDate, academicYear).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateConditionMet_False()
        {
            DateTime? learnActEndDate = new DateTime(2020, 5, 1);
            DateTime academicYear = new DateTime(2020, 8, 1);

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(true);

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).LearnActEndDateConditionMet(learnActEndDate, academicYear).Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, "035")]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL, "034")]
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

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, learnDelFamType, _learnDelFamCodes)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False_Null()
        {
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(null, LearningDeliveryFAMTypeConstants.LDM, _learnDelFamCodes)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(null).Should().BeTrue();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, "034")]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, "353")]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, "354")]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, "355")]
        public void LearningDeliveryFAMsConditionMet_False(string learnDelFamType, string learnDelFamCode)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = learnDelFamType,
                    LearnDelFAMCode = learnDelFamCode
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = learnDelFamType,
                    LearnDelFAMCode = learnDelFamCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, learnDelFamType, _learnDelFamCodes)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            DateTime academicYear = new DateTime(2020, 8, 1);
            DateTime startDate = new DateTime(2020, 8, 1);
            DateTime? learnActEndDate = new DateTime(2020, 8, 1);
            int? progType = 24;
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

            var rule = NewRuleMock();

            rule.Setup(r => r.LearnActEndDateConditionMet(learnActEndDate, academicYear)).Returns(true);
            rule.Setup(r => r.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(true);
            rule.Setup(r => r.FCTFundingConditionMet()).Returns(true);
            rule.Setup(r => r.DD07ConditionMet(progType)).Returns(true);

            rule.Object.ConditionMet(progType, academicYear, learnActEndDate, learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true, true, true)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, false, true)]
        [InlineData(true, true, true, false)]
        [InlineData(false, false, false, true)]
        [InlineData(true, true, false, false)]
        [InlineData(false, false, true, true)]
        [InlineData(true, false, false, true)]
        [InlineData(false, false, false, false)]
        public void ConditionMet_False(bool condition1, bool condition2, bool condition3, bool condition4)
        {
            DateTime academicYear = new DateTime(2020, 8, 1);
            DateTime startDate = new DateTime(2020, 8, 1);
            DateTime? learnActEndDate = new DateTime(2020, 8, 1);
            int? progType = 24;
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

            var rule = NewRuleMock();

            rule.Setup(r => r.LearnActEndDateConditionMet(learnActEndDate, academicYear)).Returns(condition1);
            rule.Setup(r => r.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(condition2);
            rule.Setup(r => r.FCTFundingConditionMet()).Returns(condition3);
            rule.Setup(r => r.DD07ConditionMet(progType)).Returns(condition4);

            rule.Object.ConditionMet(progType, academicYear, learnActEndDate, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            int ukprn = 12345678;
            DateTime academicYear = new DateTime(2020, 8, 1);
            DateTime startDate = new DateTime(2020, 8, 1);
            DateTime? learnActEndDate = new DateTime(2020, 8, 1);
            int? progType = 24;

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 81,
                        ProgTypeNullable = progType,
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = "033"
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

            var fileDataServiceMock = new Mock<IFileDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            if (learnActEndDate != null)
            {
                academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);
            }

            fileDataServiceMock.Setup(ds => ds.UKPRN()).Returns(ukprn);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(startDate);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, "LDM", _learnDelFamCodes)).Returns(false);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    academicYearDataServiceMock.Object,
                    academicYearQueryServiceMock.Object,
                    fcsDataServiceMock.Object,
                    dd07Mock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            int ukprn = 12345678;
            DateTime academicYear = new DateTime(2020, 8, 1);
            DateTime startDate = new DateTime(2020, 8, 1);
            DateTime? learnActEndDate = new DateTime(2020, 8, 1);
            int? progType = 24;

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 81,
                        ProgTypeNullable = progType,
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = "033"
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

            var fileDataServiceMock = new Mock<IFileDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            if (learnActEndDate != null)
            {
                academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);
            }

            fileDataServiceMock.Setup(ds => ds.UKPRN()).Returns(ukprn);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(startDate);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, "LDM", _learnDelFamCodes)).Returns(false);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    academicYearDataServiceMock.Object,
                    academicYearQueryServiceMock.Object,
                    fcsDataServiceMock.Object,
                    dd07Mock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_FundModel()
        {
            int ukprn = 12345678;
            DateTime academicYear = new DateTime(2020, 8, 1);
            DateTime startDate = new DateTime(2020, 8, 1);
            DateTime? learnActEndDate = new DateTime(2020, 8, 1);
            int? progType = 24;

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 99,
                        ProgTypeNullable = progType,
                        LearnStartDate = startDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = "033"
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

            var fileDataServiceMock = new Mock<IFileDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();

            if (learnActEndDate != null)
            {
                academicYearQueryServiceMock.Setup(qs => qs.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYear)).Returns(false);
            }

            fileDataServiceMock.Setup(ds => ds.UKPRN()).Returns(ukprn);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(startDate);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, "LDM", _learnDelFamCodes)).Returns(false);
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    academicYearDataServiceMock.Object,
                    academicYearQueryServiceMock.Object,
                    fcsDataServiceMock.Object,
                    dd07Mock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var fundModel = 81;
            int? progType = 24;
            var ukprn = 12345678;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(ukprn, fundModel, progType);

            validationErrorHandlerMock.Verify();
        }

        private UKPRN_09Rule NewRule(
            IFileDataService fileDataService = null,
            IAcademicYearDataService academicYearDataService = null,
            IAcademicYearQueryService academicYearQueryService = null,
            IFCSDataService fCSDataService = null,
            IDerivedData_07Rule dd07 = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_09Rule(fileDataService, academicYearDataService, academicYearQueryService, fCSDataService, dd07, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
