using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_06RuleTests : AbstractRuleTests<LearnStartDate_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnStartDate_06");
        }

        [Fact]
        public void ApprenticeshipConditionMet_True()
        {
            var progType = 24;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False()
        {
            var progType = 99;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeFalse();
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
        public void FrameworkConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 10, 01);
            var progType = 1;
            var fworkCode = 1;
            var pwayCode = 1;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm =>
                    ldsm.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fworkCode, pwayCode))
                .Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).FrameworkConditionMet(learnAimRef, learnStartDate, progType, fworkCode, pwayCode).Should().BeTrue();
        }

        [Fact]
        public void FrameworkConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 10, 01);
            var progType = 0;
            var fworkCode = 0;
            var pwayCode = 0;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm =>
                    ldsm.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fworkCode, pwayCode))
                .Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).FrameworkConditionMet(learnAimRef, learnStartDate, progType, fworkCode, pwayCode).Should().BeFalse();
        }

        [Fact]
        public void FrameworkConditionMet_FalseNull()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 10, 01);
            int? progType = null;
            int? fworkCode = null;
            int? pwayCode = null;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm =>
                    ldsm.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fworkCode, pwayCode))
                .Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).FrameworkConditionMet(learnAimRef, learnStartDate, progType, fworkCode, pwayCode).Should().BeFalse();
        }

        [Fact]
        public void Excluded_TrueProgType()
        {
            var progType = 25;

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
            };

            NewRule().Excluded(progType, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void Excluded_TrueLDFAM()
        {
            var progType = 0;
            var learnDelFAMType = "RES";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = learnDelFAMType
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "XXX"
                },
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, learnDelFAMType))
                .Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).Excluded(progType, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void Excluded_False()
        {
            var progType = 1;
            var learnDelFAMType = "XXX";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = learnDelFAMType
                },
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, learnDelFAMType))
                .Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).Excluded(progType, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void Excluded_FalseNull()
        {
            var progType = 0;
            var learnDelFAMType = "XXX";

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(null, learnDelFAMType))
                .Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).Excluded(progType, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_true()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 10, 01);
            var aimType = 1;
            var progType = 24;
            var fworkCode = 15;
            var pwayCode = 15;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "XXX"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock
                .Setup(dm => dm.IsApprenticeship(progType))
                .Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES"))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(learnAimRef, progType, aimType, learnStartDate, fworkCode, pwayCode, learningDeliveryFAMs)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseApprenticeship()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 10, 01);
            var aimType = 1;
            var progType = 99;
            var fworkCode = 15;
            var pwayCode = 15;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "XXX"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock
                .Setup(dm => dm.IsApprenticeship(progType))
                .Returns(false);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES"))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(learnAimRef, progType, aimType, learnStartDate, fworkCode, pwayCode, learningDeliveryFAMs)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseAimType()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 10, 01);
            var aimType = 0;
            var progType = 24;
            var fworkCode = 15;
            var pwayCode = 15;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "XXX"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock
                .Setup(dm => dm.IsApprenticeship(progType))
                .Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES"))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(learnAimRef, progType, aimType, learnStartDate, fworkCode, pwayCode, learningDeliveryFAMs)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFramework()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 10, 01);
            var aimType = 1;
            var progType = 0;
            var fworkCode = 0;
            var pwayCode = 0;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "XXX"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock
                .Setup(dm => dm.IsApprenticeship(progType))
                .Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fworkCode, pwayCode))
                .Returns(false);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES"))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(learnAimRef, progType, aimType, learnStartDate, fworkCode, pwayCode, learningDeliveryFAMs)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseExcluded()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 10, 01);
            var aimType = 1;
            var progType = 25;
            var fworkCode = 15;
            var pwayCode = 15;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "RES"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock
                .Setup(dm => dm.IsApprenticeship(progType))
                .Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES"))
                .Returns(true);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(learnAimRef, progType, aimType, learnStartDate, fworkCode, pwayCode, learningDeliveryFAMs)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 10, 01);
            var aimType = 1;
            var progType = 24;
            var fworkCode = 15;
            var pwayCode = 15;

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        LearnStartDate = learnStartDate,
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "XXX"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMs = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock
                .Setup(dm => dm.IsApprenticeship(progType))
                .Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES"))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2018, 10, 01);
            var aimType = 1;
            var progType = 25;
            var fworkCode = 15;
            var pwayCode = 15;

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = learnAimRef,
                        LearnStartDate = learnStartDate,
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "RES"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMs = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock
                .Setup(dm => dm.IsApprenticeship(progType))
                .Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fworkCode, pwayCode))
                .Returns(true);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES"))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, larsDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/10/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 10, 01));

            validationErrorHandlerMock.Verify();
        }

        private LearnStartDate_06Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnStartDate_06Rule(dd07, larsDataService, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
