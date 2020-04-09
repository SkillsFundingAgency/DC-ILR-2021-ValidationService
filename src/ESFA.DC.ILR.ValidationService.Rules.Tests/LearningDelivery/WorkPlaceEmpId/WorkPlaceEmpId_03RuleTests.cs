using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEmpId;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceEmpId
{
    public class WorkPlaceEmpId_03RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IFileDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new WorkPlaceEmpId_03Rule(null, service.Object));
        }

        /// <summary>
        /// New rule with null data service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDataServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IFileDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new WorkPlaceEmpId_03Rule(handler.Object, null));
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
            Assert.Equal("WorkPlaceEmpId_03", result);
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
            Assert.Equal(WorkPlaceEmpId_03Rule.Name, result);
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
        /// Is qualifying programme meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel6, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(TypeOfLearningProgramme.IntermediateLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.Traineeship, true)]
        [InlineData(null, false)]
        public void IsQualifyingProgrammeMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);

            // act
            var result = sut.IsQualifyingProgramme(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has exceed registration period meets expectation
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="fileDate">The file date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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
            // arrange
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

            // act
            var result = sut.IsInsideTheRegistrationPeriod(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Requires employer registration meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(WorkPlaceEmpId_03Rule.TemporaryEmpID, true)]
        [InlineData(123456, false)]
        [InlineData(null, false)]
        public void RequiresEmployerRegistrationMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDeliveryWorkPlacement>();
            mockDelivery
                .SetupGet(y => y.WorkPlaceEmpIdNullable)
                .Returns(candidate);

            // act
            var result = sut.RequiresEmployerRegistration(mockDelivery.Object);

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

            var mockItem = new Mock<ILearningDeliveryWorkPlacement>();
            mockItem
                .SetupGet(y => y.WorkPlaceEmpIdNullable)
                .Returns(WorkPlaceEmpId_03Rule.TemporaryEmpID);
            mockItem
                .SetupGet(y => y.WorkPlaceStartDate)
                .Returns(DateTime.Parse("2018-06-15"));

            var placements = new List<ILearningDeliveryWorkPlacement>();
            placements.Add(mockItem.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.Traineeship);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryWorkPlacements)
                .Returns(placements);

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
                .Setup(x => x.Handle(WorkPlaceEmpId_03Rule.Name, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceEmpId, WorkPlaceEmpId_03Rule.TemporaryEmpID))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<IFileDataService>(MockBehavior.Strict);
            service
                .Setup(xc => xc.FilePreparationDate())
                .Returns(DateTime.Parse("2018-08-14"));

            var sut = new WorkPlaceEmpId_03Rule(handler.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockItem = new Mock<ILearningDeliveryWorkPlacement>();
            mockItem
                .SetupGet(y => y.WorkPlaceEmpIdNullable)
                .Returns(WorkPlaceEmpId_03Rule.TemporaryEmpID);
            mockItem
                .SetupGet(y => y.WorkPlaceStartDate)
                .Returns(DateTime.Parse("2018-05-14"));

            var placements = new List<ILearningDeliveryWorkPlacement>();
            placements.Add(mockItem.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.Traineeship);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryWorkPlacements)
                .Returns(placements);

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
            var service = new Mock<IFileDataService>(MockBehavior.Strict);
            service
                .Setup(xc => xc.FilePreparationDate())
                .Returns(DateTime.Parse("2018-08-14"));

            var sut = new WorkPlaceEmpId_03Rule(handler.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public WorkPlaceEmpId_03Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IFileDataService>(MockBehavior.Strict);

            return new WorkPlaceEmpId_03Rule(handler.Object, service.Object);
        }
    }
}
