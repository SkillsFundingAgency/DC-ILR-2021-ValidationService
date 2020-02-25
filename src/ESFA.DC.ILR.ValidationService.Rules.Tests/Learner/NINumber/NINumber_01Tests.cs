using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.NiNumber;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.NINumber
{
    public class NINumber_01Tests : AbstractRuleTests<NINumber_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("NINumber_01");
        }

        [Theory]
        [InlineData("AA123456AA")]
        [InlineData("A")]
        public void ConditionMet_True_Length(string niNumber)
        {
            NewRule().ConditionMet(niNumber).Should().BeTrue();
        }

        [Theory]
        [InlineData("DZ123456C")]
        [InlineData("FZ123456C")]
        [InlineData("IZ123456C")]
        [InlineData("QZ123456C")]
        [InlineData("UZ123456C")]
        [InlineData("VZ123456C")]
        public void ConditionMet_True_FirstCharacter(string niNumber)
        {
            NewRule().ConditionMet(niNumber).Should().BeTrue();
        }

        [Theory]
        [InlineData("AD123456C")]
        [InlineData("AF123456C")]
        [InlineData("AI123456C")]
        [InlineData("AO123456C")]
        [InlineData("AQ123456C")]
        [InlineData("AU123456C")]
        [InlineData("AV123456C")]
        public void ConditionMet_True_SecondCharacter(string niNumber)
        {
            NewRule().ConditionMet(niNumber).Should().BeTrue();
        }

        [Theory]
        [InlineData("AAX23456C")]
        [InlineData("AA1X3456C")]
        [InlineData("AAX2X456C")]
        [InlineData("AAX23X56C")]
        [InlineData("AAX234X6C")]
        [InlineData("AAX2345XC")]
        public void ConditionMet_True_Characters3To8(string niNumber)
        {
            NewRule().ConditionMet(niNumber).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_LastCharacter()
        {
            for (char letter = 'E'; letter <= 'Z'; letter++)
            {
                var niNumber = $"AA123456{letter}";
                NewRule().ConditionMet(niNumber).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("aA123456A")]
        [InlineData("Aa123456A")]
        [InlineData("AA123456a")]
        public void ConditionMet_True_LowerCase(string niNumber)
        {
            NewRule().ConditionMet(niNumber).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        public void ConditionMet_False_NullOrEmpty(string niNumber)
        {
            NewRule().ConditionMet(niNumber).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FirstCharacter()
        {
            var notAllowed = new List<char>() { 'D', 'F', 'I', 'Q', 'U', 'V' };
            char[] allowed = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).Where(x => !notAllowed.Contains(x)).ToArray();

            foreach (char letter in allowed)
            {
                var niNumber = $"{letter}A123456A";
                NewRule().ConditionMet(niNumber).Should().BeFalse();
            }
        }

        [Fact]
        public void ConditionMet_False_SecondCharacter()
        {
            var notAllowed = new List<char>() { 'D', 'F', 'I', 'O', 'Q', 'U', 'V' };
            var allowed = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).Where(x => !notAllowed.Contains(x)).ToArray();

            foreach (char letter in allowed)
            {
                var niNumber = $"{letter}A123456A";
                NewRule().ConditionMet(niNumber).Should().BeFalse();
            }
        }

        [Fact]
        public void ConditionMet_False_LastCharacter()
        {
            var allowed = new List<char>() { 'A', 'B', 'C', 'D' };

            foreach (char letter in allowed)
            {
                var niNumber = $"AA123456{letter}";
                NewRule().ConditionMet(niNumber).Should().BeFalse();
            }
        }

        [Fact]
        public void ConditionMet_False_LastCharacterEmpty()
        {
            var niNumber = "AA123456";

            NewRule().ConditionMet(niNumber).Should().BeFalse();
        }

        [Theory]
        [InlineData("AA123456A")]
        [InlineData("AA123456")]
        [InlineData(null)]
        [InlineData(" ")]
        public void Validate_NoErrors(string niNumber)
        {
            var learner = new TestLearner
            {
                NINumber = niNumber
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData("AA123456AA")]
        [InlineData("AA23456A")]
        [InlineData("DD123456A")]
        public void Validate_Error(string niNumber)
        {
            var learner = new TestLearner
            {
                NINumber = niNumber
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("NINumber", " ")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(" ");

            validationErrorHandlerMock.Verify();
        }

        private NINumber_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new NINumber_01Rule(validationErrorHandler);
        }
    }
}
