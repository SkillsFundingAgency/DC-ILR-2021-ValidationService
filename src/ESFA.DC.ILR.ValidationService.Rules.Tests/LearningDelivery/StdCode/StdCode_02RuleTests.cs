using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.StdCode;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.StdCode
{
    public class StdCode_02RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("StdCode_02", result);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(null, false)]
        public void HasStandardCodeMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var result = sut.HasStandardCode(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(23, true)]
        [InlineData(38, true)]
        [InlineData(2, false)]
        [InlineData(23, false)]
        [InlineData(38, false)]
        public void IsValidStandardCodeLARSDataServiceVerifiesOK(int candidate, bool expectation)
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.ContainsStandardFor(candidate))
                .Returns(expectation);

            var sut = new StdCode_02Rule(handler.Object, service.Object);

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var result = sut.IsValidStandardCode(mockItem.Object);

            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(23)]
        [InlineData(38)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mock = new Mock<ILearner>();
            mock
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mock
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == "StdCode_02"),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == StdCode_02Rule.MessagePropertyName),
                    candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.ContainsStandardFor(candidate))
                .Returns(false);

            var sut = new StdCode_02Rule(handler.Object, service.Object);

            sut.Validate(mock.Object);

            handler.VerifyAll();
            service.VerifyAll();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(23)]
        [InlineData(38)]
        public void ValidItemDoesNotRaiseAValidationMessage(int candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mock = new Mock<ILearner>();
            mock
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mock
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.ContainsStandardFor(candidate))
                .Returns(true);

            var sut = new StdCode_02Rule(handler.Object, service.Object);

            sut.Validate(mock.Object);

            handler.VerifyAll();
            service.VerifyAll();
        }

        public StdCode_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new StdCode_02Rule(handler.Object, service.Object);
        }
    }
}
