using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_88RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_88Rule(null, provider.Object, service.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_88Rule(handler.Object, null, service.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_88Rule(handler.Object, provider.Object, null));
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
            Assert.Equal("LearnAimRef_88", result);
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
            Assert.Equal(RuleNameConstants.LearnAimRef_88, result);
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
        /// Has valid start range with validity last new start date meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", true)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", true)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", true)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", false)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", false)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", false)]
        public void HasValidStartRangeWithValidityLastNewStartDateMeetsExpectation(string candidate, string startDate, string endDate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockValidity = new Mock<ILARSLearningDeliveryValidity>(MockBehavior.Strict);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns((DateTime?)null);
            mockValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns(DateTime.Parse(endDate));

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.HasValidStartRange(mockValidity.Object, mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has valid start range with validity end date meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", true)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", true)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", true)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", false)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", false)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", false)]

        // the next 4 test cases account for the 'custom and practice' of
        // withdrawing funding for a learning aim by shifting the end date
        // of the validty to one day before the start date
        [InlineData("2013-07-17", "2013-07-16", "2013-07-15", false)]
        [InlineData("2013-07-16", "2013-07-16", "2013-07-15", false)]
        [InlineData("2013-07-15", "2013-07-16", "2013-07-15", false)]
        [InlineData("2013-07-14", "2013-07-16", "2013-07-15", false)]
        public void HasValidStartRangeWithValidityEndDateMeetsExpectation(string candidate, string startDate, string endDate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            // for testing, effective from and effect to,
            // this establishes whether the item is current or not
            var mockValidity = new Mock<ILARSLearningDeliveryValidity>(MockBehavior.Strict);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.Parse(endDate));
            mockValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns((DateTime?)null);

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.HasValidStartRange(mockValidity.Object, mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has valid learning aim meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", true)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", true)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", true)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", false)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", false)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", false)]
        public void HasValidLearningAimMeetsExpectation(string candidate, string startDate, string endDate, bool expectation)
        {
            // arrange
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var category = "larsCat";
            var mockValidity = new Mock<ILARSLearningDeliveryValidity>(MockBehavior.Strict);
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.Parse(endDate));
            mockValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns((DateTime?)null);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            larsValidities.Add(mockValidity.Object);

            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var sut = new LearnAimRef_88Rule(handler.Object, provider.Object, service.Object);

            // act
            var result = sut.HasValidLearningAim(mockDelivery.Object, category);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            service.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="category">The category.</param>
        [Theory]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfLARSValidity.EFA16To19)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfLARSValidity.EFA16To19)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfLARSValidity.AdvancedLearnerLoan)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfLARSValidity.AdvancedLearnerLoan)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfLARSValidity.AdvancedLearnerLoan)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfLARSValidity.Any)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfLARSValidity.Any)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfLARSValidity.Any)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfLARSValidity.Apprenticeships)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfLARSValidity.Apprenticeships)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfLARSValidity.Apprenticeships)]
        [InlineData("2014-04-01", "2015-05-09", "2016-07-15", TypeOfLARSValidity.Unemployed)]
        [InlineData("2011-09-01", "2016-07-14", "2016-07-15", TypeOfLARSValidity.Unemployed)]
        [InlineData("2013-07-16", "2015-05-09", "2015-07-15", TypeOfLARSValidity.Unemployed)]
        public void InvalidItemRaisesValidationMessage(string candidate, string startDate, string endDate, string category)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

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
                .Setup(x => x.Handle("LearnAimRef_88", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("Expected Category", category))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockResult = new Mock<IBranchResult>();
            mockResult
                .SetupGet(x => x.OutOfScope)
                .Returns(false);
            mockResult
                .SetupGet(x => x.Category)
                .Returns(category);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            provider
                .Setup(x => x.GetBranchingResultFor(mockDelivery.Object, mockLearner.Object))
                .Returns(mockResult.Object);

            var mockValidity = new Mock<ILARSLearningDeliveryValidity>(MockBehavior.Strict);
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.Parse(endDate));
            mockValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns((DateTime?)null);

            var larsValidities = new ILARSLearningDeliveryValidity[]
            {
                mockValidity.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);

            var sut = new LearnAimRef_88Rule(handler.Object, provider.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="category">The category.</param>
        [Theory]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfLARSValidity.EFA16To19)]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfLARSValidity.AdvancedLearnerLoan)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfLARSValidity.AdvancedLearnerLoan)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfLARSValidity.AdvancedLearnerLoan)]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfLARSValidity.CommunityLearning)]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfLARSValidity.Any)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfLARSValidity.Any)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfLARSValidity.Any)]
        [InlineData("2014-04-01", "2012-05-09", "2016-07-15", TypeOfLARSValidity.Unemployed)]
        [InlineData("2014-04-01", "2012-05-09", "2015-11-10", TypeOfLARSValidity.Unemployed)]
        [InlineData("2014-04-01", "2012-05-09", "2014-09-07", TypeOfLARSValidity.Unemployed)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, string startDate, string endDate, string category)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockResult = new Mock<IBranchResult>();
            mockResult
                .SetupGet(x => x.OutOfScope)
                .Returns(false);
            mockResult
                .SetupGet(x => x.Category)
                .Returns(category);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            provider
                .Setup(x => x.GetBranchingResultFor(mockDelivery.Object, mockLearner.Object))
                .Returns(mockResult.Object);

            var mockValidity = new Mock<ILARSLearningDeliveryValidity>(MockBehavior.Strict);
            mockValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(category);
            mockValidity
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(startDate));
            mockValidity
                .SetupGet(x => x.EndDate)
                .Returns(DateTime.Parse(endDate));
            mockValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns((DateTime?)null);

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
            larsValidities.Add(mockValidity.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());

            var sut = new LearnAimRef_88Rule(handler.Object, provider.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRef_88Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new LearnAimRef_88Rule(handler.Object, provider.Object, service.Object);
        }
    }
}
