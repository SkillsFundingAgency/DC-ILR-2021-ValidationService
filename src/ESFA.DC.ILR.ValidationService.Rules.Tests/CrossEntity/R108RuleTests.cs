using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R108RuleTests : AbstractRuleTests<R108Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R108");
        }

        [Theory]
        [InlineData(24)]
        [InlineData(25)]
        public void ProgTypeConditionMet_False(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(12)]
        [InlineData(null)]
        public void ProgTypeConditionMet_True(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(99)]
        [InlineData(36)]
        public void FundModelConditionMet_False(int fundModel)
        {
            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { FundModel = fundModel }
            };
            NewRule().FundModelConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(35)]
        [InlineData(70)]
        [InlineData(FundModels.OtherAdult)]
        public void FundModelConditionMet_True(int fundModel)
        {
            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { FundModel = fundModel }
            };
            NewRule().FundModelConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void AllAimsClosedConditionMet_False()
        {
            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { LearnActEndDateNullable = null },
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2018, 06, 01) }
            };
            NewRule().AllAimsClosedConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void AllAimsClosedConditionMet_True()
        {
            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2017, 05, 01) },
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2018, 06, 01) }
            };
            NewRule().AllAimsClosedConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void CompStatusConditionMet_False()
        {
            DateTime learnActEndDateExpected = new DateTime(2018, 06, 01);
            DateTime? learnActEndDate;

            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { LearnActEndDateNullable = null, CompStatus = 1 },
                new TestLearningDelivery() { LearnActEndDateNullable = learnActEndDateExpected, CompStatus = 6 }
            };
            NewRule().CompStatusConditionMet(learningDeliveries, out learnActEndDate).Should().BeFalse();
            learnActEndDate.Should().Be(learnActEndDateExpected);
        }

        [Fact]
        public void CompStatusConditionMet_True()
        {
            DateTime learnActEndDateExpected = new DateTime(2018, 06, 01);
            DateTime? learnActEndDate;

            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2017, 05, 02), CompStatus = 1 },
                new TestLearningDelivery() { LearnActEndDateNullable = learnActEndDateExpected }
            };
            NewRule().CompStatusConditionMet(learningDeliveries, out learnActEndDate).Should().BeTrue();
            learnActEndDate.Should().Be(learnActEndDateExpected);
        }

        [Fact]
        public void FilePreparationDateConditionMet_False()
        {
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(fds => fds.FilePreparationDate()).Returns(new DateTime(2018, 03, 01));
            NewRule(fileDataService: fileDataServiceMock.Object).FilePreparationDateConditionMet(new DateTime(2018, 06, 01)).Should().BeFalse();
        }

        [Fact]
        public void FilePreparationDateConditionMet_True()
        {
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(fds => fds.FilePreparationDate()).Returns(DateTime.Now.AddMonths(4));
            NewRule(fileDataService: fileDataServiceMock.Object).FilePreparationDateConditionMet(DateTime.Now).Should().BeTrue();
        }

        [Fact]
        public void DPOutComeConditionMet_False()
        {
            string learnRefNumber = "00100310";

            var learnerDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                LearnRefNumber = "00100310",
                DPOutcomes = new[]
                {
                    new TestDPOutcome() { OutStartDate = new DateTime(2018, 09, 01) },
                    new TestDPOutcome() { OutStartDate = new DateTime(2012, 01, 01) }
                }
            };

            NewRule().DPOutComeConditionMet(
                learnRefNumber,
                new List<TestLearnerDestinationAndProgression> { learnerDestinationAndProgression },
                new DateTime(2018, 06, 01)).Should().BeFalse();
        }

        [Theory]
        [InlineData("00100309")]
        [InlineData("00100310")]
        public void DPOutComeConditionMet_True(string learnRefNumber)
        {
            var learnerDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                LearnRefNumber = "00100309",
                DPOutcomes = new[]
                {
                    new TestDPOutcome() { OutStartDate = new DateTime(2017, 05, 01) },
                    new TestDPOutcome() { OutStartDate = new DateTime(2012, 01, 01) }
                }
            };

            string ldapLearnRefNumberExpectedValue =
                learnRefNumber == learnerDestinationAndProgression.LearnRefNumber
                ? learnRefNumber
                : string.Empty;

            NewRule()
                .DPOutComeConditionMet(
                learnRefNumber,
                new List<TestLearnerDestinationAndProgression> { learnerDestinationAndProgression },
                new DateTime(2018, 06, 01)).Should().BeTrue();
        }

        [Fact]
        public void DPOutComeConditionMet_True_DPOutComeNullCheck()
        {
            string learnRefNumber = "00100309";

            var learnerDestinationAndProgression = new List<TestLearnerDestinationAndProgression>()
            {
                new TestLearnerDestinationAndProgression()
                {
                    LearnRefNumber = "00100309",
                    DPOutcomes = null
                },
                new TestLearnerDestinationAndProgression()
                {
                    LearnRefNumber = "00100310",
                    DPOutcomes = null
                }
            };

            NewRule()
                .DPOutComeConditionMet(
                learnRefNumber,
                learnerDestinationAndProgression,
                new DateTime(2018, 06, 01)).Should().BeTrue();
        }

        [Fact]
        public void DPOutComeConditionMet_True_NullCheck()
        {
            string learnRefNumber = "00100309";

            List<TestLearnerDestinationAndProgression> learnerDestinationAndProgression = null;

            NewRule()
                .DPOutComeConditionMet(
                learnRefNumber,
                learnerDestinationAndProgression,
                new DateTime(2018, 06, 01)).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learners = new TestLearner[]
            {
                new TestLearner()
                {
                    LearnRefNumber = "00100308",
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery()
                        {
                            ProgTypeNullable = 24,
                            FundModel = 35,
                            LearnActEndDateNullable = new DateTime(2017, 05, 01),
                            CompStatus = 1
                        },
                        new TestLearningDelivery()
                        {
                            ProgTypeNullable = null,
                            FundModel = 70,
                            LearnActEndDateNullable = new DateTime(2018, 06, 01),
                            CompStatus = 2
                        }
                    }
                },
                new TestLearner()
                {
                    LearnRefNumber = "00100309",
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery()
                        {
                            ProgTypeNullable = 12,
                            FundModel = 35,
                            LearnActEndDateNullable = new DateTime(2017, 05, 01),
                            CompStatus = 1
                        },
                        new TestLearningDelivery()
                        {
                            ProgTypeNullable = null,
                            FundModel = 70,
                            LearnActEndDateNullable = new DateTime(2018, 06, 01),
                            CompStatus = 2
                        }
                    }
                }
            };

            var learnerDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                LearnRefNumber = "00100309",
                DPOutcomes = new[]
                {
                    new TestDPOutcome() { OutStartDate = new DateTime(2017, 05, 01) },
                    new TestDPOutcome() { OutStartDate = new DateTime(2012, 01, 01) }
                }
            };

            var message = new TestMessage()
            {
                Learners = learners,
                LearnerDestinationAndProgressions = new TestLearnerDestinationAndProgression[]
                {
                    learnerDestinationAndProgression
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(fds => fds.FilePreparationDate()).Returns(DateTime.Now.AddMonths(4));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    fileDataService: fileDataServiceMock.Object).Validate(message);
            }
        }

        [Theory]
        [InlineData(25, 36, 2, "2018-06-01", "2018-06-01")]
        [InlineData(24, 99, 3, "2018-06-02", "2018-06-02")]
        [InlineData(2, 35, 2, "2018-06-02", "2018-06-02")]
        [InlineData(3, 70, 3, "2018-06-02", "2018-06-02")]
        [InlineData(2, 36, 6, "2018-06-02", "2018-06-02")]
        [InlineData(3, 99, 6, "2018-06-02", "2018-06-02")]
        [InlineData(25, 35, 6, null, null)]
        [InlineData(24, 70, 6, null, "2018-06-02")]
        [InlineData(24, 70, 6, "2018-06-02", null)]
        public void Validate_NoError(int? progType, int fundModel, int compStatus, string learnActEndDate1, string learnActEndDate2)
        {
            DateTime? learnActEndDateLD1 = string.IsNullOrEmpty(learnActEndDate1) ? (DateTime?)null : DateTime.Parse(learnActEndDate1);
            DateTime? learnActEndDateLD2 = string.IsNullOrEmpty(learnActEndDate2) ? (DateTime?)null : DateTime.Parse(learnActEndDate2);

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 3,
                        FundModel = fundModel,
                        LearnActEndDateNullable = learnActEndDateLD1,
                        CompStatus = compStatus
                    },
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        FundModel = fundModel,
                        LearnActEndDateNullable = learnActEndDateLD2,
                        CompStatus = compStatus
                    }
                }
            };

            var learnerDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                LearnRefNumber = "00100310",
                DPOutcomes = new[]
                {
                    new TestDPOutcome() { OutStartDate = new DateTime(2018, 09, 01) },
                    new TestDPOutcome() { OutStartDate = new DateTime(2012, 01, 01) }
                }
            };

            var message = new TestMessage()
            {
                Learners = new TestLearner[] { learner },
                LearnerDestinationAndProgressions = new TestLearnerDestinationAndProgression[] { learnerDestinationAndProgression }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(fds => fds.FilePreparationDate()).Returns(new DateTime(2018, 03, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    fileDataService: fileDataServiceMock.Object).Validate(message);
            }
        }

        public R108Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFileDataService fileDataService = null)
        {
            return new R108Rule(validationErrorHandler: validationErrorHandler, fileDataService: fileDataService);
        }
    }
}
