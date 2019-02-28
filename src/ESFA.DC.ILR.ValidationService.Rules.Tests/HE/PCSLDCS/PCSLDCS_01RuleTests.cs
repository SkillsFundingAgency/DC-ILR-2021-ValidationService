using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCSLDCS;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.PCSLDCS
{
    public class PCSLDCS_01RuleTests : AbstractRuleTests<PCSLDCS_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PCSLDCS_01");
        }

        [Fact]
        public void StartDateConditionMet_False()
        {
            NewRule().StartDateConditionMet(new DateTime(2008, 08, 01)).Should().BeFalse();
        }

        [Fact]
        public void StartDateConditionMet_True()
        {
            NewRule().StartDateConditionMet(new DateTime(2009, 08, 01)).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            TestLearningDeliveryHE testLearningDeliveryHE = new TestLearningDeliveryHE()
            {
                PCSLDCSNullable = 1.5M
            };

            NewRule().LearningDeliveryHEConditionMet(testLearningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True()
        {
            TestLearningDeliveryHE testLearningDeliveryHE = new TestLearningDeliveryHE()
            {
                PCSLDCSNullable = null
            };

            NewRule().LearningDeliveryHEConditionMet(testLearningDeliveryHE).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(lds => lds.LearnDirectClassSystemCode2MatchForLearnAimRef("123")).Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSConditionMet("123").Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(lds => lds.LearnDirectClassSystemCode2MatchForLearnAimRef("456")).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSConditionMet("456").Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var testLearningDeliveryHE = new TestLearningDeliveryHE()
            {
                PCSLDCSNullable = 1.5M
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(lds => lds.LearnDirectClassSystemCode2MatchForLearnAimRef("123")).Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).ConditionMet(new DateTime(2018, 08, 02), testLearningDeliveryHE, "123").Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var testLearningDeliveryHE = new TestLearningDeliveryHE()
            {
                PCSLDCSNullable = null
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(lds => lds.LearnDirectClassSystemCode2MatchForLearnAimRef("123")).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).ConditionMet(new DateTime(2009, 08, 01), testLearningDeliveryHE, "123").Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearningDeliveryHE = new TestLearningDeliveryHE()
            {
                PCSLDCSNullable = null
            };

            ILearner testLearner = new TestLearner()
            {
                LearnRefNumber = "123Learner",
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "123",
                        LearnStartDate = new DateTime(2009, 08, 01),
                        LearningDeliveryHEEntity = testLearningDeliveryHE
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(lds => lds.LearnDirectClassSystemCode2MatchForLearnAimRef("123")).Returns(true);
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, larsDataService: larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearningDeliveryHE = new TestLearningDeliveryHE()
            {
                PCSLDCSNullable = null
            };

            ILearner testLearner = new TestLearner()
            {
                LearnRefNumber = "456Learner",
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "456",
                        LearnStartDate = new DateTime(2010, 08, 02),
                        LearningDeliveryHEEntity = testLearningDeliveryHE
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(lds => lds.LearnDirectClassSystemCode2MatchForLearnAimRef("456")).Returns(false);
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, larsDataService: larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearningDeliveryHENull()
        {
            ILearner testLearner = new TestLearner()
            {
                LearnRefNumber = "456Learner",
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "456",
                        LearnStartDate = new DateTime(2010, 08, 02),
                        LearningDeliveryHEEntity = null
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(lds => lds.LearnDirectClassSystemCode2MatchForLearnAimRef("456")).Returns(false);
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, larsDataService: larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/08/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 08, 01));

            validationErrorHandlerMock.Verify();
        }

        public PCSLDCS_01Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILARSDataService larsDataService = null)
        {
            return new PCSLDCS_01Rule(validationErrorHandler: validationErrorHandler, lARSDataService: larsDataService);
        }
    }
}
