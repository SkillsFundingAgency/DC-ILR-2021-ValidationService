using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    public class AFinType_11RuleTests : AbstractRuleTests<AFinType_11Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinType_11");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            var fundModel = 36;

            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            var fundModel = 0;

            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void AFinConditionMet_True()
        {
            var aFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord() { AFinCode = 2 },
                new TestAppFinRecord() { AFinCode = 5 },
                new TestAppFinRecord()
            };

            var codes = new int[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodes(aFinRecords, codes))
                .Returns(true);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).AFinCodeConditionMet(aFinRecords).Should().BeTrue();
        }

        [Fact]
        public void AFinConditionMet_False()
        {
            var aFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord() { AFinCode = 0 },
                new TestAppFinRecord() { AFinCode = 5 },
                new TestAppFinRecord()
            };

            var codes = new int[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodes(aFinRecords, codes))
                .Returns(false);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).AFinCodeConditionMet(aFinRecords).Should().BeFalse();
        }

        [Fact]
        public void AFinConditionMet_FalseNull()
        {
            var codes = new int[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodes(null, codes))
                .Returns(false);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).AFinCodeConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void Excluded_True()
        {
            var progType = 25;

            NewRule().Excluded(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(1)]
        public void Excluded_False(int? progType)
        {
            NewRule().Excluded(progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 36;
            var progType = 1;

            var aFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord() { AFinCode = 2 },
                new TestAppFinRecord() { AFinCode = 5 },
                new TestAppFinRecord()
            };

            var codes = new int[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodes(aFinRecords, codes))
                .Returns(true);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).ConditionMet(fundModel, aFinRecords, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var fundModel = 1;
            var progType = 1;

            var aFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord() { AFinCode = 2 },
                new TestAppFinRecord() { AFinCode = 5 },
                new TestAppFinRecord()
            };

            var codes = new int[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodes(aFinRecords, codes))
                .Returns(true);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).ConditionMet(fundModel, aFinRecords, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseAFin()
        {
            var fundModel = 36;
            var progType = 1;

            var aFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord() { AFinCode = 0 },
                new TestAppFinRecord() { AFinCode = 5 },
                new TestAppFinRecord()
            };

            var codes = new int[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodes(aFinRecords, codes))
                .Returns(false);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).ConditionMet(fundModel, aFinRecords, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseExcluded()
        {
            var fundModel = 36;
            var progType = 25;

            var aFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord() { AFinCode = 2 },
                new TestAppFinRecord() { AFinCode = 5 },
                new TestAppFinRecord()
            };

            var codes = new int[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodes(aFinRecords, codes))
                .Returns(true);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).ConditionMet(fundModel, aFinRecords, progType).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var fundModel = 36;
            var progType = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 2
                            },
                            new TestAppFinRecord()
                            {
                                AFinCode = 5
                            },
                        }
                    }
                }
            };

            var aFinRecords = learner.LearningDeliveries.SelectMany(ld => ld.AppFinRecords);

            var codes = new int[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodes(aFinRecords, codes))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var fundModel = 0;
            var progType = 25;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 0
                            },
                            new TestAppFinRecord()
                            {
                                AFinCode = 5
                            },
                        }
                    }
                }
            };

            var aFinRecords = learner.LearningDeliveries.SelectMany(ld => ld.AppFinRecords);

            var codes = new int[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodes(aFinRecords, codes))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var fundModel = 36;
            var progType = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel, progType);

            validationErrorHandlerMock.Verify();
        }

        private AFinType_11Rule NewRule(
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinType_11Rule(learningDeliveryAppFinRecordQueryService, validationErrorHandler);
        }
    }
}
