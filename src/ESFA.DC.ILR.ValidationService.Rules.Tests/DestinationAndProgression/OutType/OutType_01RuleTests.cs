using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.DestinationAndProgression.OutType
{
    public class OutType_01RuleTests : AbstractRuleTests<OutType_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutType_01");
        }

        [Fact]
        public void OutTypeConditionMet_True()
        {
            var dpOutcome = new TestDPOutcome
            {
               OutType = "EDU"
            };

            NewRule().OutTypeNullConditionMet(dpOutcome).Should().BeTrue();
        }

        [Fact]
        public void OutTypeConditionMet_False()
        {
            var dpOutcome = new TestDPOutcome
            {
                OutType = null
            };

            NewRule().OutTypeNullConditionMet(dpOutcome).Should().BeFalse();
        }

        [Theory]
        [InlineData(null, 10)]
        [InlineData("EDU", 10)]
        [InlineData("XXX", 1)]
        public void OutCodeConditionMet_True(string outType, int outCode)
        {
            var dpOutcome = new TestDPOutcome
            {
                OutType = outType,
                OutCode = outCode
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(TypeOfLimitedLifeLookup.OutType, $"{dpOutcome.OutType}{dpOutcome.OutCode}")).Returns(false);

            NewRule(lookupsMock.Object).OutCodeConditionMet(dpOutcome).Should().BeTrue();
        }

        [Fact]
        public void OutCodeConditionMet_False()
        {
            var dpOutcome = new TestDPOutcome
            {
                OutType = "EDU",
                OutCode = 1
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(TypeOfLimitedLifeLookup.OutType, $"{dpOutcome.OutType}{dpOutcome.OutCode}")).Returns(true);

            NewRule(lookupsMock.Object).OutCodeConditionMet(dpOutcome).Should().BeFalse();
        }

        [Theory]
        [InlineData("EDU", 10)]
        [InlineData("XXX", 1)]
        public void ConditionMet_True(string outType, int outCode)
        {
            var dpOutcome = new TestDPOutcome
            {
                OutType = outType,
                OutCode = outCode
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(TypeOfLimitedLifeLookup.OutType, $"{dpOutcome.OutType}{dpOutcome.OutCode}")).Returns(false);

            NewRule(lookupsMock.Object).ConditionMet(dpOutcome).Should().BeTrue();
        }

        [Theory]
        [InlineData("EDU", 1)]
        [InlineData(null, 1)]
        public void ConditionMet_False(string outType, int outCode)
        {
            var dpOutcome = new TestDPOutcome
            {
                OutType = outType,
                OutCode = outCode
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(TypeOfLimitedLifeLookup.OutType, $"{dpOutcome.OutType}{dpOutcome.OutCode}")).Returns(true);

            NewRule(lookupsMock.Object).ConditionMet(dpOutcome).Should().BeFalse();
        }

        [Theory]
        [InlineData("EDU", 10)]
        [InlineData("XXX", 1)]
        public void Validate_Errors(string outType, int outCode)
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutType = outType,
                        OutCode = outCode
                    }
                }
            };

            var dpOutcome = learnerDP.DPOutcomes.FirstOrDefault();

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(TypeOfLimitedLifeLookup.OutType, $"{dpOutcome.OutType}{dpOutcome.OutCode}")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(lookupsMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Theory]
        [InlineData("EDU", 1)]
        [InlineData(null, 1)]
        public void Validate_NoErrors(string outType, int outCode)
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutType = outType,
                        OutCode = outCode
                    }
                }
            };

            var dpOutcome = learnerDP.DPOutcomes.FirstOrDefault();

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(TypeOfLimitedLifeLookup.OutType, $"{dpOutcome.OutType}{dpOutcome.OutCode}")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(lookupsMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutType", "Type")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutCode", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("Type", 1);

            validationErrorHandlerMock.Verify();
        }

        private OutType_01Rule NewRule(IProvideLookupDetails lookups = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutType_01Rule(lookups, validationErrorHandler);
        }
    }
}
