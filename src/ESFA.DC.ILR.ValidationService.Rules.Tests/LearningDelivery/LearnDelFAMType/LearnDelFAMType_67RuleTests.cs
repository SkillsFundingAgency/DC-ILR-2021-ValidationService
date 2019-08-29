using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_67RuleTests : AbstractRuleTests<LearnDelFAMType_67Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_67");
        }

        [Fact]
        public void Validation_NoError_IrrelevantFamType()
        {
            var learnAimRef = "00100309";

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
            larsDataServiceMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns(mockDelivery.Object);

            larsDataServiceMock
                .Setup(m => m.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(false);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        FundModel = 36,
                        AimType = 3,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validation_NoError_BasicSkill()
        {
            var learnAimRef = "00100309";

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

            larsDataServiceMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns(mockDelivery.Object);

            larsDataServiceMock
                .Setup(m => m.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(true);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        FundModel = 36,
                        AimType = 3,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void ValidationPasses_NoLDs()
        {
            var testLearner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void ValidationPasses_NoFAMs()
        {
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 36,
                        AimType = 3
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void ValidationPasses_IrrelevantFundingModel()
        {
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 3,
                        AimType = 3,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void ValidationPasses_IrrelevantAimType()
        {
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 36,
                        AimType = 13,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void IsCommonComponent_True()
        {
            ILARSLearningDelivery lARSDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef1",
                EffectiveFrom = new DateTime(2018, 8, 1),
                FrameworkCommonComponent = 20
            };

            var result = NewRule().IsCommonComponent(lARSDelivery);
            result.Should().BeTrue();
        }

        [Fact]
        public void IsCommonComponent_False_AsWrongComponent()
        {
            ILARSLearningDelivery lARSDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef1",
                EffectiveFrom = new DateTime(2018, 8, 1),
                FrameworkCommonComponent = 21
            };

            var result = NewRule().IsCommonComponent(lARSDelivery);
            result.Should().BeFalse();
        }

        [Fact]
        public void IsCommonComponent_False_DueToEmptyList()
        {
            ILARSLearningDelivery lARSDelivery = new Data.External.LARS.Model.LearningDelivery();

            var result = NewRule().IsCommonComponent(lARSDelivery);
            result.Should().BeFalse();
        }

        [Fact]
        public void IsCommonComponent_False_DueToNull()
        {
            ILARSLearningDelivery lARSDelivery = null;

            var result = NewRule().IsCommonComponent(lARSDelivery);
            result.Should().BeFalse();
        }

        [Fact]
        public void Validation_NoError_CommonComponentNotMatched()
        {
            var learnAimRef = "00100309";

            ILARSLearningDelivery lARSDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef1",
                EffectiveFrom = new DateTime(2018, 8, 1),
                FrameworkCommonComponent = 21
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns(lARSDelivery);

            larsDataServiceMock
                .Setup(m => m.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(true);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        FundModel = 36,
                        AimType = 3,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void ValidationErrors_DueToBasicSkillsIsFalse()
        {
            var learnAimRef = "00100309";

            ILARSLearningDelivery lARSDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef1",
                EffectiveFrom = new DateTime(2018, 8, 1),
                FrameworkCommonComponent = 20
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns(lARSDelivery);

            larsDataServiceMock
                   .Setup(m => m.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                   .Returns(false);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        FundModel = 36,
                        AimType = 3,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<string>()));
            }
        }

        [Fact]
        public void ValidationErrors_DueToComonComponent()
        {
            var learnAimRef = "00100309";

            ILARSLearningDelivery lARSDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef1",
                EffectiveFrom = new DateTime(2018, 8, 1),
                FrameworkCommonComponent = 20
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns(lARSDelivery);

            larsDataServiceMock
                   .Setup(m => m.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                   .Returns(true);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        FundModel = 36,
                        AimType = 3,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<string>()));
            }
        }

        private LearnDelFAMType_67Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILARSDataService larsDataService = null)
        {
            return new LearnDelFAMType_67Rule(validationErrorHandler, larsDataService);
        }
    }
}
