using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_16RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnStartDate_16", result);
        }

        [Theory]
        [InlineData(70, true)]
        [InlineData(35, false)]
        public void HasQualifyingModelMeetsExpectation(int fundModel, bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .Setup(x => x.FundModel).Returns(fundModel);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new LearnStartDate_16Rule(handler.Object, fcsData.Object, dateTimeQS.Object);
            var result = sut.HasQualifyingModel(mockItem.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("ZESF0001", true)]
        [InlineData("Z0002347", false)]
        [InlineData("Z0007834", false)]
        [InlineData("Z0007835", false)]
        [InlineData("Z0007836", false)]
        [InlineData("Z0007837", false)]
        [InlineData("Z0007838", false)]
        [InlineData("ZWRKX001", false)]
        [InlineData("ZWRKX002", false)]
        public void HasQualifyingAimMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(candidate);

            var result = sut.HasQualifyingAim(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("Z32cty", "2017-12-31", true)]
        [InlineData("Btr4567", "2017-12-31", false)]
        [InlineData("Byfru", "2018-01-01", true)]
        [InlineData("MdR4es23", "2018-01-01", false)]
        [InlineData("Pfke^5b", "2018-02-01", true)]
        [InlineData("Ax3gBu6", "2018-02-01", false)]
        public void HasQualifyingStartMeetsExpectation(string contractRef, string candidate, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(contractRef);

            var testDate = DateTime.Parse(candidate);

            var allocation = new Mock<IFcsContractAllocation>();
            allocation
                .SetupGet(x => x.StartDate)
                .Returns(testDate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(delivery.Object.LearnStartDate, testDate, DateTime.MaxValue, true))
                .Returns(expectation);

            var sut = new LearnStartDate_16Rule(handler.Object, fcsData.Object, dateTimeQS.Object);

            var result = sut.HasQualifyingStart(delivery.Object, allocation.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            fcsData.VerifyAll();
            dateTimeQS.VerifyAll();

            allocation.VerifyGet(x => x.StartDate, Times.AtLeastOnce);
        }

        [Fact]
        public void HasQualifyingStartWithNullAllocationsMeetsExpectation()
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new LearnStartDate_16Rule(handler.Object, fcsData.Object, dateTimeQS.Object);

            var result = sut.HasQualifyingStart(delivery.Object, null);

            Assert.False(result);

            handler.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData("Z32cty", "2017-12-31")]
        [InlineData("Btr4567", "2017-12-31")]
        [InlineData("Byfru", "2018-01-01")]
        [InlineData("MdR4es23", "2018-01-01")]
        [InlineData("Pfke^5b", "2018-02-01")]
        [InlineData("Ax3gBu6", "2018-02-01")]
        public void InvalidItemRaisesValidationMessage(string contractRef, string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(70);
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns("ZESF0001");
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(contractRef);
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(contractRef);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            var allocation = new Mock<IFcsContractAllocation>();
            allocation
                .SetupGet(x => x.StartDate)
                .Returns(testDate);

            var allocations = new List<IFcsContractAllocation>();
            allocations.Add(allocation.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(delivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnStartDate_16, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationFor(contractRef))
                .Returns(allocation.Object);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(delivery.Object.LearnStartDate, testDate, DateTime.MaxValue, true))
                .Returns(false);

            var sut = new LearnStartDate_16Rule(handler.Object, fcsData.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            fcsData.VerifyAll();

            allocation.VerifyGet(x => x.StartDate, Times.AtLeastOnce);
        }

        [Theory]
        [InlineData("Z32cty", "2017-12-31")]
        [InlineData("Btr4567", "2017-12-31")]
        [InlineData("Byfru", "2018-01-01")]
        [InlineData("MdR4es23", "2018-01-01")]
        [InlineData("Pfke^5b", "2018-02-01")]
        [InlineData("Ax3gBu6", "2018-02-01")]
        public void ValidItemDoesNotRaiseValidationMessage(string contractRef, string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns("ZESF0001");
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(contractRef);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(70);

            var allocation = new Mock<IFcsContractAllocation>();
            allocation
                .SetupGet(x => x.StartDate)
                .Returns(testDate);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(delivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationFor(contractRef))
                .Returns(allocation.Object);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(delivery.Object.LearnStartDate, testDate, DateTime.MaxValue, true))
                .Returns(true);

            var sut = new LearnStartDate_16Rule(handler.Object, fcsData.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            fcsData.VerifyAll();

            allocation.VerifyGet(x => x.StartDate, Times.AtLeastOnce);
        }

        public LearnStartDate_16Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            return new LearnStartDate_16Rule(handler.Object, fcsData.Object, dateTimeQS.Object);
        }
    }
}
