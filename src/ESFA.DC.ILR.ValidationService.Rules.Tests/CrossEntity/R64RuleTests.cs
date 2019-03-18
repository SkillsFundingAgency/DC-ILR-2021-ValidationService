using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R64RuleTests : AbstractRuleTests<R64Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R64");
        }

        [Theory]
        [InlineData(88)]
        [InlineData(99)]
        [InlineData(10)]
        [InlineData(25)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelsConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelsConditionMet(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(4)]
        public void AimTypeConditionMet_False(int aimType)
        {
            NewRule().AimTypeConditionMet(aimType).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(3).Should().BeTrue();
        }

        [Fact]
        public void CompletedLearningDeliveryConditionMet_True()
        {
            NewRule().CompletedLearningDeliveryConditionMet(2, 1).Should().BeTrue();
        }

        [Theory]
        [InlineData(2, null)]
        [InlineData(1, 1)]
        public void CompletedLearningDeliveryConditionMet_False(int compStatus, int? outCome)
        {
            NewRule().CompletedLearningDeliveryConditionMet(compStatus, outCome).Should().BeFalse();
        }

        [Fact]
        public void LarsComponentTypeConditionMet_True()
        {
            string learnAimRef = "ZESF98765";
            var progType = 3;
            var fworkCode = 445;
            var pwayCode = 1;
            var learnStartDate = new DateTime(2019, 01, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.FrameworkCodeExistsForFrameworkAimsAndFrameworkComponentTypes(learnAimRef, progType, fworkCode, pwayCode, frameWorkComponentTypes, learnStartDate))
                .Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).LarsComponentTypeConditionMet(learnAimRef, progType, fworkCode, pwayCode, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void LarsComponentTypeConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            var progType = 3;
            var fworkCode = 445;
            var pwayCode = 1;
            var learnStartDate = new DateTime(2019, 01, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.FrameworkCodeExistsForFrameworkAimsAndFrameworkComponentTypes(learnAimRef, progType, fworkCode, pwayCode, frameWorkComponentTypes, learnStartDate))
                .Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).LarsComponentTypeConditionMet(learnAimRef, progType, fworkCode, pwayCode, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveries = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = 3,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 2,
                    StdCodeNullable = 3,
                    LearnStartDate = new DateTime(2018, 10, 10)
                },
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = 3,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 5,
                    StdCodeNullable = 3,
                    LearnStartDate = new DateTime(2017, 10, 10)
                }
            };

            var completedLearningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = "101",
                AimType = 3,
                FworkCodeNullable = 1,
                PwayCodeNullable = 2,
                StdCodeNullable = 3,
                LearnStartDate = new DateTime(2018, 10, 09)
            };

            NewRule().ConditionMet(learningDeliveries, completedLearningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(1, null, null, null, null)]
        [InlineData(3, 99, null, null, null)]
        [InlineData(3, 99, 100, 200, null)]
        [InlineData(3, 80, 0, 0, 100)]
        [InlineData(3, 0, 0, 0, 0)]
        [InlineData(3, 1, null, 20, null)]
        [InlineData(3, 1, null, 2, 9999)]
        [InlineData(3, 1, 2, 2, 9999)]
        [InlineData(3, 2, null, 3, 3)]
        public void ConditionMet_False(int aimType, int? frameworkCode, int? standardCode, int?pwayCode, int? progType)
        {
            var learningDeliveries = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = aimType,
                    FworkCodeNullable = frameworkCode,
                    PwayCodeNullable = pwayCode,
                    StdCodeNullable = standardCode,
                    ProgTypeNullable = progType,
                    LearnStartDate = new DateTime(2018, 10, 10)
                },
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = aimType,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 5,
                    StdCodeNullable = 3,
                    ProgTypeNullable = progType,
                    LearnStartDate = new DateTime(2017, 10, 10)
                }
            };

            var completedLearningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = "101",
                AimType = aimType,
                FworkCodeNullable = 1,
                PwayCodeNullable = 2,
                StdCodeNullable = null,
                ProgTypeNullable = 3,
                LearnStartDate = new DateTime(2018, 10, 09)
            };

            NewRule().ConditionMet(learningDeliveries, completedLearningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_LearnStartDate_False()
        {
            var learningDeliveries = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = 3,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 2,
                    StdCodeNullable = null,
                    ProgTypeNullable = 3,
                    LearnStartDate = new DateTime(2018, 10, 10)
                },
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = 3,
                    FworkCodeNullable = null,
                    PwayCodeNullable = null,
                    StdCodeNullable = null,
                    ProgTypeNullable = 3,
                    LearnStartDate = new DateTime(2017, 10, 10)
                }
            };

            var completedLearningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = "101",
                AimType = 3,
                FworkCodeNullable = 1,
                PwayCodeNullable = 2,
                StdCodeNullable = null,
                ProgTypeNullable = 3,
                LearnStartDate = new DateTime(2018, 10, 10)
            };

            NewRule().ConditionMet(learningDeliveries, completedLearningDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(100)]
        [InlineData(99)]
        public void ExcludeConditionMet_False(int? progType)
        {
            NewRule().ExcludeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void ExcludeConditionMet_True()
        {
            NewRule().ExcludeConditionMet(25).Should().BeTrue();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        public void Validate_Error(int fundModel)
        {
            var learningDeliveries = new[]
            {
                new TestLearningDelivery() // this will cause the condition to be met
                {
                    LearnAimRef = "101",
                    AimType = 3,
                    ProgTypeNullable = 2,
                    FworkCodeNullable = 2,
                    PwayCodeNullable = 3,
                    StdCodeNullable = null,
                    FundModel = fundModel,
                    LearnStartDate = new DateTime(2017, 10, 11)
                },
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = 3,
                    ProgTypeNullable = 2,
                    FworkCodeNullable = 2,
                    PwayCodeNullable = 3,
                    StdCodeNullable = null,
                    FundModel = fundModel,
                    OutcomeNullable = 1,
                    CompStatus = 2,
                    LearnStartDate = new DateTime(2017, 10, 10)
                },
                new TestLearningDelivery() // this will be excluded as fund model is 25
                {
                    LearnAimRef = "101",
                    AimType = 3,
                    ProgTypeNullable = 2,
                    FworkCodeNullable = 2,
                    PwayCodeNullable = 3,
                    StdCodeNullable = null,
                    FundModel = 25,
                    LearnStartDate = new DateTime(2017, 10, 11)
                }
            };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "101Learner",
                LearningDeliveries = learningDeliveries
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.FrameworkCodeExistsForFrameworkAimsAndFrameworkComponentTypes(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<HashSet<int?>>(), It.IsAny<DateTime>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveries = new[]
           {
                new TestLearningDelivery() // this will cause the condition to be met
                {
                    LearnAimRef = "101",
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                    FworkCodeNullable = 2,
                    PwayCodeNullable = 3,
                    StdCodeNullable = null,
                    FundModel = 26,
                    LearnStartDate = new DateTime(2017, 10, 10)
                },
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                    FworkCodeNullable = 2,
                    PwayCodeNullable = 3,
                    StdCodeNullable = null,
                    FundModel = 26,
                    OutcomeNullable = 1,
                    CompStatus = 2,
                    LearnStartDate = new DateTime(2017, 10, 10)
                },
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                    FworkCodeNullable = 2,
                    PwayCodeNullable = 3,
                    StdCodeNullable = null,
                    FundModel = 26,
                    LearnStartDate = new DateTime(2017, 10, 09)
                }
            };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "101Learner",
                LearningDeliveries = learningDeliveries
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.FrameWorkComponentTypeExistsInFrameworkAims(It.IsAny<string>(), It.IsAny<HashSet<int?>>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NullLearningDeliveries_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "101Learner",
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("AimType", 3)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("FundModel", 36)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("ProgType", 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("FworkCode", 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("PwayCode", 3)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("StdCode", null)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("Outcome", 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("CompStatus", 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("LearnStartDate", "10/10/2017")).Verifiable();

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = "101",
                AimType = 3,
                ProgTypeNullable = 2,
                FworkCodeNullable = 2,
                PwayCodeNullable = 3,
                StdCodeNullable = null,
                FundModel = 36,
                OutcomeNullable = 1,
                CompStatus = 2,
                LearnStartDate = new DateTime(2017, 10, 10)
            };

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learningDelivery);

            validationErrorHandlerMock.Verify();
        }

        private R64Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILARSDataService larsDataService = null)
        {
            return new R64Rule(larsDataService, validationErrorHandler);
        }
    }
}
