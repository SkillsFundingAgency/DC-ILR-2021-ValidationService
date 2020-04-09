using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinDate
{
    /// <summary>
    /// apprenticeship record financial date (tests)
    /// </summary>
    public class AFinDate_03RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new AFinDate_03Rule(null, fileData.Object));
        }

        [Fact]
        public void NewRuleWithNullFileDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new AFinDate_03Rule(handler.Object, null));
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
            Assert.Equal("AFinDate_03", result);
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
            Assert.Equal(RuleNameConstants.AFinDate_03, result);
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
        /// Has invalid financial date meets expectation
        /// </summary>
        /// <param name="finDate">The fin date.</param>
        /// <param name="fileDate">The file date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-04-01", "2016-04-02", false)]
        [InlineData("2016-04-01", "2016-03-31", true)]
        [InlineData("2016-04-30", "2016-05-01", false)]
        [InlineData("2016-05-01", "2016-05-01", false)]
        [InlineData("2016-05-02", "2016-05-01", true)]
        public void HasInvalidFinancialDateMeetsExpectation(string finDate, string fileDate, bool expectation)
        {
            // arrange
            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(DateTime.Parse(finDate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(DateTime.Parse(fileDate));

            var sut = new AFinDate_03Rule(handler.Object, fileData.Object);

            // act
            var result = sut.HasInvalidFinancialDate(mockFinRec.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            fileData.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="finDate">The fin date.</param>
        /// <param name="fileDate">The file date.</param>
        [Theory]
        [InlineData("2016-04-01", "2016-03-31")]
        [InlineData("2016-05-02", "2016-05-01")]
        public void InvalidItemRaisesValidationMessage(string finDate, string fileDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(finDate);
            var preparationDate = DateTime.Parse(fileDate);

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(testDate);

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.AFinDate_03, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AFinDate", testDate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FilePrepDate", preparationDate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(preparationDate);

            var sut = new AFinDate_03Rule(handler.Object, fileData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fileData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="finDate">The fin date.</param>
        /// <param name="fileDate">The file date.</param>
        [Theory]
        [InlineData("2016-04-01", "2016-04-02")]
        [InlineData("2016-04-30", "2016-05-01")]
        [InlineData("2016-05-01", "2016-05-01")]
        public void ValidItemDoesNotRaiseAValidationMessage(string finDate, string fileDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(finDate);
            var preparationDate = DateTime.Parse(fileDate);

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(testDate);

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(preparationDate);

            var sut = new AFinDate_03Rule(handler.Object, fileData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fileData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public AFinDate_03Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);

            return new AFinDate_03Rule(handler.Object, fileData.Object);
        }
    }
}
