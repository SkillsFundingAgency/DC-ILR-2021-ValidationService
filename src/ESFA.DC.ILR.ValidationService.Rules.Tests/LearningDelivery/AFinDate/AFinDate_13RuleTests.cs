using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
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
    public class AFinDate_13RuleTests : AbstractRuleTests<AFinDate_13Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinDate_13");
        }

        [Fact]
        public void TNPRecordAfterDate_True()
        {
            NewRule().TNPRecordAfterDate(new DateTime(2019, 8, 1), new DateTime(2019, 8, 2)).Should().BeTrue();
        }

        [Fact]
        public void TNPRecordAfterDate_False_NullDate()
        {
            NewRule().TNPRecordAfterDate(null, new DateTime(2019, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void TNPRecordAfterDate_False()
        {
            NewRule().TNPRecordAfterDate(new DateTime(2019, 8, 1), new DateTime(2019, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void WithdrawReasonReturnedCondition_False()
        {
            NewRule().WithdrawReasonReturnedCondition(null).Should().BeFalse();
        }

        [Fact]
        public void WithdrawReasonReturnedCondition_True()
        {
            NewRule().WithdrawReasonReturnedCondition(1).Should().BeTrue();
        }

        [Fact]
        public void WAchDateReturnedCondition_False()
        {
            NewRule().AchDateReturnedCondition(null).Should().BeFalse();
        }

        [Fact]
        public void AchDateReturnedCondition_True()
        {
            NewRule().AchDateReturnedCondition(new DateTime(2019, 8, 1)).Should().BeTrue();
        }

        [Theory]
        [InlineData(24, true)]
        [InlineData(99, false)]
        [InlineData(null, false)]
        public void ApprenticeshipStandardProgrammeCondition_False(int? progType, bool mockValue)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(mockValue);

            NewRule(dd07Mock.Object).ApprenticeshipStandardProgrammeCondition(progType).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardProgrammeCondition_True()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(25)).Returns(true);

            NewRule(dd07Mock.Object).ApprenticeshipStandardProgrammeCondition(25).Should().BeTrue();
        }

        [Fact]
        public void LearnerWithdrawnCondition_True()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(25)).Returns(true);

            NewRule(dd07Mock.Object).LearnerWithdrawnCondition(25, null, 1, new DateTime(2019, 8, 1), new DateTime(2019, 8, 2)).Should().BeTrue();
        }

        [Fact]
        public void LearnerWithdrawnCondition_False_AchDate()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(25)).Returns(true);

            NewRule(dd07Mock.Object)
                .LearnerWithdrawnCondition(25, new DateTime(2019, 8, 1), 1, new DateTime(2019, 8, 1), new DateTime(2019, 8, 2))
                .Should().BeFalse();
        }

        [Theory]
        [InlineData(null, 1, 1, 2, false)]
        [InlineData(99, 1, 1, 2, false)]
        [InlineData(25, null, 1, 2, true)]
        [InlineData(25, 1, 2, 2, true)]
        [InlineData(25, 1, 2, 1, true)]
        public void LearnerWithdrawnCondition_False(int? progType, int? withdrawReason, int learnActEndDateDay, int aFinDateDay, bool mock)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(mock);

            NewRule(dd07Mock.Object)
                .LearnerWithdrawnCondition(progType, null, withdrawReason, new DateTime(2019, 8, learnActEndDateDay), new DateTime(2019, 8, aFinDateDay))
                .Should().BeFalse();
        }

        [Fact]
        public void LearnerAchievedCondition_True()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(25)).Returns(true);

            NewRule(dd07Mock.Object).LearnerAchievedCondition(25, new DateTime(2019, 8, 1), new DateTime(2019, 8, 2)).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, 1, 2, false)]
        [InlineData(99, 1, 2, false)]
        [InlineData(25, 2, 2, true)]
        [InlineData(25, 2, 1, true)]
        public void LearnerAchievedCondition_False(int? progType, int achDateDay, int aFinDateDay, bool mock)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(mock);

            NewRule(dd07Mock.Object)
                .LearnerAchievedCondition(progType, new DateTime(2019, 8, achDateDay), new DateTime(2019, 8, aFinDateDay))
                .Should().BeFalse();
        }

        [Fact]
        public void Validate_Error_Achieved()
        {
            var progType = 25;
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2019, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnActEndDateNullable = new DateTime(2017, 8, 1),
                        AchDateNullable = new DateTime(2017, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            var appFinRecordMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordMock.Setup(af => af.GetAppFinRecordsForType(It.IsAny<IEnumerable<IAppFinRecord>>(), "TNP")).Returns(appFinRecords);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, appFinRecordMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_Withdrawn()
        {
            var progType = 25;
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2019, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnActEndDateNullable = new DateTime(2017, 8, 1),
                        WithdrawReasonNullable = 1,
                        ProgTypeNullable = progType,
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            var appFinRecordMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordMock.Setup(af => af.GetAppFinRecordsForType(It.IsAny<IEnumerable<IAppFinRecord>>(), "TNP")).Returns(appFinRecords);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, appFinRecordMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullLearningDeliveries()
        {
            var learner = new TestLearner()
            {
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullAppFinRecords()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                    },
                    new TestLearningDelivery
                    {
                    }
                }
            };

            var appFinRecordMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordMock.Setup(af => af.GetAppFinRecordsForType(It.IsAny<IEnumerable<IAppFinRecord>>(), "TNP")).Returns(Enumerable.Empty<IAppFinRecord>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryService: appFinRecordMock.Object, validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppStandardProg()
        {
            var progType = 99;
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2019, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnActEndDateNullable = new DateTime(2017, 8, 1),
                        AchDateNullable = new DateTime(2017, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(false);

            var appFinRecordMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordMock.Setup(af => af.GetAppFinRecordsForType(It.IsAny<IEnumerable<IAppFinRecord>>(), "TNP")).Returns(appFinRecords);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, appFinRecordMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_DatesInRange()
        {
            var progType = 25;
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2019, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnActEndDateNullable = new DateTime(2019, 8, 1),
                        AchDateNullable = new DateTime(2019, 9, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            var appFinRecordMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordMock.Setup(af => af.GetAppFinRecordsForType(It.IsAny<IEnumerable<IAppFinRecord>>(), "TNP")).Returns(appFinRecords);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, appFinRecordMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AFinType()
        {
            var progType = 25;
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2019, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnActEndDateNullable = new DateTime(2017, 8, 1),
                        AchDateNullable = new DateTime(2017, 8, 1),
                        ProgTypeNullable = progType,
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            var appFinRecordMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordMock.Setup(af => af.GetAppFinRecordsForType(It.IsAny<IEnumerable<IAppFinRecord>>(), "TNP")).Returns(Enumerable.Empty<IAppFinRecord>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, appFinRecordMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinDate_13Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinDate_13Rule(dd07, appFinRecordQueryService, validationErrorHandler);
        }
    }
}
