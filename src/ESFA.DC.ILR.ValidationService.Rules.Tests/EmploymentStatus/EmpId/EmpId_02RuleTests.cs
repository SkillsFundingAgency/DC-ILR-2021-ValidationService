using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpId_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpId_02Rule(null, ddRule05.Object));
        }

        /// <summary>
        /// New rule with null derived data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpId_02Rule(handler.Object, null));
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
            Assert.Equal("EmpId_02", result);
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
            Assert.Equal(RuleNameConstants.EmpId_02, result);
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
        /// <param name="checksum">The checksum.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(100000001, '1', true)]
        [InlineData(100000001, '2', false)]
        [InlineData(100000002, '2', true)]
        [InlineData(100000002, '3', false)]
        [InlineData(200000003, '3', true)]
        [InlineData(200000003, '4', false)]
        [InlineData(200000003, 'X', false)]
        [InlineData(20000000, 'X', false)]
        public void IsNotValidMeetsExpectation(int candidate, char checksum, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);
            ddRule05
                .SetupGet(x => x.InvalidLengthChecksum)
                .Returns('X');
            ddRule05
                .Setup(x => x.GetEmployerIDChecksum(candidate))
                .Returns(checksum);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            // act
            var result = sut.HasValidChecksum(candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="checksum">The checksum.</param>
        [Theory]
        [InlineData(100000001, '2')]
        [InlineData(100000002, '3')]
        [InlineData(200000003, '4')]
        public void InvalidItemRaisesValidationMessage(int candidate, char checksum)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.EmpIdNullable)
                .Returns(candidate);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(status.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == RuleNameConstants.EmpId_02),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.EmpId),
                    candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);
            ddRule05
                .SetupGet(x => x.InvalidLengthChecksum)
                .Returns('X');
            ddRule05
                .Setup(x => x.GetEmployerIDChecksum(candidate))
                .Returns(checksum);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule05.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="checksum">The checksum.</param>
        [Theory]
        [InlineData(100000001, '1')]
        [InlineData(100000002, '2')]
        [InlineData(200000003, '3')]
        public void ValidItemDoesNotRaiseValidationMessage(int candidate, char checksum)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.EmpIdNullable)
                .Returns(candidate);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(status.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);
            ddRule05
                .SetupGet(x => x.InvalidLengthChecksum)
                .Returns('X');
            ddRule05
                .Setup(x => x.GetEmployerIDChecksum(candidate))
                .Returns(checksum);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule05.VerifyAll();
        }

        /// <summary>
        /// Valid temporary item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidTemporaryItemDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.EmpIdNullable)
                .Returns(ValidationConstants.TemporaryEmployerId);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(status.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule05.VerifyAll();
        }

        /// <summary>
        /// Valid item with empty employments does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemWithEmptyEmploymentsDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var statii = Collection.EmptyAndReadOnly<ILearnerEmploymentStatus>();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule05.VerifyAll();
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
            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule05.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        private EmpId_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);

            return new EmpId_02Rule(handler.Object, ddRule05.Object);
        }
    }
}
