using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_77RuleTests : AbstractRuleTests<LearnDelFAMType_77Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_77");
        }

        [Fact]
        public void ValidationPasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.GetLearningDeliveryFAMsCountByFAMType(It.IsAny<List<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM))
                .Returns(4);

            NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(It.IsAny<TestLearner>());
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.GetLearningDeliveryFAMsCountByFAMType(It.IsAny<List<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM))
                .Returns(0);

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_NoFAMs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.GetLearningDeliveryFAMsCountByFAMType(It.IsAny<List<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM))
                .Returns(0);

            NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationFails()
        {
            List<TestLearningDeliveryFAM> learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                   LearnDelFAMType = "DAM"
                },
                new TestLearningDeliveryFAM()
                {
                   LearnDelFAMType = "DAM"
                },
                new TestLearningDeliveryFAM()
                {
                   LearnDelFAMType = "DAM"
                },
                new TestLearningDeliveryFAM()
                {
                   LearnDelFAMType = "DAM"
                },
                new TestLearningDeliveryFAM()
                {
                   LearnDelFAMType = "DAM"
                },
            };

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.DAM))
                .Returns(5);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        private LearnDelFAMType_77Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearnDelFAMType_77Rule(
                validationErrorHandler: validationErrorHandler,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }

        private void VerifyHandleNotCalled(ValidationErrorHandlerMock errorHandlerMock)
        {
            errorHandlerMock.Verify(
                m => m.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()),
                Times.Never);
        }
    }
}
