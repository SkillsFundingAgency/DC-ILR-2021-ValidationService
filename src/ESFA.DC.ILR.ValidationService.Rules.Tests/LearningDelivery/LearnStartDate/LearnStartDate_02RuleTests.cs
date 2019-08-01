using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_02Rule(null, service.Object));
        }

        /// <summary>
        /// New rule with null data service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDataServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_02Rule(handler.Object, null));
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
            Assert.Equal("LearnStartDate_02", result);
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
            Assert.Equal(LearnStartDate_02Rule.Name, result);
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
        /// Has exceed registration period meets expectation
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="operationDate">The operation date.</param>
        /// <param name="commencementDate">The commencement date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2004-07-31", "2015-02-13", "2014-08-01", true)] // 2014-08-01 => 2004-08-01
        [InlineData("2004-04-14", "2015-04-14", "2014-08-01", true)] // 2014-08-01 => 2004-08-01
        [InlineData("2004-07-31", "2015-06-15", "2014-08-01", true)] // 2014-08-01 => 2004-08-01
        [InlineData("2004-08-01", "2015-06-15", "2014-08-01", false)] // 2014-08-01 => 2004-08-01
        [InlineData("2005-07-31", "2015-08-15", "2015-08-01", true)] // 2015-08-01 => 2005-08-01
        [InlineData("2005-08-01", "2015-08-15", "2015-08-01", false)] // 2015-08-01 => 2005-08-01
        [InlineData("2006-07-31", "2016-08-15", "2016-08-01", true)] // 2016-08-01 => 2006-08-01
        [InlineData("2006-08-01", "2016-08-15", "2016-08-01", false)] // 2016-08-01 => 2006-08-01
        public void IsOutsideValidSubmissionPeriod(string startDate, string operationDate, string commencementDate, bool expectation)
        {
            // arrange
            var testDate = DateTime.Parse(operationDate);
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.Start())
                .Returns(DateTime.Parse(commencementDate));

            var sut = new LearnStartDate_02Rule(handler.Object, service.Object);

            // act
            var result = sut.IsOutsideValidSubmissionPeriod(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="operationDate">The operation date.</param>
        /// <param name="commencementDate">The commencement date.</param>
        [Theory]
        [InlineData("2004-07-31", "2015-02-13", "2014-08-01")] // 2014-08-01 => 2004-08-01
        [InlineData("2004-04-14", "2015-04-14", "2014-08-01")] // 2014-08-01 => 2004-08-01
        [InlineData("2004-07-31", "2015-06-15", "2014-08-01")] // 2014-08-01 => 2004-08-01
        [InlineData("2005-07-31", "2015-08-15", "2015-08-01")] // 2015-08-01 => 2005-08-01
        [InlineData("2006-07-31", "2016-08-15", "2016-08-01")] // 2016-08-01 => 2006-08-01
        public void InvalidItemRaisesValidationMessage(string startDate, string operationDate, string commencementDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testdate = DateTime.Parse(startDate);
            var opDate = DateTime.Parse(operationDate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testdate);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(LearnStartDate_02Rule.Name, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testdate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.Start())
                .Returns(DateTime.Parse(commencementDate));

            var sut = new LearnStartDate_02Rule(handler.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="operationDate">The operation date.</param>
        /// <param name="commencementDate">The commencement date.</param>
        [Theory]
        [InlineData("2004-08-01", "2015-06-15", "2014-08-01")] // 2014-08-01 => 2004-08-01
        [InlineData("2005-08-01", "2015-08-15", "2015-08-01")] // 2015-08-01 => 2005-08-01
        [InlineData("2006-08-01", "2016-08-15", "2016-08-01")] // 2016-08-01 => 2006-08-01
        public void ValidItemDoesNotRaiseAValidationMessage(string startDate, string operationDate, string commencementDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testdate = DateTime.Parse(startDate);
            var opDate = DateTime.Parse(operationDate);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testdate);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.Start())
                .Returns(DateTime.Parse(commencementDate));

            var sut = new LearnStartDate_02Rule(handler.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnStartDate_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            return new LearnStartDate_02Rule(handler.Object, service.Object);
        }
    }
}
