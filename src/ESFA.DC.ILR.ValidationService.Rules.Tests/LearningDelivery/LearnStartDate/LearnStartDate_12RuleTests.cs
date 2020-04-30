using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_12RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnStartDate_12", result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsApprenticeshipMeetsExpectation(bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var rule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            rule07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);

            var sut = new LearnStartDate_12Rule(handler.Object, service.Object, rule07.Object);

            var result = sut.IsApprenticeship(mockItem.Object);

            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            rule07.VerifyAll();
        }

        [Theory]
        [InlineData("2018-04-18", "2018-04-18", true)]
        [InlineData("2019-04-17", "2018-04-18", true)]
        [InlineData("2019-04-18", "2018-04-18", false)]
        [InlineData("2019-04-19", "2018-04-18", false)]
        public void HasQualifyingStartDateMeetsExpectation(string startDate, string yearEndDate, bool expectation)
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.End())
                .Returns(DateTime.Parse(yearEndDate));

            var rule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);

            var sut = new LearnStartDate_12Rule(handler.Object, service.Object, rule07.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            var result = sut.HasQualifyingStartDate(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            var referenceDate = DateTime.Parse("2019-04-19");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(referenceDate);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(ProgTypes.ApprenticeshipStandard);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == LearnStartDate_12Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == LearnStartDate_12Rule.MessagePropertyName),
                    referenceDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.End())
                .Returns(referenceDate.AddYears(-1));
            var rule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            rule07
                .Setup(x => x.IsApprenticeship(ProgTypes.ApprenticeshipStandard))
                .Returns(true);

            var sut = new LearnStartDate_12Rule(handler.Object, service.Object, rule07.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            rule07.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var referenceDate = DateTime.Parse("2019-04-19");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(referenceDate);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(ProgTypes.ApprenticeshipStandard);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.End())
                .Returns(referenceDate.AddYears(-1).AddDays(1));
            var rule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            rule07
                .Setup(x => x.IsApprenticeship(ProgTypes.ApprenticeshipStandard))
                .Returns(true);

            var sut = new LearnStartDate_12Rule(handler.Object, service.Object, rule07.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            rule07.VerifyAll();
        }

        public LearnStartDate_12Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var rule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);

            return new LearnStartDate_12Rule(handler.Object, service.Object, rule07.Object);
        }
    }
}
