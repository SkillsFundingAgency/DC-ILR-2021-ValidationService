using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_22RuleTests
    {
        /// <summary>
        /// Get latest learning start for esf contract with null candidate throws.
        /// </summary>
        [Fact]
        public void GetLatestLearningStartForESFContractWithNullCandidateThrows()
        {
            // arrange
            var sut = NewRule();
            var sources = new List<ILearningDelivery>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.GetLatestLearningStartForESFContract(null, sources));
        }

        /// <summary>
        /// Get latest learning start for esf contract with null sources throws.
        /// </summary>
        [Fact]
        public void GetLatestLearningStartForESFContractWithNullSourcesThrows()
        {
            // arrange
            var sut = NewRule();
            var candidate = new Mock<ILearningDelivery>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.GetLatestLearningStartForESFContract(candidate.Object, null));
        }

        /// <summary>
        /// Determines whether [has matching contract reference meets expectation] [the specified source reference].
        /// </summary>
        /// <param name="sourceRef">The source reference.</param>
        /// <param name="candidateRef">The candidate reference.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, null, false)]
        [InlineData("", "", false)]
        [InlineData("123", "123", true)]
        [InlineData("321", "123", false)]
        [InlineData("A123", "A123", true)]
        [InlineData("A-3£$%^123", "A-3£$%^123", true)]
        [InlineData("ASDFGH123", "asdfgh123", true)]
        public void HasMatchingContractReferenceMeetsExpectation(string sourceRef, string candidateRef, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ConRefNumber)
                .Returns(sourceRef);
            var mockDelivery2 = new Mock<ILearningDelivery>();
            mockDelivery2
                .SetupGet(y => y.ConRefNumber)
                .Returns(candidateRef);

            // act
            var result = sut.HasMatchingContractReference(mockDelivery.Object, mockDelivery2.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, true)]
        [InlineData(TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasTemporarilyWithdrawn, false)]
        [InlineData(TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasWithdrawn, false)]
        [InlineData(TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.IsOngoing, false)]
        [InlineData(TypeOfAim.References.IndustryPlacement, CompletionState.HasCompleted, false)]
        [InlineData(TypeOfAim.References.WorkExperience, CompletionState.HasCompleted, false)]
        public void IsNotEmployedMeetsExpectation(string aimRef, int completionState, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimRef);
            mockDelivery
                .SetupGet(y => y.CompStatus)
                .Returns(completionState);

            // act
            var result = sut.IsCompletedQualifyingAim(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get latest learning start for esf contract returns null with empty sources.
        /// </summary>
        [Fact]
        public void GetLatestLearningStartForESFContractReturnsNullWithEmptySources()
        {
            // arrange
            var sut = NewRule();
            var candidate = new Mock<ILearningDelivery>();

            var sources = new List<ILearningDelivery>();

            // act / assert
            var result = sut.GetLatestLearningStartForESFContract(candidate.Object, sources);

            // assert
            Assert.Null(result);
        }

        /// <summary>
        /// Gets the latest learning start for esf contract returns todays date.
        /// </summary>
        [Fact]
        public void GetLatestLearningStartForESFContractReturnsTodaysDate()
        {
            // arrange
            const string _conRefNumber = "1234-ILR-TEST-3";

            var rand = new Random(256);
            var sut = NewRule();
            var candidate = new Mock<ILearningDelivery>();
            candidate
                .SetupGet(x => x.LearnAimRef)
                .Returns(TypeOfAim.References.ESFLearnerStartandAssessment);
            candidate
                .SetupGet(x => x.CompStatus)
                .Returns(CompletionState.HasCompleted);
            candidate
                .SetupGet(x => x.ConRefNumber)
                .Returns(_conRefNumber);
            candidate
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Today);

            var sources = new List<ILearningDelivery>();
            for (int i = 0; i < 10; i++)
            {
                var del = new Mock<ILearningDelivery>();

                del
                    .SetupGet(x => x.LearnAimRef)
                    .Returns(TypeOfAim.References.ESFLearnerStartandAssessment);
                del
                    .SetupGet(x => x.CompStatus)
                    .Returns(CompletionState.HasCompleted);
                del
                    .SetupGet(x => x.LearnStartDate)
                    .Returns(DateTime.Today.AddDays(-rand.Next(365)));
                del
                    .SetupGet(x => x.ConRefNumber)
                    .Returns("12346-546-EEE-44");

                sources.Add(del.Object);
            }

            sources.Add(candidate.Object);

            // act / assert
            var result = sut.GetLatestLearningStartForESFContract(candidate.Object, sources);

            // assert
            Assert.Equal(DateTime.Today, result);
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up derived data rule</returns>
        public DerivedData_22Rule NewRule()
        {
            return new DerivedData_22Rule();
        }
    }
}
