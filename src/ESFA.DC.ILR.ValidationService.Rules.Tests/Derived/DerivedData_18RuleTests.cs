using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_18RuleTests
    {
        [Fact]
        public void GetApprenticeshipStandardProgrammeStartDateForWithNullDeliveryThrows()
        {
            // arrange
            var sut = NewRule();

            var deliveries = new List<ILearningDelivery>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.GetApprenticeshipStandardProgrammeStartDateFor(null, deliveries));
        }

        [Fact]
        public void GetApprenticeshipStandardProgrammeStartDateForWithNullSourcesThrows()
        {
            // arrange
            var sut = NewRule();

            var delivery = new Mock<ILearningDelivery>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.GetApprenticeshipStandardProgrammeStartDateFor(delivery.Object, null));
        }

        [Fact]
        public void GetApprenticeshipStandardProgrammeStartDateForWithDeliveryNotInSourcesThrows()
        {
            // arrange
            var sut = NewRule();

            var delivery = new Mock<ILearningDelivery>();
            var deliveries = new List<ILearningDelivery>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.GetApprenticeshipStandardProgrammeStartDateFor(delivery.Object, deliveries));
        }

        /// <summary>
        /// Has matching standard code meets with null delivery returns false
        /// </summary>
        [Fact]
        public void HasMatchingStandardCodeMeetsWithNullDeliveryReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasMatchingStandardCode(null, null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has matching standard code meets expectation
        /// </summary>
        /// <param name="deliveryCode">The delivery code.</param>
        /// <param name="candidateCode">The candidate code.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, 1, false)]
        public void HasMatchingStandardCodeMeetsExpectation(int? deliveryCode, int? candidateCode, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(deliveryCode);

            var candidate = new Mock<ILearningDelivery>();
            candidate
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidateCode);

            // act
            var result = sut.HasMatchingStandardCode(delivery.Object, candidate.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get apprenticeship standard programme start date for, meets expectation.
        /// </summary>
        /// <param name="aimType">Type of the aim.</param>
        /// <param name="learnStartDate">The learn start date.</param>
        /// <param name="progType">Type of the prog.</param>
        /// <param name="stdcode">The stdcode.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData(TypeOfAim.ProgrammeAim, "2018-01-01", TypeOfLearningProgramme.ApprenticeshipStandard, 1, "2018-01-01")]
        [InlineData(TypeOfAim.ComponentAimInAProgramme, "2018-01-10", TypeOfLearningProgramme.ApprenticeshipStandard, 1, "2018-01-01")]
        [InlineData(TypeOfAim.AimNotPartOfAProgramme, "2018-01-20", null, null, null)]
        [InlineData(TypeOfAim.ProgrammeAim, "2019-01-01", TypeOfLearningProgramme.ApprenticeshipStandard, 2, "2019-01-01")]
        [InlineData(TypeOfAim.ComponentAimInAProgramme, "2019-01-10", TypeOfLearningProgramme.ApprenticeshipStandard, 2, "2019-01-01")]
        public void GetApprenticeshipStandardProgrammeStartDateForMeetsExpectation(int aimType, string learnStartDate, int? progType, int? stdcode, string expectation)
        {
            // arrange
            var delivery = GetTestDelivery(aimType, learnStartDate, progType, stdcode);
            var expectedDate = string.IsNullOrWhiteSpace(expectation)
                ? (DateTime?)null
                : DateTime.Parse(expectation);

            var service = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            service
                .Setup(x => x.IsStandardApprenticeship(Moq.It.IsAny<ILearningDelivery>()))
                .Returns(true);
            service
                .Setup(x => x.InAProgramme(Moq.It.IsAny<ILearningDelivery>()))
                .Returns(true);

            var sut = new DerivedData_18Rule(service.Object);

            // act
            var result = sut.GetApprenticeshipStandardProgrammeStartDateFor(delivery, GetTestDeliveries());

            // assert
            Assert.Equal(expectedDate, result);
        }

        /// <summary>
        /// Gets the test deliveries.
        /// </summary>
        /// <returns>returns  a list of test candidates provided by Khush</returns>
        public IReadOnlyCollection<ILearningDelivery> GetTestDeliveries()
        {
            /*
            Aim 1    01/01/2018       ProgType 25  StdCode 1  DD18 01/01/2018
            Aim 3    10/01/2018       ProgType 25  StdCode 1  DD18 01/01/2018
            Aim 4    20/01/2018                               DD18 NULL
            Aim 1    01/01/2019       ProgType 25  StdCode 2  DD18 01/01/2019
            Aim 3    10/01/2019       ProgType 25  StdCode 2  DD18 01/01/2019
            */

            var candidates = new List<ILearningDelivery>();

            candidates.Add(GetTestDelivery(TypeOfAim.ProgrammeAim, "2018-01-01", TypeOfLearningProgramme.ApprenticeshipStandard, 1));
            candidates.Add(GetTestDelivery(TypeOfAim.ComponentAimInAProgramme, "2018-01-10", TypeOfLearningProgramme.ApprenticeshipStandard, 1));
            candidates.Add(GetTestDelivery(TypeOfAim.AimNotPartOfAProgramme, "2018-01-20", null, null));
            candidates.Add(GetTestDelivery(TypeOfAim.ProgrammeAim, "2019-01-01", TypeOfLearningProgramme.ApprenticeshipStandard, 2));
            candidates.Add(GetTestDelivery(TypeOfAim.ComponentAimInAProgramme, "2019-01-10", TypeOfLearningProgramme.ApprenticeshipStandard, 2));

            return candidates;
        }

        /// <summary>
        /// Gets the test delivery.
        /// </summary>
        /// <param name="aimType">Type of the aim.</param>
        /// <param name="learnStartDate">The learn start date.</param>
        /// <param name="progType">the programme type.</param>
        /// <param name="stdcode">The standard code.</param>
        /// <returns>a configured learning delivery</returns>
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

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up derived data rule</returns>
        public DerivedData_18Rule NewRule()
        {
            var service = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new DerivedData_18Rule(service.Object);
        }
    }
}
