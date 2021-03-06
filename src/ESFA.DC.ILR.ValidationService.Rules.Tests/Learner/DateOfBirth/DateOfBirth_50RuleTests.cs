﻿using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
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
    public class DateOfBirth_50RuleTests : AbstractRuleTests<DateOfBirth_50Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_50");
        }

        [Theory]
        [InlineData("2019-08-02", false)]
        [InlineData("2019-07-31", true)]
        public void ConditionMet(string learnStartDateString, bool expected)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            var firstAugustForAcademicYearOfLearnersSixteenthBirthDate = new DateTime(2019, 08, 01);

            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = ProgTypes.Traineeship,
                AimType = AimTypes.ProgrammeAim,
                LearnStartDate = learnStartDate
            };

            NewRule().ConditionMet(learningDelivery, firstAugustForAcademicYearOfLearnersSixteenthBirthDate).Should().Be(expected);
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            var firstAugustForAcademicYearOfLearnersSixteenthBirthDate = new DateTime(2017, 08, 01);

            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = ProgTypes.ApprenticeshipStandard,
                AimType = AimTypes.ProgrammeAim,
                LearnStartDate = new DateTime(2018, 01, 01)
            };

            NewRule().ConditionMet(learningDelivery, firstAugustForAcademicYearOfLearnersSixteenthBirthDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var firstAugustForAcademicYearOfLearnersSixteenthBirthDate = new DateTime(2017, 08, 01);

            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = ProgTypes.Traineeship,
                AimType = AimTypes.ComponentAimInAProgramme,
                LearnStartDate = new DateTime(2018, 07, 01)
            };

            NewRule().ConditionMet(learningDelivery, firstAugustForAcademicYearOfLearnersSixteenthBirthDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnerIsOldEnough()
        {
            var firstAugustForAcademicYearOfLearnersSixteenthBirthDate = new DateTime(2018, 08, 01);

            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = ProgTypes.Traineeship,
                AimType = AimTypes.ProgrammeAim,
                LearnStartDate = new DateTime(2018, 08, 02)
            };

            NewRule().ConditionMet(learningDelivery, firstAugustForAcademicYearOfLearnersSixteenthBirthDate).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            DateTime? dateOfBirth = new DateTime(2002, 08, 01);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = ProgTypes.Traineeship,
                        AimType = AimTypes.ProgrammeAim,
                        LearnStartDate = new DateTime(2018, 07, 31)
                    }
                }
            };

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(It.IsAny<DateTime>(), AcademicYearDates.TraineeshipsAugust1))
                .Returns(new DateTime(2018, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(academicYearQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_DOBNull()
        {
            var learner = new TestLearner()
            {
                DateOfBirthNullable = null,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = ProgTypes.Traineeship,
                        AimType = AimTypes.ProgrammeAim,
                        LearnStartDate = new DateTime(2018, 01, 01)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            DateTime? dateOfBirth = new DateTime(2002, 08, 01);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = ProgTypes.Traineeship,
                        AimType = AimTypes.ProgrammeAim,
                        LearnStartDate = new DateTime(2018, 08, 01)
                    }
                }
            };

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(It.IsAny<DateTime>(), AcademicYearDates.TraineeshipsAugust1))
                .Returns(new DateTime(2018, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(academicYearQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

        private DateOfBirth_50Rule NewRule(IAcademicYearQueryService academicYearQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_50Rule(academicYearQueryService, validationErrorHandler);
        }
    }
}
