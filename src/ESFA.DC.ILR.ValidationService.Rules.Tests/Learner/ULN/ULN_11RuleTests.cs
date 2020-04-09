using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_11RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var yearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ULN_11Rule(null, fileData.Object, yearService.Object));
        }

        [Fact]
        public void NewRuleWithNullFileDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var yearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ULN_11Rule(handler.Object, null, yearService.Object));
        }

        [Fact]
        public void NewRuleWithNullAcademicYearThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ULN_11Rule(handler.Object, fileData.Object, null));
        }

        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("ULN_11", result);
        }

        /// <summary>
        /// Rule name 2, matches the constant.
        /// </summary>
        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(ULN_11Rule.Name, result);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        /// <summary>
        /// Validate with null learner throws.
        /// </summary>
        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Is externally funded meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            // act
            var result = sut.IsExternallyFunded(mockDelivery.Object);

            // assert
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
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsHEFCEFunded(mockItem.Object);

            // assert
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
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));
            mockItem
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(DateTime.Parse(endDate));

            // act
            var result = sut.IsPlannedShortCourse(mockItem.Object);

            // assert
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
            // arrange
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

            // act
            var result = sut.IsCompletedShortCourse(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has exceed registration period meets expectation
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="fileDate">The file (preparation) date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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
            // arrange
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

            // act
            var result = sut.HasExceedRegistrationPeriod(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is inside general registration threshold meets expectation
        /// the year date is expectd to be jaunary 1st; but the test is fundamentally
        /// that the file date must precede the year date
        /// </summary>
        /// <param name="fileDate">The file date.</param>
        /// <param name="yearDate">The year date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2015-06-15", "2015-06-16", true)]
        [InlineData("2015-06-16", "2015-06-16", false)]
        [InlineData("2016-08-14", "2016-08-15", true)]
        [InlineData("2016-08-15", "2016-08-15", false)]
        public void IsInsideGeneralRegistrationThresholdMeetsExpectation(string fileDate, string yearDate, bool expectation)
        {
            // arrange
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

            // act
            var result = sut.IsInsideGeneralRegistrationThreshold();

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2345, true)]
        [InlineData(654321, true)]
        [InlineData(9999999999, false)]
        public void IsRegisteredLearnerMeetsExpectation(long candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(y => y.ULN)
                .Returns(candidate);

            // act
            var result = sut.IsRegisteredLearner(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is learner in custody meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsLearnerInCustody(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fileData.VerifyAll();
            yearService.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
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
                .Returns(DateTime.Parse("2019-03-02")); // <= push the learn start date inside the registration period
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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fileData.VerifyAll();
            yearService.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public ULN_11Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var yearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            return new ULN_11Rule(handler.Object, fileData.Object, yearService.Object);
        }
    }
}
