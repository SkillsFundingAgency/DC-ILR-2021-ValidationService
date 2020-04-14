using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_01RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_01Rule(null));
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
            Assert.Equal("LearnDelFAMType_01", result);
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
            Assert.Equal(LearnDelFAMType_01Rule.Name, result);
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
        /// Condition met with null financial record returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullFinancialRecordReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with fam record meets expectation.
        /// </summary>
        /// <param name="famType">Type of the fam.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.Types.ApprenticeshipContract, false)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, false)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, false)]
        [InlineData(Monitoring.Delivery.Types.CommunityLearningProvision, false)]
        [InlineData(Monitoring.Delivery.Types.HouseholdSituation, false)]
        [InlineData(Monitoring.Delivery.Types.Learning, false)]
        [InlineData(Monitoring.Delivery.Types.LearningSupportFunding, false)]
        [InlineData(Monitoring.Delivery.Types.Restart, false)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding, true)]
        public void ConditionMetWithFAMRecordMeetsExpectation(string famType, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(famType);

            // act
            var result = sut.ConditionMet(mockFAM.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Determines whether [is funded meets expectation] [(for) the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, true)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, true)]
        [InlineData(TypeOfFunding.CommunityLearning, true)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, true)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.Other16To19, true)]
        [InlineData(TypeOfFunding.OtherAdult, true)]
        public void IsFundedMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            // act
            var result = sut.IsFunded(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="fundingModel">The funding model.</param>
        /// <param name="candidates">The candidates.</param>
        [Theory]
        [InlineData(TypeOfFunding.CommunityLearning, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.LearningSupportFunding)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart)]
        [InlineData(TypeOfFunding.Other16To19, Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart)]
        [InlineData(TypeOfFunding.OtherAdult, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart)]
        [InlineData(TypeOfFunding.AdultSkills, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart)]
        public void InvalidItemRaisesValidationMessage(int fundingModel, params string[] candidates)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var records = new List<ILearningDeliveryFAM>();
            candidates.ForEach(x =>
            {
                var mockFAM = new Mock<ILearningDeliveryFAM>();
                mockFAM
                    .SetupGet(y => y.LearnDelFAMType)
                    .Returns(x);

                records.Add(mockFAM.Object);
            });

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundingModel);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
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
                Moq.It.Is<string>(y => y == LearnDelFAMType_01Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            var sut = new LearnDelFAMType_01Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="fundingModel">The funding model.</param>
        /// <param name="candidates">The candidates.</param>
        [Theory]
        [InlineData(TypeOfFunding.CommunityLearning, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart, Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(TypeOfFunding.NotFundedByESFA, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart, Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(TypeOfFunding.NotFundedByESFA, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart)]
        [InlineData(TypeOfFunding.Other16To19, Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart, Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(TypeOfFunding.OtherAdult, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart, Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(TypeOfFunding.AdultSkills, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart, Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.CommunityLearningProvision, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart, Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart, Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart, Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(TypeOfFunding.NotFundedByESFA, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart, Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(TypeOfFunding.NotFundedByESFA, Monitoring.Delivery.Types.ApprenticeshipContract, Monitoring.Delivery.Types.AdvancedLearnerLoan, Monitoring.Delivery.Types.HouseholdSituation, Monitoring.Delivery.Types.Learning, Monitoring.Delivery.Types.LearningSupportFunding, Monitoring.Delivery.Types.Restart)]
        public void ValidItemDoesNotRaiseAValidationMessage(int fundingModel, params string[] candidates)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var records = new List<ILearningDeliveryFAM>();
            candidates.ForEach(x =>
            {
                var mockFAM = new Mock<ILearningDeliveryFAM>();
                mockFAM
                    .SetupGet(y => y.LearnDelFAMType)
                    .Returns(x);

                records.Add(mockFAM.Object);
            });

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundingModel);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
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

            var sut = new LearnDelFAMType_01Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnDelFAMType_01Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new LearnDelFAMType_01Rule(mock.Object);
        }
    }
}
