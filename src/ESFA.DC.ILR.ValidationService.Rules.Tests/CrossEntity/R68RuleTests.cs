using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
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
    public class R68RuleTests : AbstractRuleTests<R68Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R68");
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_SingleDelivery()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 2,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_Complex()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 2,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        FworkCodeNullable = 2,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        FworkCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 2, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        FworkCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(20));
            }
        }

        [Fact]
        public void Validate_NoError_SingleDelivery()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 2, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 2,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
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
        public void Validate_NoError_Standards()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type1", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type2", AFinCode = 2, AFinDate = new DateTime(2018, 1, 1) },
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
        public void Validate_NoError_Frameworks()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        FworkCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type1", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        FworkCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type2", AFinCode = 2, AFinDate = new DateTime(2018, 1, 1) },
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
        public void Validate_NoError_Mixed_FworkAndStd()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        FworkCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type1", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type1", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
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
        public void Validate_NoError_StdCodes()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 2,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
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
        public void Validate_NoError_When_NoLearningDeliveries()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "10002345"
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule().Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(1, 36, 77)]
        [InlineData(1, 25, 77)]
        [InlineData(1, 25, 2)]
        [InlineData(2, 25, 2)]
        [InlineData(2, 36, 2)]
        [InlineData(2, 25, 77)]
        public void Validate_NoError_When_NoAimDeliveries(int aimType, int fundModel, int? progType)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AimType = aimType,
                FundModel = fundModel,
                ProgTypeNullable = progType
            };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "10002345",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    learningDelivery
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule().Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship)]
        [InlineData(TypeOfLearningProgramme.IntermediateLevelApprenticeship)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel6)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus)]
        public void IsApprenticeshipProgrammeAim_True(int progType)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ProgrammeAim,
                FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                ProgTypeNullable = progType
            };

            NewRule().IsApprenticeshipProgrammeAim(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void IsApprenticeshipProgrammeAim_False_AimType()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.AimNotPartOfAProgramme,
                FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship
            };

            NewRule().IsApprenticeshipProgrammeAim(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void IsApprenticeshipProgrammeAim_False_FundModel()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ProgrammeAim,
                FundModel = TypeOfFunding.AdultSkills,
                ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship
            };

            NewRule().IsApprenticeshipProgrammeAim(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void IsApprenticeshipProgrammeAim_False_ProgType()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                AimType = TypeOfAim.ProgrammeAim,
                FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                ProgTypeNullable = TypeOfLearningProgramme.Traineeship,
            };

            NewRule().IsApprenticeshipProgrammeAim(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var appFindRecordOne = new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) };
            var appFindRecordTwo = new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) };

            NewRule().ConditionMet(appFindRecordOne, appFindRecordTwo).Should().BeTrue();
        }

        [Theory]
        [InlineData("Type", 2, 2018)]
        [InlineData("Type", 1, 2017)]
        [InlineData("NotType", 1, 2018)]
        [InlineData("NotType", 2, 2018)]
        [InlineData("Type", 2, 2017)]
        [InlineData("NotType", 2, 2017)]
        public void ConditionMet_False(string aFinType, int aFinCode, int year)
        {
            var appFindRecordOne = new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) };
            var appFindRecordTwo = new TestAppFinRecord() { AFinType = aFinType, AFinCode = aFinCode, AFinDate = new DateTime(year, 1, 1) };

            NewRule().ConditionMet(appFindRecordOne, appFindRecordTwo).Should().BeFalse();
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            IAppFinRecord appFinRecord = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2017, 7, 1)
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("FworkCode", 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("StdCode", 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinType", "TNP")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinCode", 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinDate", "01/07/2017")).Verifiable();

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(1, 1, appFinRecord);
            validationErrorHandlerMock.Verify();
        }

        private R68Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R68Rule(validationErrorHandler);
        }
    }
}
