using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R56RuleTests : AbstractRuleTests<R56Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R56");
        }

        [Fact]
        public void Validate_Null_LearningDeliveries()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(new TestLearner());
            }
        }

        [Fact]
        public void Validate_Pass_No_NonFundedAims()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        FundModel = 35
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 656
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            }
        }

        [Fact]
        public void Validate_Pass_ClosedNonFundedAims()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        FundModel = TypeOfFunding.NotFundedByESFA,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnActEndDateNullable = new DateTime(2018, 10, 10),
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                        StdCodeNullable = null,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        FundModel = 35,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                        StdCodeNullable = null,
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
            }
        }

        [Fact]
        public void Validate_Pass_SameComponentAimFundModel()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        FundModel = TypeOfFunding.NotFundedByESFA,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnActEndDateNullable = null,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                        StdCodeNullable = null,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        FundModel = TypeOfFunding.NotFundedByESFA,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                        StdCodeNullable = null,
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
            }
        }

        [Fact]
        public void Validate_Fail_One_LearningDelivery()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        FundModel = TypeOfFunding.NotFundedByESFA,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnActEndDateNullable = null,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                        StdCodeNullable = null,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        FundModel = TypeOfFunding.NotFundedByESFA,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnActEndDateNullable = null,
                        ProgTypeNullable = 23,
                        FworkCodeNullable = 24,
                        PwayCodeNullable = 25,
                        StdCodeNullable = null,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 3,
                        FundModel = 35,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                        StdCodeNullable = null,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 4,
                        FundModel = TypeOfFunding.NotFundedByESFA,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                        StdCodeNullable = null,
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), 3, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Once);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), 4, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
            }
        }

        [Theory]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, null, null, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, null, null, null, null)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, null, null, null, null)]
        [InlineData(TypeOfFunding.CommunityLearning, null, null, null, null)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, null, null, null, null)]
        [InlineData(TypeOfFunding.Other16To19, null, null, null, null)]
        [InlineData(TypeOfFunding.OtherAdult, null, null, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, 1, null, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, 1, 2, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, 1, 2, 3, null)]
        [InlineData(TypeOfFunding.AdultSkills, null, null, null, 100)]
        public void Validate_Fail(int fundModel, int? progType, int? frameworkCode, int? pathwayCode, int? standardCode)
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        FundModel = TypeOfFunding.NotFundedByESFA,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnActEndDateNullable = null,
                        ProgTypeNullable = progType,
                        FworkCodeNullable = frameworkCode,
                        PwayCodeNullable = pathwayCode,
                        StdCodeNullable = standardCode,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        FundModel = fundModel,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = progType,
                        FworkCodeNullable = frameworkCode,
                        PwayCodeNullable = pathwayCode,
                        StdCodeNullable = standardCode,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 3,
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = progType,
                        FworkCodeNullable = frameworkCode,
                        PwayCodeNullable = pathwayCode,
                        StdCodeNullable = standardCode,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), 3, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), 2, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Once);
            }
        }

        [Theory]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, null, null, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, null, null, null, null)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, null, null, null, null)]
        [InlineData(TypeOfFunding.CommunityLearning, null, null, null, null)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, null, null, null, null)]
        [InlineData(TypeOfFunding.Other16To19, null, null, null, null)]
        [InlineData(TypeOfFunding.OtherAdult, null, null, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, 1, null, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, 1, 2, null, null)]
        [InlineData(TypeOfFunding.AdultSkills, 1, 2, 3, null)]
        [InlineData(TypeOfFunding.AdultSkills, null, null, null, 100)]
        public void Validate_Pass_MismatchCourse(int fundModel, int? progType, int? frameworkCode, int? pathwayCode, int? standardCode)
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        FundModel = TypeOfFunding.NotFundedByESFA,
                        AimType = TypeOfAim.ProgrammeAim,
                        LearnActEndDateNullable = null,
                        ProgTypeNullable = progType,
                        FworkCodeNullable = frameworkCode,
                        PwayCodeNullable = pathwayCode,
                        StdCodeNullable = standardCode,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        FundModel = fundModel,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = progType.HasValue ? progType + 1 : 1,
                        FworkCodeNullable = frameworkCode.HasValue ? frameworkCode + 1 : 2,
                        PwayCodeNullable = pathwayCode.HasValue ? pathwayCode + 1 : 3,
                        StdCodeNullable = standardCode.HasValue ? standardCode + 1 : 1,
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 3,
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = progType,
                        FworkCodeNullable = frameworkCode,
                        PwayCodeNullable = pathwayCode,
                        StdCodeNullable = standardCode,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R56, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.AimType, TypeOfAim.ComponentAimInAProgramme)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.ApprenticeshipsFrom1May2017)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProgType, TypeOfLearningProgramme.AdvancedLevelApprenticeship)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FworkCode, 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.PwayCode, 3)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.StdCode, null)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "10/10/2017")).Verifiable();

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = "101",
                AimType = TypeOfAim.ComponentAimInAProgramme,
                ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                FworkCodeNullable = 2,
                PwayCodeNullable = 3,
                StdCodeNullable = null,
                FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                LearnActEndDateNullable = new DateTime(2017, 10, 10)
            };

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learningDelivery);

            validationErrorHandlerMock.Verify();
        }

        private R56Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R56Rule(validationErrorHandler);
        }
    }
}
