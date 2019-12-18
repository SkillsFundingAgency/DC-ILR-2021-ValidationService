using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinDate
{
    public class AFinDate_04RuleTests : AbstractRuleTests<AFinDate_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinDate_04");
        }

        [Theory]
        [InlineData(36, 35)]
        [InlineData(25, 25)]
        [InlineData(25, 36)]
        public void Exclusion_False(int fundModel, int? progType)
        {
            NewRule().Exclusion(fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void Exclusion_True()
        {
            NewRule().Exclusion(36, 25).Should().BeTrue();
        }

        [Theory]
        [InlineData(99, 1, false)]
        [InlineData(99, 99, false)]
        [InlineData(25, 99, true)]
        public void ApprenticeshipFrameworkProgrammeFilter_False(int? progType, int aimType, bool mockValue)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(mockValue);

            NewRule(dd07Mock.Object).ApprenticeshipFrameworkProgrammeFilter(progType, aimType).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipFrameworkProgrammeFilter_True()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(25)).Returns(true);

            NewRule(dd07Mock.Object).ApprenticeshipFrameworkProgrammeFilter(25, 1).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateIsKnown_True()
        {
            NewRule().LearnActEndDateIsKnown(new DateTime(2018, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateIsKnown_False()
        {
            NewRule().LearnActEndDateIsKnown(null).Should().BeFalse();
        }

        [Fact]
        public void TNPRecordAfterLearnActEndDate_Returns()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNPRecordAfterLearnActEndDate(new DateTime(2018, 8, 1), appFinRecords).Should().BeEquivalentTo(appFinRecords[0]);
        }

        [Fact]
        public void TNPRecordAfterLearnActEndDate_Multiple_Returns()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNPRecordAfterLearnActEndDate(new DateTime(2018, 8, 1), appFinRecords).Should().BeEquivalentTo(appFinRecords[0]);
        }

        [Fact]
        public void TNPRecordAfterLearnActEndDate_Null_DateMisMatch()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 8, 1)
                }
            };

            NewRule().TNPRecordAfterLearnActEndDate(new DateTime(2018, 9, 1), appFinRecords).Should().BeNull();
        }

        [Fact]
        public void TNPRecordAfterLearnActEndDate_Null_AFinTypeMisMatch()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNPRecordAfterLearnActEndDate(new DateTime(2018, 8, 1), appFinRecords).Should().BeNull();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2018, 8, 1),
                        FundModel = 35,
                        ProgTypeNullable = 25,
                        AimType = 1,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(It.IsAny<int?>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullLearnActEndDate()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        ProgTypeNullable = 25,
                        AimType = 1,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(It.IsAny<int?>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullAppFinRecords()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        ProgTypeNullable = 25,
                        AimType = 1,
                        LearnActEndDateNullable = new DateTime(2018, 8, 1)
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(It.IsAny<int?>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_Exclusion()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2018, 8, 1),
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        AimType = 1,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(It.IsAny<int?>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoMatchingDeliveries()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 99,
                        ProgTypeNullable = 25,
                        AimType = 1,
                        LearnActEndDateNullable = new DateTime(2018, 8, 1)
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(It.IsAny<int?>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinDate_04Rule NewRule(IDerivedData_07Rule dd07 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinDate_04Rule(dd07, validationErrorHandler);
        }
    }
}
