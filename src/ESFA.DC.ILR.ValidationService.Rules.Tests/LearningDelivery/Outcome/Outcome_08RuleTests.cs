using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.Outcome
{
    public class Outcome_08RuleTests : AbstractRuleTests<Outcome_08Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Outcome_08");
        }

        [Fact]
        public void ValidationPasses_LearningStartDateTooEarly()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2015, 7, 31),
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.Traineeship,
                        OutcomeNullable = OutcomeConstants.Achieved
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_IrrelevantAimType()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2015, 8, 1),
                        AimType = TypeOfAim.AimNotPartOfAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.Traineeship,
                        OutcomeNullable = OutcomeConstants.Achieved
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_IrrelevantProgType()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2015, 8, 1),
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        OutcomeNullable = OutcomeConstants.Achieved
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_IrrelevantOutcome()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2015, 8, 1),
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.Traineeship,
                        OutcomeNullable = OutcomeConstants.PartialAchievement
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(OutTypeConstants.PaidEmployment, DPOutcomeCodeConstants.EMP_PaidEmployment16PlusHours)]
        [InlineData(OutTypeConstants.PaidEmployment, DPOutcomeCodeConstants.EMP_SelfEmployed16PlusHours)]
        [InlineData(OutTypeConstants.Education, DPOutcomeCodeConstants.EDU_Apprenticeship)]
        [InlineData(OutTypeConstants.Education, DPOutcomeCodeConstants.EDU_OtherFEFullTime)]
        [InlineData(OutTypeConstants.Education, DPOutcomeCodeConstants.EDU_OtherFEPartTime)]
        public void ValidationPasses_MatchingDPOutcome(string dpOutcomeType, int dpOutcomeCode)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            const string LearnRefNum = "123456";

            var matchingDpOutcome = new TestLearnerDestinationAndProgression
            {
                LearnRefNumber = LearnRefNum,
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutStartDate = new DateTime(2018, 9, 1),
                        OutType = dpOutcomeType,
                        OutCode = dpOutcomeCode
                    }
                }
            };

            var queryServiceMock = new Mock<ILearnerDPQueryService>();
            queryServiceMock
                .Setup(m => m.GetDestinationAndProgressionForLearner(It.IsAny<string>()))
                .Returns(matchingDpOutcome);

            var testLearner = new TestLearner
            {
                LearnRefNumber = LearnRefNum,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2015, 8, 1),
                        LearnActEndDateNullable = null,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.Traineeship,
                        OutcomeNullable = OutcomeConstants.Achieved
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, queryServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            const string LearnRefNum = "123456";

            var matchingDpOutcome = new TestLearnerDestinationAndProgression
            {
                LearnRefNumber = LearnRefNum,
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome // wrong type/code
                    {
                        OutStartDate = new DateTime(2015, 9, 1),
                        OutType = OutTypeConstants.Other,
                        OutCode = DPOutcomeCodeConstants.OTH_NotKnown
                    },
                    new TestDPOutcome // wrong date
                    {
                        OutStartDate = new DateTime(2018, 9, 1),
                        OutType = OutTypeConstants.PaidEmployment,
                        OutCode = DPOutcomeCodeConstants.EMP_PaidEmployment16PlusHours
                    }
                }
            };

            var queryServiceMock = new Mock<ILearnerDPQueryService>();
            queryServiceMock
                .Setup(m => m.GetDestinationAndProgressionForLearner(It.IsAny<string>()))
                .Returns(matchingDpOutcome);

            var testLearner = new TestLearner
            {
                LearnRefNumber = LearnRefNum,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2015, 8, 1),
                        LearnActEndDateNullable = new DateTime(2017, 7, 31),
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.Traineeship,
                        OutcomeNullable = OutcomeConstants.Achieved
                    },
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2014, 1, 1),
                        LearnActEndDateNullable = new DateTime(2017, 7, 31),
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.Traineeship,
                        OutcomeNullable = OutcomeConstants.Achieved
                    },
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2015, 8, 1),
                        LearnActEndDateNullable = new DateTime(2017, 7, 31),
                        AimType = TypeOfAim.AimNotPartOfAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.Traineeship,
                        OutcomeNullable = OutcomeConstants.Achieved
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, queryServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private Outcome_08Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearnerDPQueryService learnerDpQueryService = null)
        {
            return new Outcome_08Rule(learnerDpQueryService, validationErrorHandler);
        }
    }
}
