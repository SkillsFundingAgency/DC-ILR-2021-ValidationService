using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.ESMType
{
    public class ESMType_17RuleTests : AbstractRuleTests<ESMType_17Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ESMType_17");
        }

        [Fact]
        public void LearningDeliveryConditionMet_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    LearningDeliveryFAMs = new ILearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                            LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_375
                        }
                    }
                }
            };

            NewRule().LearningDeliveryConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryConditionMet_False()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    LearningDeliveryFAMs = new ILearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                            LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_375
                        }
                    }
                }
            };

            NewRule().LearningDeliveryConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryConditionMet_FalseNullDelivery()
        {
            NewRule().LearningDeliveryConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new ILearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_375
                            }
                        }
                    }
                },
                LearnerEmploymentStatuses = new ILearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmploymentStatusMonitorings = new IEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = Monitoring.EmploymentStatus.Types.BenefitStatusIndicator,
                                ESMCode = LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfOtherStateBenefits
                            }
                        }
                    }
                },
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new ILearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_375
                            }
                        }
                    }
                },
                LearnerEmploymentStatuses = new ILearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmploymentStatusMonitorings = new IEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = Monitoring.EmploymentStatus.Types.BenefitStatusIndicator,
                                ESMCode = LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfUniversalCredit
                            }
                        }
                    }
                },
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoEmploymentStatuses()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new ILearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_375
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", LearningDeliveryFAMTypeConstants.LDM)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMCode", LearningDeliveryFAMCodeConstants.LDM_375)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ESMType", Monitoring.EmploymentStatus.Types.BenefitStatusIndicator)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ESMCode", 6)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(6);

            validationErrorHandlerMock.Verify();
        }

        private ESMType_17Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ESMType_17Rule(validationErrorHandler);
        }
    }
}
