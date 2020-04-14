using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R85RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new R85Rule(null));
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
            Assert.Equal("R85", result);
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
            Assert.Equal(R85Rule.Name, result);
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
        public void ValidateWithNullMessageThrows()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Blahes the meets expectation.
        /// </summary>
        /// <param name="dAndPULN">The destination and progression uln.</param>
        /// <param name="learnerULN">The learner uln.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(99998, 99999, true)]
        [InlineData(99999, 99999, false)]
        [InlineData(99958, 92959, true)]
        [InlineData(92958, 92958, false)]
        public void IsNotMatchingLearnerNumberMeetsExpectation(long dAndPULN, long learnerULN, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var dAndP = new Mock<ILearnerDestinationAndProgression>();
            dAndP.SetupGet(x => x.ULN).Returns(dAndPULN);

            var learner = new Mock<ILearner>();
            learner.SetupGet(x => x.ULN).Returns(learnerULN);

            // act
            var result = sut.IsNotMatchingLearnerNumber(dAndP.Object, learner.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("99998", "99999", false)]
        [InlineData("99999", "99999", true)]
        [InlineData("99958", "92959", false)]
        [InlineData("92958", "92958", true)]
        public void HasMatchingReferenceNumberMeetsExpectation(string dAndPRefNum, string learnerRefNum, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var dAndP = new Mock<ILearnerDestinationAndProgression>();
            dAndP.SetupGet(x => x.LearnRefNumber).Returns(dAndPRefNum);

            var learner = new Mock<ILearner>();
            learner.SetupGet(x => x.LearnRefNumber).Returns(learnerRefNum);

            // act
            var result = sut.HasMatchingReferenceNumber(dAndP.Object, learner.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const long learnerULN = 999998;
            const long dAndP__ULN = 999999;

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(y => y.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(y => y.ULN)
                .Returns(learnerULN);

            var learners = new List<ILearner>();
            learners.Add(learner.Object);

            var dAndP = new Mock<ILearnerDestinationAndProgression>();
            dAndP
                .SetupGet(y => y.LearnRefNumber)
                .Returns(LearnRefNumber);
            dAndP
                .SetupGet(y => y.ULN)
                .Returns(dAndP__ULN);

            var records = new List<ILearnerDestinationAndProgression>();
            records.Add(dAndP.Object);

            var message = new Mock<IMessage>();
            message
                .SetupGet(y => y.Learners)
                .Returns(learners);
            message
                .SetupGet(x => x.LearnerDestinationAndProgressions)
                .Returns(records);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(R85Rule.Name, LearnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("ULN", learnerULN))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnerDestinationandProgression.ULN", dAndP__ULN))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnerDestinationandProgression.LearnRefNumber", LearnRefNumber))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new R85Rule(handler.Object);

            // act
            sut.Validate(message.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const int learnerULN = 999999;
            const int dAndP__ULN = 999999;

            var dAndP = new Mock<ILearnerDestinationAndProgression>();
            dAndP
                .SetupGet(y => y.LearnRefNumber)
                .Returns(LearnRefNumber);
            dAndP
                .SetupGet(y => y.ULN)
                .Returns(dAndP__ULN);

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(y => y.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(y => y.ULN)
                .Returns(learnerULN);

            var records = new List<ILearnerDestinationAndProgression>();
            records.Add(dAndP.Object);

            var learners = new List<ILearner>();
            learners.Add(learner.Object);

            var message = new Mock<IMessage>();
            message
                .SetupGet(y => y.Learners)
                .Returns(learners);
            message
                .SetupGet(x => x.LearnerDestinationAndProgressions)
                .Returns(records);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new R85Rule(handler.Object);

            // act
            sut.Validate(message.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a new rule (with a strict mock message handler)</returns>
        private R85Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new R85Rule(handler.Object);
        }
    }
}
