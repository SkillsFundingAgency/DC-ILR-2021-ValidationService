using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.SOC2000;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.SOC2000
{
    public class SOC2000_04RuleTests : AbstractRuleTests<SOC2000_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("SOC2000_04");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2014, 08, 02);
            var soc2000 = 1;

            var provideLookupDetailsMock = new Mock<IProvideLookupDetails>();
            provideLookupDetailsMock.Setup(pldm => pldm.Contains(TypeOfIntegerCodedLookup.SOC2010, soc2000)).Returns(false);

            NewRule(provideLookupDetailsMock.Object).ConditionMet(learnStartDate, soc2000).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDateBeforeFirstAugust2014()
        {
            var learnStartDate = new DateTime(2014, 07, 31);
            var soc2000 = 1;

            var provideLookupDetailsMock = new Mock<IProvideLookupDetails>();
            provideLookupDetailsMock.Setup(pldm => pldm.Contains(TypeOfIntegerCodedLookup.SOC2010, soc2000)).Returns(false);

            NewRule(provideLookupDetailsMock.Object).ConditionMet(learnStartDate, soc2000).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ValidSoc2000Lookup()
        {
            var learnStartDate = new DateTime(2014, 08, 02);
            var soc2000 = 1;

            var provideLookupDetailsMock = new Mock<IProvideLookupDetails>();
            provideLookupDetailsMock.Setup(pldm => pldm.Contains(TypeOfIntegerCodedLookup.SOC2010, soc2000)).Returns(true);

            NewRule(provideLookupDetailsMock.Object).ConditionMet(learnStartDate, soc2000).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var soc2000 = 1;
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2014, 08, 02),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            SOC2000Nullable = soc2000
                        }
                    }
                }
            };

            var provideLookupDetailsMock = new Mock<IProvideLookupDetails>();
            provideLookupDetailsMock.Setup(pldm => pldm.Contains(TypeOfIntegerCodedLookup.SOC2010, soc2000)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(provideLookupDetailsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var soc2000 = 1;
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2014, 07, 31),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            SOC2000Nullable = soc2000
                        }
                    }
                }
            };

            var provideLookupDetailsMock = new Mock<IProvideLookupDetails>();
            provideLookupDetailsMock.Setup(pldm => pldm.Contains(TypeOfIntegerCodedLookup.SOC2010, soc2000)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(provideLookupDetailsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_NullSoc2000()
        {
            int? soc2000 = null;
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2014, 08, 02),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            SOC2000Nullable = soc2000
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_NullLearningDeliveryHe()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2014, 08, 02),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/10/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.SOC2000, 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 10, 01), 1);

            validationErrorHandlerMock.Verify();
        }

        private SOC2000_04Rule NewRule(
            IProvideLookupDetails provideLookupDetails = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new SOC2000_04Rule(provideLookupDetails, validationErrorHandler);
        }
    }
}
