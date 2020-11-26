using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_57RuleTests : AbstractRuleTests<LearnAimRef_57Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_57");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 1;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 1, 1);
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.RuleNotTriggeredConditionMet(learningDeliveryFams)).Returns(true);
            ruleMock.Setup(r => r.LARSCategoryConditionMet(learnAimRef)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.EsmConditionMet(learnStartDate, learnerEmploymentStatuses, learningDeliveryFams)).Returns(true);

            ruleMock.Object.ConditionMet(fundModel, learnAimRef, learnStartDate, learningDeliveryFams, learnerEmploymentStatuses).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var fundModel = 1;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 1, 1);
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, learnAimRef, learnStartDate, learningDeliveryFams, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_RuleNotTriggered()
        {
            var fundModel = 1;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 1, 1);
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.RuleNotTriggeredConditionMet(learningDeliveryFams)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, learnAimRef, learnStartDate, learningDeliveryFams, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LARSCategory()
        {
            var fundModel = 1;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 1, 1);
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.LARSCategoryConditionMet(learnAimRef)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, learnAimRef, learnStartDate, learningDeliveryFams, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            var fundModel = 1;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 1, 1);
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, learnAimRef, learnStartDate, learningDeliveryFams, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ESM()
        {
            var fundModel = 1;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 1, 1);
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();
            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.EsmConditionMet(learnStartDate, learnerEmploymentStatuses, learningDeliveryFams)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, learnAimRef, learnStartDate, learningDeliveryFams, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(70)]
        [InlineData(81)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(36).Should().BeFalse();
        }

        [Fact]
        public void RuleNotTriggeredConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(ds => ds.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "347")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).RuleNotTriggeredConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void RuleNotTriggeredConditionMet_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "347"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(ds => ds.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "347")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).RuleNotTriggeredConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LARSCategoryConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, 20)).Returns(true);

            NewRule(larsDataServiceMock.Object).LARSCategoryConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LARSCategoryConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, 20)).Returns(false);

            NewRule(larsDataServiceMock.Object).LARSCategoryConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
           NewRule().LearnStartDateConditionMet(new DateTime(2016, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False_Before()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2014, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_False_After()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2017, 1, 1)).Should().BeFalse();
        }

        [Theory]
        [InlineData("BSI", 3, "SOF", "318")]
        [InlineData("BSI", 4, "LDM", "320")]
        public void EsmConditionMet_True(string esmType, int esmCode, string famType, string famCode)
        {
                var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus
                    {
                        DateEmpStatApp = new DateTime(2018, 08, 01),
                        EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring
                            {
                                ESMType = esmType,
                                ESMCode = esmCode
                            }
                        }
                    }
                };

                var learningDelivery = new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2018, 08, 01),
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                    }
                };

                var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
                var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();

                learningDeliveryFAMQueryServiceMock
                .Setup(ds => ds.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, famType, famCode))
                .Returns(true);
                learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(ds => ds.HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, esmType, esmCode))
                .Returns(true);

                NewRule(
                learnerEmploymentStatusMonitoringQueryService: learnerEmploymentStatusMonitoringQueryServiceMock.Object,
                learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .EsmConditionMet(learningDelivery.LearnStartDate, learnerEmploymentStatuses, learningDelivery.LearningDeliveryFAMs)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("XXX", 3, "SOF", "318")]
        [InlineData("BSI", 2, "SOF", "318")]
        [InlineData("XXX", 4, "SOF", "318")]
        [InlineData("BSI", 2, "LDM", "318")]
        [InlineData("BSI", 4, "SOF", "318")]
        [InlineData("BSI", 4, "LDM", "100")]
        public void EsmConditionMet_False(string esmType, int esmCode, string famType, string famCode)
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                    new TestLearnerEmploymentStatus
                    {
                        DateEmpStatApp = new DateTime(2018, 08, 01),
                        EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring
                            {
                                ESMType = esmType,
                                ESMCode = esmCode
                            }
                        }
                    }
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnStartDate = new DateTime(2018, 08, 01),
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = famType,
                            LearnDelFAMCode = famCode
                        }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(ds => ds.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, famType, famCode))
                .Returns(false);
            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(ds => ds.HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, esmType, esmCode))
                .Returns(false);

            NewRule(
                learnerEmploymentStatusMonitoringQueryService: learnerEmploymentStatusMonitoringQueryServiceMock.Object,
                learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .EsmConditionMet(learningDelivery.LearnStartDate, learnerEmploymentStatuses, learningDelivery.LearningDeliveryFAMs)
                .Should().BeFalse();
        }

        [Fact]
        public void EsmConditionMet_False_Null()
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[] { };

            var learningDelivery = new TestLearningDelivery
            {
                LearnStartDate = new DateTime(2018, 08, 01),
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "BSI",
                            LearnDelFAMCode = "3"
                        }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(ds => ds.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "LDM", "3"))
                .Returns(false);
            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(ds => ds.HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "BSI", 3))
                .Returns(false);

            NewRule(
                learnerEmploymentStatusMonitoringQueryService: learnerEmploymentStatusMonitoringQueryServiceMock.Object,
                learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .EsmConditionMet(learningDelivery.LearnStartDate, learnerEmploymentStatuses, learningDelivery.LearningDeliveryFAMs)
                .Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnAimRef = "LearnAimRef";
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
                {
                    new TestLearnerEmploymentStatus
                    {
                        DateEmpStatApp = new DateTime(2015, 08, 01),
                        EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                        {
                            new TestEmploymentStatusMonitoring
                            {
                                ESMType = "BSI",
                                ESMCode = 3
                            }
                        }
                    }
                };

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                 new TestLearningDeliveryFAM
                 {
                     LearnDelFAMType = "LDM",
                     LearnDelFAMCode = "318"
                 }
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        FundModel = 81,
                        LearnStartDate = new DateTime(2015, 08, 01),
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();

            larsDataServiceMock.Setup(ds => ds.LearnAimRefExistsForLearningDeliveryCategoryRef(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            learningDeliveryFAMQueryServiceMock
                .Setup(ds => ds.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "318"))
                .Returns(true);
            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(ds => ds.HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    larsDataServiceMock.Object,
                    learnerEmploymentStatusMonitoringQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2011, 1, 1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2017, 1, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", learnAimRef)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2017")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnAimRef, learnStartDate);

            validationErrorHandlerMock.Verify();
        }

        private LearnAimRef_57Rule NewRule(
            ILARSDataService larsDataService = null,
            ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnAimRef_57Rule(
                larsDataService,
                learnerEmploymentStatusMonitoringQueryService,
                learningDeliveryFamQueryService,
                validationErrorHandler);
        }
    }
}
