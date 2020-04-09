using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R107RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new R107Rule(null));
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
            Assert.Equal("R107", result);
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
            Assert.Equal(R107Rule.Name, result);
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
        /// Get last delivery returns delivery with latest actual end date.
        /// </summary>
        /// <param name="expectedDate">The expected date.</param>
        /// <param name="testDates">The test dates.</param>
        [Theory]
        [InlineData("2012-07-31", "2012-01-30", "2012-02-28", "2012-07-31", "2012-03-30", "2012-04-30")]
        [InlineData("2013-03-30", "2012-01-30", "2012-07-31", "2013-03-30", "2012-04-30")]
        [InlineData("2014-07-31", "2012-01-30", "2012-02-28", "2012-07-31", "2014-07-31", "2012-04-30")]
        [InlineData("2015-07-31", "2015-07-31", "2012-02-28", "2012-07-31", "2012-04-30")]
        [InlineData("2016-07-31", "2012-02-28", "2016-07-31", "2012-04-30")]
        public void GetLastDeliveryReturnsMeetsExpectation(string expectedDate, params string[] testDates)
        {
            // arrange
            var testDate = DateTime.Parse(expectedDate);
            var sut = NewRule();

            var deliveries = new List<ILearningDelivery>();
            testDates.ForEach(x =>
            {
                var mockDel = new Mock<ILearningDelivery>();
                mockDel
                    .SetupGet(y => y.LearnActEndDateNullable)
                    .Returns(DateTime.Parse(x));

                deliveries.Add(mockDel.Object);
            });

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            // act
            var result = sut.GetLastDelivery(mockItem.Object);

            // assert
            Assert.Equal(testDate, result.LearnActEndDateNullable);
        }

        [Fact]
        public void GetLastDeliveryWithNullDeliveriesMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();

            // act
            var result = sut.GetLastDelivery(mockItem.Object);

            // assert
            Assert.Null(result);
        }

        /// <summary>
        /// Get destination and progression meets expectation.
        /// </summary>
        /// <param name="learnRN">The learn rn.</param>
        /// <param name="candidateCount">The candidate count.</param>
        [Theory]
        [InlineData("sldfkajwefo asjf", 3)]
        [InlineData("alwerkasvf as", 2)]
        [InlineData("zxc,vmnsdlih", 5)]
        [InlineData(",samvnasorgdhkn", 1)]
        public void GetDAndPMeetsExpectation(string learnRN, int candidateCount)
        {
            // arrange
            var outcomes = new List<IDPOutcome>();
            for (int i = 0; i < candidateCount; i++)
            {
                outcomes.Add(new Mock<IDPOutcome>().Object);
            }

            var mockItem = new Mock<ILearnerDestinationAndProgression>();
            mockItem
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRN);
            mockItem
                .SetupGet(x => x.DPOutcomes)
                .Returns(outcomes);

            var collection = new List<ILearnerDestinationAndProgression>();
            collection.Add(mockItem.Object);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var message = new Mock<IMessage>(MockBehavior.Strict);

            message
                .SetupGet(x => x.LearnerDestinationAndProgressions)
                .Returns(collection);

            var sut = NewRule();

            // act
            var result = sut.GetDAndP(learnRN, message.Object);

            // assert
            message.VerifyAll();
            Assert.Equal(candidateCount, result.DPOutcomes.Count);
        }

        [Theory]
        [InlineData("sldfkajwefo asjf")]
        [InlineData("alwerkasvf as")]
        [InlineData("zxc,vmnsdlih")]
        [InlineData(",samvnasorgdhkn")]
        public void GetDAndPWithNullDAndPsMeetsExpectation(string learnRN)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var message = new Mock<IMessage>(MockBehavior.Strict);
            message
                .SetupGet(x => x.LearnerDestinationAndProgressions)
                .Returns((IReadOnlyCollection<ILearnerDestinationAndProgression>)null);

            var sut = NewRule();

            // act
            var result = sut.GetDAndP(learnRN, message.Object);

            // assert
            message.VerifyAll();
            Assert.Null(result);
        }

        /// <summary>
        /// Has qualifying outcome meets expectation
        /// </summary>
        /// <param name="aEndDate">actual end date.</param>
        /// <param name="oStartDate">outcome start date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2012-08-31", "2012-07-30", false)]
        [InlineData("2012-07-31", "2012-07-30", false)]
        [InlineData("2012-01-30", "2012-01-30", true)]
        [InlineData("2012-07-29", "2012-07-30", true)]
        public void HasQualifyingOutcomeMeetsExpectation(string aEndDate, string oStartDate, bool expectation)
        {
            // arrange
            var testDate = DateTime.Parse(aEndDate);
            var sut = NewRule();

            var mockItem = new Mock<IDPOutcome>();
            mockItem
                .SetupGet(x => x.OutStartDate)
                .Returns(DateTime.Parse(oStartDate));

            // act
            var result = sut.HasQualifyingOutcome(mockItem.Object, testDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying fund model meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, true)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, true)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, true)]
        public void HasQualifyingFundModelMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.FundModel)
                .Returns(candidate);

            // act
            var result = sut.HasQualifyingFundModel(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying fund model with null deliveries returns false
        /// </summary>
        [Fact]
        public void HasQualifyingFundModelWithNullDeliveriesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();

            // act
            var result = sut.HasQualifyingFundModel(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has qualifying fund model with empty deliveries returns false
        /// </summary>
        [Fact]
        public void HasQualifyingFundModelWithEmptyDeliveriesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(new List<ILearningDelivery>());

            // act
            var result = sut.HasQualifyingFundModel(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Determines whether [has temporarily withdrawn meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(CompletionState.HasCompleted, false)]
        [InlineData(CompletionState.HasTemporarilyWithdrawn, true)]
        [InlineData(CompletionState.HasWithdrawn, false)]
        [InlineData(CompletionState.IsOngoing, false)]
        [InlineData(4, false)] // covers the unassigned gaps in the range
        [InlineData(5, false)]
        [InlineData(7, false)]
        public void HasTemporarilyWithdrawnMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.CompStatus)
                .Returns(candidate);

            // act
            var result = sut.HasTemporarilyWithdrawn(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Determines whether [has completed course meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData("2013-03-17", true)]
        public void HasCompletedCourseMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var testDate = !string.IsNullOrWhiteSpace(candidate)
                ? DateTime.Parse(candidate)
                : (DateTime?)null;

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(testDate);

            // act
            var result = sut.HasCompletedCourse(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has completed course with null deliveries returns false
        /// </summary>
        [Fact]
        public void HasCompletedCourseWithNullDeliveriesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();

            // act
            var result = sut.HasCompletedCourse(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has completed course with empty deliveries returns false
        /// </summary>
        [Fact]
        public void HasCompletedCourseWithEmptyDeliveriesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(new List<ILearningDelivery>());

            // act
            var result = sut.HasCompletedCourse(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// In training meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, true)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel6, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(TypeOfLearningProgramme.IntermediateLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.Traineeship, true)]
        public void InTrainingMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(candidate);

            // act
            var result = sut.InTraining(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// In training with null deliveries returns false.
        /// </summary>
        [Fact]
        public void InTrainingWithNullDeliveriesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();

            // act
            var result = sut.InTraining(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// In training with empty deliveries returns false.
        /// </summary>
        [Fact]
        public void InTrainingWithEmptyDeliveriesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(new List<ILearningDelivery>());

            // act
            var result = sut.InTraining(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="fundModel">The fund model.</param>
        /// <param name="progType">Type of learning programme.</param>
        /// <param name="completionState">State of completion.</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasWithdrawn)]
        public void InvalidItemRaisesValidationMessage(int fundModel, int progType, int completionState)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDel = new Mock<ILearningDelivery>();
            mockDel
                .SetupGet(x => x.FundModel)
                .Returns(fundModel);
            mockDel
                .SetupGet(x => x.CompStatus)
                .Returns(completionState);
            mockDel
                .SetupGet(y => y.LearnActEndDateNullable)
                .Returns(DateTime.Parse("2013-04-01"));
            mockDel
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDel.Object);

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var learners = new List<ILearner>();
            learners.Add(mockItem.Object);

            var outcome = new Mock<IDPOutcome>();
            outcome
                .SetupGet(x => x.OutStartDate)
                .Returns(DateTime.Parse("2013-03-30"));

            var outcomes = new List<IDPOutcome>();
            outcomes.Add(outcome.Object);

            var mockDAndP = new Mock<ILearnerDestinationAndProgression>();
            mockDAndP
                .SetupGet(x => x.DPOutcomes)
                .Returns(outcomes);

            var destinationProgressions = new List<ILearnerDestinationAndProgression>();
            destinationProgressions.Add(mockDAndP.Object);

            var message = new Mock<IMessage>(MockBehavior.Strict);
            message
                .SetupGet(x => x.Learners)
                .Returns(learners);
            message
                .SetupGet(x => x.LearnerDestinationAndProgressions)
                .Returns(destinationProgressions);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == R107Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    null));

            var sut = new R107Rule(handler.Object);

            // act
            sut.Validate(message.Object);

            // assert
            handler.VerifyAll();
            message.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="fundModel">The fund model.</param>
        /// <param name="progType">Type of learning programme.</param>
        /// <param name="completionState">State of completion.</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.AdvancedLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel4, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel5, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel6, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasWithdrawn)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.IntermediateLevelApprenticeship, CompletionState.HasWithdrawn)]
        public void ValidItemDoesNotRaiseValidationMessage(int fundModel, int progType, int completionState)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDel = new Mock<ILearningDelivery>();
            mockDel
                .SetupGet(x => x.FundModel)
                .Returns(fundModel);
            mockDel
                .SetupGet(x => x.CompStatus)
                .Returns(completionState);
            mockDel
                .SetupGet(y => y.LearnActEndDateNullable)
                .Returns(DateTime.Parse("2013-04-01"));
            mockDel
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDel.Object);

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var learners = new List<ILearner>();
            learners.Add(mockItem.Object);

            var outcome = new Mock<IDPOutcome>();
            outcome
                .SetupGet(x => x.OutStartDate)
                .Returns(DateTime.Parse("2013-04-02"));

            var outcomes = new List<IDPOutcome>();
            outcomes.Add(outcome.Object);

            var mockDAndP = new Mock<ILearnerDestinationAndProgression>();
            mockDAndP
                .SetupGet(x => x.DPOutcomes)
                .Returns(outcomes);

            var destinationProgressions = new List<ILearnerDestinationAndProgression>();
            destinationProgressions.Add(mockDAndP.Object);

            var message = new Mock<IMessage>(MockBehavior.Strict);
            message
                .SetupGet(x => x.Learners)
                .Returns(learners);
            message
                .SetupGet(x => x.LearnerDestinationAndProgressions)
                .Returns(destinationProgressions);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == R107Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    null));

            var sut = new R107Rule(handler.Object);

            // act
            sut.Validate(message.Object);

            // assert
            handler.VerifyAll();
            message.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public R107Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new R107Rule(handler.Object);
        }
    }
}
