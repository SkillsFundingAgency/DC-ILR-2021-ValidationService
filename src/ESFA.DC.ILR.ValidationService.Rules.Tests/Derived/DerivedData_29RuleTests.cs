using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    /// <summary>
    /// derived data rule 29 tests
    /// </summary>
    public class DerivedData_29RuleTests
    {
        /// <summary>
        /// Determines whether [is inflexible element of training aim with null learneing delivery throws].
        /// </summary>
        [Fact]
        public void IsInflexibleElementOfTrainingAimLearningDeliveryWithNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => NewRule().IsInflexibleElementOfTrainingAimLearningDelivery(null));
        }

        /// <summary>
        /// Determines whether [is traineeship meets expectation] [the specified candidate].
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
        public void IsTraineeshipMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);

            // act
            var result = sut.IsTraineeship(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        /// <summary>
        /// Determines whether [is work experience meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfLARSCategory.WorkPlacementSFAFunded, true)]
        [InlineData(TypeOfLARSCategory.WorkPreparationSFATraineeships, true)]
        [InlineData(1, false)]
        [InlineData(3, false)]
        public void IsWorkExperienceMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILARSLearningCategory>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.CategoryRef)
                .Returns(candidate);

            // act
            var result = sut.IsWorkExperience(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        /// <summary>
        /// Determines whether [is work experience (2) meets expectation] [the specified aim reference].
        /// </summary>
        /// <param name="aimRef">The aim reference.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("asdflaskdfjl", TypeOfLARSCategory.WorkPlacementSFAFunded, true)]
        [InlineData("eprtyodityp", TypeOfLARSCategory.WorkPreparationSFATraineeships, true)]
        [InlineData("xcmvzx", 1, false)]
        [InlineData("sfieasfn", 3, false)]
        public void IsWorkExperience2MeetsExpectation(string aimRef, int candidate, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimRef);

            var mockItem = new Mock<ILARSLearningCategory>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.CategoryRef)
                .Returns(candidate);

            var categories = new List<ILARSLearningCategory>();
            categories.Add(mockItem.Object);

            var mockLARS = new Mock<ILARSDataService>(MockBehavior.Strict);
            mockLARS
                .Setup(x => x.GetCategoriesFor(aimRef))
                .Returns(categories);

            var sut = new DerivedData_29Rule(mockLARS.Object);

            // act
            var result = sut.IsWorkExperience(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up derived data rule</returns>
        public DerivedData_29Rule NewRule()
        {
            var mockLARS = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new DerivedData_29Rule(mockLARS.Object);
        }
    }
}
