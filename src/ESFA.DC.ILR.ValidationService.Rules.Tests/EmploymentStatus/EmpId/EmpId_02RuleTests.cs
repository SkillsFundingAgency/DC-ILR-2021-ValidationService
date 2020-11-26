using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpId_02RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpId_02", result);
        }

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
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);
            ddRule05
                .SetupGet(x => x.InvalidLengthChecksum)
                .Returns('X');
            ddRule05
                .Setup(x => x.GetEmployerIDChecksum(candidate))
                .Returns(checksum);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            var result = sut.HasValidChecksum(candidate);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(100000001, '2')]
        [InlineData(100000002, '3')]
        [InlineData(200000003, '4')]
        public void InvalidItemRaisesValidationMessage(int candidate, char checksum)
        {
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

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule05.VerifyAll();
        }

        [Theory]
        [InlineData(100000001, '1')]
        [InlineData(100000002, '2')]
        [InlineData(200000003, '3')]
        public void ValidItemDoesNotRaiseValidationMessage(int candidate, char checksum)
        {
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

            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);
            ddRule05
                .SetupGet(x => x.InvalidLengthChecksum)
                .Returns('X');
            ddRule05
                .Setup(x => x.GetEmployerIDChecksum(candidate))
                .Returns(checksum);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule05.VerifyAll();
        }

        [Fact]
        public void ValidTemporaryItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.EmpIdNullable)
                .Returns(ValidationConstants.TemporaryEmployerId);

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

            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule05.VerifyAll();
        }

        [Fact]
        public void ValidItemWithEmptyEmploymentsDoesNotRaiseValidationMessage()
        {
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
            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule05.VerifyAll();
        }

        [Fact]
        public void ValidItemWithNullEmploymentsDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);

            var sut = new EmpId_02Rule(handler.Object, ddRule05.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule05.VerifyAll();
        }

        private EmpId_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule05 = new Mock<IDerivedData_05Rule>(MockBehavior.Strict);

            return new EmpId_02Rule(handler.Object, ddRule05.Object);
        }
    }
}
