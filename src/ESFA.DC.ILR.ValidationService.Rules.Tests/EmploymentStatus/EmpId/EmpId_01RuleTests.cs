using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpId_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpId_01", result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(999999999, false)]
        [InlineData(1, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        public void IsNotValidMeetsExpectation(int? candidate, bool expectation)
        {
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

            var result = sut.IsNotValid(candidate);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void InvalidItemRaisesValidationMessage(int? candidate)
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
                    Moq.It.Is<string>(y => y == RuleNameConstants.EmpId_01),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.EmpId),
                    candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);
            edrsData
                .Setup(x => x.IsValid(candidate))
                .Returns(false);

            var sut = new EmpId_01Rule(handler.Object, edrsData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            edrsData.VerifyAll();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void ValidItemDoesNotRaiseValidationMessage(int? candidate)
        {
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

            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);
            edrsData
                .Setup(x => x.IsValid(candidate))
                .Returns(true);

            var sut = new EmpId_01Rule(handler.Object, edrsData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            edrsData.VerifyAll();
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
            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);

            var sut = new EmpId_01Rule(handler.Object, edrsData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            edrsData.VerifyAll();
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
            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);

            var sut = new EmpId_01Rule(handler.Object, edrsData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            edrsData.VerifyAll();
        }

        public EmpId_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var edrsData = new Mock<IEmployersDataService>(MockBehavior.Strict);

            return new EmpId_01Rule(handler.Object, edrsData.Object);
        }
    }
}
