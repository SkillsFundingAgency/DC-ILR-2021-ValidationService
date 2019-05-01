using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    public class AFinType_02RuleTests : AbstractRuleTests<AFinType_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinType_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var appFinRecord = new TestAppFinRecord()
            {
                AFinAmount = 1,
                AFinCode = 1,
                AFinDate = new DateTime(2018, 10, 01),
                AFinType = "TNP"
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock
                .Setup(l => l.Contains(TypeOfStringCodedLookup.AppFinType, $"{appFinRecord.AFinType}{appFinRecord.AFinCode}"))
                .Returns(false);

            NewRule(lookupsMock.Object).ConditionMet(appFinRecord).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var appFinRecord = new TestAppFinRecord()
            {
                AFinAmount = 1,
                AFinCode = 2,
                AFinDate = new DateTime(2018, 10, 01),
                AFinType = "TNP"
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock
                .Setup(l => l.Contains(TypeOfStringCodedLookup.AppFinType, $"{appFinRecord.AFinType}{appFinRecord.AFinCode}"))
                .Returns(true);

            NewRule(lookupsMock.Object).ConditionMet(appFinRecord).Should().BeFalse();
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
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinAmount = 1,
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 01),
                                AFinType = "TNP"
                            }
                        }
                    }
                }
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock
                .Setup(l => l.Contains(TypeOfStringCodedLookup.AppFinType, It.IsAny<string>()))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(lookupsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_Null()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                    }
                }
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock
                .Setup(l => l.Contains(TypeOfStringCodedLookup.AppFinType, It.IsAny<string>()))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(lookupsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_AFinCOde()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinAmount = 1,
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 01),
                                AFinType = "TNP"
                            }
                        }
                    }
                }
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock
                .Setup(l => l.Contains(TypeOfStringCodedLookup.AppFinType, It.IsAny<string>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(lookupsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var aFinType = "TNP";
            var aFinCode = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinType", aFinType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinCode", aFinCode)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(aFinType, aFinCode);

            validationErrorHandlerMock.Verify();
        }

        private AFinType_02Rule NewRule(IProvideLookupDetails lookups = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinType_02Rule(lookups, validationErrorHandler);
        }
    }
}
