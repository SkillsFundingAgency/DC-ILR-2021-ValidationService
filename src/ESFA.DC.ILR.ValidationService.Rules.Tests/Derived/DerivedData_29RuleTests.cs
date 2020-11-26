using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_29RuleTests
    {
        [Theory]
        [InlineData(ProgTypes.AdvancedLevelApprenticeship, false)]
        [InlineData(ProgTypes.ApprenticeshipStandard, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel4, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel5, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel6, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(ProgTypes.IntermediateLevelApprenticeship, false)]
        [InlineData(ProgTypes.Traineeship, true)]
        public void IsTraineeshipMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);

            var result = sut.IsTraineeship(mockItem.Object);

            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        [Theory]
        [InlineData(LARSConstants.Categories.WorkPlacementSFAFunded, true)]
        [InlineData(LARSConstants.Categories.WorkPreparationSFATraineeships, true)]
        [InlineData(1, false)]
        [InlineData(3, false)]
        public void IsWorkExperienceMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILARSLearningCategory>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.CategoryRef)
                .Returns(candidate);

            var result = sut.IsWorkExperience(mockItem.Object);

            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        [Theory]
        [InlineData("asdflaskdfjl", LARSConstants.Categories.WorkPlacementSFAFunded, true)]
        [InlineData("eprtyodityp", LARSConstants.Categories.WorkPreparationSFATraineeships, true)]
        [InlineData("xcmvzx", 1, false)]
        [InlineData("sfieasfn", 3, false)]
        public void IsWorkExperience2MeetsExpectation(string aimRef, int candidate, bool expectation)
        {
            var mockDelivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimRef);

            var mockItem = new Mock<ILARSLearningCategory>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.CategoryRef)
                .Returns(candidate);

            var categories = new List<ILARSLearningCategory>
            {
                mockItem.Object
            };

            var mockLARS = new Mock<ILARSDataService>(MockBehavior.Strict);
            mockLARS
                .Setup(x => x.GetCategoriesFor(aimRef))
                .Returns(categories);

            var sut = new DerivedData_29Rule(mockLARS.Object);

            var result = sut.IsWorkExperience(mockDelivery.Object);

            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        public DerivedData_29Rule NewRule()
        {
            var mockLARS = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new DerivedData_29Rule(mockLARS.Object);
        }
    }
}
