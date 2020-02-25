using System;
using System.Collections.Generic;
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
    public class AFinType_08RuleTests : AbstractRuleTests<AFinType_08Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinType_08");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(36).Should().BeFalse();
        }

        [Fact]
        public void AppFinRecordConditionMet_True()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinAmount = 1,
                    AFinCode = 3,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "TNP"
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(
                appFinRecords,
                "TNP",
                It.IsAny<IEnumerable<int>>())).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).AppFinRecordConditionMet(appFinRecords).Should().BeTrue();
        }

        [Fact]
        public void AppFinRecordConditionMet_False()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinAmount = 1,
                    AFinCode = 3,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "TNP"
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(
                appFinRecords,
                "TNP",
                It.IsAny<IEnumerable<int>>())).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).AppFinRecordConditionMet(appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinAmount = 1,
                    AFinCode = 3,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "TNP"
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(
                appFinRecords,
                "TNP",
                It.IsAny<IEnumerable<int>>())).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(35, appFinRecords).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinAmount = 1,
                    AFinCode = 3,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "TNP"
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(
                appFinRecords,
                "TNP",
                It.IsAny<IEnumerable<int>>())).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(36, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AFinType()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinAmount = 1,
                    AFinCode = 3,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "PMR"
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(
                appFinRecords,
                "TNP",
                It.IsAny<IEnumerable<int>>())).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(36, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AFinCode()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinAmount = 1,
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "TNP"
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(
                appFinRecords,
                "TNP",
                It.IsAny<IEnumerable<int>>())).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(36, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinAmount = 1,
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "TNP"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(
                appFinRecords,
                "TNP",
                It.IsAny<IEnumerable<int>>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_FundMode()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinAmount = 1,
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "TNP"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(
                appFinRecords,
                "TNP",
                It.IsAny<IEnumerable<int>>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_AppFinRecord()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinAmount = 1,
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "TNP"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(
                appFinRecords,
                "TNP",
                It.IsAny<IEnumerable<int>>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var fundModel = 35;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel);

            validationErrorHandlerMock.Verify();
        }

        private AFinType_08Rule NewRule(ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinType_08Rule(appFinRecordQueryService, validationErrorHandler);
        }
    }
}
