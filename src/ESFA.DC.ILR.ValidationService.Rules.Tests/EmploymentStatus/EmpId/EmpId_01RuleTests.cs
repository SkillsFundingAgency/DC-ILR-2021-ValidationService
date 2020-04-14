using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpId_01RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpId_01Rule(null, edrsData.Object));
        }

        [Fact]
        public void NewRuleWithNullDataProviderThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpId_01Rule(handler.Object, null));
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
            Assert.Equal("EmpId_01", result);
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
            Assert.Equal(RuleNameConstants.EmpId_01, result);
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
        /// Is invalid domain item meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData(999999999, false)]
        [InlineData(1, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        public void IsNotValidMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);
            edrsData
                .Setup(x => x.IsValid(candidate))
                .Returns(!expectation);

            var sut = new EmpId_01Rule(handler.Object, edrsData.Object);

            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(x => x.EmpIdNullable)
                .Returns(candidate);

            // act
            var result = sut.IsNotValid(candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void InvalidItemRaisesValidationMessage(int? candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.EmpIdNullable)
                .Returns(candidate);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(status.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == RuleNameConstants.EmpId_01),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.EmpId),
                    candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            // the crux of the test runs on the return value from this call
            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);
            edrsData
                .Setup(x => x.IsValid(candidate))
                .Returns(false);

            var sut = new EmpId_01Rule(handler.Object, edrsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            edrsData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void ValidItemDoesNotRaiseValidationMessage(int? candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.EmpIdNullable)
                .Returns(candidate);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(new List<ILearnerEmploymentStatus> { status.Object });

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // the crux of the test runs on the return value from this call
            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);
            edrsData
                .Setup(x => x.IsValid(candidate))
                .Returns(true);

            var sut = new EmpId_01Rule(handler.Object, edrsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            edrsData.VerifyAll();
        }

        /// <summary>
        /// Valid item with empty employments does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemWithEmptyEmploymentsDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var statii = new List<ILearnerEmploymentStatus>();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);

            var sut = new EmpId_01Rule(handler.Object, edrsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            edrsData.VerifyAll();
        }

        /// <summary>
        /// Valid item with null employments does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemWithNullEmploymentsDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);

            var sut = new EmpId_01Rule(handler.Object, edrsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            edrsData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public EmpId_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);

            return new EmpId_01Rule(handler.Object, edrsData.Object);
        }
    }
}
