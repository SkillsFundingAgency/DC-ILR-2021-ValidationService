using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    public class AFinType_13RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("AFinType_13", result);
        }

        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null, null);

            Assert.True(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveryAndNullFinancialRecordReturnsTrue()
        {
            var sut = NewRule();
            var mock = new Mock<ILearningDelivery>();

            var result = sut.ConditionMet(mock.Object, null);

            Assert.True(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveryAndFinancialRecordWithNullDateReturnsFalse()
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            var mockFinRec = new Mock<IAppFinRecord>();

            var result = sut.ConditionMet(mockDelivery.Object, mockFinRec.Object);

            Assert.False(result);
        }

        [Theory]
        [InlineData("2016-04-01", "2016-04-01", true)]
        [InlineData("2016-04-01", "2016-04-02", false)]
        [InlineData("2016-04-01", "2016-03-31", false)]
        [InlineData("2016-05-01", "2016-05-01", true)]
        [InlineData("2016-04-30", "2016-05-01", false)]
        [InlineData("2016-05-02", "2016-05-01", false)]
        public void ConditionMetWithLearningDeliveryAndFinancialRecordReturnsExpectation(string learnDate, string finDate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(learnDate));

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(DateTime.Parse(finDate));

            var result = sut.ConditionMet(mockDelivery.Object, mockFinRec.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void ValidateWithNullAppFinRecordsDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            const string aFinType = "TNP";
            var aFinDate = string.Empty;
            var learnStartDate = new DateTime(2018, 8, 1);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(learnStartDate);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(FundModels.ApprenticeshipsFrom1May2017);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == AFinType_13Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnStartDate),
                    Moq.It.Is<DateTime>(y => y == learnStartDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.AFinType),
                    Moq.It.Is<string>(y => y == aFinType)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.AFinDate),
                    Moq.It.Is<string>(y => y == aFinDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new AFinType_13Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Fact]
        public void ValidateWithEmptyAppFinRecordsDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            const string aFinType = "TNP";
            var aFinDate = string.Empty;
            var learnStartDate = new DateTime(2018, 8, 1);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
               .SetupGet(x => x.LearnStartDate)
               .Returns(learnStartDate);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(FundModels.ApprenticeshipsFrom1May2017);

            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(new List<IAppFinRecord>());

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == AFinType_13Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnStartDate),
                    Moq.It.Is<DateTime>(y => y == learnStartDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.AFinType),
                    Moq.It.Is<string>(y => y == aFinType)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.AFinDate),
                    Moq.It.Is<string>(y => y == aFinDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new AFinType_13Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Theory]
        [InlineData("2016-04-01", "2016-04-02", ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice)]
        [InlineData("2016-04-01", "2016-03-31", "TNP")]
        [InlineData("2016-04-30", "2016-05-01", "tnp")]
        [InlineData("2016-05-02", "2016-05-01", "tnp")]
        public void InvalidItemRaisesValidationMessage(string learnDate, string finDate, string aFinType)
        {
            const string LearnRefNumber = "123456789X";
            var aFinDate = string.Empty;
            var learnStartDate = DateTime.Parse(learnDate);

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinType)
                .Returns(aFinType);
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(DateTime.Parse(finDate));

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(1);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(36);
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(learnStartDate);
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

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == AFinType_13Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "LearnStartDate"),
                    Moq.It.Is<DateTime>(y => y == learnStartDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "AFinType"),
                    Moq.It.Is<string>(y => y == aFinType.ToUpper())))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "AFinDate"),
                    Moq.It.Is<string>(y => y == aFinDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new AFinType_13Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Theory]
        [InlineData("2016-04-01", "2016-04-01", ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice)]
        [InlineData("2016-05-01", "2016-05-01", "tnp")]
        public void ValidItemDoesNotRaiseAValidationMessage(string learnDate, string finDate, string aFinType)
        {
            const string LearnRefNumber = "123456789X";

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinType)
                .Returns(aFinType);
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(DateTime.Parse(finDate));

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(FundModels.ApprenticeshipsFrom1May2017);
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(learnDate));
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

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new AFinType_13Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public AFinType_13Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new AFinType_13Rule(mock.Object);
        }
    }
}
