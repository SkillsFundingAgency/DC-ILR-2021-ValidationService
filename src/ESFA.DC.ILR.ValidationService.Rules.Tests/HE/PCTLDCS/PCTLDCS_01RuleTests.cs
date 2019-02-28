using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCTLDCS;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.PCTLDCS
{
    public class PCTLDCS_01RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new PCTLDCS_01Rule(null, service.Object, commonChecks.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new PCTLDCS_01Rule(handler.Object, null, commonChecks.Object));
        }

        /// <summary>
        /// New rule with null common checks throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedData07Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new PCTLDCS_01Rule(handler.Object, service.Object, null));
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
            Assert.Equal("PCTLDCS_01", result);
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
            Assert.Equal(RuleNameConstants.PCTLDCS_01, result);
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

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// First viable date meets expectation.
        /// </summary>
        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            // arrange / act
            var result = PCTLDCS_01Rule.FirstViableDate;

            // assert
            Assert.Equal(DateTime.Parse("2009-08-01"), result);
        }

        /// <summary>
        /// Has known LDCS code meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData(null, true)]
        [InlineData("testAim1", false)]
        [InlineData("testAim1", true)]
        [InlineData("testAim2", false)]
        [InlineData("testAim2", true)]
        public void HasKnownLDCSCodeMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.HasKnownLearnDirectClassSystemCode3For(candidate))
                .Returns(expectation);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new PCTLDCS_01Rule(handler.Object, service.Object, commonChecks.Object);

            // act
            var result = sut.HasKnownLDCSCode(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying PCTLDCS with null meets expectation
        /// </summary>
        [Fact]
        public void HasQualifyingPCTLDCSWithNullMeetsExpectation()
        {
            // arrange
            var sut = NewRule();
            decimal? pCTLDCSValue = null;

            var mockHE = new Mock<ILearningDeliveryHE>();
            mockHE
                .SetupGet(x => x.PCTLDCSNullable)
                .Returns(pCTLDCSValue);

            // act
            var result = sut.HasQualifyingPCTLDCSNull(mockHE.Object);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Has qualifying PCTLDCS with null learner HE meets expectation
        /// </summary>
        [Fact]
        public void HasQualifyingPCTLDCSWithNullLearnerHEMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingPCTLDCSNull(null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has qualifying PCTLDCS with value meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(9.2)]
        [InlineData(33.06)]
        [InlineData(19)]
        [InlineData(123769.23456)]
        public void HasQualifyingPCTLDCSWithValueMeetsExpectation(double candidate)
        {
            // arrange
            var sut = NewRule();

            var mockHE = new Mock<ILearningDeliveryHE>();
            mockHE
                .SetupGet(x => x.PCTLDCSNullable)
                .Returns((decimal)candidate);

            // act
            var result = sut.HasQualifyingPCTLDCSNull(mockHE.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
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
                .Returns(TypeOfFunding.AdultSkills);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockHE.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

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
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 35)) // TypeOfFunding.AdultSkills
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.HasKnownLearnDirectClassSystemCode3For(learnAimRef))
                .Returns(true);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, PCTLDCS_01Rule.FirstViableDate, null))
                .Returns(true);

            var sut = new PCTLDCS_01Rule(handler.Object, service.Object, commonChecks.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
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
                .Returns(TypeOfFunding.AdultSkills);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockHE.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.HasKnownLearnDirectClassSystemCode3For(learnAimRef))
                .Returns(true);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, PCTLDCS_01Rule.FirstViableDate, null))
                .Returns(true);

            var sut = new PCTLDCS_01Rule(handler.Object, service.Object, commonChecks.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public PCTLDCS_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new PCTLDCS_01Rule(handler.Object, service.Object, commonChecks.Object);
        }
    }
}
