using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using IEsfEligibilityRuleLocalAuthority = ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface.IEsfEligibilityRuleLocalAuthority;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_17RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DelLocPostCode_17Rule(null, common.Object, fcsData.Object, postcodes.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DelLocPostCode_17Rule(handler.Object, null, fcsData.Object, postcodes.Object));
        }

        /// <summary>
        /// New rule with null contract data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullContractDataThrows()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DelLocPostCode_17Rule(handler.Object, common.Object, null, postcodes.Object));
        }

        /// <summary>
        /// New rule with null postcode data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullPostcodeDataThrows()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, null));
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
            Assert.Equal("DelLocPostCode_17", result);
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
            Assert.Equal(RuleNameConstants.DelLocPostCode_17, result);
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
        /// Gets the qualifying aim meets null expectation.
        /// </summary>
        [Fact]
        public void GetQualifyingAimMeetsNullExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.GetQualifyingAim(null);

            // assert
            Assert.Null(result);
        }

        /// <summary>
        /// Get qualifying aim meets empty expectation.
        /// </summary>
        [Fact]
        public void GetQualifyingAimMeetsEmptyExpectation()
        {
            // arrange
            var deliveries = Collection.EmptyAndReadOnly<ILearningDelivery>();
            var sut = NewRule();

            // act
            var result = sut.GetQualifyingAim(deliveries);

            // assert
            Assert.Null(result);
        }

        /// <summary>
        /// Get eligibility item meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("TestThingy1")]
        [InlineData("TestThingy2")]
        [InlineData("TestThingy3")]
        [InlineData("TestThingy4")]
        public void GetEligibilityItemMeetsExpectation(string candidate)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(candidate);

            var expectation = new[] { new Mock<IEsfEligibilityRuleLocalAuthority>().Object };

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleLocalAuthoritiesFor(candidate))
                .Returns(expectation);

            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object);

            // act
            var result = sut.GetEligibilityItemsFor(delivery.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodes.VerifyAll();

            Assert.IsAssignableFrom<IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority>>(result);
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get ons postcode meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("TestThingy1")]
        [InlineData("TestThingy2")]
        [InlineData("TestThingy3")]
        [InlineData("TestThingy4")]
        public void GetONSPostcodeMeetsExpectation(string candidate)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.DelLocPostCode)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            postcodes
                .Setup(x => x.GetONSPostcodes(candidate))
                .Returns(new[] { new Mock<IONSPostcode>().Object });

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object);

            // act
            var result = sut.GetONSPostcodes(delivery.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodes.VerifyAll();

            Assert.IsAssignableFrom<IONSPostcode[]>(result);
        }

        /// <summary>
        /// Has qualifying eligibility meets null postcode expectation
        /// </summary>
        [Fact]
        public void HasQualifyingEligibilityMeetsNullPostcodeExpectation()
        {
            // arrange
            var sut = NewRule();
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(new DateTime(2018, 09, 01));

            // act
            var result = sut.HasQualifyingEligibility(delivery.Object, null, Collection.EmptyAndReadOnly<IEsfEligibilityRuleLocalAuthority>());

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has qualifying eligibility meets null eligibility expectation
        /// </summary>
        [Fact]
        public void HasQualifyingEligibilityMeetsNullEligibilityExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingEligibility(new Mock<ILearningDelivery>().Object, new[] { new Mock<IONSPostcode>().Object }, null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has qualifying eligibility meets null learningdelivery expectation
        /// </summary>
        [Fact]
        public void HasQualifyingEligibilityMeetsNullLearningDeliveryExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingEligibility(null, new[] { new Mock<IONSPostcode>().Object }, Collection.EmptyAndReadOnly<IEsfEligibilityRuleLocalAuthority>());

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has qualifying eligibility meets expectation
        /// </summary>
        /// <param name="learnStartDateString">The learn start date.</param>
        /// <param name="elCode">The el authority.</param>
        /// <param name="pcCode">The pc authority.</param>
        /// <param name="termination">The termination date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2018-10-01", "ESF0002", "tt_9972", "2018-10-02", false)]
        [InlineData("2019-10-01", "tt_9972", "ESF0002", "2018-10-02", false)]
        [InlineData("2018-09-01", "TT_9972", "tt_9972", "2018-10-01", true)]
        [InlineData("2018-11-02", "tt_9972", "TT_9972", "2018-10-02", false)]
        [InlineData("2018-09-02", "tt_9972", "tt_9972", "2018-10-02", true)]
        [InlineData("2018-09-02", "TT_9973", "tt_9972", null, false)]
        [InlineData("2018-09-02", "tt_9972", "TT_9973", null, false)]
        [InlineData("2018-09-02", "tt_9973", "tt_9972", null, false)]
        [InlineData("2018-09-02", "tt_9972", "tt_9973", null, false)]
        public void HasQualifyingEligibilityMeetsExpectation(string learnStartDateString, string elCode, string pcCode, string termination, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var learnStartDate = DateTime.Parse(learnStartDateString);
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(learnStartDate);
            var effectiveFrom = new DateTime(2018, 09, 01);
            var effectiveTo = new DateTime(2018, 11, 01);
            var terminationDate = string.IsNullOrEmpty(termination) ? (DateTime?)null : DateTime.Parse(termination);
            var postcode = new Mock<IONSPostcode>();
            postcode
                .SetupGet(x => x.LocalAuthority)
                .Returns(pcCode);
            postcode
                .SetupGet(x => x.EffectiveFrom)
                .Returns(effectiveFrom);
            postcode
                .SetupGet(x => x.EffectiveTo)
                .Returns(effectiveTo);
            postcode
                .SetupGet(x => x.Termination)
                .Returns(terminationDate);
            var authority = new Mock<IEsfEligibilityRuleLocalAuthority>();
            authority
                .SetupGet(x => x.Code)
                .Returns(elCode);

            // act
            var result = sut.HasQualifyingEligibility(delivery.Object, new[] { postcode.Object }, new[] { authority.Object });

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// In qualifying period meets expectation.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="termination">Termination.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-02-29", "2016-03-01", "2016-03-10", "2016-03-10", false)]
        [InlineData("2016-03-11", "2016-03-01", "2016-03-10", "2016-03-10", false)]
        [InlineData("2016-03-06", "2016-03-01", "2016-03-10", "2016-03-10", true)]
        [InlineData("2016-03-03", "2016-03-01", null, "2016-03-10", true)]
        [InlineData("2016-02-28", "2016-02-28", "2016-03-01", "2016-03-01", true)]
        [InlineData("2016-02-26", "2016-02-27", "2016-03-01", "2016-03-01", false)]
        [InlineData("2016-02-29", "2016-02-28", null, "2016-02-29", true)]
        [InlineData("2016-03-01", "2016-02-28", null, "2016-02-29", false)]
        public void InQualifyingPeriodMeetsExpectation(string startDate, string from, string to, string termination, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            var toDate = string.IsNullOrWhiteSpace(to)
                ? (DateTime?)null
                : DateTime.Parse(to);
            var terminationDate = string.IsNullOrWhiteSpace(termination)
                ? (DateTime?)null
                : DateTime.Parse(termination);

            var postcode = new Mock<IONSPostcode>();
            postcode
                .SetupGet(x => x.EffectiveFrom)
                .Returns(DateTime.Parse(from));
            postcode
                .SetupGet(x => x.EffectiveTo)
                .Returns(toDate);
            postcode
                .SetupGet(x => x.Termination)
                .Returns(terminationDate);

            // act
            var result = sut.InQualifyingPeriod(mockDelivery.Object, postcode.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="termination">The termination date.</param>
        [Theory]
        [InlineData("2016-04-01", "2016-02-28", "2016-03-10", "2016-03-01")]
        [InlineData("2016-01-01", "2016-02-01", null, "2016-03-01")]
        [InlineData("2016-01-01", "2016-02-01", null, null)]
        public void InvalidItemRaisesValidationMessage(string startDate, string from, string to, string termination)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string localAuthority = "LA0001";
            const string delLocPC = "testPostcode";
            const string conRefNum = "tt_1234";
            const string learnAimRef = "ZESF0001";
            const int testFunding = 70; // TypeOfFunding.EuropeanSocialFund

            var learnStart = DateTime.Parse(startDate);
            var terminationDate = string.IsNullOrEmpty(termination) ? (DateTime?)null : DateTime.Parse(termination);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(testFunding);
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(x => x.CompStatus)
                .Returns(2); // has completed
            mockDelivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(conRefNum);
            mockDelivery
                .SetupGet(x => x.DelLocPostCode)
                .Returns(delLocPC);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStart);

            var toDate = string.IsNullOrWhiteSpace(to)
                ? (DateTime?)null
                : DateTime.Parse(to);

            var postcode = new Mock<IONSPostcode>();
            postcode
                .SetupGet(x => x.LocalAuthority)
                .Returns(localAuthority);
            postcode
                .SetupGet(x => x.EffectiveFrom)
                .Returns(DateTime.Parse(from));
            postcode
                .SetupGet(x => x.EffectiveTo)
                .Returns(toDate);
            postcode
                .SetupGet(x => x.Termination)
                .Returns(terminationDate);

            var postcodes = new IONSPostcode[] { postcode.Object };

            var authority = new Mock<IEsfEligibilityRuleLocalAuthority>();
            authority
                .SetupGet(x => x.Code)
                .Returns(localAuthority);

            var authorities = new IEsfEligibilityRuleLocalAuthority[] { authority.Object };
            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.DelLocPostCode_17, learnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", testFunding))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("DelLocPostCode", delLocPC))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ConRefNumber", conRefNum))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DelLocPostCode_17Rule.FirstViableDate, null))
                .Returns(true);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleLocalAuthoritiesFor(conRefNum))
                .Returns(authorities);

            var postcodesds = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            postcodesds
                .Setup(x => x.GetONSPostcodes(delLocPC))
                .Returns(postcodes);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodesds.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodesds.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="termination">The termination date.</param>
        [Theory]
        [InlineData("2016-02-28", "2016-02-28", "2016-03-01", "2016-03-26")]
        [InlineData("2016-02-28", "2016-02-27", "2016-03-01", "2016-03-26")]
        [InlineData("2016-02-28", "2016-02-28", null, "2016-03-26")]
        [InlineData("2016-02-28", "2016-02-27", null, "2016-03-26")]
        [InlineData("2016-02-28", "2016-02-28", null, null)]
        [InlineData("2016-02-28", "2016-02-27", null, null)]
        public void ValidItemDoesNotRaiseAValidationMessage(string startDate, string from, string to, string termination)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string localAuthority = "LA0001";
            const string delLocPC = "testPostcode";
            const string conRefNum = "tt_1234";
            const string learnAimRef = "ZESF0001";
            const int testFunding = 70; // TypeOfFunding.EuropeanSocialFund

            var learnStart = DateTime.Parse(startDate);
            var terminationDate = string.IsNullOrEmpty(termination) ? (DateTime?)null : DateTime.Parse(termination);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(testFunding);
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(x => x.CompStatus)
                .Returns(2); // has completed
            mockDelivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(conRefNum);
            mockDelivery
                .SetupGet(x => x.DelLocPostCode)
                .Returns(delLocPC);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStart);

            var toDate = string.IsNullOrWhiteSpace(to)
                ? (DateTime?)null
                : DateTime.Parse(to);

            var postcode = new Mock<IONSPostcode>();
            postcode
                .SetupGet(x => x.LocalAuthority)
                .Returns(localAuthority);
            postcode
                .SetupGet(x => x.EffectiveFrom)
                .Returns(DateTime.Parse(from));
            postcode
                .SetupGet(x => x.EffectiveTo)
                .Returns(toDate);
            postcode
                .SetupGet(x => x.Termination)
                .Returns(terminationDate);

            var postcodes = new IONSPostcode[] { postcode.Object };

            var authority = new Mock<IEsfEligibilityRuleLocalAuthority>();
            authority
                .SetupGet(x => x.Code)
                .Returns(localAuthority);

            var authorities = new IEsfEligibilityRuleLocalAuthority[] { authority.Object };
            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DelLocPostCode_17Rule.FirstViableDate, null))
                .Returns(true);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleLocalAuthoritiesFor(conRefNum))
                .Returns(authorities);

            var postcodesds = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            postcodesds
                .Setup(x => x.GetONSPostcodes(delLocPC))
                .Returns(postcodes);

            var sut = new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodesds.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
            fcsData.VerifyAll();
            postcodesds.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public DelLocPostCode_17Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var postcodes = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            return new DelLocPostCode_17Rule(handler.Object, common.Object, fcsData.Object, postcodes.Object);
        }
    }
}
