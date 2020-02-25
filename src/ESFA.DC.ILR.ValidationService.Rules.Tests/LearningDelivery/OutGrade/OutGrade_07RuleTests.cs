using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OutGrade
{
    public class OutGrade_07RuleTests : AbstractRuleTests<OutGrade_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutGrade_07");
        }

        [Fact]
        public void ValidatePasses_OutsideDateRange()
        {
            var testLearnAimRef = "12345";
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var larsMock = new Mock<ILARSDataService>();
            larsMock
                .Setup(m => m.GetDeliveryFor(It.IsAny<string>()))
                .Returns(new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = testLearnAimRef,
                LearnAimRefType = "0001"
            });

            var lookupMock = new Mock<IProvideLookupDetails>();
            lookupMock
                .Setup(m => m.Contains(TypeOfListItemLookup.LearningAimType, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = testLearnAimRef,
                        LearnActEndDateNullable = new DateTime(2014, 7, 31),
                        OutcomeNullable = 1,
                        OutGrade = "VVV"
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, larsMock.Object, lookupMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_OutcomeNotApplicable()
        {
            var testLearnAimRef = "12345";
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var larsMock = new Mock<ILARSDataService>();
            larsMock
                .Setup(m => m.GetDeliveryFor(It.IsAny<string>()))
                .Returns(new Data.External.LARS.Model.LearningDelivery
                {
                    LearnAimRef = testLearnAimRef,
                    LearnAimRefType = "0001"
                });

            var lookupMock = new Mock<IProvideLookupDetails>();
            lookupMock
                .Setup(m => m.Contains(TypeOfListItemLookup.LearningAimType, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = testLearnAimRef,
                        LearnActEndDateNullable = new DateTime(2018, 7, 31),
                        OutcomeNullable = 2,
                        OutGrade = "VVV"
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, larsMock.Object, lookupMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoMatchingLearnAimRefType()
        {
            var testLearnAimRef = "12345";
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var larsMock = new Mock<ILARSDataService>();
            larsMock
                .Setup(m => m.GetDeliveryFor(It.IsAny<string>()))
                .Returns((ILARSLearningDelivery)null);

            var lookupMock = new Mock<IProvideLookupDetails>();
            lookupMock
                .Setup(m => m.Contains(TypeOfListItemLookup.LearningAimType, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = testLearnAimRef,
                        LearnActEndDateNullable = new DateTime(2018, 7, 31),
                        OutcomeNullable = 1,
                        OutGrade = "VVV"
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, larsMock.Object, lookupMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_OutGradeMatches()
        {
            var testLearnAimRef = "12345";
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var larsMock = new Mock<ILARSDataService>();
            larsMock
                .Setup(m => m.GetDeliveryFor(It.IsAny<string>()))
                .Returns(new Data.External.LARS.Model.LearningDelivery
                {
                    LearnAimRef = testLearnAimRef,
                    LearnAimRefType = "0001"
                });

            var lookupMock = new Mock<IProvideLookupDetails>();
            lookupMock
                .Setup(m => m.Contains(TypeOfListItemLookup.LearningAimType, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = testLearnAimRef,
                        LearnActEndDateNullable = new DateTime(2018, 7, 31),
                        OutcomeNullable = 1,
                        OutGrade = "A"
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, larsMock.Object, lookupMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_OutGradeNull()
        {
            var testLearnAimRef = "12345";
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var larsMock = new Mock<ILARSDataService>();
            larsMock
                .Setup(m => m.GetDeliveryFor(It.IsAny<string>()))
                .Returns(new Data.External.LARS.Model.LearningDelivery
                {
                    LearnAimRef = testLearnAimRef,
                    LearnAimRefType = "0001"
                });

            var lookupMock = new Mock<IProvideLookupDetails>();
            lookupMock
                .Setup(m => m.Contains(TypeOfListItemLookup.LearningAimType, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = testLearnAimRef,
                        LearnActEndDateNullable = new DateTime(2018, 7, 31),
                        OutcomeNullable = 1,
                        OutGrade = null
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, larsMock.Object, lookupMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidateFails()
        {
            var testLearnAimRef = "12345";
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var larsMock = new Mock<ILARSDataService>();
            larsMock
                .Setup(m => m.GetDeliveryFor(It.IsAny<string>()))
                .Returns(new Data.External.LARS.Model.LearningDelivery
                {
                    LearnAimRef = testLearnAimRef,
                    LearnAimRefType = "0001"
                });

            var lookupMock = new Mock<IProvideLookupDetails>();
            lookupMock
                .Setup(m => m.Contains(TypeOfListItemLookup.LearningAimType, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = testLearnAimRef,
                        LearnActEndDateNullable = new DateTime(2018, 7, 31),
                        OutcomeNullable = 1,
                        OutGrade = "VVV"
                    },
                    new TestLearningDelivery
                    {
                        LearnAimRef = testLearnAimRef,
                        LearnActEndDateNullable = new DateTime(2013, 7, 31),
                        OutcomeNullable = 1,
                        OutGrade = "A"
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, larsMock.Object, lookupMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private OutGrade_07Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILARSDataService larsDataService = null,
            IProvideLookupDetails lookup = null)
        {
            return new OutGrade_07Rule(larsDataService, lookup, validationErrorHandler);
        }
    }
}
