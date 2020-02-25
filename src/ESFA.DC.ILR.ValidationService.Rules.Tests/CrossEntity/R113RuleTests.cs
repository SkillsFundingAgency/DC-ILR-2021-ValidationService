using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R113RuleTests : AbstractRuleTests<R113Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R113");
        }

        [Fact]
        public void FundModelCondition_True()
        {
            NewRule().FundModelConditionMet(36).Should().BeTrue();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(70)]
        public void FundModelCondition_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeCondition_True()
        {
            NewRule().ProgTypeConditionMet(25).Should().BeTrue();
        }

        [Theory]
        [InlineData(20)]
        [InlineData(-8)]
        public void ProgTypeCondition_False(int progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(null, true)] // returns TRUE due to null
        [InlineData("2018-08-29", false)] // returns FALSE
        public void LearnActEndDate_ConditionMet(string strAchDate, bool expectedResult)
        {
            DateTime? achDate = string.IsNullOrEmpty(strAchDate) ? (DateTime?)null : DateTime.Parse(strAchDate);
            var result = NewRule().LearnActEndDateNotKnown(achDate);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ContractTypeConditionMet(bool expectedResult)
        {
            var testDelFAMs = new List<TestLearningDeliveryFAM>();
            var mockFAMQuerySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            mockFAMQuerySrvc.Setup(x => x.HasLearningDeliveryFAMType(testDelFAMs, "ACT")).Returns(expectedResult);

            NewRule(learningDeliveryFAMQueryService: mockFAMQuerySrvc.Object).ContractTypeConditionMet(testDelFAMs).Should().Be(expectedResult);

            mockFAMQuerySrvc.Verify(x => x.HasLearningDeliveryFAMType(testDelFAMs, "ACT"), Times.Exactly(1));
        }

        [Fact]
        public void FamDateCondition_True_AsDelFAMDateTo_Exists()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
               new TestLearningDeliveryFAM()
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMCode = "01",
                   LearnDelFAMDateToNullable = fAMDateTo,
                   LearnDelFAMDateFromNullable = fAMDateFrom
               }
            };

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs);
            result.Should().BeTrue();
        }

        [Fact]
        public void FamDateCondition_False_AsDelFAMDateTo_NotExisting()
        {
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
               new TestLearningDeliveryFAM()
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMCode = "01",
                   LearnDelFAMDateFromNullable = fAMDateFrom
               }
            };

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs);
            result.Should().BeFalse();
        }

        [Fact]
        public void FamDateCondition_False_AsLearnDeliveryIsNull()
        {
            List<TestLearningDeliveryFAM> learningDeliveryFAMs = null;

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs);
            result.Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_AsFAMDateTo_Exists()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMCode = "01",
                   LearnDelFAMDateFromNullable = fAMDateFrom,
                   LearnDelFAMDateToNullable = fAMDateTo
               }
            };

            var learnDelivery = new TestLearningDelivery
            {
                FundModel = 36,
                LearnActEndDateNullable = null,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"))
                               .Returns(true);

            var result = NewRule(mockFamQueryService.Object, null)
                            .ConditionMet(learnDelivery.FundModel, learnDelivery.LearnActEndDateNullable, learningDeliveryFAMs);

            result.Should().BeTrue();
            mockFamQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"), Times.Exactly(1));
        }

        [Fact]
        public void ConditionMet_False_As_ActEndDate_Exists()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();

            var learnDelivery = new TestLearningDelivery
            {
                FundModel = 36,
                LearnActEndDateNullable = new DateTime(2019, 07, 15),
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            var result = NewRule(mockFamQueryService.Object, null)
                            .ConditionMet(learnDelivery.FundModel, learnDelivery.LearnActEndDateNullable, learningDeliveryFAMs);

            result.Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AsFAMDateTo_NotExisting()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMCode = "01",
                   LearnDelFAMDateFromNullable = fAMDateFrom
               }
            };

            var learnDelivery = new TestLearningDelivery
            {
                FundModel = 36,
                LearnActEndDateNullable = null,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"))
                               .Returns(true);

            var result = NewRule(mockFamQueryService.Object, null)
                            .ConditionMet(learnDelivery.FundModel, learnDelivery.LearnActEndDateNullable, learningDeliveryFAMs);

            result.Should().BeFalse();
            mockFamQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"), Times.Exactly(1));
        }

        [Fact]
        public void ConditionMet_False_AsDeliveryFAMsList_Null()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();

            var learnDelivery = new TestLearningDelivery
            {
                FundModel = 36,
                LearnActEndDateNullable = null,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"))
                               .Returns(true);

            var result = NewRule(mockFamQueryService.Object, null)
                            .ConditionMet(learnDelivery.FundModel, learnDelivery.LearnActEndDateNullable, learningDeliveryFAMs);

            result.Should().BeFalse();
            mockFamQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"), Times.Exactly(1));
        }

        [Fact]
        public void Validate_NoError_HasActEndDate()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            var actEndDate = fAMDateTo;

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "01",
                    LearnDelFAMDateFromNullable = fAMDateFrom,
                    LearnDelFAMDateToNullable = fAMDateTo
                }
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "refNumber007",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = "00100325",
                        AimSeqNumber = 1,
                        FundModel = 36,
                        LearnActEndDateNullable = actEndDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                },
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"))
                               .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(mockFamQueryService.Object, validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AsNoFamDateTo()
        {
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "01",
                    LearnDelFAMDateFromNullable = fAMDateFrom
                }
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "refNumber007",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = "00100325",
                        AimSeqNumber = 1,
                        FundModel = 36,
                        LearnActEndDateNullable = null,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                },
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"))
                               .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(mockFamQueryService.Object, validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_DueToExclusion()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();

            var learner = new TestLearner
            {
                LearnRefNumber = "refNumber007",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = "00100325",
                        AimSeqNumber = 1,
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnActEndDateNullable = null,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                },
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"))
                               .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(mockFamQueryService.Object, validationErrorHandlerMock.Object)
                    .Validate(learner);
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
        public void Validate_Error_DueTo_FamDateTo()
        {
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "01",
                    LearnDelFAMDateFromNullable = fAMDateFrom,
                    LearnDelFAMDateToNullable = fAMDateFrom.AddDays(15)
                }
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "refNumber007",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = "00100325",
                        AimSeqNumber = 1,
                        FundModel = 36,
                        LearnActEndDateNullable = null,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                },
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"))
                               .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(mockFamQueryService.Object, validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "05/01/2018")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, "05/01/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(new DateTime(2018, 01, 05), LearningDeliveryFAMTypeConstants.ACT, new DateTime(2018, 01, 05));

            validationErrorHandlerMock.Verify();
        }

        public R113Rule NewRule(
                        ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
                        IValidationErrorHandler validationErrorHandler = null)
        {
            return new R113Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
