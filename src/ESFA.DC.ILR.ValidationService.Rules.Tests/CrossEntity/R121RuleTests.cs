using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
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
    public class R121RuleTests : AbstractRuleTests<R121Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R121");
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
        [InlineData(true)]
        [InlineData(false)]
        public void FamTypeConditionMet_True(bool expectedResult)
        {
            var deliveryFAMs = new List<ILearningDeliveryFAM>();
            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(deliveryFAMs, "ACT"))
                               .Returns(expectedResult);

            var rule = NewRule(mockFamQueryService.Object, null).FAMTypeConditionMet(deliveryFAMs);

            rule.Should().Be(expectedResult);
            mockFamQueryService.Verify(x => x.HasLearningDeliveryFAMType(deliveryFAMs, "ACT"), Times.Exactly(1));
        }

        [Theory]
        [InlineData(null, false)] // returns FALSE due to null
        [InlineData("2018-08-28", true)] // returns TRUE
        public void AchDateConditionMet(string strAchDate, bool expectedResult)
        {
            DateTime? achDate = string.IsNullOrEmpty(strAchDate) ? (DateTime?)null : DateTime.Parse(strAchDate);
            var result = NewRule().AchDateIsKnown(achDate);

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void FamDateCondition_True_AsActEndDateNotEqual()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            var actEndDate = fAMDateTo.AddDays(5);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "RES", LearnDelFAMCode = "02", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "act", LearnDelFAMCode = "03", LearnDelFAMDateToNullable = fAMDateTo.AddDays(10), LearnDelFAMDateFromNullable = fAMDateFrom }
            };

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs, actEndDate);
            result.Should().BeTrue();
        }

        [Fact]
        public void FamDateCondition_False_AsActEndDateIsEqual()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            var actEndDate = fAMDateTo;

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "RES", LearnDelFAMCode = "02", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "act", LearnDelFAMCode = "03", LearnDelFAMDateToNullable = fAMDateTo.AddDays(10), LearnDelFAMDateFromNullable = fAMDateFrom }
            };

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs, actEndDate);
            result.Should().BeFalse();
        }

        [Fact]
        public void FamDateCondition_False_AsFoundNull()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            var actEndDate = fAMDateTo.AddDays(5);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "RES", LearnDelFAMCode = "02", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom }
            };

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs, actEndDate);
            result.Should().BeFalse();
        }

        [Fact]
        public void FamDateCondition_False_AsLearnDeliveryIsNull()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var actEndDate = fAMDateTo.AddDays(5);

            List<ILearningDeliveryFAM> learningDeliveryFAMs = null;

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs, actEndDate);
            result.Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_AsActEndDateNotEqual()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            var actEndDate = fAMDateTo.AddDays(5); // actEndDate is not equal to famDateTo

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "00100325",
                AimSeqNumber = 1,
                AchDateNullable = actEndDate.AddDays(2),
                LearnActEndDateNullable = actEndDate,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"))
                               .Returns(true);

            var result = NewRule(mockFamQueryService.Object, null).ConditionMet(learningDelivery);
            result.Should().BeTrue();

            mockFamQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"), Times.Exactly(1));
        }

        [Fact]
        public void ConditionMet_False_AsActEndDateIsEqual()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            var achDate = fAMDateTo; // achDate is equal to famDateTo

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "00100325",
                AimSeqNumber = 1,
                AchDateNullable = achDate,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"))
                               .Returns(true);

            var result = NewRule(mockFamQueryService.Object, null).ConditionMet(learningDelivery);
            result.Should().BeFalse();

            mockFamQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"), Times.Exactly(1));
        }

        [Fact]
        public void Validate_NoError()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            var actEndDate = fAMDateTo;

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
            };

            ILearner learner = new TestLearner
            {
                LearnRefNumber = "DOB32Trig",
                DateOfBirthNullable = new DateTime(1991, 08, 01),
                LearningDeliveries = new List<ILearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = "00100325",
                        AimSeqNumber = 1,
                        AchDateNullable = fAMDateTo,
                        LearnActEndDateNullable = actEndDate,
                        FundModel = 36,
                        ProgTypeNullable = 25,
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
        public void ValidateError_AsAchDate_NotEqualsFAMDateTo()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            var achDate = fAMDateTo.AddDays(5); // Error as actEndDate not equal to famDateTo

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
            };

            ILearner learner = new TestLearner
            {
                LearnRefNumber = "DOB32Trig",
                LearningDeliveries = new List<ILearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = "00100325",
                        AimSeqNumber = 1,
                        AchDateNullable = achDate,
                        FundModel = 36,
                        ProgTypeNullable = 25,
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

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, "28/08/2019")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.AchDate, "05/07/2019")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(
                                              LearningDeliveryFAMTypeConstants.ACT,
                                              new DateTime(2019, 08, 28),
                                              new DateTime(2019, 07, 05));

            validationErrorHandlerMock.Verify();
        }

        public R121Rule NewRule(
                        ILearningDeliveryFAMQueryService fAMQueryService = null,
                        IValidationErrorHandler validationErrorHandler = null)
        {
            return new R121Rule(fAMQueryService, validationErrorHandler);
        }
    }
}
