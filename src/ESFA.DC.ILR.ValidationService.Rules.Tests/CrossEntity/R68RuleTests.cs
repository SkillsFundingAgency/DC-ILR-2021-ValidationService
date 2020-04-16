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
    using ESFA.DC.ILR.ValidationService.Rules.Structs;

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
                        AimSeqNumber = 1,
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
                        AimSeqNumber = 2,
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
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(10));
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
                        AimSeqNumber = 1,
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        StdCodeNullable = 1,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                            new TestAppFinRecord() { AFinType = "Type", AFinCode = 1, AFinDate = new DateTime(2018, 1, 1) },
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(5));
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
                        AimSeqNumber = 1,
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
                        AimSeqNumber = 2,
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
                        AimSeqNumber = 3,
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
                        AimSeqNumber = 4,
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
                        AimSeqNumber = 5,
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
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(15));
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
            var appFindRecordOne = new R68AppFinRecord(1, 1, 1, "Type", 1, new DateTime(2018, 1, 1));
            var appFindRecordTwo = new R68AppFinRecord(1, 1, 1, "Type", 1, new DateTime(2018, 1, 1));

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
            var appFindRecordOne = new R68AppFinRecord(1, 1, 1, "Type", 1, new DateTime(2018, 1, 1));
            var appFindRecordTwo = new R68AppFinRecord(1, 1, 1, aFinType, aFinCode, new DateTime(year, 1, 1));

            NewRule().ConditionMet(appFindRecordOne, appFindRecordTwo).Should().BeFalse();
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var aFinType = "TNP";
            var aFinCode = 2;
            var aFinDate = new DateTime(2017, 7, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("FworkCode", 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("StdCode", 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinType", "TNP")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinCode", 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AFinDate", "01/07/2017")).Verifiable();

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(1, 1, aFinType, aFinCode, aFinDate);
            validationErrorHandlerMock.Verify();
        }

        [Fact]
        public void Validate_NoError_OneAppFinTwoAims()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        StdCodeNullable = 12,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord() { AFinType = "ACT", AFinCode = 1, AFinDate = new DateTime(2019, 10, 14) },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        StdCodeNullable = 12,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateAims_NoError_Null()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).ValidateAims("Learner", null);
            }
        }

        [Fact]
        public void ValidateAims_NoError()
        {
            var dictionary = new Dictionary<int?, IEnumerable<R68AppFinRecord>>
            {
                {
                    1, new List<R68AppFinRecord>
                    {
                        new R68AppFinRecord(1, 1, null, "Type", 1, new DateTime(2018, 1, 1)),
                        new R68AppFinRecord(1, 1, null, "Type", 2, new DateTime(2018, 1, 1))
                    }
                },
                {
                    2, new List<R68AppFinRecord>
                    {
                        new R68AppFinRecord(1, 2, null, "Type", 1, new DateTime(2018, 1, 1)),
                        new R68AppFinRecord(2, 2, null, "Type", 2, new DateTime(2018, 1, 1))
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).ValidateAims("Learner", dictionary);
            }
        }

        [Fact]
        public void ValidateAims_Error()
        {
            var dictionary = new Dictionary<int?, IEnumerable<R68AppFinRecord>>
            {
                {
                    1, new List<R68AppFinRecord>
                    {
                        new R68AppFinRecord(1, 1, null, "Type", 1, new DateTime(2018, 1, 1)),
                        new R68AppFinRecord(1, 1, null, "Type", 1, new DateTime(2018, 1, 1))
                    }
                },
                {
                    2, new List<R68AppFinRecord>
                    {
                        new R68AppFinRecord(1, 2, null, "Type", 1, new DateTime(2018, 1, 1)),
                        new R68AppFinRecord(2, 2, null, "Type", 1, new DateTime(2018, 1, 1))
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).ValidateAims("Learner", dictionary);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(15));
            }
        }

        private R68Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R68Rule(validationErrorHandler);
        }
    }
}
