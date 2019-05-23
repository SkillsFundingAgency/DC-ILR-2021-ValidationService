using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class ProgType_13RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var mockService = new Mock<IFileDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ProgType_13Rule(null, mockService.Object));
        }

        /// <summary>
        /// New rule with null file data service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullFileDataServiceThrows()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ProgType_13Rule(mockHandler.Object, null));
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
            Assert.Equal("ProgType_13", result);
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
            Assert.Equal(ProgType_13Rule.Name, result);
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
        /// Condition met with null learning delivery returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with learning deliveries containing open training meet expectation.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="filePreparationDate">The file preparation date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2017-08-01", "2017-09-30", true)]
        [InlineData("2016-09-01", "2017-09-30", false)]
        [InlineData("2017-01-01", "2017-06-30", true)]
        [InlineData("2017-02-01", "2017-07-31", true)]
        [InlineData("2017-02-26", "2017-11-30", false)]
        [InlineData("2017-03-14", "2017-11-30", false)]
        [InlineData("2017-03-31", "2017-12-01", false)]
        [InlineData("2017-04-01", "2017-11-30", true)]
        [InlineData("2015-04-01", "2017-12-01", false)]
        [InlineData("2015-07-31", "2015-10-01", true)]
        [InlineData("2015-08-01", "2016-03-31", true)]
        [InlineData("2015-08-01", "2016-04-01", true)]
        [InlineData("2015-08-01", "2016-04-02", false)]
        public void ConditionMetWithLearningDeliveriesContainingOpenTrainingMeetExpectation(string startDate, string filePreparationDate, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var service = new Mock<IFileDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.FilePreparationDate())
                .Returns(DateTime.Parse(filePreparationDate));

            var sut = new ProgType_13Rule(handler.Object, service.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="filePreparationDate">The file preparation date.</param>
        [Theory]
        [InlineData("2016-08-01", "2017-09-30")]
        [InlineData("2016-01-01", "2017-06-30")]
        [InlineData("2016-02-01", "2017-07-31")]
        public void InvalidItemRaisesValidationMessage(string startDate, string filePreparationDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            var testStartDate = DateTime.Parse(startDate);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testStartDate);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.Traineeship);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(TypeOfAim.ProgrammeAim);

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
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == ProgType_13Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "LearnStartDate"),
                    Moq.It.Is<DateTime>(y => y == testStartDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<IFileDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.FilePreparationDate())
                .Returns(DateTime.Parse(filePreparationDate));

            var sut = new ProgType_13Rule(handler.Object, service.Object);

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
        /// <param name="filePreparationDate">The file preparation date.</param>
        [Theory]
        [InlineData("2017-08-01", "2017-09-30")]
        [InlineData("2017-01-01", "2017-06-30")]
        [InlineData("2017-02-01", "2017-07-31")]
        public void ValidItemDoesNotRaiseAValidationMessage(string startDate, string filePreparationDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.Traineeship);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(TypeOfAim.ProgrammeAim);

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

            var service = new Mock<IFileDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.FilePreparationDate())
                .Returns(DateTime.Parse(filePreparationDate));

            var sut = new ProgType_13Rule(handler.Object, service.Object);

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
        public ProgType_13Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IFileDataService>(MockBehavior.Strict);

            return new ProgType_13Rule(handler.Object, service.Object);
        }
    }
}
