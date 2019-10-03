using System;
using System.Collections.Generic;
using System.Linq;
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
    public class R123RuleTests : AbstractRuleTests<R123Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R123");
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
        [InlineData(1, true)]
        [InlineData(2, false)]
        public void CompStatusConditionMet(int compStatus, bool expectedResult)
        {
            var result = NewRule().CompStatusConditionMet(compStatus);

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void FamDateCondition_False_AsLearnDelFAMDateTo_NotReturned()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "RES", LearnDelFAMCode = "02", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "act", LearnDelFAMCode = "03", LearnDelFAMDateToNullable = null, LearnDelFAMDateFromNullable = fAMDateFrom.AddDays(1) }
            };

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs);
            result.Should().BeFalse();
        }

        [Fact]
        public void FamDateCondition_True_AsLearnDelFAMDateTo_Returned()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "RES", LearnDelFAMCode = "02", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "act", LearnDelFAMCode = "03", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom.AddDays(1) }
            };

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs);
            result.Should().BeTrue();
        }

        [Fact]
        public void FamDateCondition_False_AsLearnDeliveryIsNull()
        {
            List<ILearningDeliveryFAM> learningDeliveryFAMs = null;

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs);
            result.Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_LearnDelFAMDateToReturned()
        {
            var famDateTo = new DateTime(2019, 08, 15);
            var famDateFrom = new DateTime(2019, 07, 15);
            var achDate = famDateTo; // achDate is equal to famDateTo

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = famDateTo, LearnDelFAMDateFromNullable = famDateFrom },
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "00100325",
                AimSeqNumber = 1,
                AchDateNullable = achDate,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearningDeliveryFAMs = learningDeliveryFAMs,
                CompStatus = 1
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"))
                .Returns(true);

            var result = NewRule(mockFamQueryService.Object, null).ConditionMet(learningDelivery);
            result.Should().BeTrue();

            mockFamQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"), Times.Exactly(1));
        }

        [Fact]
        public void ConditionMet_False_LearnDelFAMDateToNotReturned()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            var achDate = fAMDateTo; // achDate is equal to famDateTo

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = null, LearnDelFAMDateFromNullable = fAMDateFrom },
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "00100325",
                AimSeqNumber = 1,
                AchDateNullable = achDate,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearningDeliveryFAMs = learningDeliveryFAMs,
                CompStatus = 1
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
        public void ValidateError_AsLearnDelFAMDateTo_Returned()
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
                        LearningDeliveryFAMs = learningDeliveryFAMs,
                        CompStatus = 1
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

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, "28/08/2019")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.CompStatus, 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(
                                              new DateTime(2019, 08, 28),
                                              1);

            validationErrorHandlerMock.Verify();
        }

        public R123Rule NewRule(
                        ILearningDeliveryFAMQueryService fAMQueryService = null,
                        IValidationErrorHandler validationErrorHandler = null)
        {
            return new R123Rule(fAMQueryService, validationErrorHandler);
        }
    }
}
