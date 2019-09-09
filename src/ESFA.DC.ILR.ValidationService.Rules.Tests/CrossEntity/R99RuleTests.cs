using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    /// <summary>
    /// cross record rule 99 unit tests
    /// </summary>
    public class R99RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R99Rule(null, commonOps.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R99Rule(handler.Object, null));
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
            Assert.Equal("R99", result);
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
            Assert.Equal(RuleNameConstants.R99, result);
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
        /// Get candidate deliveries meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetCandidateDeliveriesMeetsExpectation(int expectation)
        {
            // arrange
            var deliveries = Collection.Empty<ILearningDelivery>();
            for (var i = 0; i < expectation; i++)
            {
                var delivery = new Mock<ILearningDelivery>();

                deliveries.Add(delivery.Object);
            }

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.InAProgramme(Moq.It.IsAny<ILearningDelivery>()))
                .Returns(true);

            var sut = new R99Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.GetCandidateDeliveries(deliveries.AsSafeReadOnlyList());

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();

            Assert.Equal(expectation, result.Count);
        }

        /// <summary>
        /// Is programe aim meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsProgrammeAimMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.InAProgramme(delivery.Object))
                .Returns(expectation);

            var sut = new R99Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.IsProgrammeAim(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has viable count meets expectation
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        public void HasViableCountMeetsExpectation(int count, bool expectation)
        {
            // arrange
            var deliveries = Collection.Empty<ILearningDelivery>();
            for (var i = 0; i < count; i++)
            {
                var delivery = new Mock<ILearningDelivery>();
                deliveries.Add(delivery.Object);
            }

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new R99Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.HasViableCount(deliveries.AsSafeReadOnlyList());

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Against other deliveries meets expectation.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="aimSeq">The aim seq.</param>
        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        public void AgainstOtherDeliveriesMeetsExpectation(int count, int aimSeq)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .Setup(x => x.AimSeqNumber)
                .Returns(aimSeq);

            var candidates = Collection.Empty<ILearningDelivery>();
            for (var i = 0; i < count; i++)
            {
                var candidate = new Mock<ILearningDelivery>();
                candidate
                    .Setup(x => x.AimSeqNumber)
                    .Returns(i + 1);
                candidates.Add(candidate.Object);
            }

            var sut = NewRule();

            // act
            var result = sut.AgainstOtherDeliveries(delivery.Object, candidates.AsSafeReadOnlyList());

            // assert
            Assert.DoesNotContain(result, x => x.AimSeqNumber == aimSeq);
        }

        /// <summary>
        /// Is not self meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="aimSeq">The aim seq.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(1, 2, true)]
        [InlineData(2, 1, true)]
        [InlineData(3, 2, true)]
        [InlineData(3, 3, false)]
        public void IsNotSelfMeetsExpectation(int candidate, int aimSeq, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .Setup(x => x.AimSeqNumber)
                .Returns(aimSeq);

            var deliveryToo = new Mock<ILearningDelivery>();
            deliveryToo
                .Setup(x => x.AimSeqNumber)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.IsNotSelf(delivery.Object, deliveryToo.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Gets the nullable date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a nullable date</returns>
        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        /// <summary>
        /// Is open aim meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, true)]
        [InlineData("2016-08-14", false)]
        public void IsOpenAimMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .Setup(x => x.LearnActEndDateNullable)
                .Returns(GetNullableDate(candidate));

            var sut = NewRule();

            // act
            var result = sut.IsOpenAim(delivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has open aim meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, true)]
        [InlineData("2016-08-14", false)]
        public void HasOpenAimMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var candidates = Collection.Empty<ILearningDelivery>();
            for (var i = 0; i < 5; i++)
            {
                var temp = new Mock<ILearningDelivery>();
                temp
                    .Setup(x => x.LearnActEndDateNullable)
                    .Returns(GetNullableDate(candidate));
                candidates.Add(temp.Object);
            }

            var sut = NewRule();

            // act
            var result = sut.HasOpenAim(candidates.AsSafeReadOnlyList());

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has overlapping aim end dates with null candidates returns false
        /// </summary>
        [Fact]
        public void HasOverlappingAimEndDatesWithNullCandidatesReturnsFalse()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            var sut = NewRule();

            // act
            var result = sut.HasOverlappingAimEndDates(delivery.Object, (IReadOnlyCollection<ILearningDelivery>)null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has overlapping aim end dates meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-03-31", "2016-04-01", "2017-04-01", false)] // below lower limit
        [InlineData("2016-04-01", "2016-04-01", "2017-04-01", true)] // on lower limit
        [InlineData("2016-09-16", "2016-04-01", "2017-04-01", true)] // inside
        [InlineData("2017-04-01", "2016-04-01", "2017-04-01", true)] // on upper limit
        [InlineData("2017-04-02", "2016-04-01", "2017-04-01", false)] // outside upper limit
        [InlineData("2019-06-09", "2016-04-01", null, true)] // open ended
        public void HasOverlappingAimEndDatesMeetsExpectation(string candidate, string start, string end, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .Setup(x => x.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var temp = new Mock<ILearningDelivery>(MockBehavior.Strict);
            temp
                .Setup(x => x.LearnStartDate)
                .Returns(DateTime.Parse(start));
            temp
                .Setup(x => x.LearnActEndDateNullable)
                .Returns(GetNullableDate(end));

            var sut = NewRule();

            // act
            var result = sut.HasOverlappingAimEndDates(delivery.Object, temp.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying model meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void HasQualifyingModelMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36)) // TypeOfFunding.ApprenticeshipsFrom1May2017
                .Returns(expectation);

            var sut = new R99Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingModel(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Is standard apprencticeship meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsStandardApprencticeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsStandardApprencticeship(delivery.Object))
                .Returns(expectation);

            var sut = new R99Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.IsStandardApprencticeship(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has overlapping aim achievement dates with null candidates returns false
        /// </summary>
        [Fact]
        public void HasOverlappingAimAchievementDatesWithNullCandidatesReturnsFalse()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            var sut = NewRule();

            // act
            var result = sut.HasOverlappingAimAchievementDates(delivery.Object, (IReadOnlyCollection<ILearningDelivery>)null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has overlapping aim achievement dates meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-03-31", "2016-04-01", "2017-04-01", false)] // below lower limit
        [InlineData("2016-04-01", "2016-04-01", "2017-04-01", true)] // on lower limit
        [InlineData("2016-09-16", "2016-04-01", "2017-04-01", true)] // inside
        [InlineData("2017-04-01", "2016-04-01", "2017-04-01", true)] // on upper limit
        [InlineData("2017-04-02", "2016-04-01", "2017-04-01", false)] // outside upper limit
        [InlineData("2019-06-09", "2016-04-01", null, true)] // open ended
        public void HasOverlappingAimAchievementDatesMeetsExpectation(string candidate, string start, string end, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .Setup(x => x.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var temp = new Mock<ILearningDelivery>(MockBehavior.Strict);
            temp
                .Setup(x => x.LearnStartDate)
                .Returns(DateTime.Parse(start));
            temp
                .Setup(x => x.AchDateNullable)
                .Returns(GetNullableDate(end));

            var sut = NewRule();

            // act
            var result = sut.HasOverlappingAimAchievementDates(delivery.Object, temp.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// date ranges that generate one error item
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [Theory]
        [InlineData("2016-09-16", "2016-04-01", "2017-04-01")] // inside
        [InlineData("2017-04-01", "2016-04-01", "2017-04-01")] // on upper limit
        public void InvalidItemRaisesValidationMessage(string candidate, string start, string end)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.AimType)
                .Returns(1);
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(1);
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(1);
            delivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(1);
            delivery
                .Setup(x => x.LearnActEndDateNullable)
                .Returns((DateTime?)null);
            delivery
                .Setup(x => x.AchDateNullable)
                .Returns((DateTime?)null);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            var testStart2 = GetNullableDate(start);
            var testEnd = GetNullableDate(end);
            var temp = new Mock<ILearningDelivery>(MockBehavior.Strict);
            temp
                .SetupGet(x => x.AimType)
                .Returns(2);
            temp
                .SetupGet(x => x.FundModel)
                .Returns(2);
            temp
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(2);
            temp
                .SetupGet(x => x.AimSeqNumber)
                .Returns(2);
            temp
                .Setup(x => x.LearnStartDate)
                .Returns(DateTime.Parse(start));
            temp
                .Setup(x => x.LearnActEndDateNullable)
                .Returns(testEnd);
            temp
                .Setup(x => x.AchDateNullable)
                .Returns(testEnd);

            var deliveries = new ILearningDelivery[] { delivery.Object, temp.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.R99, LearnRefNumber, 1, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AimType", 1))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnActEndDate", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 1))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", 1))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("AchDate", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.InAProgramme(Moq.It.IsAny<ILearningDelivery>()))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingFunding(Moq.It.IsAny<ILearningDelivery>(), 36))
                .Returns(true);
            commonOps
                .Setup(x => x.IsStandardApprencticeship(Moq.It.IsAny<ILearningDelivery>()))
                .Returns(true);

            var sut = new R99Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation messages.
        /// date ranges that generate two error items
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [Theory]
        [InlineData("2016-04-01", "2016-04-01", "2017-04-01")] // on lower limit
        [InlineData("2019-06-09", "2016-04-01", null)] // open ended
        public void InvalidItemRaisesValidationMessages(string candidate, string start, string end)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.AimType)
                .Returns(1);
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(1);
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(1);
            delivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(1);
            delivery
                .Setup(x => x.LearnActEndDateNullable)
                .Returns((DateTime?)null);
            delivery
                .Setup(x => x.AchDateNullable)
                .Returns((DateTime?)null);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            var testStart2 = GetNullableDate(start);
            var testEnd = GetNullableDate(end);
            var temp = new Mock<ILearningDelivery>(MockBehavior.Strict);
            temp
                .SetupGet(x => x.AimType)
                .Returns(2);
            temp
                .SetupGet(x => x.FundModel)
                .Returns(2);
            temp
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(2);
            temp
                .SetupGet(x => x.AimSeqNumber)
                .Returns(2);
            temp
                .Setup(x => x.LearnStartDate)
                .Returns(DateTime.Parse(start));
            temp
                .Setup(x => x.LearnActEndDateNullable)
                .Returns(testEnd);
            temp
                .Setup(x => x.AchDateNullable)
                .Returns(testEnd);

            var deliveries = new ILearningDelivery[] { delivery.Object, temp.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.R99, LearnRefNumber, 1, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AimType", 1))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnActEndDate", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 1))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", 1))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("AchDate", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            handler
                .Setup(x => x.Handle(RuleNameConstants.R99, LearnRefNumber, 2, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AimType", 2))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testStart2)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnActEndDate", AbstractRule.AsRequiredCultureDate(testEnd)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 2))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", 2))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("AchDate", AbstractRule.AsRequiredCultureDate(testEnd)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.InAProgramme(Moq.It.IsAny<ILearningDelivery>()))
                .Returns(true);

            var sut = new R99Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// we make the candidate a one day aim so there can be no over lap with the start and end dates
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [Theory]
        [InlineData("2016-03-31", "2016-04-01", "2017-04-01")] // below lower limit
        [InlineData("2017-04-02", "2016-04-01", "2017-04-01")] // outside upper limit
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, string start, string end)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.AimType)
                .Returns(1);
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(1);
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(1);
            delivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(1);
            delivery
                .Setup(x => x.LearnActEndDateNullable)
                .Returns(testDate);
            delivery
                .Setup(x => x.AchDateNullable)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            var testStart2 = GetNullableDate(start);
            var testEnd = GetNullableDate(end);
            var temp = new Mock<ILearningDelivery>(MockBehavior.Strict);
            temp
                .SetupGet(x => x.AimType)
                .Returns(2);
            temp
                .SetupGet(x => x.FundModel)
                .Returns(2);
            temp
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(2);
            temp
                .SetupGet(x => x.AimSeqNumber)
                .Returns(2);
            temp
                .Setup(x => x.LearnStartDate)
                .Returns(DateTime.Parse(start));
            temp
                .Setup(x => x.LearnActEndDateNullable)
                .Returns(testEnd);
            temp
                .Setup(x => x.AchDateNullable)
                .Returns(testEnd);

            var deliveries = new ILearningDelivery[] { delivery.Object, temp.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.InAProgramme(Moq.It.IsAny<ILearningDelivery>()))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingFunding(Moq.It.IsAny<ILearningDelivery>(), 36))
                .Returns(true);
            commonOps
                .Setup(x => x.IsStandardApprencticeship(Moq.It.IsAny<ILearningDelivery>()))
                .Returns(true);

            var sut = new R99Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public R99Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new R99Rule(handler.Object, commonOps.Object);
        }
    }
}
