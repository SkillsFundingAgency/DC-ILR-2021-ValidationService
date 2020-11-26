using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    public class AFinType_10RuleTests : AbstractRuleTests<AFinType_10Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinType_10");
        }

        [Fact]
        public void Exclusion_True()
        {
            NewRule().Exclusion(99, 25).Should().BeTrue();
        }

        [Theory]
        [InlineData(25, 25)]
        [InlineData(99, 24)]
        [InlineData(99, null)]
        [InlineData(25, null)]
        public void Exclusion_False(int fundModel, int? progTypeNullable)
        {
            NewRule().Exclusion(fundModel, progTypeNullable).Should().BeFalse();
        }

        [Fact]
        public void AppFinRecordConditionMet_False()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 2
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                AppFinRecords = appFinRecords
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).AppFinRecordConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void AppFinRecordConditionMet_True()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 1
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                AppFinRecords = appFinRecords
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).AppFinRecordConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void AppFinRecordConditionMet_True_Empty()
        {
            var learningDelivery = new TestLearningDelivery();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(null, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).AppFinRecordConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 1
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                AimType = 1,
                AppFinRecords = appFinRecords
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_TNPExists()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 2
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                AimType = 1,
                AppFinRecords = appFinRecords
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Exclusion()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 1
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                AimType = 1,
                FundModel = 99,
                AppFinRecords = appFinRecords
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 1
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                AimType = 1,
                AppFinRecords = appFinRecords
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 1
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24,
                AimType = 1,
                AppFinRecords = appFinRecords
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_Exclusion()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 1
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                AimType = 1,
                FundModel = 99,
                AppFinRecords = appFinRecords
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_HasTNP()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 2
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                AimType = 1,
                AppFinRecords = appFinRecords
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinType_10Rule NewRule(ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinType_10Rule(validationErrorHandler, appFinRecordQueryService ?? Mock.Of<ILearningDeliveryAppFinRecordQueryService>());
        }
    }
}
