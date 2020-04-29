using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_06RuleTests
    {
        public const int TestProviderID = 123456789;

        public static readonly DateTime TestPreparationDate = DateTime.Parse("2018-04-02");

        public static readonly DateTime TestNewYearDate = DateTime.Parse("2018-04-01");

        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ULN_06", result);
        }

        [Fact]
        public void MinimumCourseDurationMeetsExpectation()
        {
            var sut = NewRule();

            Assert.Equal(5, ULN_06Rule.MinimumCourseDuration);
        }

        [Fact]
        public void RuleLeniencyPeriodMeetsExpectation()
        {
            var sut = NewRule();

            Assert.Equal(60, ULN_06Rule.RuleLeniencyPeriod);
        }

        [Fact]
        public void FilePreparationDateMeetsExpectation()
        {
            var sut = NewRule();

            Assert.Equal(TestPreparationDate, sut.FilePreparationDate);
        }

        [Fact]
        public void FirstJanuaryMeetsExpectation()
        {
            var sut = NewRule();

            Assert.Equal(TestNewYearDate, sut.FirstJanuary);
        }

        [Fact]
        public void RuleName3()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        [Theory]
        [InlineData("2018-04-02", "2018-04-01", false)]
        [InlineData("2018-04-01", "2018-04-01", false)]
        [InlineData("2018-03-31", "2018-04-01", true)]
        public void IsOutsideQualifyingPeriodMeetsExpectation(string candidate, string yearStart, bool expectation)
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(DateTime.Parse(yearStart));

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(DateTime.Parse(candidate));

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            var result = sut.IsOutsideQualifyingPeriod();

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Fact]
        public void TemporaryULNMeetsExpectation()
        {
            Assert.Equal(9999999999, ValidationConstants.TemporaryULN);
        }

        [Theory]
        [InlineData(12345, true)]
        [InlineData(1234, true)]
        [InlineData(9999999999, false)]
        public void IsValidULNMeetsExpectation(long candidate, bool expectation)
        {
            var learner = new Mock<ILearner>();
            learner.SetupGet(x => x.ULN).Returns(candidate);
            var sut = NewRule();

            var result = sut.IsValidULN(learner.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsLearnerInCustodyMeetsExpectation(bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(delivery.Object.LearningDeliveryFAMs, "LDM", "034"))
                .Returns(expectation);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            var result = sut.IsLearnerInCustody(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Theory]
        [InlineData(25, TypeOfFunding.Age16To19ExcludingApprenticeships)]
        [InlineData(35, TypeOfFunding.AdultSkills)]
        [InlineData(36, TypeOfFunding.ApprenticeshipsFrom1May2017)]
        [InlineData(70, TypeOfFunding.EuropeanSocialFund)]
        [InlineData(81, TypeOfFunding.OtherAdult)]
        [InlineData(82, TypeOfFunding.Other16To19)]
        [InlineData(99, TypeOfFunding.NotFundedByESFA)]
        public void TypeOfFundingMeetsExpectation(int expectation, int candidate)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData(25, true)]
        [InlineData(35, true)]
        [InlineData(36, true)]
        [InlineData(70, true)]
        [InlineData(81, true)]
        [InlineData(82, true)]
        [InlineData(99, false)]
        [InlineData(1, false)]
        public void HasQualifyingModelMeetsExpectation(int fundModel, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
               .Setup(x => x.FundModel)
               .Returns(fundModel);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            var result = sut.HasQualifyingModel(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Theory]
        [InlineData(99, true)]
        [InlineData(35, false)]
        public void IsNotFundedByESFAMeetsExpectation(int fundModel, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
               .Setup(x => x.FundModel)
               .Returns(fundModel);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            var result = sut.IsNotFundedByESFA(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Theory]
        [InlineData("ADL1", Monitoring.Delivery.FinancedByAdvancedLearnerLoans)]
        [InlineData("LDM034", Monitoring.Delivery.OLASSOffendersInCustody)]
        public void MonitoringCodeMeetsExpectation(string expectation, string candidate)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasAdvancedLearnerLoanMeetsExpectation(bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(delivery.Object.LearningDeliveryFAMs, "ADL", "1"))
                .Returns(expectation);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            var result = sut.HasAdvancedLearnerLoan(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, true)]
        [InlineData(6, true)]
        public void HasQualifyingPlannedDurationMeetsExpectation(int duration, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTime
                .Setup(x => x.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(duration);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            var result = sut.HasQualifyingPlannedDuration(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Theory]
        [InlineData("2018-04-02", true)]
        [InlineData(null, false)]
        public void HasActualEndDateMeetsExpectation(string actEnd, bool expectation)
        {
            var testDate = GetNullableDate(actEnd);
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(testDate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            var result = sut.HasActualEndDate(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, true)]
        [InlineData(6, true)]
        public void HasQualifyingActualDurationMeetsExpectation(int duration, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(DateTime.Today);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTime
                .Setup(x => x.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(duration);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            var result = sut.HasQualifyingActualDuration(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Theory]
        [InlineData(59, true)]
        [InlineData(60, true)]
        [InlineData(61, false)]
        public void IsInsideLeniencyPeriodMeetsExpectation(int duration, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTime
                .Setup(x => x.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(duration);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            var result = sut.IsInsideLeniencyPeriod(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var testStart = DateTime.Parse("2016-05-01");
            var testEnd = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(99);
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testStart);
            delivery
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(testEnd);
            delivery
                .SetupGet(y => y.LearnActEndDateNullable)
                .Returns(DateTime.Today);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.ULN)
                .Returns(ValidationConstants.TemporaryULN);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.ULN_06, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("ULN", ValidationConstants.TemporaryULN))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FilePrepDate", AbstractRule.AsRequiredCultureDate(TestPreparationDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testStart)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTime
                .Setup(x => x.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsInOrder(4, 5, 60);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                  delivery.Object.LearningDeliveryFAMs,
                  "LDM",
                  "034"))
                .Returns(false);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                  delivery.Object.LearningDeliveryFAMs,
                  "ACT",
                  "1"))
                .Returns(false);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                  delivery.Object.LearningDeliveryFAMs,
                  "ADL",
                  "1"))
                .Returns(true);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var testStart = DateTime.Parse("2016-05-01");
            var testEnd = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(99);
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testStart);
            delivery
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(testEnd);
            delivery
                .SetupGet(y => y.LearnActEndDateNullable)
                .Returns(DateTime.Today);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.ULN)
                .Returns(ValidationConstants.TemporaryULN);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTime
                .Setup(x => x.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsInOrder(4, 5, 61);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                  delivery.Object.LearningDeliveryFAMs,
                  "LDM",
                  "034"))
                .Returns(false);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                  delivery.Object.LearningDeliveryFAMs,
                  "ACT",
                  "1"))
                .Returns(false);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                  delivery.Object.LearningDeliveryFAMs,
                  "ADL",
                  "1"))
                .Returns(true);

            var sut = new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        public ULN_06Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            return new ULN_06Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, learningDeliveryFAMQS.Object);
        }
    }
}
