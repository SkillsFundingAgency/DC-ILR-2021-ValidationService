using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R64RuleTests : AbstractRuleTests<R64Rule>
    {
        [Fact]
        public void RuleName1()
        {
            NewRule().RuleName.Should().Be("R64");
        }

        [Fact]
        public void RuleName2()
        {
            NewRule().RuleName.Should().Be(RuleNameConstants.R64);
        }

        [Theory]
        [InlineData(36)]
        [InlineData(null)]
        public void Exclusion_False(int? progType)
        {
            NewRule().Exclusion(progType).Should().BeFalse();
        }

        [Fact]
        public void Exclusion_True()
        {
            NewRule().Exclusion(25).Should().BeTrue();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        public void FundModelFilter_True(int fundModel)
        {
            NewRule().FundModelFilter(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelFilter_False()
        {
            NewRule().FundModelFilter(25).Should().BeFalse();
        }

        [Fact]
        public void AimTypeFilter_True()
        {
            NewRule().AimTypeFilter(3).Should().BeTrue();
        }

        [Fact]
        public void AimTypeFilter_False()
        {
            NewRule().AimTypeFilter(2).Should().BeFalse();
        }

        [Fact]
        public void CompletionStatusFilter_True()
        {
            NewRule().CompStatusFilter(2).Should().BeTrue();
        }

        [Fact]
        public void CompletionStatusFilter_False()
        {
            NewRule().CompStatusFilter(1).Should().BeFalse();
        }

        [Fact]
        public void OutcomeFilter_True()
        {
            NewRule().OutcomeFilter(1).Should().BeTrue();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(null)]
        public void OutcomeFilter_False(int? outcome)
        {
            NewRule().OutcomeFilter(outcome).Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void FrameworkComponentTypeFilter_True(int componentType)
        {
            var learnAimRef = "learnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(s => s.GetFrameworkAimsFor(learnAimRef))
                .Returns(new List<ILARSFrameworkAim>()
                {
                    new FrameworkAim()
                    {
                        FrameworkComponentType = componentType
                    }
                });

            NewRule(larsDataServiceMock.Object).FrameworkComponentTypeFilter(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void FrameworkComponentTypeFilter_False()
        {
            var learnAimRef = "learnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(s => s.GetFrameworkAimsFor(learnAimRef))
                .Returns(new List<ILARSFrameworkAim>()
                {
                    new FrameworkAim()
                    {
                        FrameworkComponentType = 2
                    }
                });

            NewRule(larsDataServiceMock.Object).FrameworkComponentTypeFilter(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void FrameworkComponentTypeFilter_False_Null()
        {
            var learnAimRef = "learnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(s => s.GetFrameworkAimsFor(learnAimRef)).Returns(null as IReadOnlyCollection<ILARSFrameworkAim>);

            NewRule(larsDataServiceMock.Object).FrameworkComponentTypeFilter(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void GroupingConditionMet_True()
        {
            var learningDeliveryOne = new TestLearningDelivery()
            {
                ProgTypeNullable = 1,
                FworkCodeNullable = 2,
                PwayCodeNullable = 3,
                FundModel = 4,
            };

            var learningDeliveryTwo = new TestLearningDelivery()
            {
                ProgTypeNullable = 1,
                FworkCodeNullable = 2,
                PwayCodeNullable = 3,
                FundModel = 4,
            };

            var learningDeliveryThree = new TestLearningDelivery()
            {
                ProgTypeNullable = 5,
                FworkCodeNullable = 6,
                PwayCodeNullable = 7,
                FundModel = 8,
            };

            var learningDeliveries = new TestLearningDelivery[]
            {
                learningDeliveryOne,
                learningDeliveryTwo,
                learningDeliveryThree,
            };

            var groups = NewRule().ApplyGroupingCondition(learningDeliveries).ToList();

            groups.Should().HaveCount(1);

            var group = groups.First().ToList();

            group.Should().HaveCount(2);
            group.Should().Contain(new List<ILearningDelivery>() { learningDeliveryOne, learningDeliveryTwo });
        }

        [Fact]
        public void GroupingConditionMet_True_Multiple()
        {
            var learningDeliveryOne = new TestLearningDelivery()
            {
                ProgTypeNullable = 1,
                FworkCodeNullable = 2,
                PwayCodeNullable = 3,
                FundModel = 4,
            };

            var learningDeliveryTwo = new TestLearningDelivery()
            {
                ProgTypeNullable = 1,
                FworkCodeNullable = 2,
                PwayCodeNullable = 3,
                FundModel = 4,
            };

            var learningDeliveryThree = new TestLearningDelivery()
            {
                ProgTypeNullable = 5,
                FworkCodeNullable = 6,
                PwayCodeNullable = 7,
                FundModel = 8,
            };

            var learningDeliveryFour = new TestLearningDelivery()
            {
                ProgTypeNullable = 5,
                FworkCodeNullable = 6,
                PwayCodeNullable = 7,
                FundModel = 8,
            };

            var learningDeliveries = new TestLearningDelivery[]
            {
                learningDeliveryOne,
                learningDeliveryTwo,
                learningDeliveryThree,
                learningDeliveryFour,
            };

            var groups = NewRule().ApplyGroupingCondition(learningDeliveries).ToList();

            groups.Should().HaveCount(2);

            var groupOne = groups[0].ToList();

            groupOne.Should().HaveCount(2);
            groupOne.Should().Contain(new List<ILearningDelivery>() { learningDeliveryOne, learningDeliveryTwo });

            var groupTwo = groups[1].ToList();

            groupTwo.Should().HaveCount(2);
            groupTwo.Should().Contain(new List<ILearningDelivery>() { learningDeliveryThree, learningDeliveryFour });
        }

        [Fact]
        public void GroupingConditionMet_False()
        {
            var learningDeliveryOne = new TestLearningDelivery()
            {
                ProgTypeNullable = 1,
                FworkCodeNullable = 2,
                PwayCodeNullable = 3,
                FundModel = 4,
            };

            var learningDeliveryThree = new TestLearningDelivery()
            {
                ProgTypeNullable = 5,
                FworkCodeNullable = 6,
                PwayCodeNullable = 7,
                FundModel = 8,
            };

            var learningDeliveries = new TestLearningDelivery[]
            {
                learningDeliveryOne,
                learningDeliveryThree,
            };

            var groups = NewRule().ApplyGroupingCondition(learningDeliveries).ToList();

            groups.Should().BeEmpty();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnAimRef = "learnAimRef";

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        AimType = 3,
                        LearnAimRef = learnAimRef,
                        CompStatus = 2,
                        OutcomeNullable = 1,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        AimType = 3,
                        LearnAimRef = learnAimRef,
                        CompStatus = 2,
                        OutcomeNullable = 1,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                    },
                }
            };

            var frameworkAims = new FrameworkAim[]
            {
                new FrameworkAim()
                {
                    FrameworkComponentType = 1
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(s => s.GetFrameworkAimsFor(learnAimRef)).Returns(frameworkAims);

            using (var validationErrorHandler = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandler.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_bug()
        {
            var learnAimRef = "learnAimRef";

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        FundModel = 35,
                        AimType = 3,
                        LearnAimRef = learnAimRef,
                        CompStatus = 2,
                        OutcomeNullable = 1,
                        ProgTypeNullable = 2,
                        FworkCodeNullable = 420,
                        PwayCodeNullable = 1,
                        PriorLearnFundAdjNullable = 25
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        FundModel = 35,
                        AimType = 3,
                        LearnAimRef = learnAimRef,
                        CompStatus = 2,
                        OutcomeNullable = 1,
                        ProgTypeNullable = 2,
                        FworkCodeNullable = 420,
                        PwayCodeNullable = 1,
                        PriorLearnFundAdjNullable = 7
                    },
                }
            };

            var frameworkAims = new FrameworkAim[]
            {
                new FrameworkAim()
                {
                    FrameworkComponentType = 3
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(s => s.GetFrameworkAimsFor(learnAimRef)).Returns(frameworkAims);

            using (var validationErrorHandler = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandler.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnAimRef = "learnAimRef";

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        AimType = 3,
                        LearnAimRef = learnAimRef,
                        CompStatus = 2,
                        OutcomeNullable = 1,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        AimType = 3,
                        LearnAimRef = learnAimRef,
                        CompStatus = 2,
                        OutcomeNullable = 1,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                    },
                }
            };

            var frameworkAims = new FrameworkAim[]
            {
                new FrameworkAim()
                {
                    FrameworkComponentType = 1
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(s => s.GetFrameworkAimsFor(learnAimRef)).Returns(frameworkAims);

            using (var validationErrorHandler = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandler.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_Exclusion()
        {
            var learnAimRef = "learnAimRef";

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        AimType = 3,
                        LearnAimRef = learnAimRef,
                        CompStatus = 2,
                        OutcomeNullable = 1,
                        ProgTypeNullable = 25,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        AimType = 3,
                        LearnAimRef = learnAimRef,
                        CompStatus = 2,
                        OutcomeNullable = 1,
                        ProgTypeNullable = 1,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                    },
                }
            };

            var frameworkAims = new FrameworkAim[]
            {
                new FrameworkAim()
                {
                    FrameworkComponentType = 1
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(s => s.GetFrameworkAimsFor(learnAimRef)).Returns(frameworkAims);

            using (var validationErrorHandler = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandler.Object).Validate(learner);
            }
        }

        private R64Rule NewRule(ILARSDataService larsDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R64Rule(
                validationErrorHandler ?? Mock.Of<IValidationErrorHandler>(),
                larsDataService ?? Mock.Of<ILARSDataService>());
        }
    }
}
