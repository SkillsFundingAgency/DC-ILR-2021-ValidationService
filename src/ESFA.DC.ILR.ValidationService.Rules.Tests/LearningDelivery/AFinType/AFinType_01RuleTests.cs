using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    public class AFinType_01RuleTests : AbstractRuleTests<AFinType_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinType_01");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            var fundModel = 81;

            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            var fundModel = 0;

            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            var progType = 25;

            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void ProgTypeConditionMet_False(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            var aimType = 1;

            NewRule().AimTypeConditionMet(aimType).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            var aimType = 0;

            NewRule().AimTypeConditionMet(aimType).Should().BeFalse();
        }

        [Fact]
        public void AppFinConditionMet_True()
        {
            var aFinType = "XXX";
            var aFinCode = 0;

            var appFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinType = aFinType,
                    AFinCode = aFinCode
                }
            };

            var appFinQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(appFinRecords, aFinType, aFinCode))
                .Returns(false);

            NewRule(appFinQueryServiceMock.Object).AppFinConditionMet(appFinRecords).Should().BeTrue();
        }

        [Fact]
        public void AppFinConditionMet_False()
        {
            var aFinType = "TNP";
            var aFinCode = 1;

            var appFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinType = aFinType,
                    AFinCode = aFinCode
                }
            };

            var appFinQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(appFinRecords, aFinType, aFinCode))
                .Returns(true);

            NewRule(appFinQueryServiceMock.Object).AppFinConditionMet(appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 81;
            var progType = 25;
            var aimType = 1;
            var aFinType = "XXX";
            var aFinCode = 0;

            var appFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinType = aFinType,
                    AFinCode = aFinCode
                }
            };

            var appFinQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(appFinRecords, aFinType, aFinCode))
                .Returns(false);

            NewRule(appFinQueryServiceMock.Object).ConditionMet(fundModel, progType, aimType, appFinRecords).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var fundModel = 0;
            var progType = 25;
            var aimType = 1;
            var aFinType = "XXX";
            var aFinCode = 0;

            var appFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinType = aFinType,
                    AFinCode = aFinCode
                }
            };

            NewRule().ConditionMet(fundModel, progType, aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseProgType()
        {
            var fundModel = 81;
            var progType = 0;
            var aimType = 1;
            var aFinType = "XXX";
            var aFinCode = 0;

            var appFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinType = aFinType,
                    AFinCode = aFinCode
                }
            };

            NewRule().ConditionMet(fundModel, progType, aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseAimType()
        {
            var fundModel = 81;
            var progType = 25;
            var aimType = 0;
            var aFinType = "XXX";
            var aFinCode = 0;

            var appFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinType = aFinType,
                    AFinCode = aFinCode
                }
            };

            NewRule().ConditionMet(fundModel, progType, aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseAppFin()
        {
            var fundModel = 81;
            var progType = 25;
            var aimType = 1;
            var aFinType = "TNP";
            var aFinCode = 1;

            var appFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinType = aFinType,
                    AFinCode = aFinCode
                }
            };

            var appFinQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(appFinRecords, aFinType, aFinCode))
                .Returns(true);

            NewRule(appFinQueryServiceMock.Object).ConditionMet(fundModel, progType, aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var aFinType = "XXX";
            var aFinCode = 0;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 81,
                        ProgTypeNullable = 25,
                        AimType = 1,
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = aFinType,
                                AFinCode = aFinCode
                            }
                        }
                    }
                }
            };

            var appFinRecords = learner.LearningDeliveries.SelectMany(ld => ld.AppFinRecords);

            var appFinQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(appFinRecords, aFinType, aFinCode))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var aFinType = "TNP";
            var aFinCode = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 81,
                        ProgTypeNullable = 25,
                        AimType = 1,
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = aFinType,
                                AFinCode = aFinCode
                            }
                        }
                    }
                }
            };

            var appFinRecords = learner.LearningDeliveries.SelectMany(ld => ld.AppFinRecords);

            var appFinQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(appFinRecords, aFinType, aFinCode))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var afinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice;
            var aFinCode = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinType", afinType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinCode", aFinCode)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(afinType, aFinCode);

            validationErrorHandlerMock.Verify();
        }

        private AFinType_01Rule NewRule(
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinType_01Rule(learningDeliveryAppFinRecordQueryService, validationErrorHandler);
        }
    }
}
