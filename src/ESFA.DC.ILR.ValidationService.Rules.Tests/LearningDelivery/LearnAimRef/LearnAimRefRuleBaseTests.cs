using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRefRuleBaseTests
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
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleBaseTestRule(null, provider.Object, service.Object));
        }

        /// <summary>
        /// New rule with null action provider throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullActionProviderThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleBaseTestRule(handler.Object, null, service.Object));
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
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleBaseTestRule(handler.Object, provider.Object, null));
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
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="category">The category.</param>
        [Theory]
        [InlineData("ADULT_SKILLS")] // TypeOfLARSValidity.AdultSkills
        [InlineData("ADV_LEARN_LOAN")] // TypeOfLARSValidity.AdvancedLearnerLoan
        [InlineData("ANY")] // TypeOfLARSValidity.Any
        [InlineData("APPRENTICESHIPS")] // TypeOfLARSValidity.Apprenticeships
        [InlineData("COMM_LEARN")] // TypeOfLARSValidity.CommunityLearning
        [InlineData("1619_EFA")] // TypeOfLARSValidity.EFA16To19
        [InlineData("EFACONFUNDENGLISH")] // TypeOfLARSValidity.EFAConFundEnglish
        [InlineData("EFACONFUNDMATHS")] // TypeOfLARSValidity.EFAConFundMaths
        [InlineData("ESF")] // TypeOfLARSValidity.EuropeanSocialFund
        [InlineData("OLASS_ADULT")] // TypeOfLARSValidity.OLASSAdult
        [InlineData("UNEMPLOYED")] // TypeOfLARSValidity.Unemployed
        public void InvalidItemRaisesValidationMessage(string category)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

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
                .Setup(x => x.Handle("LearnAimRefRuleBaseTestRule", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
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

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleBaseTestRule(handler.Object, provider.Object, service.Object);

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
        /// <param name="category">The category.</param>
        [Theory]
        [InlineData("ADULT_SKILLS")] // TypeOfLARSValidity.AdultSkills
        [InlineData("ADV_LEARN_LOAN")] // TypeOfLARSValidity.AdvancedLearnerLoan
        [InlineData("ANY")] // TypeOfLARSValidity.Any
        [InlineData("APPRENTICESHIPS")] // TypeOfLARSValidity.Apprenticeships
        [InlineData("COMM_LEARN")] // TypeOfLARSValidity.CommunityLearning
        [InlineData("1619_EFA")] // TypeOfLARSValidity.EFA16To19
        [InlineData("EFACONFUNDENGLISH")] // TypeOfLARSValidity.EFAConFundEnglish
        [InlineData("EFACONFUNDMATHS")] // TypeOfLARSValidity.EFAConFundMaths
        [InlineData("ESF")] // TypeOfLARSValidity.EuropeanSocialFund
        [InlineData("OLASS_ADULT")] // TypeOfLARSValidity.OLASSAdult
        [InlineData("UNEMPLOYED")] // TypeOfLARSValidity.Unemployed
        public void ValidItemDoesNotRaiseValidationMessage(string category)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);

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

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleBaseTestRule(handler.Object, provider.Object, service.Object);

            // we have to ensure the secondary conditions check generate a 'pass' value
            sut.SetPassValue(true);

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
        public LearnAimRefRuleBaseTestRule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLearnAimRefRuleActions>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new LearnAimRefRuleBaseTestRule(handler.Object, provider.Object, service.Object);
        }
    }
}
