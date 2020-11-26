using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceStartDate;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceStartDate
{
    public class WorkPlaceStartDate_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("WorkPlaceStartDate_01", result);
        }

        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.LastInviableDate;

            Assert.Equal(DateTime.Parse("2014-07-31"), result);
        }

        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null);

            Assert.True(result);
        }

        [Theory]
        [InlineData("2013-08-01", false)]
        [InlineData("2014-07-31", false)]
        [InlineData("2014-08-01", true)]
        [InlineData("2014-09-14", true)]
        public void IsViableStartMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var result = sut.IsViableStart(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(AimTypes.References.IndustryPlacement, true)]
        [InlineData(AimTypes.References.SupportedInternship16To19, true)]
        [InlineData(AimTypes.References.WorkExperience, true)]
        [InlineData(AimTypes.References.WorkPlacement0To49Hours, true)]
        [InlineData(AimTypes.References.WorkPlacement100To199Hours, true)]
        [InlineData(AimTypes.References.WorkPlacement200To499Hours, true)]
        [InlineData(AimTypes.References.WorkPlacement500PlusHours, true)]
        [InlineData(AimTypes.References.WorkPlacement50To99Hours, true)]
        [InlineData("asdflkasroas i", false)]
        [InlineData("w;oraeijwq rf;oiew ", false)]
        [InlineData(null, false)]
        public void IsWorkPlacementMeetsExpectation(string aimReference, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimReference);

            var result = sut.IsWorkPlacement(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void ConditionMetForLearningDeliveriesWithWorkPlacementReturnsTrue()
        {
            var workplacements = new List<ILearningDeliveryWorkPlacement>
            {
                new Mock<ILearningDeliveryWorkPlacement>().Object
            };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearningDeliveryWorkPlacements)
                .Returns(workplacements);

            var sut = NewRule();

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.True(result);
        }

        [Fact]
        public void ConditionMetForLearningDeliveriesWithNoWorkPlacementReturnsFalse()
        {
            var workplacements = new List<ILearningDeliveryWorkPlacement>();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearningDeliveryWorkPlacements)
                .Returns(workplacements);

            var sut = NewRule();

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.False(result);
        }

        [Fact]
        public void ConditionMetForLearningDeliveriesWithNullWorkPlacementReturnsFalse()
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            var sut = NewRule();

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.False(result);
        }

        [Theory]
        [InlineData(AimTypes.References.IndustryPlacement, "2014-08-01")]
        [InlineData(AimTypes.References.SupportedInternship16To19, "2015-01-14")]
        [InlineData(AimTypes.References.WorkExperience, "2014-08-26")]
        [InlineData(AimTypes.References.WorkPlacement0To49Hours, "2016-02-09")]
        [InlineData(AimTypes.References.WorkPlacement100To199Hours, "2015-05-18")]
        [InlineData(AimTypes.References.WorkPlacement200To499Hours, "2014-12-28")]
        [InlineData(AimTypes.References.WorkPlacement500PlusHours, "2017-11-07")]
        [InlineData(AimTypes.References.WorkPlacement50To99Hours, "2016-04-04")]
        public void InvalidItemRaisesValidationMessage(string aimReference, string startDate)
        {
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

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

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

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Theory]
        [InlineData(AimTypes.References.IndustryPlacement, "2014-08-01")]
        [InlineData(AimTypes.References.SupportedInternship16To19, "2015-01-14")]
        [InlineData(AimTypes.References.WorkExperience, "2014-08-26")]
        [InlineData(AimTypes.References.WorkPlacement0To49Hours, "2016-02-09")]
        [InlineData(AimTypes.References.WorkPlacement100To199Hours, "2015-05-18")]
        [InlineData(AimTypes.References.WorkPlacement200To499Hours, "2014-12-28")]
        [InlineData(AimTypes.References.WorkPlacement500PlusHours, "2017-11-07")]
        [InlineData(AimTypes.References.WorkPlacement50To99Hours, "2016-04-04")]
        public void ValidItemDoesNotRaiseAValidationMessage(string aimReference, string startDate)
        {
            const string LearnRefNumber = "123456789X";

            var workplacements = new List<ILearningDeliveryWorkPlacement>
            {
                new Mock<ILearningDeliveryWorkPlacement>().Object
            };

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

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new WorkPlaceStartDate_01Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public WorkPlaceStartDate_01Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new WorkPlaceStartDate_01Rule(mock.Object);
        }
    }
}
