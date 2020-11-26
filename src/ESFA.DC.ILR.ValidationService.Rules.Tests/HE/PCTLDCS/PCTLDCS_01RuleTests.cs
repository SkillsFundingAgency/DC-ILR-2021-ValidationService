using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCTLDCS;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.PCTLDCS
{
    public class PCTLDCS_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("PCTLDCS_01", result);
        }

        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            var result = PCTLDCS_01Rule.FirstViableDate;

            Assert.Equal(DateTime.Parse("2009-08-01"), result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(null, true)]
        [InlineData("testAim1", false)]
        [InlineData("testAim1", true)]
        [InlineData("testAim2", false)]
        [InlineData("testAim2", true)]
        public void HasKnownLDCSCodeMeetsExpectation(string candidate, bool expectation)
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.HasKnownLearnDirectClassSystemCode3For(candidate))
                .Returns(expectation);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new PCTLDCS_01Rule(handler.Object, service.Object, dateTimeQS.Object);

            var result = sut.HasKnownLDCSCode(mockDelivery.Object);

            handler.VerifyAll();
            service.VerifyAll();
            dateTimeQS.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasQualifyingPCTLDCSWithNullMeetsExpectation()
        {
            var sut = NewRule();
            decimal? pCTLDCSValue = null;

            var mockHE = new Mock<ILearningDeliveryHE>();
            mockHE
                .SetupGet(x => x.PCTLDCSNullable)
                .Returns(pCTLDCSValue);

            var result = sut.HasQualifyingPCTLDCSNull(mockHE.Object);

            Assert.True(result);
        }

        [Fact]
        public void HasQualifyingPCTLDCSWithNullLearnerHEMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.HasQualifyingPCTLDCSNull(null);

            Assert.False(result);
        }

        [Theory]
        [InlineData(9.2)]
        [InlineData(33.06)]
        [InlineData(19)]
        [InlineData(123769.23456)]
        public void HasQualifyingPCTLDCSWithValueMeetsExpectation(double candidate)
        {
            var sut = NewRule();

            var mockHE = new Mock<ILearningDeliveryHE>();
            mockHE
                .SetupGet(x => x.PCTLDCSNullable)
                .Returns((decimal)candidate);

            var result = sut.HasQualifyingPCTLDCSNull(mockHE.Object);

            Assert.False(result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse("2016-08-01");

            var mockHE = new Mock<ILearningDeliveryHE>();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockHE.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.PCTLDCS_01, learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 35))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.HasKnownLearnDirectClassSystemCode3For(learnAimRef))
                .Returns(true);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, PCTLDCS_01Rule.FirstViableDate, DateTime.MaxValue, true))
                .Returns(true);

            var sut = new PCTLDCS_01Rule(handler.Object, service.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse("2016-08-01");

            var mockHE = new Mock<ILearningDeliveryHE>();
            mockHE
                .SetupGet(x => x.PCTLDCSNullable)
                .Returns(18.345M);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockHE.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.HasKnownLearnDirectClassSystemCode3For(learnAimRef))
                .Returns(true);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, PCTLDCS_01Rule.FirstViableDate, DateTime.MaxValue, true))
                .Returns(true);

            var sut = new PCTLDCS_01Rule(handler.Object, service.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        public PCTLDCS_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            return new PCTLDCS_01Rule(handler.Object, service.Object, dateTimeQS.Object);
        }
    }
}
