using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    public class AFinType_04RuleTests : AbstractRuleTests<AFinType_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinType_04");
        }

        [Fact]
        public void AppFinRecordConditionMet_True()
        {
            var appFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinAmount = 1,
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "X"
                },
                new TestAppFinRecord(),
            };

            NewRule().AppFinRecordConditionMet(appFinRecords).Should().BeTrue();
        }

        [Fact]
        public void AppFinRecordConditionMet_False()
        {
            NewRule().AppFinRecordConditionMet(null).Should().BeFalse();
        }

        [Theory]
        [InlineData(81, 1, 2)]
        [InlineData(81, 1, 1)]
        [InlineData(1, 1, 25)]
        [InlineData(36, 2, null)]
        [InlineData(1, 1, null)]
        public void AimConditionMet_True(int fundModel, int aimType, int? progType)
        {
            NewRule().AimConditionMet(fundModel, aimType, progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(81, 1, 25)]
        [InlineData(36, 1, null)]
        public void AimConditionMet_False(int fundModel, int aimType, int? progType)
        {
            NewRule().AimConditionMet(fundModel, aimType, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 99;
            var aimType = 1;
            var progType = 1;

            var appFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinAmount = 1,
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "X"
                },
                new TestAppFinRecord(),
            };

            NewRule().ConditionMet(appFinRecords, fundModel, aimType, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var fundModel = 81;
            var aimType = 1;
            var progType = 25;

            var appFinRecords = new List<TestAppFinRecord>()
            {
                new TestAppFinRecord()
                {
                    AFinAmount = 1,
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 01),
                    AFinType = "X"
                },
                new TestAppFinRecord(),
            };

            NewRule().ConditionMet(appFinRecords, fundModel, aimType, progType).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var fundModel = 81;
            var aimType = 1;
            var progType = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinAmount = 1,
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 01),
                                AFinType = "X"
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var fundModel = 81;
            var aimType = 1;
            var progType = 25;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinAmount = 1,
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 01),
                                AFinType = "X"
                            }
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
        public void BuildErrorMessageParameters()
        {
            var fundModel = 36;
            var progType = 1;
            var aimType = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", aimType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel, aimType, progType);

            validationErrorHandlerMock.Verify();
        }

        private AFinType_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinType_04Rule(validationErrorHandler);
        }
    }
}
