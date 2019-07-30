using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_40RuleTests : AbstractRuleTests<DateOfBirth_40Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_40");
        }

        [Theory]
        [InlineData(365)]
        [InlineData(370)]
        public void ValidatePasses_AsMinDuration_GreaterThan365(int totalDays)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(totalDays);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 35,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2017, 8, 10)
                    }
                }
            };

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);

            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidateFails_AsMinDuration_LessThan365()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT"
                }
            };

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 35,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = null,
                        OutcomeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        FundModel = 35,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1),
                        OutcomeNullable = 1,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES"))
                .Returns(false);

            dateTimeServiceMock.Setup(m => m.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(100);

            dateTimeServiceMock.Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            dateTimeServiceMock.Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(0);

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(81)]
        public void ValidatePasses_OutsideAgeRange(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(18);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1)
                    }
                }
            };

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePassesIrrelevantFundingModel()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.CommunityLearning,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1)
                    }
                }
            };

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(81)]
        public void ValidatePassesIrrelevantProgType(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = 1,
                        ProgTypeNullable = 20,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1)
                    }
                }
            };

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(81)]
        public void ValidatePassesIrrelevantAimType(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = 4,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1)
                    }
                }
            };

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(81)]
        public void ValidatePassesStartDateAfterCutOff(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2018, 7, 31),
                        LearnActEndDateNullable = new DateTime(2018, 9, 1)
                    }
                }
            };

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(81)]
        public void ValidatePassesRestartException(int fundModel)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "RES"
                }
            };

            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var fAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();

            fAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "RES"
                            }
                        }
                    }
                }
            };

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                learningDeliveryFAMQueryService: fAMsQueryServiceMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(81)]
        public void ValidatePassesNoCourseEndDate(int fundModel)
        {
            TestLearningDeliveryFAM[] learningDeliveryFAMs = null;
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        OutcomeNullable = 1,
                        LearnActEndDateNullable = null
                    }
                }
            };

            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(81)]
        public void ValidatePassesCourse12Months(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);
            dateTimeServiceMock
                .Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(1);

            TestLearningDeliveryFAM[] learningDeliveryFAMs = null;
            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2017, 8, 31),
                        OutcomeNullable = 1
                    }
                }
            };

            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearnerFAMs = new List<TestLearnerFAM>()
            };

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(81)]
        public void ValidateFails(int fundModel)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT"
                }
            };

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1)
                    },
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = null,
                        OutcomeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = 1,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1),
                        OutcomeNullable = 1,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            dateTimeServiceMock
                .Setup(m => m.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(100);
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);
            dateTimeServiceMock
                .Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(0);

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                dateTimeQueryService: dateTimeServiceMock.Object)
                .Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private DateOfBirth_40Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            return new DateOfBirth_40Rule(
                dateTimeQueryService: dateTimeQueryService,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService,
                validationErrorHandler: validationErrorHandler);
        }
    }
}
