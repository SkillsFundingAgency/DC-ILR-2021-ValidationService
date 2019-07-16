using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_64RuleTests : AbstractRuleTests<LearnDelFAMType_64Rule>
    {
        private readonly IEnumerable<int> _basicSkillsTypes = new List<int>()
        {
            TypeOfLARSBasicSkill.Certificate_AdultLiteracy,
            TypeOfLARSBasicSkill.Certificate_AdultNumeracy,
            TypeOfLARSBasicSkill.GCSE_EnglishLanguage,
            TypeOfLARSBasicSkill.GCSE_Mathematics,
            TypeOfLARSBasicSkill.KeySkill_Communication,
            TypeOfLARSBasicSkill.KeySkill_ApplicationOfNumbers,
            TypeOfLARSBasicSkill.FunctionalSkillsMathematics,
            TypeOfLARSBasicSkill.FunctionalSkillsEnglish,
            TypeOfLARSBasicSkill.UnitsOfTheCertificate_AdultNumeracy,
            TypeOfLARSBasicSkill.UnitsOfTheCertificate_AdultLiteracy,
            TypeOfLARSBasicSkill.NonNQF_QCFS4LLiteracy,
            TypeOfLARSBasicSkill.NonNQF_QCFS4LNumeracy,
            TypeOfLARSBasicSkill.QCFBasicSkillsEnglishLanguage,
            TypeOfLARSBasicSkill.QCFBasicSkillsMathematics,
            TypeOfLARSBasicSkill.UnitQCFBasicSkillsEnglishLanguage,
            TypeOfLARSBasicSkill.UnitQCFBasicSkillsMathematics,
            TypeOfLARSBasicSkill.InternationalGCSEEnglishLanguage,
            TypeOfLARSBasicSkill.InternationalGCSEMathematics,
            TypeOfLARSBasicSkill.FreeStandingMathematicsQualification,
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_64");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.AdultSkills).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.ApprenticeshipsFrom1May2017).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(true);

            var rule = NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);

            rule.Should().BeFalse();
            learningDeliveryFAMsQueryServiceMock.Verify(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT), Times.AtLeastOnce);
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL },
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(false);

            var rule = NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);

            rule.Should().BeTrue();
            learningDeliveryFAMsQueryServiceMock.Verify(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(2, "00100310", "2017-01-01")]
        [InlineData(3, "00100310", "2017-01-01")]
        [InlineData(3, "00100309", "2017-01-01")]
        [InlineData(5, "00100310", "2018-06-01")]
        public void AimTypeConditionMet_False(int aimType, string learnAimRef, string learnStartDateString)
        {
            var frameworks = new List<Framework>
            {
                new Framework
                {
                    FrameworkAim = new FrameworkAim()
                    {
                        FworkCode = 1,
                        ProgType = 2,
                        PwayCode = 3,
                    },
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>
                    {
                        new FrameworkCommonComponent
                        {
                            FworkCode = 1,
                            ProgType = 2,
                            PwayCode = 3,
                            CommonComponent = 1,
                            EffectiveFrom = new DateTime(2018, 8, 1),
                        }
                    }
                }
            };
            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery.SetupGet(x => x.Frameworks).Returns(frameworks);

            DateTime.TryParse(learnStartDateString, out DateTime learnStartDate);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns(mockDelivery.Object);

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, learnAimRef, new DateTime(2018, 6, 1))).Returns(false);

            NewRule(lARSDataService: larsDataServiceMock.Object).AimTypeConditionMet(aimType, learnAimRef, learnStartDate)
                    .Should()
                    .BeFalse();
        }

        [Theory]
        [InlineData(1, "00100309", "2018-06-01")]
        [InlineData(3, "00100309", "2018-06-01")]
        public void AimTypeConditionMet_True(int aimType, string learnAimRef, string learnStartDateString)
        {
            var aimRef = "00100309";

            var frameworks = new List<Framework>();
            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery.SetupGet(x => x.Frameworks).Returns(frameworks);

            DateTime.TryParse(learnStartDateString, out DateTime learnStartDate);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetDeliveryFor(aimRef)).Returns(mockDelivery.Object);

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, aimRef, new DateTime(2018, 6, 1))).Returns(true);

            NewRule(lARSDataService: larsDataServiceMock.Object).AimTypeConditionMet(aimType, learnAimRef, learnStartDate)
                    .Should()
                    .BeTrue();
        }

        [Fact]
        public void LarsConditionMet_False()
        {
            var aimRef = "00100309";
            var startDate = new DateTime(2017, 01, 01);

            var frameworks = new List<Framework>
            {
                new Framework
                {
                    FrameworkAim = new FrameworkAim()
                    {
                        FworkCode = 1,
                        ProgType = 2,
                        PwayCode = 3,
                    },
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>
                    {
                        new FrameworkCommonComponent
                        {
                            FworkCode = 1,
                            ProgType = 2,
                            PwayCode = 3,
                            CommonComponent = 1,
                            EffectiveFrom = new DateTime(2018, 8, 1),
                        }
                    }
                }
            };

            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery.SetupGet(x => x.Frameworks).Returns(frameworks);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetDeliveryFor(aimRef)).Returns(mockDelivery.Object);
            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, aimRef, startDate)).Returns(false);

            var result = NewRule(lARSDataService: larsDataServiceMock.Object).LarsConditionMet(aimRef, startDate);

            result.Should().BeFalse();

            mockDelivery.VerifyGet(x => x.Frameworks, Times.AtLeastOnce);
            larsDataServiceMock.Verify(x => x.GetDeliveryFor(aimRef), Times.AtLeastOnce);
            larsDataServiceMock.Verify(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, aimRef, startDate), Times.AtLeastOnce);
        }

        [Fact]
        public void LarsConditionMet_True()
        {
            var frameworks = new List<Framework>
            {
                new Framework
                {
                    FrameworkAim = new FrameworkAim()
                    {
                        FworkCode = 1,
                        ProgType = 2,
                        PwayCode = 3,
                    },
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>
                    {
                        new FrameworkCommonComponent
                        {
                            FworkCode = 1,
                            ProgType = 2,
                            PwayCode = 3,
                            CommonComponent = 1,
                            EffectiveFrom = new DateTime(2018, 8, 1),
                        }
                    }
                }
            };

            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery.SetupGet(x => x.Frameworks).Returns(frameworks);

            var aimRef = "00100309";
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetDeliveryFor(aimRef)).Returns(mockDelivery.Object);

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, aimRef, new DateTime(2018, 6, 1))).Returns(true);

            var result = NewRule(lARSDataService: larsDataServiceMock.Object).LarsConditionMet(aimRef, new DateTime(2018, 06, 01));

            result.Should().BeTrue();

            mockDelivery.VerifyGet(x => x.Frameworks, Times.AtLeastOnce);
            larsDataServiceMock.Verify(x => x.GetDeliveryFor(aimRef), Times.AtLeastOnce);
            larsDataServiceMock.Verify(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, aimRef, new DateTime(2018, 6, 1)), Times.AtLeastOnce);
        }

        [Fact]
        public void LarsConditionMet_True_DueToFrameworkComponent()
        {
            var frameworks = new List<Framework>
            {
                new Framework
                {
                    FrameworkAim = new FrameworkAim()
                    {
                        FworkCode = 1,
                        ProgType = 2,
                        PwayCode = 3,
                    },
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>
                    {
                        new FrameworkCommonComponent
                        {
                            FworkCode = 1,
                            ProgType = 2,
                            PwayCode = 3,
                            CommonComponent = 20,
                            EffectiveFrom = new DateTime(2018, 8, 1),
                        }
                    }
                }
            };

            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery.SetupGet(x => x.Frameworks).Returns(frameworks);

            var aimRef = "00100309";
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetDeliveryFor(aimRef)).Returns(mockDelivery.Object);

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, aimRef, new DateTime(2018, 6, 1))).Returns(false);

            var result = NewRule(lARSDataService: larsDataServiceMock.Object).LarsConditionMet(aimRef, new DateTime(2018, 06, 01));

            result.Should().BeTrue();

            mockDelivery.VerifyGet(x => x.Frameworks, Times.AtLeastOnce);
            larsDataServiceMock.Verify(x => x.GetDeliveryFor(aimRef), Times.AtLeastOnce);
            larsDataServiceMock.Verify(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, aimRef, new DateTime(2018, 6, 1)), Times.AtLeastOnce);
        }

        [Fact]
        public void IsCommonComponent_True()
        {
            var frameworkComponents = new List<ILARSFrameworkCommonComponent>()
            {
                new FrameworkCommonComponent { CommonComponent = TypeOfLARSCommonComponent.BritishSignLanguage },
                new FrameworkCommonComponent { CommonComponent = TypeOfLARSCommonComponent.FunctionalSkillsEnglish }
            };

            var mockLarsFramework = new Mock<ILARSFramework>();
            mockLarsFramework.SetupGet(y => y.FrameworkCommonComponents)
                             .Returns(frameworkComponents);

            var result = NewRule().IsCommonComponent(mockLarsFramework.Object);

            result.Should().BeTrue();
            mockLarsFramework.VerifyGet(x => x.FrameworkCommonComponents, Times.AtLeastOnce);
        }

        [Fact]
        public void IsCommonComponent_False_AsWrongComponent()
        {
            var frameworkComponents = new List<ILARSFrameworkCommonComponent>()
            {
                new FrameworkCommonComponent { CommonComponent = TypeOfLARSCommonComponent.FunctionalSkillsEnglish }
            };

            var mockLarsFramework = new Mock<ILARSFramework>();
            mockLarsFramework.SetupGet(y => y.FrameworkCommonComponents)
                             .Returns(frameworkComponents);

            var result = NewRule().IsCommonComponent(mockLarsFramework.Object);

            result.Should().BeFalse();
            mockLarsFramework.VerifyGet(x => x.FrameworkCommonComponents, Times.AtLeastOnce);
        }

        [Fact]
        public void IsCommonComponent_False_DueToEmptyList()
        {
            var frameworkComponents = new List<ILARSFrameworkCommonComponent>();

            var mockLarsFramework = new Mock<ILARSFramework>();
            mockLarsFramework.SetupGet(y => y.FrameworkCommonComponents)
                             .Returns(frameworkComponents);

            var result = NewRule().IsCommonComponent(mockLarsFramework.Object);

            result.Should().BeFalse();
            mockLarsFramework.VerifyGet(x => x.FrameworkCommonComponents, Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, 2, LearningDeliveryFAMTypeConstants.ACT, "00100310", "2017-01-01")]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, 2, LearningDeliveryFAMTypeConstants.ACT, "00100310", "2017-01-01")]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, 1, LearningDeliveryFAMTypeConstants.ACT, "00100310", "2017-01-01")]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, 1, LearningDeliveryFAMTypeConstants.ADL, "00100310", "2017-01-01")]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, 1, LearningDeliveryFAMTypeConstants.ADL, "00100309", "2017-01-01")]
        public void ConditionMet_False(int fundModel, int aimType, string learnDelFAMType, string learnAimRef, string learnStartDateString)
        {
            var frameworks = new List<Framework>
            {
                new Framework
                {
                    FrameworkAim = new FrameworkAim(),
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>
                    {
                        new FrameworkCommonComponent
                        {
                            FworkCode = 1,
                            ProgType = 2,
                            PwayCode = 3,
                            CommonComponent = 1,
                            EffectiveFrom = new DateTime(2018, 8, 1),
                        }
                    }
                }
            };

            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery.SetupGet(x => x.Frameworks).Returns(frameworks);

            DateTime.TryParse(learnStartDateString, out DateTime learnStartDate);

            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = learnDelFAMType },
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns(mockDelivery.Object);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(true);
            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, learnAimRef, new DateTime(2018, 6, 1))).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object, lARSDataService: larsDataServiceMock.Object)
                    .ConditionMet(fundModel, aimType, learningDeliveryFAMs, learnAimRef, learnStartDate)
                    .Should()
                    .BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, 1, LearningDeliveryFAMTypeConstants.ADL, "00100309", "2018-06-01")]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, 3, LearningDeliveryFAMTypeConstants.LDM, "00100309", "2018-06-01")]
        public void ConditionMet_True(int fundModel, int aimType, string learnDelFAMType, string learnAimRef, string learnStartDateString)
        {
            var frameworks = new List<Framework>
            {
                new Framework
                {
                    FrameworkAim = new FrameworkAim()
                    {
                        FworkCode = 1,
                        ProgType = 2,
                        PwayCode = 3,
                    },
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>
                    {
                        new FrameworkCommonComponent
                        {
                            FworkCode = 1,
                            ProgType = 2,
                            PwayCode = 3,
                            CommonComponent = 20,
                            EffectiveFrom = new DateTime(2018, 8, 1),
                        }
                    }
                }
            };

            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery.SetupGet(x => x.Frameworks).Returns(frameworks);

            DateTime.TryParse(learnStartDateString, out DateTime learnStartDate);

            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = learnDelFAMType },
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns(mockDelivery.Object);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(false);
            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, learnAimRef, new DateTime(2018, 6, 1))).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object, lARSDataService: larsDataServiceMock.Object)
                    .ConditionMet(fundModel, aimType, learningDeliveryFAMs, learnAimRef, learnStartDate)
                    .Should()
                    .BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnAimRef = "00100309";

            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL },
            };

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 06, 01),
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                }
            };

            var frameworks = new List<Framework>
            {
                new Framework
                {
                    FrameworkAim = new FrameworkAim()
                    {
                        FworkCode = 1,
                        ProgType = 2,
                        PwayCode = 3,
                    },
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>
                    {
                        new FrameworkCommonComponent
                        {
                            FworkCode = 1,
                            ProgType = 2,
                            PwayCode = 3,
                            CommonComponent = 1,
                            EffectiveFrom = new DateTime(2018, 8, 1),
                        }
                    }
                }
            };

            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery.SetupGet(x => x.Frameworks).Returns(frameworks);

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns(mockDelivery.Object);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(false);
            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, learnAimRef, new DateTime(2018, 6, 1))).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnAimRef = "00100309";

            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
            };

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 06, 01),
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                }
            };

            var frameworks = new List<Framework>
            {
                new Framework
                {
                    FrameworkAim = new FrameworkAim()
                    {
                        FworkCode = 1,
                        ProgType = 2,
                        PwayCode = 3,
                    },
                    FrameworkCommonComponents = new List<FrameworkCommonComponent>
                    {
                        new FrameworkCommonComponent
                        {
                            FworkCode = 1,
                            ProgType = 2,
                            PwayCode = 3,
                            CommonComponent = 20,
                            EffectiveFrom = new DateTime(2018, 8, 1),
                        }
                    }
                }
            };

            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery.SetupGet(x => x.Frameworks).Returns(frameworks);

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns(mockDelivery.Object);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(true);
            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, learnAimRef, new DateTime(2017, 1, 1))).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.AimType, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.AdultSkills)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, TypeOfFunding.AdultSkills, LearningDeliveryFAMTypeConstants.ACT);

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_64Rule NewRule(
                                         IValidationErrorHandler validationErrorHandler = null,
                                         ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
                                         ILARSDataService lARSDataService = null)
        {
            return new LearnDelFAMType_64Rule(
                                         validationErrorHandler: validationErrorHandler,
                                         learningDeliveryFAMQueryService: learningDeliveryFAMQueryService,
                                         lARSDataService: lARSDataService);
        }
    }
}
