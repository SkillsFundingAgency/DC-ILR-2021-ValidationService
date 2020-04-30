using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEmpId;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceEmpId
{
    public class WorkPlaceEmpId_03RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("WorkPlaceEmpId_03", result);
        }

        [Theory]
        [InlineData(ProgTypes.AdvancedLevelApprenticeship, false)]
        [InlineData(ProgTypes.ApprenticeshipStandard, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel4, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel5, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel6, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(ProgTypes.IntermediateLevelApprenticeship, false)]
        [InlineData(ProgTypes.Traineeship, true)]
        [InlineData(null, false)]
        public void IsQualifyingProgrammeMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);

            var result = sut.IsQualifyingProgramme(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2015-04-15", "2015-06-13", true)]
        [InlineData("2015-04-15", "2015-06-14", true)]
        [InlineData("2015-04-15", "2015-06-15", false)]
        [InlineData("2015-04-15", "2015-06-16", false)]
        [InlineData("2016-06-14", "2016-08-15", false)]
        [InlineData("2016-06-15", "2016-08-15", false)]
        [InlineData("2016-06-16", "2016-08-15", true)]
        [InlineData("2016-06-17", "2016-08-15", true)]
        public void IsInsideTheRegistrationPeriodMeetsExpectation(string startDate, string fileDate, bool expectation)
        {
            var mockItem = new Mock<ILearningDeliveryWorkPlacement>();
            mockItem
                .SetupGet(y => y.WorkPlaceStartDate)
                .Returns(DateTime.Parse(startDate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IFileDataService>(MockBehavior.Strict);
            service
                .Setup(xc => xc.FilePreparationDate())
                .Returns(DateTime.Parse(fileDate));

            var sut = new WorkPlaceEmpId_03Rule(handler.Object, service.Object);

            var result = sut.IsInsideTheRegistrationPeriod(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(WorkPlaceEmpId_03Rule.TemporaryEmpID, true)]
        [InlineData(123456, false)]
        [InlineData(null, false)]
        public void RequiresEmployerRegistrationMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDeliveryWorkPlacement>();
            mockDelivery
                .SetupGet(y => y.WorkPlaceEmpIdNullable)
                .Returns(candidate);

            var result = sut.RequiresEmployerRegistration(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockItem = new Mock<ILearningDeliveryWorkPlacement>();
            mockItem
                .SetupGet(y => y.WorkPlaceEmpIdNullable)
                .Returns(WorkPlaceEmpId_03Rule.TemporaryEmpID);
            mockItem
                .SetupGet(y => y.WorkPlaceStartDate)
                .Returns(DateTime.Parse("2018-06-15"));

            var placements = new List<ILearningDeliveryWorkPlacement>
            {
                mockItem.Object
            };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(ProgTypes.Traineeship);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryWorkPlacements)
                .Returns(placements);

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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(WorkPlaceEmpId_03Rule.Name, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceEmpId, WorkPlaceEmpId_03Rule.TemporaryEmpID))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<IFileDataService>(MockBehavior.Strict);
            service
                .Setup(xc => xc.FilePreparationDate())
                .Returns(DateTime.Parse("2018-08-14"));

            var sut = new WorkPlaceEmpId_03Rule(handler.Object, service.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockItem = new Mock<ILearningDeliveryWorkPlacement>();
            mockItem
                .SetupGet(y => y.WorkPlaceEmpIdNullable)
                .Returns(WorkPlaceEmpId_03Rule.TemporaryEmpID);
            mockItem
                .SetupGet(y => y.WorkPlaceStartDate)
                .Returns(DateTime.Parse("2018-05-14"));

            var placements = new List<ILearningDeliveryWorkPlacement>
            {
                mockItem.Object
            };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(ProgTypes.Traineeship);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryWorkPlacements)
                .Returns(placements);

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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IFileDataService>(MockBehavior.Strict);
            service
                .Setup(xc => xc.FilePreparationDate())
                .Returns(DateTime.Parse("2018-08-14"));

            var sut = new WorkPlaceEmpId_03Rule(handler.Object, service.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
        }

        public WorkPlaceEmpId_03Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IFileDataService>(MockBehavior.Strict);

            return new WorkPlaceEmpId_03Rule(handler.Object, service.Object);
        }
    }
}
