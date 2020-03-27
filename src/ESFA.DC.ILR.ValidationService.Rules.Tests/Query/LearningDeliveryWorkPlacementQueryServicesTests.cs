using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearningDeliveryWorkPlacementQueryServicesTests
    {
        [Fact]
        public void HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate_True()
        {
            DateTime? learnActEndDate = new DateTime(2018, 11, 1);

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 10, 1) },
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 09, 1) },
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 12, 1) },
            };

            NewService().HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate).Should().BeTrue();
        }

        [Fact]
        public void HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate_False()
        {
            DateTime? learnActEndDate = new DateTime(2018, 11, 1);

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 10, 1) },
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 09, 1) },
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = null },
            };

            NewService().HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate_FalseNull()
        {
            DateTime? learnActEndDate = null;

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = null },
            };

            NewService().HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void HasAnyEmpIdNullAndStartDateNotNull_True()
        {
            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceStartDate = new DateTime(2018, 10, 01) },
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceEmpIdNullable = 123456789,
                    WorkPlaceStartDate = new DateTime(2018, 10, 01)
                },
            };

            NewService().HasAnyEmpIdNullAndStartDateNotNull(learningDeliveryWorkPlacements).Should().BeTrue();
        }

        [Fact]
        public void HasAnyEmpIdNullAndStartDateNotNull_False()
        {
            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceStartDate = new DateTime(2018, 10, 01),
                    WorkPlaceEmpIdNullable = 123456789
                },
            };

            NewService().HasAnyEmpIdNullAndStartDateNotNull(learningDeliveryWorkPlacements).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(0, false)]
        public void IsValidWorkPlaceModeMeetsExpectation(int workPlaceMode, bool expectation)
        {
            var isValidWorkPlaceMode = NewService().IsValidWorkPlaceMode(workPlaceMode);
            isValidWorkPlaceMode.Should().Be(expectation);
        }

        [Fact]
        public void HasAnyEmpIdNullAndStartDateNotNull_FalseNull()
        {
            NewService().HasAnyEmpIdNullAndStartDateNotNull(null).Should().BeFalse();
        }

        private LearningDeliveryWorkPlacementQueryService NewService()
        {
            return new LearningDeliveryWorkPlacementQueryService();
        }
    }
}
