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
    public class AFinType_07RuleTests : AbstractRuleTests<AFinType_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinType_07");
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
            var aFinType = "PMR";
            var aFinCode = 2;

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

            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new[] { 2, 4 }))
                .Returns(false);

            NewRule(appFinQueryServiceMock.Object).AppFinConditionMet(appFinRecords).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void AppFinConditionMet_False(bool mockResultForCode, bool mockResultForCodes)
        {
            var aFinType = "XXX";
            var aFinCode = 2;

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
                .Returns(mockResultForCode);

            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new[] { 2, 4 }))
                .Returns(mockResultForCodes);

            NewRule(appFinQueryServiceMock.Object).AppFinConditionMet(appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 25;
            var aimType = 1;
            var aFinType = "PMR";
            var aFinCode = 2;

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

            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new[] { 2, 4 }))
                .Returns(false);

            NewRule(appFinQueryServiceMock.Object).ConditionMet(progType, aimType, appFinRecords).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseProgType()
        {
            var progType = 0;
            var aimType = 1;
            var aFinType = "PMR";
            var aFinCode = 2;

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

            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new[] { 2, 4 }))
                .Returns(false);

            NewRule(appFinQueryServiceMock.Object).ConditionMet(progType, aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseAimType()
        {
            var progType = 25;
            var aimType = 0;
            var aFinType = "PMR";
            var aFinCode = 2;

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

            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new[] { 2, 4 }))
                .Returns(false);

            NewRule(appFinQueryServiceMock.Object).ConditionMet(progType, aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseAppFin()
        {
            var progType = 25;
            var aimType = 1;
            var aFinType = "XXX";
            var aFinCode = 2;

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

            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new[] { 2, 4 }))
                .Returns(false);

            NewRule(appFinQueryServiceMock.Object).ConditionMet(progType, aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var aFinType = "PMR";
            var aFinCode = 2;

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

            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new[] { 2, 4 }))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var aFinType = "XXX";
            var aFinCode = 2;

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

            appFinQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new[] { 2, 4 }))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinType", "PMR")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinCode", 2)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("PMR", 2);

            validationErrorHandlerMock.Verify();
        }

        private AFinType_07Rule NewRule(
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinType_07Rule(learningDeliveryAppFinRecordQueryService, validationErrorHandler);
        }
    }
}
