using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    /// <summary>
    /// learn start date rule 16 tests
    /// </summary>
    public class LearnStartDate_16RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_16Rule(null, fcsData.Object, commonOps.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 18 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule18Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_16Rule(handler.Object, null, commonOps.Object));
        }

        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_16Rule(handler.Object, fcsData.Object, null));
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
            Assert.Equal("LearnStartDate_16", result);
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
            Assert.Equal(RuleNameConstants.LearnStartDate_16, result);
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
        /// Validate with null learner throws.
        /// </summary>
        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Has qualifying model meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingModelMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockItem.Object, 70))
                .Returns(expectation);

            var sut = new LearnStartDate_16Rule(handler.Object, fcsData.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingModel(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has qualifying aim meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData("ZESF0001", true)] // TypeOfAim.References.ESFLearnerStartandAssessment
        [InlineData("Z0002347", false)] // TypeOfAim.References.SupportedInternship16To19
        [InlineData("Z0007834", false)] // TypeOfAim.References.WorkPlacement0To49Hours
        [InlineData("Z0007835", false)] // TypeOfAim.References.WorkPlacement50To99Hours
        [InlineData("Z0007836", false)] // TypeOfAim.References.WorkPlacement100To199Hours
        [InlineData("Z0007837", false)] // TypeOfAim.References.WorkPlacement200To499Hours
        [InlineData("Z0007838", false)] // TypeOfAim.References.WorkPlacement500PlusHours
        [InlineData("ZWRKX001", false)] // TypeOfAim.References.WorkExperience
        [InlineData("ZWRKX002", false)] // TypeOfAim.References.IndustryPlacement
        public void HasQualifyingAimMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(candidate);

            // act
            var result = sut.HasQualifyingAim(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying start meets expectation
        /// </summary>
        /// <param name="contractRef">The contract reference.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("Z32cty", "2017-12-31", true)]
        [InlineData("Btr4567", "2017-12-31", false)]
        [InlineData("Byfru", "2018-01-01", true)]
        [InlineData("MdR4es23", "2018-01-01", false)]
        [InlineData("Pfke^5b", "2018-02-01", true)]
        [InlineData("Ax3gBu6", "2018-02-01", false)]
        public void HasQualifyingStartMeetsExpectation(string contractRef, string candidate, bool expectation)
        {
            // arrange
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
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, testDate, null))
                .Returns(expectation);

            var sut = new LearnStartDate_16Rule(handler.Object, fcsData.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingStart(delivery.Object, allocation.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            fcsData.VerifyAll();
            commonOps.VerifyAll();

            allocation.VerifyGet(x => x.StartDate, Times.AtLeastOnce);
        }

        /// <summary>
        /// Has qualifying start with null allocations meets expectation
        /// </summary>
        [Fact]
        public void HasQualifyingStartWithNullAllocationsMeetsExpectation()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnStartDate_16Rule(handler.Object, fcsData.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingStart(delivery.Object, null);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            fcsData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="contractRef">The contract reference.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("Z32cty", "2017-12-31")]
        [InlineData("Btr4567", "2017-12-31")]
        [InlineData("Byfru", "2018-01-01")]
        [InlineData("MdR4es23", "2018-01-01")]
        [InlineData("Pfke^5b", "2018-02-01")]
        [InlineData("Ax3gBu6", "2018-02-01")]
        public void InvalidItemRaisesValidationMessage(string contractRef, string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(70); // TypeOfFunding.EuropeanSocialFund
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns("ZESF0001"); // TypeOfAim.References.ESFLearnerStartandAssessment
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

            var allocations = Collection.Empty<IFcsContractAllocation>();
            allocations.Add(allocation.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(delivery.Object);
            var safeDeliveries = deliveries.AsSafeReadOnlyList();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(safeDeliveries);

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

            // pass or fail is based on the return of this function
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 70)) // TypeOfFunding.EuropeanSocialFund
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, testDate, null))
                .Returns(false);

            var sut = new LearnStartDate_16Rule(handler.Object, fcsData.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();
            commonOps.VerifyAll();

            allocation.VerifyGet(x => x.StartDate, Times.AtLeastOnce);
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="contractRef">The contract reference.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("Z32cty", "2017-12-31")]
        [InlineData("Btr4567", "2017-12-31")]
        [InlineData("Byfru", "2018-01-01")]
        [InlineData("MdR4es23", "2018-01-01")]
        [InlineData("Pfke^5b", "2018-02-01")]
        [InlineData("Ax3gBu6", "2018-02-01")]
        public void ValidItemDoesNotRaiseValidationMessage(string contractRef, string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns("ZESF0001"); // TypeOfAim.References.ESFLearnerStartandAssessment
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

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(delivery.Object);
            var safeDeliveries = deliveries.AsSafeReadOnlyList();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(safeDeliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationFor(contractRef))
                .Returns(allocation.Object);

            // pass or fail is based on the return of this function
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 70)) // TypeOfFunding.EuropeanSocialFund
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, testDate, null))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, testDate, null))
                .Returns(true);

            var sut = new LearnStartDate_16Rule(handler.Object, fcsData.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();
            commonOps.VerifyAll();

            allocation.VerifyGet(x => x.StartDate, Times.AtLeastOnce);
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnStartDate_16Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnStartDate_16Rule(handler.Object, fcsData.Object, commonOps.Object);
        }
    }
}
