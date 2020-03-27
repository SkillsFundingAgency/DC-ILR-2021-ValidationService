using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutULN;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class OutULN_02RuleTests : AbstractRuleTests<OutULN_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutULN_02");
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearnerDestinationAndProgression
            {
                ULN = 1
            };

            var ulnDataServiceMock = new Mock<IULNDataService>();

            ulnDataServiceMock.Setup(urds => urds.Exists(1)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(ulnDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_TempULNPasses()
        {
            var learner = new TestLearnerDestinationAndProgression
            {
                ULN = ValidationConstants.TemporaryULN
            };

            var ulnDataServiceMock = new Mock<IULNDataService>();

            ulnDataServiceMock.Setup(urds => urds.Exists(It.IsAny<long>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(ulnDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearnerDestinationAndProgression
            {
                ULN = 1,
            };

            var ulnDataServiceMock = new Mock<IULNDataService>();

            ulnDataServiceMock.Setup(urds => urds.Exists(1)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(ulnDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private OutULN_02Rule NewRule(IULNDataService ulnDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutULN_02Rule(ulnDataService, validationErrorHandler);
        }
    }
}
