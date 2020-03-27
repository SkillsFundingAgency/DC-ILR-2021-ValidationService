using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Sex;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.Sex
{
    public class Sex_01RuleTests : AbstractRuleTests<Sex_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Sex_01");
        }

        /// <summary>
        /// is valid sex meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("A", true)]
        [InlineData("A", false)]
        [InlineData("F", true)]
        [InlineData("F", false)]
        [InlineData("M", true)]
        [InlineData("M", false)]
        [InlineData("", true)]
        [InlineData("", false)]
        [InlineData(null, true)]
        [InlineData(null, false)]
        [InlineData(" ", true)]
        [InlineData(" ", false)]
        [InlineData("0", true)]
        [InlineData("0", false)]
        public void IsValidSexMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(p => p.Contains(TypeOfStringCodedLookup.Sex, candidate))
                .Returns(expectation);

            var sut = new Sex_01Rule(handler.Object, provider.Object);

            // act
            var result = sut.IsValidSex(candidate);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                Sex = "X"
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails
                .Setup(p => p.Contains(TypeOfStringCodedLookup.Sex, learner.Sex))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                Sex = "F"
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();
            provideLookupDetails
                .Setup(p => p.Contains(TypeOfStringCodedLookup.Sex, learner.Sex))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, provideLookupDetails.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.Sex, "A"))
                .Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("A");

            validationErrorHandlerMock.Verify();
        }

        private Sex_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IProvideLookupDetails provideLookupDetails = null)
        {
            return new Sex_01Rule(validationErrorHandler, provideLookupDetails);
        }
    }
}