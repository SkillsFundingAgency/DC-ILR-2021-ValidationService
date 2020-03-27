using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.QUALENT3;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.QUALENT3
{
    public class QUALENT3_03RuleTests : AbstractRuleTests<QUALENT3_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("QUALENT3_03");
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES
                    }
                };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL
                    }
                };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(TypeOfQualEnt3.CambridgePreUDiploma31072013, "2019/01/01")]
        [InlineData("Q1", "2019/01/01")]
        public void LearningDeliveryHEConditionMet_False(string qualent3, string learnStartDateString)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);

            var qualent3DataServiceMock = new Mock<IProvideLookupDetails>();

            qualent3DataServiceMock.Setup(q => q.IsCurrent(TypeOfLimitedLifeLookup.QualEnt3, qualent3, learnStartDate)).Returns(true);

            NewRule(provideLookupDetails: qualent3DataServiceMock.Object).LearningDeliveryHEConditionMet(learnStartDate, qualent3).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfQualEnt3.CambridgePreUDiploma31072013, "2013/01/01")]
        [InlineData(TypeOfQualEnt3.CertificateAtLevelMPostgraduateCertificate, "2019/01/01")]
        public void LearningDeliveryHEConditionMet_True(string qualent3, string learnStartDateString)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);

            var qualent3DataServiceMock = new Mock<IProvideLookupDetails>();

            qualent3DataServiceMock.Setup(q => q.IsCurrent(TypeOfLimitedLifeLookup.QualEnt3, qualent3, learnStartDate)).Returns(false);

            NewRule(provideLookupDetails: qualent3DataServiceMock.Object).LearningDeliveryHEConditionMet(learnStartDate, qualent3).Should().BeTrue();
        }

        [Theory]
        [InlineData(TypeOfQualEnt3.CambridgePreUDiploma31072013, "2019/01/01")]
        [InlineData("Q1", "2019/01/01")]
        public void ConditionMet_False(string qualent3, string learnStartDateString)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES
                    }
                };

            var qualent3DataServiceMock = new Mock<IProvideLookupDetails>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            qualent3DataServiceMock.Setup(q => q.IsCurrent(TypeOfLimitedLifeLookup.QualEnt3, qualent3, learnStartDate)).Returns(true);
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(true);

            NewRule(
                provideLookupDetails: qualent3DataServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object)
                .ConditionMet(learnStartDate, qualent3, testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfQualEnt3.CambridgePreUDiploma31072013, "2013/01/01")]
        [InlineData(TypeOfQualEnt3.CertificateAtLevelMPostgraduateCertificate, "2019/01/01")]
        public void ConditionMet_True(string qualent3, string learnStartDateString)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL
                    }
                };

            var qualent3DataServiceMock = new Mock<IProvideLookupDetails>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            qualent3DataServiceMock.Setup(q => q.IsCurrent(TypeOfLimitedLifeLookup.QualEnt3, qualent3, learnStartDate)).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            NewRule(
                provideLookupDetails: qualent3DataServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object)
                .ConditionMet(learnStartDate, qualent3, testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES
                }
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2019, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            QUALENT3 = TypeOfQualEnt3.CambridgePreUDiploma31072013
                        },
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            var qualent3DataServiceMock = new Mock<IProvideLookupDetails>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            qualent3DataServiceMock.Setup(q => q.IsCurrent(TypeOfLimitedLifeLookup.QualEnt3, TypeOfQualEnt3.CambridgePreUDiploma31072013, new DateTime(2019, 01, 01))).Returns(true);
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandlerMock.Object,
                    qualent3DataServiceMock.Object,
                    learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL
                }
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2013, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            QUALENT3 = TypeOfQualEnt3.CambridgePreUDiploma31072013
                        },
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            var qualent3DataServiceMock = new Mock<IProvideLookupDetails>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            qualent3DataServiceMock.Setup(q => q.IsCurrent(TypeOfLimitedLifeLookup.QualEnt3, TypeOfQualEnt3.CambridgePreUDiploma31072013, new DateTime(2013, 01, 01))).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandlerMock.Object,
                    qualent3DataServiceMock.Object,
                    learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "31/07/2013")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.QUALENT3, TypeOfQualEnt3.CertificateAtLevelMPostgraduateCertificate)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2013, 07, 31), TypeOfQualEnt3.CertificateAtLevelMPostgraduateCertificate);

            validationErrorHandlerMock.Verify();
        }

        public QUALENT3_03Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IProvideLookupDetails provideLookupDetails = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new QUALENT3_03Rule(
                validationErrorHandler,
                provideLookupDetails,
                learningDeliveryFAMQueryService);
        }
    }
}
