using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceStartDate;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceStartDate
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class WorkPlaceStartDate_01RuleTests
    {
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
            Assert.Equal("WorkPlaceStartDate_01", result);
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
            Assert.Equal(RuleNameConstants.WorkPlaceStartDate_01, result);
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
        /// Last inviable date meets expectation.
        /// </summary>
        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.LastInviableDate;

            // assert
            Assert.Equal(DateTime.Parse("2014-07-31"), result);
        }

        /// <summary>
        /// Condition met with null learning delivery returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Is viable start meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2013-08-01", false)]
        [InlineData("2014-07-31", false)]
        [InlineData("2014-08-01", true)]
        [InlineData("2014-09-14", true)]
        public void IsViableStartMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.IsViableStart(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfAim.References.IndustryPlacement, true)]
        [InlineData(TypeOfAim.References.SupportedInternship16To19, true)]
        [InlineData(TypeOfAim.References.WorkExperience, true)]
        [InlineData(TypeOfAim.References.WorkPlacement0To49Hours, true)]
        [InlineData(TypeOfAim.References.WorkPlacement100To199Hours, true)]
        [InlineData(TypeOfAim.References.WorkPlacement200To499Hours, true)]
        [InlineData(TypeOfAim.References.WorkPlacement500PlusHours, true)]
        [InlineData(TypeOfAim.References.WorkPlacement50To99Hours, true)]
        [InlineData("asdflkasroas i", false)]
        [InlineData("w;oraeijwq rf;oiew ", false)]
        [InlineData(null, false)]
        public void IsWorkPlacementMeetsExpectation(string aimReference, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimReference);

            // act
            var result = sut.IsWorkPlacement(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Condition met for learning deliveries with work placement returns true.
        /// </summary>
        [Fact]
        public void ConditionMetForLearningDeliveriesWithWorkPlacementReturnsTrue()
        {
            // arrange
            var workplacements = new List<ILearningDeliveryWorkPlacement>();
            workplacements.Add(new Mock<ILearningDeliveryWorkPlacement>().Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearningDeliveryWorkPlacements)
                .Returns(workplacements);

            var sut = NewRule();

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met for learning deliveries with no work placement returns false.
        /// </summary>
        [Fact]
        public void ConditionMetForLearningDeliveriesWithNoWorkPlacementReturnsFalse()
        {
            // arrange
            var workplacements = new List<ILearningDeliveryWorkPlacement>();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearningDeliveryWorkPlacements)
                .Returns(workplacements);

            var sut = NewRule();

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met for learning deliveries with null work placement returns false.
        /// </summary>
        [Fact]
        public void ConditionMetForLearningDeliveriesWithNullWorkPlacementReturnsFalse()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="aimReference">The aim reference.</param>
        /// <param name="startDate">The start date.</param>
        [Theory]
        [InlineData(TypeOfAim.References.IndustryPlacement, "2014-08-01")]
        [InlineData(TypeOfAim.References.SupportedInternship16To19, "2015-01-14")]
        [InlineData(TypeOfAim.References.WorkExperience, "2014-08-26")]
        [InlineData(TypeOfAim.References.WorkPlacement0To49Hours, "2016-02-09")]
        [InlineData(TypeOfAim.References.WorkPlacement100To199Hours, "2015-05-18")]
        [InlineData(TypeOfAim.References.WorkPlacement200To499Hours, "2014-12-28")]
        [InlineData(TypeOfAim.References.WorkPlacement500PlusHours, "2017-11-07")]
        [InlineData(TypeOfAim.References.WorkPlacement50To99Hours, "2016-04-04")]
        public void InvalidItemRaisesValidationMessage(string aimReference, string startDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimReference);
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(startDate));
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(0);

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
            mockHandler
              .Setup(x => x.Handle("WorkPlaceStartDate_01", LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", aimReference))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new WorkPlaceStartDate_01Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="aimReference">The aim reference.</param>
        /// <param name="startDate">The start date.</param>
        [Theory]
        [InlineData(TypeOfAim.References.IndustryPlacement, "2014-08-01")]
        [InlineData(TypeOfAim.References.SupportedInternship16To19, "2015-01-14")]
        [InlineData(TypeOfAim.References.WorkExperience, "2014-08-26")]
        [InlineData(TypeOfAim.References.WorkPlacement0To49Hours, "2016-02-09")]
        [InlineData(TypeOfAim.References.WorkPlacement100To199Hours, "2015-05-18")]
        [InlineData(TypeOfAim.References.WorkPlacement200To499Hours, "2014-12-28")]
        [InlineData(TypeOfAim.References.WorkPlacement500PlusHours, "2017-11-07")]
        [InlineData(TypeOfAim.References.WorkPlacement50To99Hours, "2016-04-04")]
        public void ValidItemDoesNotRaiseAValidationMessage(string aimReference, string startDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var workplacements = new List<ILearningDeliveryWorkPlacement>();
            workplacements.Add(new Mock<ILearningDeliveryWorkPlacement>().Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearningDeliveryWorkPlacements)
                .Returns(workplacements);
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimReference);
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

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

            var sut = new WorkPlaceStartDate_01Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public WorkPlaceStartDate_01Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new WorkPlaceStartDate_01Rule(mock.Object);
        }
    }
}
