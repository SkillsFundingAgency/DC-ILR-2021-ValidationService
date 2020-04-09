using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ContPrefType
{
    public class ContPrefType_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new ContPrefType_02Rule(null));
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
            Assert.Equal("ContPrefType_02", result);
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
            Assert.Equal(ContPrefType_02Rule.Name, result);
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

        [Theory]
        [InlineData(ContactPreference.AgreesContactByEmailPostGDPR, false)]
        [InlineData(ContactPreference.AgreesContactByPhonePostGDPR, false)]
        [InlineData(ContactPreference.AgreesContactByPostPostGDPR, false)]
        [InlineData(ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR, false)]
        [InlineData(ContactPreference.AgreesContactSurveysAndResearchPostGDPR, false)]
        [InlineData(ContactPreference.NoContactByEmailPreGDPR, false)]
        [InlineData(ContactPreference.NoContactByPhonePreGDPR, false)]
        [InlineData(ContactPreference.NoContactByPostPreGDPR, false)]
        [InlineData(ContactPreference.NoContactCoursesOrOpportunitiesPreGDPR, false)]
        [InlineData(ContactPreference.NoContactDueToDeath, true)]
        [InlineData(ContactPreference.NoContactDueToIllness, true)]
        [InlineData(ContactPreference.NoContactIllnessOrDied_ValidTo20130731, true)]
        [InlineData(ContactPreference.NoContactSurveysAndResearchPreGDPR, false)]
        public void HasRestrictedContactIndicatorMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var item = new Mock<IContactPreference>();
            item
                .SetupGet(y => y.ContPrefType)
                .Returns(candidate.Substring(0, 3));
            item
                .SetupGet(y => y.ContPrefCode)
                .Returns(int.Parse(candidate.Substring(3)));

            // act
            var result = sut.HasRestrictedContactIndicator(item.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(ContactPreference.AgreesContactByEmailPostGDPR, true)]
        [InlineData(ContactPreference.AgreesContactByPhonePostGDPR, true)]
        [InlineData(ContactPreference.AgreesContactByPostPostGDPR, true)]
        [InlineData(ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR, true)]
        [InlineData(ContactPreference.AgreesContactSurveysAndResearchPostGDPR, true)]
        [InlineData(ContactPreference.NoContactByEmailPreGDPR, true)]
        [InlineData(ContactPreference.NoContactByPhonePreGDPR, true)]
        [InlineData(ContactPreference.NoContactByPostPreGDPR, true)]
        [InlineData(ContactPreference.NoContactCoursesOrOpportunitiesPreGDPR, true)]
        [InlineData(ContactPreference.NoContactDueToDeath, false)]
        [InlineData(ContactPreference.NoContactDueToIllness, false)]
        [InlineData(ContactPreference.NoContactIllnessOrDied_ValidTo20130731, false)]
        [InlineData(ContactPreference.NoContactSurveysAndResearchPreGDPR, true)]
        public void HasDisqualifyingContactIndicatorMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var item = new Mock<IContactPreference>();
            item
                .SetupGet(y => y.ContPrefType)
                .Returns(candidate.Substring(0, 3));
            item
                .SetupGet(y => y.ContPrefCode)
                .Returns(int.Parse(candidate.Substring(3)));

            // act
            var result = sut.HasDisqualifyingContactIndicator(item.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="conflicts">The conflicts.</param>
        [Theory]
        [InlineData(ContactPreference.NoContactIllnessOrDied_ValidTo20130731, ContactPreference.AgreesContactByEmailPostGDPR, ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR)]
        [InlineData(ContactPreference.NoContactDueToIllness, ContactPreference.AgreesContactByEmailPostGDPR, ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR)]
        [InlineData(ContactPreference.NoContactDueToDeath, ContactPreference.AgreesContactByEmailPostGDPR, ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR)]
        public void InvalidItemRaisesValidationMessage(string candidate, params string[] conflicts)
        {
            // arrange
            const string learnRefNumber = "123456789X";

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("ContPrefType_02", learnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            var preferences = new List<IContactPreference>();

            var preference = new Mock<IContactPreference>();
            preference
                .SetupGet(y => y.ContPrefType)
                .Returns(candidate.Substring(0, 3));
            preference
                .SetupGet(y => y.ContPrefCode)
                .Returns(int.Parse(candidate.Substring(3)));
            preferences.Add(preference.Object);

            conflicts.ForEach(x =>
            {
                var prefType = x.Substring(0, 3);
                var prefCode = int.Parse(x.Substring(3));

                var conflict = new Mock<IContactPreference>();
                conflict
                    .SetupGet(y => y.ContPrefType)
                    .Returns(x.Substring(0, 3));
                conflict
                    .SetupGet(y => y.ContPrefCode)
                    .Returns(int.Parse(x.Substring(3)));

                handler
                    .Setup(y => y.BuildErrorMessageParameter("ContPrefType", prefType))
                    .Returns(new Mock<IErrorMessageParameter>().Object);
                handler
                    .Setup(y => y.BuildErrorMessageParameter("ContPrefCode", prefCode))
                    .Returns(new Mock<IErrorMessageParameter>().Object);

                preferences.Add(conflict.Object);
            });

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.ContactPreferences)
                .Returns(preferences);

            var sut = new ContPrefType_02Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        [Theory]
        [InlineData(ContactPreference.NoContactByPostPreGDPR, ContactPreference.AgreesContactByEmailPostGDPR, ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR)]
        [InlineData(ContactPreference.NoContactSurveysAndResearchPreGDPR, ContactPreference.AgreesContactByEmailPostGDPR, ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR)]
        [InlineData(ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR, ContactPreference.AgreesContactByEmailPostGDPR, ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR)]
        public void ValidItemDoesNotRaiseValidationMessage(params string[] candidates)
        {
            // arrange
            const string learnRefNumber = "123456789X";

            var preferences = new List<IContactPreference>();

            candidates.ForEach(x =>
            {
                var prefType = x.Substring(0, 3);
                var prefCode = int.Parse(x.Substring(3));

                var conflict = new Mock<IContactPreference>();
                conflict
                    .SetupGet(y => y.ContPrefType)
                    .Returns(x.Substring(0, 3));
                conflict
                    .SetupGet(y => y.ContPrefCode)
                    .Returns(int.Parse(x.Substring(3)));

                preferences.Add(conflict.Object);
            });

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.ContactPreferences)
                .Returns(preferences);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new ContPrefType_02Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a newly contructed rule</returns>
        private ContPrefType_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new ContPrefType_02Rule(handler.Object);
        }
    }
}