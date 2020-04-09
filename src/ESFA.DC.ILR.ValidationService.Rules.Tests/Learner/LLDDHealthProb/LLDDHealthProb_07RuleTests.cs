using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_07RuleTests : AbstractRuleTests<LLDDHealthProb_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LLDDHealthProb_07");
        }

        [Fact]
        public void ValidatePasses_IrrelevantFundModelFound()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = 1,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = 10
                    },
                    new TestLearningDelivery
                    {
                        FundModel = 81
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_AgeExceptionApplies()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(26);

            var derivedDataMock = new Mock<IDerivedData_06Rule>();
            derivedDataMock.Setup(m => m.Derive(It.IsAny<IEnumerable<ILearningDelivery>>()))
                .Returns(new DateTime(2018, 9, 1));

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1993, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = 1,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = 10
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, derivedDataMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_PlannedHoursTooLow()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 5,
                LLDDHealthProb = 1,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = 10
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_IrrelevantHealthProblem()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = 2,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = 10
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_HealthProblemRecordFound()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = 1,
                LLDDAndHealthProblems = new List<TestLLDDAndHealthProblem>
                {
                    new TestLLDDAndHealthProblem()
                },
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = 10
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = 1,
                LLDDAndHealthProblems = null
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(99)]
        public void ValidateFails(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(20);

            var derivedDataMock = new Mock<IDerivedData_06Rule>();
            derivedDataMock.Setup(m => m.Derive(It.IsAny<IEnumerable<ILearningDelivery>>()))
                .Returns(new DateTime(2018, 9, 1));

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = 1,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = fundModel,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "SOF",
                                LearnDelFAMCode = "108"
                            }
                        }
                    }
                }
            };

            var testLearnerPasses = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = 1,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = fundModel,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "SOF",
                                LearnDelFAMCode = "108"
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        FundModel = 35
                    }
                }
            };

            var rule = NewRule(validationErrorHandlerMock.Object, derivedDataMock.Object, dateTimeServiceMock.Object);
            rule.Validate(testLearner);
            rule.Validate(testLearnerPasses);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private LLDDHealthProb_07Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDerivedData_06Rule dd06 = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            return new LLDDHealthProb_07Rule(dd06, dateTimeQueryService, validationErrorHandler);
        }
    }
}
