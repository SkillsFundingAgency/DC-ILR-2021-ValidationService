using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R119RuleTests : AbstractRuleTests<R119Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R119");
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2019, 01, 31)).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2019, 02, 01)).Should().BeTrue();
        }

        [Theory]
        [InlineData("PMR", 2, "2018-07-01", "2018-08-01")]
        [InlineData("TNP", 1, "2018-07-01", "2018-08-01")]
        public void AppFinRecodConditionMet_NoMatch(string aFinType, int aFinCode, string learnStartDateString, string aFinDateString)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            var appFinRecords = new TestAppFinRecord[]
                {
                    new TestAppFinRecord()
                    {
                         AFinType = aFinType,
                         AFinCode = aFinCode,
                         AFinDate = DateTime.Parse(aFinDateString)
                    },
                    null
                };

            NewRule().AppFinRecordsConditionMet(appFinRecords, learnStartDate).Should().BeNullOrEmpty();
        }

        [Fact]
        public void AppFinRecodConditionMet_Matched()
        {
            DateTime learnStartDate = new DateTime(2018, 11, 01);
            TestAppFinRecord testAppFinRecord1 = new TestAppFinRecord()
            {
                AFinType = "TNP",
                AFinCode = 1,
                AFinDate = new DateTime(2018, 07, 01)
            };
            TestAppFinRecord testAppFinRecord2 = new TestAppFinRecord()
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2017, 01, 01)
            };

            var appFinRecordsExpected = new TestAppFinRecord[]
                {
                    testAppFinRecord1,
                    testAppFinRecord2
                };

            var appFinRecords = new TestAppFinRecord[]
                {
                    testAppFinRecord1,
                    null,
                    testAppFinRecord2
                };

            NewRule().AppFinRecordsConditionMet(appFinRecords, learnStartDate).Should().BeEquivalentTo(appFinRecordsExpected);
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A1234",
                        LearnStartDate = new DateTime(2019, 02, 28),
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                 AFinType = "TNP",
                                 AFinCode = 2,
                                 AFinDate = new DateTime(2019, 02, 01)
                            },
                            null,
                            new TestAppFinRecord()
                            {
                                 AFinType = "TNP",
                                 AFinCode = 3,
                                 AFinDate = new DateTime(2019, 02, 21)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                         LearnAimRef = "1A1234",
                         LearnStartDate = new DateTime(2019, 02, 01),
                         AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                 AFinType = "TNP",
                                 AFinCode = 5,
                                 AFinDate = new DateTime(2019, 01, 31)
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A1234",
                        LearnStartDate = new DateTime(2019, 01, 31),
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                 AFinType = "PMR",
                                 AFinCode = 1,
                                 AFinDate = new DateTime(2019, 02, 01)
                            },
                            null
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "2A1234",
                        LearnStartDate = new DateTime(2019, 02, 02),
                        AppFinRecords = new TestAppFinRecord[]
                        {
                            new TestAppFinRecord()
                            {
                                 AFinType = "PMR",
                                 AFinCode = 2,
                                 AFinDate = new DateTime(2019, 02, 01)
                            },
                            null
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "3B456789",
                        LearnStartDate = new DateTime(2019, 02, 25),
                        FundModel = 10
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "2B456789",
                        FundModel = 10,
                        AppFinRecords = null
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearningDeliveryNullCheck()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NullCheck()
        {
            TestLearner testLearner = null;

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("LearnStartDate", "01/07/2018")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinType", "PNR")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinCode", 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinDate", "01/11/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(
                new DateTime(2018, 07, 01),
                "PNR",
                1,
                new DateTime(2018, 11, 01));
            validationErrorHandlerMock.Verify();
        }

        public R119Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R119Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
