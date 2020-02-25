using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R105RuleTests : AbstractRuleTests<R105Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R105");
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, "2", "2018-01-01", "2018-07-31")]
        [InlineData("act", "2", "2018-01-01", "2018-07-31")]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, "1", "2018-07-01", "2018-09-01")]
        [InlineData("act", "1", "2018-07-01", "2018-09-01")]
        public void LearnDelFAMCodeConditionMet_False(string fAMType, string fAMCode, string dateFrom, string dateTo)
        {
            string learnDelFAMType;
            string learnDelFAMCode;
            DateTime? learnDelDateFrom = string.IsNullOrEmpty(dateFrom) ? (DateTime?)null : DateTime.Parse(dateFrom);
            DateTime? learnDelDateTo = string.IsNullOrEmpty(dateTo) ? (DateTime?)null : DateTime.Parse(dateTo);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2017, 09, 01),
                    LearnDelFAMDateToNullable = new DateTime(2017, 12, 31),
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = fAMType,
                    LearnDelFAMDateFromNullable = learnDelDateFrom,
                    LearnDelFAMDateToNullable = learnDelDateTo,
                    LearnDelFAMCode = fAMCode
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 08, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 12, 01),
                    LearnDelFAMCode = "1"
                }
            };

            NewRule().LearnDelFAMCodeConditionMet(learningDeliveryFAMs, out learnDelFAMType, out learnDelFAMCode).Should().BeFalse();
            learnDelFAMType.Should().BeEmpty();
            learnDelFAMCode.Should().BeEmpty();
        }

        [Theory]
        [InlineData("1", "2018-11-01", "2")]
        [InlineData("1", null, "2")]
        [InlineData("2", "2018-09-01", "2")]
        public void LearnDelFAMCodeConditionMet_True(string fAMCode, string dateTo, string learnDelFAMCodeExpected)
        {
            string learnDelFAMType;
            string learnDelFAMCode;
            DateTime? learnDelFAMDateTo = string.IsNullOrEmpty(dateTo) ? (DateTime?)null : DateTime.Parse(dateTo);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 01, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 07, 01),
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 07, 01),
                    LearnDelFAMDateToNullable = learnDelFAMDateTo,
                    LearnDelFAMCode = fAMCode
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 12, 01),
                    LearnDelFAMCode = learnDelFAMCodeExpected
                }
            };

            NewRule().LearnDelFAMCodeConditionMet(learningDeliveryFAMs, out learnDelFAMType, out learnDelFAMCode).Should().BeTrue();
            learnDelFAMType.Should().Be(LearningDeliveryFAMTypeConstants.ACT);
            learnDelFAMCode.Should().Be(learnDelFAMCodeExpected);
        }

        [Fact]
        public void Validate_Error()
        {
            string learnDelFAMCodeExpected = "2";
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 07, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 11, 01),
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 12, 01),
                    LearnDelFAMCode = "1"
                }
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "5A123",
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "5A124",
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                                LearnDelFAMDateFromNullable = new DateTime(2018, 11, 01),
                                LearnDelFAMDateToNullable = new DateTime(2018, 12, 01),
                                LearnDelFAMCode = learnDelFAMCodeExpected
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

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM)]
        public void Validate_NoError(string famType)
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "5A123",
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = famType,
                                LearnDelFAMDateFromNullable = new DateTime(2018, 08, 01),
                                LearnDelFAMDateToNullable = new DateTime(2018, 12, 01),
                                LearnDelFAMCode = "1"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "5A124",
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = famType,
                                LearnDelFAMDateFromNullable = new DateTime(2018, 01, 01),
                                LearnDelFAMDateToNullable = new DateTime(2018, 07, 01),
                                LearnDelFAMCode = "1"
                            }
                        }
                    }
                }
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
        public void Validate_NoError_NullCheck_LearningDelivery()
        {
            TestLearner testLearner = new TestLearner()
            {
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErroHandlerMock = new Mock<IValidationErrorHandler>();

            validationErroHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT)).Verifiable();
            validationErroHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")).Verifiable();

            NewRule(validationErrorHandler: validationErroHandlerMock.Object).BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.ACT, "1");
            validationErroHandlerMock.Verify();
        }

        public R105Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R105Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
