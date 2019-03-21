using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_89RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_89Rule(null, provider.Object, service.Object, yearData.Object));
        }

        /// <summary>
        /// New rule with null action provider throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullActionProviderThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_89Rule(handler.Object, null, service.Object, yearData.Object));
        }

        /// <summary>
        /// New rule with null year data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullYearDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_89Rule(handler.Object, provider.Object, service.Object, null));
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
            Assert.Equal("LearnAimRef_89", result);
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
            Assert.Equal(RuleNameConstants.LearnAimRef_89, result);
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
        /// Gets the closing date of last academic year meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData("2017-08-26", "2017-03-06")]
        [InlineData("2017-09-15", "2016-08-12")]
        [InlineData("2018-09-09", "2017-06-18")]
        [InlineData("2015-04-21", "2017-01-07")]
        [InlineData("2015-09-23", "2016-02-28")]
        public void GetClosingDateOfLastAcademicYearMeetsExpectation(string candidate, string expectation)
        {
            // arrange
            var learnStart = DateTime.Parse(candidate);
            var testDate = DateTime.Parse(expectation);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(learnStart, AcademicYearDates.PreviousYearEnd))
                .Returns(testDate);

            var sut = new LearnAimRef_89Rule(handler.Object, provider.Object, service.Object, yearData.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStart);

            // act, not interested in the result just that we hit a strict signature
            var result = sut.GetClosingDateOfLastAcademicYear(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            service.VerifyAll();
            yearData.VerifyAll();

            Assert.Equal(testDate, result);
        }

        /// <summary>
        /// Has valid learning aim meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="previousYearEnd">The previous year end.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        /// <param name="startDates">The start dates.</param>
        [Theory]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", false, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", false, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        public void HasValidLearningAimMeetsExpectation(string candidate, string previousYearEnd, bool expectation, params string[] startDates)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse(candidate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var category = "larsCat";
            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            startDates.ForEach(sd =>
            {
                var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
                mockValidity
                    .SetupGet(x => x.ValidityCategory)
                    .Returns(category);
                mockValidity
                    .SetupGet(x => x.StartDate)
                    .Returns(DateTime.Parse(sd));
                mockValidity
                    .SetupGet(x => x.EndDate)
                    .Returns(DateTime.Parse(sd));

                larsValidities.Add(mockValidity.Object);
            });

            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Parse(previousYearEnd));

            var sut = new LearnAimRef_89Rule(handler.Object, provider.Object, service.Object, yearData.Object);

            // act
            var result = sut.HasValidLearningAim(mockDelivery.Object, category);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            service.VerifyAll();
            yearData.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", true, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        public void HasValidLearningAimWithNulEndDatesMeetsExpectation(string candidate, string previousYearEnd, bool expectation, params string[] startDates)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse(candidate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var category = "larsCat";
            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            startDates.ForEach(sd =>
            {
                var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
                mockValidity
                    .SetupGet(x => x.ValidityCategory)
                    .Returns(category);
                mockValidity
                    .SetupGet(x => x.StartDate)
                    .Returns(DateTime.Parse(sd));

                larsValidities.Add(mockValidity.Object);
            });

            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Parse(previousYearEnd));

            var sut = new LearnAimRef_89Rule(handler.Object, provider.Object, service.Object, yearData.Object);

            // act
            var result = sut.HasValidLearningAim(mockDelivery.Object, category);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            service.VerifyAll();
            yearData.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="previousYearEnd">The previous year end.</param>
        /// <param name="category">The category.</param>
        /// <param name="startDates">The start dates.</param>
        [Theory]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.AdultSkills, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.AdvancedLearnerLoan, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.Any, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.Apprenticeships, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.CommunityLearning, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.EFA16To19, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.EFAConFundEnglish, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.EFAConFundMaths, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.EuropeanSocialFund, "2014-04-01", "2012-05-09", "2016-07-15", "2017-07-31")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.OLASSAdult, "2014-04-01", "2012-05-09", "2016-07-15", "2017-04-16")]
        [InlineData("2016-04-18", "2017-07-31", TypeOfLARSValidity.Unemployed, "2014-04-01", "2012-05-09", "2016-07-15", "2016-04-16")]
        public void InvalidItemRaisesValidationMessage(string candidate, string previousYearEnd, string category, params string[] startDates)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse(candidate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("LearnAimRef_89", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("Expected Category", category))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockResult = new Mock<IBranchResult>();
            mockResult
                .SetupGet(x => x.OutOfScope)
                .Returns(false);
            mockResult
                .SetupGet(x => x.Category)
                .Returns(category);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            provider
                .Setup(x => x.GetBranchingResultFor(mockDelivery.Object, mockLearner.Object))
                .Returns(mockResult.Object);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            startDates.ForEach(sd =>
            {
                var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
                mockValidity
                    .SetupGet(x => x.ValidityCategory)
                    .Returns(category);
                mockValidity
                    .SetupGet(x => x.StartDate)
                    .Returns(DateTime.Parse(sd));
                mockValidity
                    .SetupGet(x => x.EndDate)
                    .Returns(DateTime.Parse(sd));

                larsValidities.Add(mockValidity.Object);
            });

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Parse(previousYearEnd));

            var sut = new LearnAimRef_89Rule(handler.Object, provider.Object, service.Object, yearData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="previousYearEnd">The previous year end.</param>
        /// <param name="category">The category.</param>
        /// <param name="startDates">The start dates.</param>
        [Theory]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.AdultSkills, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.AdvancedLearnerLoan, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.Any, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.Apprenticeships, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.CommunityLearning, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.EFA16To19, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.EFAConFundEnglish, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.EuropeanSocialFund, "2014-04-01", "2012-05-09", "2016-07-15", "2017-11-14")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.OLASSAdult, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        [InlineData("2018-04-01", "2017-07-31", TypeOfLARSValidity.Unemployed, "2014-04-01", "2012-05-09", "2016-07-15", "2017-08-01")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, string previousYearEnd, string category, params string[] startDates)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse(candidate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockResult = new Mock<IBranchResult>();
            mockResult
                .SetupGet(x => x.OutOfScope)
                .Returns(false);
            mockResult
                .SetupGet(x => x.Category)
                .Returns(category);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            provider
                .Setup(x => x.GetBranchingResultFor(mockDelivery.Object, mockLearner.Object))
                .Returns(mockResult.Object);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            startDates.ForEach(sd =>
            {
                var mockValidity = new Mock<ILARSLearningDeliveryValidity>();
                mockValidity
                    .SetupGet(x => x.ValidityCategory)
                    .Returns(category);
                mockValidity
                    .SetupGet(x => x.StartDate)
                    .Returns(DateTime.Parse(sd));
                mockValidity
                    .SetupGet(x => x.EndDate)
                    .Returns(DateTime.Parse(sd));

                larsValidities.Add(mockValidity.Object);
            });

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Parse(previousYearEnd));

            var sut = new LearnAimRef_89Rule(handler.Object, provider.Object, service.Object, yearData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            service.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRef_89Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            return new LearnAimRef_89Rule(handler.Object, provider.Object, service.Object, yearData.Object);
        }
    }
}
