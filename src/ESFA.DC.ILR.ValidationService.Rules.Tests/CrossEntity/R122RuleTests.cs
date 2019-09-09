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
    public class R122RuleTests : AbstractRuleTests<R122Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R122");
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
        [InlineData(null, true)] // returns True due to null
        [InlineData("2018-08-28", false)] // returns False as date exists
        public void AchDateConditionMet(string strAchDate, bool expectedResult)
        {
            DateTime? achDate = string.IsNullOrEmpty(strAchDate) ? (DateTime?)null : DateTime.Parse(strAchDate);
            var result = NewRule().AchDateIsUnknown(achDate);

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void FamDateCondition_True_AsFamDateTo_Exists()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "RES", LearnDelFAMCode = "02", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "act", LearnDelFAMCode = "03", LearnDelFAMDateToNullable = fAMDateTo.AddDays(10), LearnDelFAMDateFromNullable = fAMDateFrom }
            };

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs);
            result.Should().BeTrue();
        }

        [Fact]
        public void FamDateCondition_False_AsFamDateTo_NotExisting()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMCode = "01", LearnDelFAMDateToNullable = null, LearnDelFAMDateFromNullable = fAMDateFrom },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "RES", LearnDelFAMCode = "02", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom }
            };

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs);
            result.Should().BeFalse();
        }

        [Fact]
        public void FamDateCondition_False_AsFoundNull()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "RES", LearnDelFAMCode = "02", LearnDelFAMDateToNullable = fAMDateTo, LearnDelFAMDateFromNullable = fAMDateFrom }
            };

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs);
            result.Should().BeFalse();
        }

        [Fact]
        public void FamDateCondition_False_AsLearnDeliveryIsNull()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            List<ILearningDeliveryFAM> learningDeliveryFAMs = null;

            var result = NewRule().FAMDateConditionMet(learningDeliveryFAMs);
            result.Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_AsAchDateIsNull()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            DateTime? achDate = null; // ach date is null

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
            result.Should().BeTrue();

            mockFamQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT"), Times.Exactly(1));
        }

        [Fact]
        public void ConditionMet_False_AsAchDate_NotNull()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            DateTime? achDate = fAMDateTo.AddDays(2); // Fails as ach date is known

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

            var result = NewRule(mockFamQueryService.Object, null).ConditionMet(learningDelivery);
            result.Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AsFamDateTo_IsNull()
        {
            var fAMDateFrom = new DateTime(2019, 07, 15);
            DateTime? achDate = null;

            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "01",
                    LearnDelFAMDateToNullable = null,
                    LearnDelFAMDateFromNullable = fAMDateFrom
                },
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
        public void Validate_NoError_AsAchDateIsKnown()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            var achDate = fAMDateTo.AddDays(2);

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
                        AchDateNullable = achDate,
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                },
            };

            var mockFamQueryService = new Mock<ILearningDeliveryFAMQueryService>();

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
        public void ValidateError_AsAchDate_IsNull()
        {
            var fAMDateTo = new DateTime(2019, 08, 15);
            var fAMDateFrom = new DateTime(2019, 07, 15);
            DateTime? achDate = null; // Error as achDate is null

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

        public R122Rule NewRule(
                        ILearningDeliveryFAMQueryService fAMQueryService = null,
                        IValidationErrorHandler validationErrorHandler = null)
        {
            return new R122Rule(fAMQueryService, validationErrorHandler);
        }
    }
}
