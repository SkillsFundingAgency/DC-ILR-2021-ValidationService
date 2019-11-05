using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LearnFAMType
{
    public class LearnFAMType_03RuleTests : AbstractRuleTests<LearnFAMType_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnFAMType_03");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var famType = "DLA";
            var famCode = 1;
            var learnerFam = new TestLearnerFAM
            {
                LearnFAMType = famType,
                LearnFAMCode = famCode
            };

            var dd06Date = new DateTime(2018, 08, 01);

            var lookupDetailsMock = new Mock<IProvideLookupDetails>();
            lookupDetailsMock
                .Setup(ldm => ldm.IsExpired(TypeOfLimitedLifeLookup.LearnFAMType, $"{famType}{famCode}", dd06Date))
                .Returns(true);

            NewRule(lookupDetails: lookupDetailsMock.Object).ConditionMet(learnerFam, dd06Date).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var famType = "DLA";
            var famCode = 2;
            var learnerFam = new TestLearnerFAM
            {
                LearnFAMType = famType,
                LearnFAMCode = famCode
            };

            var dd06Date = new DateTime(2018, 08, 01);

            var lookupDetailsMock = new Mock<IProvideLookupDetails>();
            lookupDetailsMock
                .Setup(ldm => ldm.IsExpired(TypeOfLimitedLifeLookup.LearnFAMType, $"{famType}{famCode}", dd06Date))
                .Returns(false);

            NewRule(lookupDetails: lookupDetailsMock.Object).ConditionMet(learnerFam, dd06Date).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learnStartDate = new DateTime(2018, 08, 01);
            var famType = "DLA";
            var famCode = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate
                    }
                },
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = famType,
                        LearnFAMCode = famCode
                    }
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();
            dd06Mock.Setup(dm => dm.Derive(learner.LearningDeliveries)).Returns(learnStartDate);

            var lookupDetailsMock = new Mock<IProvideLookupDetails>();
            lookupDetailsMock
                .Setup(ldm => ldm.IsExpired(TypeOfLimitedLifeLookup.LearnFAMType, $"{famType}{famCode}", learnStartDate))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd06Mock.Object, lookupDetailsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learnStartDate = new DateTime(2018, 08, 01);
            var famType = "DLA";
            var famCode = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate
                    }
                },
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = famType,
                        LearnFAMCode = famCode
                    }
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();
            dd06Mock.Setup(dm => dm.Derive(learner.LearningDeliveries)).Returns(learnStartDate);

            var lookupDetailsMock = new Mock<IProvideLookupDetails>();
            lookupDetailsMock
                .Setup(ldm => ldm.IsExpired(TypeOfLimitedLifeLookup.LearnFAMType, $"{famType}{famCode}", learnStartDate))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd06Mock.Object, lookupDetailsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_NullLearnerFam()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 08, 01)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private LearnFAMType_03Rule NewRule(
            IDerivedData_06Rule derivedData06 = null,
            IProvideLookupDetails lookupDetails = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_03Rule(derivedData06, lookupDetails, validationErrorHandler);
        }
    }
}
