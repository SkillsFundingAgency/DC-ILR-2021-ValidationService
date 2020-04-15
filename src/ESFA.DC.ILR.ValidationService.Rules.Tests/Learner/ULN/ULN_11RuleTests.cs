using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_11RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ULN_11", result);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, true)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void IsExternallyFundedMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            var result = sut.IsExternallyFunded(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.HigherEducationFundingCouncilEngland, true)]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void IsHEFCEFundedMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.IsHEFCEFunded(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2015-06-15", "2015-06-18", true)]
        [InlineData("2015-06-15", "2015-06-19", true)]
        [InlineData("2015-06-15", "2015-06-20", false)]
        [InlineData("2015-06-15", "2015-06-21", false)]
        [InlineData("2016-09-14", "2016-09-20", false)]
        [InlineData("2016-09-15", "2016-09-20", false)]
        [InlineData("2016-09-16", "2016-09-20", true)]
        [InlineData("2016-09-17", "2016-09-20", true)]
        public void IsPlannedShortCourseMeetsExpectation(string startDate, string endDate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));
            mockItem
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(DateTime.Parse(endDate));

            var result = sut.IsPlannedShortCourse(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2015-06-15", null, false)]
        [InlineData("2015-06-15", "2015-06-18", true)]
        [InlineData("2015-06-15", "2015-06-19", true)]
        [InlineData("2015-06-15", "2015-06-20", false)]
        [InlineData("2015-06-15", "2015-06-21", false)]
        [InlineData("2016-09-14", "2016-09-20", false)]
        [InlineData("2016-09-15", "2016-09-20", false)]
        [InlineData("2016-09-16", "2016-09-20", true)]
        [InlineData("2016-09-17", "2016-09-20", true)]
        public void IsCompletedShortCourseMeetsExpectation(string startDate, string endDate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            if (!string.IsNullOrWhiteSpace(endDate))
            {
                mockItem
                    .SetupGet(y => y.LearnActEndDateNullable)
                    .Returns(DateTime.Parse(endDate));
            }

            var result = sut.IsCompletedShortCourse(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2015-04-15", "2015-06-13", false)]
        [InlineData("2015-04-15", "2015-06-14", false)]
        [InlineData("2015-04-15", "2015-06-15", true)]
        [InlineData("2015-04-15", "2015-06-16", true)]
        [InlineData("2016-06-14", "2016-08-15", true)]
        [InlineData("2016-06-15", "2016-08-15", true)]
        [InlineData("2016-06-16", "2016-08-15", false)]
        [InlineData("2016-06-17", "2016-08-15", false)]
        public void HasExceedRegistrationPeriodMeetsExpectation(string startDate, string fileDate, bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(xc => xc.FilePreparationDate())
                .Returns(DateTime.Parse(fileDate));

            var yearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            var sut = new ULN_11Rule(handler.Object, fileData.Object, yearService.Object);

            var result = sut.HasExceedRegistrationPeriod(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2015-06-15", "2015-06-16", true)]
        [InlineData("2015-06-16", "2015-06-16", false)]
        [InlineData("2016-08-14", "2016-08-15", true)]
        [InlineData("2016-08-15", "2016-08-15", false)]
        public void IsInsideGeneralRegistrationThresholdMeetsExpectation(string fileDate, string yearDate, bool expectation)
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(xc => xc.FilePreparationDate())
                .Returns(DateTime.Parse(fileDate));

            var yearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearService
                .Setup(xc => xc.JanuaryFirst())
                .Returns(DateTime.Parse(yearDate));

            var sut = new ULN_11Rule(handler.Object, fileData.Object, yearService.Object);

            var result = sut.IsInsideGeneralRegistrationThreshold();

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2345, true)]
        [InlineData(654321, true)]
        [InlineData(9999999999, false)]
        public void IsRegisteredLearnerMeetsExpectation(long candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(y => y.ULN)
                .Returns(candidate);

            var result = sut.IsRegisteredLearner(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.HigherEducationFundingCouncilEngland, false)]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, true)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void IsLearnerInCustodyMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.IsLearnerInCustody(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            var mockFam = new Mock<ILearningDeliveryFAM>();
            mockFam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.SourceOfFunding);
            mockFam
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns("1");

            var fams = new List<ILearningDeliveryFAM>();
            fams.Add(mockFam.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.NotFundedByESFA);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse("2019-01-02"));
            mockDelivery
                .SetupGet(x => x.LearnPlanEndDate)
                .Returns(DateTime.Parse("2019-02-02"));

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.ULN)
                .Returns(9999999999);
            mockLearner
                .SetupGet(y => y.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == ULN_11Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == ULN_11Rule.MessagePropertyName),
                    Moq.It.IsAny<ILearningDelivery>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(xc => xc.FilePreparationDate())
                .Returns(DateTime.Parse("2019-04-02"));

            var yearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearService
                .Setup(xc => xc.JanuaryFirst())
                .Returns(DateTime.Parse("2019-01-01"));

            var sut = new ULN_11Rule(handler.Object, fileData.Object, yearService.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            fileData.VerifyAll();
            yearService.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            var mockFam = new Mock<ILearningDeliveryFAM>();
            mockFam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.SourceOfFunding);
            mockFam
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns("1");

            var fams = new List<ILearningDeliveryFAM>();
            fams.Add(mockFam.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.NotFundedByESFA);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse("2019-03-02"));
            mockDelivery
                .SetupGet(x => x.LearnPlanEndDate)
                .Returns(DateTime.Parse("2019-05-02"));

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.ULN)
                .Returns(9999999999);
            mockLearner
                .SetupGet(y => y.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(xc => xc.FilePreparationDate())
                .Returns(DateTime.Parse("2019-01-02"));

            var yearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearService
                .Setup(xc => xc.JanuaryFirst())
                .Returns(DateTime.Parse("2019-01-01"));

            var sut = new ULN_11Rule(handler.Object, fileData.Object, yearService.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            fileData.VerifyAll();
            yearService.VerifyAll();
        }

        public ULN_11Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var yearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            return new ULN_11Rule(handler.Object, fileData.Object, yearService.Object);
        }
    }
}
