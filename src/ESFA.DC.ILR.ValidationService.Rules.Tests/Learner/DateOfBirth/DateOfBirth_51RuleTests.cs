using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_51RuleTests : AbstractRuleTests<DateOfBirth_51Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_51");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var dateOfBirth = new DateTime(1992, 01, 01);
            var learnStartDate = new DateTime(2018, 01, 01);

            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = ProgTypes.Traineeship,
                AimType = AimTypes.ProgrammeAim,
                LearnStartDate = learnStartDate
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(26);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object).ConditionMet(learningDelivery, dateOfBirth)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Excluded()
        {
            var dateOfBirth = new DateTime(1992, 01, 01);
            var learnStartDate = new DateTime(2018, 01, 01);

            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = ProgTypes.Traineeship,
                AimType = AimTypes.ProgrammeAim,
                LearnStartDate = learnStartDate
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(26);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object).ConditionMet(learningDelivery, dateOfBirth)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDateLessThanJuly312016()
        {
            var dateOfBirth = new DateTime(1992, 01, 01);
            var learnStartDate = new DateTime(2016, 01, 01);

            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = ProgTypes.Traineeship,
                AimType = AimTypes.ProgrammeAim,
                LearnStartDate = learnStartDate
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(26);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object).ConditionMet(learningDelivery, dateOfBirth)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            var dateOfBirth = new DateTime(1992, 01, 01);
            var learnStartDate = new DateTime(2018, 01, 01);

            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = ProgTypes.ApprenticeshipStandard,
                AimType = AimTypes.ProgrammeAim,
                LearnStartDate = learnStartDate
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(26);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object).ConditionMet(learningDelivery, dateOfBirth)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var dateOfBirth = new DateTime(1992, 01, 01);
            var learnStartDate = new DateTime(2018, 01, 01);

            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = ProgTypes.Traineeship,
                AimType = AimTypes.AimNotPartOfAProgramme,
                LearnStartDate = learnStartDate
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(26);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object).ConditionMet(learningDelivery, dateOfBirth)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_YearsBetweenBirthAndLearnStartDateLessThan25()
        {
            var dateOfBirth = new DateTime(1994, 01, 01);
            var learnStartDate = new DateTime(2018, 01, 01);

            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = ProgTypes.Traineeship,
                AimType = AimTypes.ProgrammeAim,
                LearnStartDate = learnStartDate
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(24);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object).ConditionMet(learningDelivery, dateOfBirth)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var dateOfBirth = new DateTime(1992, 01, 01);
            var learnStartDate = new DateTime(2018, 01, 01);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = ProgTypes.Traineeship,
                        AimType = AimTypes.ProgrammeAim,
                        LearnStartDate = learnStartDate
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(26);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var dateOfBirth = new DateTime(1994, 01, 01);
            var learnStartDate = new DateTime(2018, 01, 01);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = ProgTypes.Traineeship,
                        AimType = AimTypes.ProgrammeAim,
                        LearnStartDate = learnStartDate
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(24);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, "01/01/2001")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/08/2016")).Verifiable();
            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2001, 1, 1), new DateTime(2016, 08, 01));
            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_51Rule NewRule(
            IDateTimeQueryService dateTimeQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_51Rule(dateTimeQueryService, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}