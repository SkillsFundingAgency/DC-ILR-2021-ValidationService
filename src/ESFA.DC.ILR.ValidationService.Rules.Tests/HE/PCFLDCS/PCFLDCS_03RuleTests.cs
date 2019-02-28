using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCFLDCS;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.PCFLDCS
{
    public class PCFLDCS_03RuleTests : AbstractRuleTests<PCFLDCS_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PCFLDCS_03");
        }

        [Fact]
        public void LDCSCodeConditionMet_True()
        {
            var learnAimRef = "XXX";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef))
                .Returns(false);

            NewRule(larsDataServiceMock.Object).LDCSCodeConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LDCSCodeConditionMet_False()
        {
            var learnAimRef = "12345678";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef))
                .Returns(true);

            NewRule(larsDataServiceMock.Object).LDCSCodeConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                PCFLDCSNullable = 1
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHe).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_FalseNullObject()
        {
            NewRule().LearningDeliveryHEConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_FalseNullPCFLDCS()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE();

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnAimRef = "XXX";

            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                PCFLDCSNullable = 1
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef))
                .Returns(false);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnAimRef, learningDeliveryHe).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnAimRef = "XXX";

            var learningDeliveryHe = new TestLearningDeliveryHE();

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef))
                .Returns(false);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnAimRef, learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "12345678",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            PCFLDCSNullable = 1
                        }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(It.IsAny<string>()))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "12345678",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            PCFLDCSNullable = null
                        }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(It.IsAny<string>()))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_LearningDeliveryHENull()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "12345678",
                        LearningDeliveryHEEntity = null
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(It.IsAny<string>()))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            decimal? pcfldcs = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PCFLDCS, pcfldcs)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(pcfldcs);

            validationErrorHandlerMock.Verify();
        }

        private PCFLDCS_03Rule NewRule(
            ILARSDataService larsDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new PCFLDCS_03Rule(larsDataService, validationErrorHandler);
        }
    }
}
