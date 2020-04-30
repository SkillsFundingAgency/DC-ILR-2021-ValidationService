using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    public class AFinType_12RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("AFinType_12", result);
        }

        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null);

            Assert.True(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveryAndNullFinancialRecordsReturnsFalse()
        {
            var sut = NewRule();
            var mock = new Mock<ILearningDelivery>();

            var result = sut.ConditionMet(mock.Object);

            Assert.False(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveryAndNoFinancialRecordsReturnsFalse()
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(new List<IAppFinRecord>());

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.False(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveryAndNoMatchingFinancialRecordsReturnsFalse()
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            var mockFinRec = new Mock<IAppFinRecord>();

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.False(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveryAndMatchingFinancialRecordReturnsTrue()
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(FundModels.ApprenticeshipsFrom1May2017);

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinType)
                .Returns(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice);

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.True(result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
               .SetupGet(x => x.AimSeqNumber)
               .Returns(AimSeqNumber);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(FundModels.ApprenticeshipsFrom1May2017);

            var mockFinRec = new Mock<IAppFinRecord>();
            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mockLearner.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == AFinType_12Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                AimSeqNumber,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "AFinType"),
                    Moq.It.Is<string>(y => y == "TNP")))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new AFinType_12Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(FundModels.ApprenticeshipsFrom1May2017);

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinType)
                .Returns(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice);

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mockLearner.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new AFinType_12Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public AFinType_12Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new AFinType_12Rule(mock.Object);
        }
    }
}
