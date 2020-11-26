using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_18RuleTests
    {
        [Fact]
        public void HasMatchingStandardCodeMeetsWithNullDeliveryReturnsFalse()
        {
            var sut = NewRule();

            var result = sut.HasMatchingStandardCode(null, null);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null, 1, false)]
        public void HasMatchingStandardCodeMeetsExpectation(int? deliveryCode, int? candidateCode, bool expectation)
        {
            var sut = NewRule();

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(deliveryCode);

            var candidate = new Mock<ILearningDelivery>();
            candidate
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidateCode);

            var result = sut.HasMatchingStandardCode(delivery.Object, candidate.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(AimTypes.ProgrammeAim, "2018-01-01", ProgTypes.ApprenticeshipStandard, 1, "2018-01-01")]
        [InlineData(AimTypes.ComponentAimInAProgramme, "2018-01-10", ProgTypes.ApprenticeshipStandard, 1, "2018-01-01")]
        [InlineData(AimTypes.AimNotPartOfAProgramme, "2018-01-20", null, null, null)]
        [InlineData(AimTypes.ProgrammeAim, "2019-01-01", ProgTypes.ApprenticeshipStandard, 2, "2019-01-01")]
        [InlineData(AimTypes.ComponentAimInAProgramme, "2019-01-10", ProgTypes.ApprenticeshipStandard, 2, "2019-01-01")]
        public void GetApprenticeshipStandardProgrammeStartDateForMeetsExpectation(int aimType, string learnStartDate, int? progType, int? stdcode, string expectation)
        {
            var delivery = GetTestDelivery(aimType, learnStartDate, progType, stdcode);
            var expectedDate = string.IsNullOrWhiteSpace(expectation)
                ? (DateTime?)null
                : DateTime.Parse(expectation);

            var result = NewRule().GetApprenticeshipStandardProgrammeStartDateFor(delivery, GetTestDeliveries());

            Assert.Equal(expectedDate, result);
        }

        public IReadOnlyCollection<ILearningDelivery> GetTestDeliveries()
        {
            var candidates = new List<ILearningDelivery>();

            candidates.Add(GetTestDelivery(AimTypes.ProgrammeAim, "2018-01-01", ProgTypes.ApprenticeshipStandard, 1));
            candidates.Add(GetTestDelivery(AimTypes.ComponentAimInAProgramme, "2018-01-10", ProgTypes.ApprenticeshipStandard, 1));
            candidates.Add(GetTestDelivery(AimTypes.AimNotPartOfAProgramme, "2018-01-20", null, null));
            candidates.Add(GetTestDelivery(AimTypes.ProgrammeAim, "2019-01-01", ProgTypes.ApprenticeshipStandard, 2));
            candidates.Add(GetTestDelivery(AimTypes.ComponentAimInAProgramme, "2019-01-10", ProgTypes.ApprenticeshipStandard, 2));

            return candidates;
        }

        public ILearningDelivery GetTestDelivery(int aimType, string learnStartDate, int? progType, int? stdcode)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(learnStartDate));
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);
            delivery
                .SetupGet(x => x.AimType)
                .Returns(aimType);
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(stdcode);

            return delivery.Object;
        }

        public DerivedData_18Rule NewRule()
        {
            return new DerivedData_18Rule();
        }
    }
}
