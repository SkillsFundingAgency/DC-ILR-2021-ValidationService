using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_02RuleTests : AbstractRuleTests<LearnStartDate_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("LearnStartDate_02", result);
        }

        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        [Fact]
        public void IsOutsideValidSubmissionPeriod_True()
        {
            var startDate = new DateTime(2000, 8, 1);
            var yearStartDate = new DateTime(2019, 8, 1);

            // arrange
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnStartDate)
                .Returns(startDate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var academicYearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYearService
                .Setup(x => x.Start())
                .Returns(yearStartDate);

            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(ds => ds.AddYearsToDate(yearStartDate, -10))
                .Returns(new DateTime(2009, 8, 1));

            var sut = new LearnStartDate_02Rule(handler.Object, academicYearService.Object, dateTimeQueryService.Object);

            // act
            var result = sut.IsOutsideValidSubmissionPeriod(mockItem.Object);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void IsOutsideValidSubmissionPeriod_False()
        {
            var startDate = new DateTime(2018, 8, 1);
            var yearStartDate = new DateTime(2019, 8, 1);

            // arrange
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnStartDate)
                .Returns(startDate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var academicYearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYearService
                .Setup(x => x.Start())
                .Returns(yearStartDate);

            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(ds => ds.AddYearsToDate(yearStartDate, -10))
                .Returns(new DateTime(2009, 8, 1));

            var sut = new LearnStartDate_02Rule(handler.Object, academicYearService.Object, dateTimeQueryService.Object);

            // act
            var result = sut.IsOutsideValidSubmissionPeriod(mockItem.Object);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_Error()
        {
            var learnRefNumber = "123456789X";
            var startDate = new DateTime(2000, 8, 1);
            var yearStartDate = new DateTime(2019, 8, 1);

            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(startDate);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var academicYearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYearService
                .Setup(x => x.Start())
                .Returns(yearStartDate);

            var dateTimeQueryService = new Mock<IDateTimeQueryService>();
            dateTimeQueryService
                .Setup(ds => ds.AddYearsToDate(yearStartDate, -10))
                .Returns(new DateTime(2009, 8, 1));

            // act
            using (var handlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(handlerMock.Object, academicYearService.Object, dateTimeQueryService.Object).Validate(mockLearner.Object);
            }

            // assert
            academicYearService.VerifyAll();
            dateTimeQueryService.VerifyAll();
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnRefNumber = "123456789X";
            var startDate = new DateTime(2019, 8, 1);
            var yearStartDate = new DateTime(2019, 8, 1);

            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(startDate);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var academicYearService = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYearService
                .Setup(x => x.Start())
                .Returns(yearStartDate);

            var dateTimeQueryService = new Mock<IDateTimeQueryService>();
            dateTimeQueryService
                .Setup(ds => ds.AddYearsToDate(yearStartDate, -10))
                .Returns(new DateTime(2009, 8, 1));

            // act
            using (var handlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(handlerMock.Object, academicYearService.Object, dateTimeQueryService.Object).Validate(mockLearner.Object);
            }

            // assert
            academicYearService.VerifyAll();
            dateTimeQueryService.VerifyAll();
        }

        public LearnStartDate_02Rule NewRule(
            IValidationErrorHandler handler = null,
            IAcademicYearDataService academicYearDataService = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            return new LearnStartDate_02Rule(handler, academicYearDataService, dateTimeQueryService);
        }
    }
}
