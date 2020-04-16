using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    public class UKPRN_08RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("UKPRN_08", result);
        }

        [Theory]
        [InlineData("2016-02-01", "2016-04-04", true)]
        [InlineData("2016-04-03", "2016-04-04", true)]
        [InlineData("2016-04-04", "2016-04-04", false)]
        [InlineData("2016-04-05", "2016-04-04", false)]
        public void IsNotPartOfThisYearMeetExpectdation(string endDate, string commencementDate, bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(DateTime.Parse(endDate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicData
                .Setup(x => x.Start())
                .Returns(DateTime.Parse(commencementDate));

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object);

            var result = sut.IsNotPartOfThisYear(mockItem.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            fileData.VerifyAll();
            academicData.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(1, 2, false)]
        public void HasQualifyingProviderIDMeetsExpectation(int provider, int deliveryID, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<IFcsContractAllocation>();
            mockItem
                .SetupGet(y => y.DeliveryUKPRN)
                .Returns(deliveryID);

            var result = sut.HasQualifyingProviderID(mockItem.Object, provider);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("ALLB1819", false)]
        [InlineData("ALLBC1819", false)]
        [InlineData("ALLB1920", true)]
        [InlineData("ALLBC1920", true)]
        [InlineData("AEBC1920", false)]
        [InlineData("AEBTO-TOL1920", false)]
        [InlineData("AEB-LS1920", false)]
        [InlineData("AEB-TOL1920", false)]
        [InlineData("ANLAP2018", false)]
        [InlineData("APPS1920", false)]
        [InlineData("16-18NLAP2018", false)]
        [InlineData("ESF1420", false)]
        [InlineData("LEVY1799", false)]
        public void HasQualifyingFundingStreamMeetsExpectation(string candidate, bool expected)
        {
            var sut = NewRule();
            var mockItem = new Mock<IFcsContractAllocation>();
            mockItem
                .SetupGet(y => y.FundingStreamPeriodCode)
                .Returns(candidate);

            var result = sut.HasQualifyingFundingStream(mockItem.Object);
            result.Should().Be(expected);
        }

        [Fact]
        public void HasFundingRelationshipWithNullDeliveryReturnsFalse()
        {
            const int testUKPRN = 123;

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(testUKPRN);

            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(testUKPRN))
                .Returns((IReadOnlyCollection<IFcsContractAllocation>)null);

            var sut = new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object);

            var result = sut.HasFundingRelationship(null);

            Assert.False(result);

            handler.VerifyAll();
            fileData.VerifyAll();
            academicData.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData("AEBC1920")]
        [InlineData("AEBTO-TOL1920")]
        [InlineData("AEB-LS1920")]
        [InlineData("AEB-TOL1920")]
        [InlineData("ANLAP2018")]
        [InlineData("APPS1920")]
        [InlineData("16-18NLAP2018")]
        [InlineData("ESF1420")]
        [InlineData("LEVY1799")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            const string contractRef = "shonkyRef12";
            const int providerID = 123;

            var testDate = DateTime.Today;

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(contractRef);
            delivery
                .SetupGet(y => y.LearnActEndDateNullable)
                .Returns(testDate);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.UKPRN_08, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("UKPRN", providerID))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "ALB"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(providerID);

            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicData
                .Setup(x => x.Start())
                .Returns(testDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsLoansBursary(delivery.Object))
                .Returns(true);

            var allocation = new Mock<IFcsContractAllocation>();
            allocation
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(providerID))
                .Returns(new[] { allocation.Object });

            var sut = new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            fileData.VerifyAll();
            academicData.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData("ALLB1920")]
        [InlineData("ALLBC1920")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            const string contractRef = "shonkyRef12";
            const int providerID = 123;

            var testDate = DateTime.Today;

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(contractRef);
            delivery
                .SetupGet(y => y.LearnActEndDateNullable)
                .Returns(testDate);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(providerID);

            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicData
                .Setup(x => x.Start())
                .Returns(testDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsLoansBursary(delivery.Object))
                .Returns(true);

            var allocation = new Mock<IFcsContractAllocation>();
            allocation
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(providerID))
                .Returns(new[] { allocation.Object });

            var sut = new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            fileData.VerifyAll();
            academicData.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
        }

        private UKPRN_08Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var academicData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            return new UKPRN_08Rule(handler.Object, fileData.Object, academicData.Object, commonOps.Object, fcsData.Object);
        }
    }
}
