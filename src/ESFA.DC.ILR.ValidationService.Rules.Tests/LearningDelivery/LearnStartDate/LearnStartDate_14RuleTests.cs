﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
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
    public class LearnStartDate_14RuleTests : AbstractRuleTests<LearnStartDate_14Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnStartDate_14");
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            NewRule().ProgTypeConditionMet(25).Should().BeTrue();
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
            NewRule().AimTypeConditionMet(3).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(0).Should().BeFalse();
        }

        [Fact]
        public void StandardCodeExists_True()
        {
            NewRule().StandardCodeExists(1).Should().BeTrue();
        }

        [Fact]
        public void StandardCodeExists_False()
        {
            NewRule().StandardCodeExists(null).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            int stdCode = 1;
            var dd18Date = new DateTime(2018, 10, 01);

            var larsStandard = new LARSStandard()
            {
                StandardCode = stdCode,
                EffectiveTo = new DateTime(2018, 08, 01)
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.GetStandardFor(stdCode)).Returns(larsStandard);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(stdCode, dd18Date).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False_NoMatchingStandard()
        {
            int stdCode = 1;
            var dd18Date = new DateTime(2018, 10, 01);

            var larsStandard = new LARSStandard();

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.GetStandardFor(1)).Returns(larsStandard);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(stdCode, dd18Date).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_False_EndDateCorrect()
        {
            int stdCode = 1;
            var dd18Date = new DateTime(2018, 8, 01);

            var larsStandard = new LARSStandard()
            {
                StandardCode = stdCode,
                EffectiveTo = new DateTime(2018, 09, 01)
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.GetStandardFor(stdCode)).Returns(larsStandard);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(stdCode, dd18Date).Should().BeFalse();
        }

        [Fact]
        public void Excluded_True()
        {
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .Excluded(It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Excluded_False()
        {
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object)
                .Excluded(It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 25;
            var aimType = 3;
            var stdCode = 1;
            var dd18Date = new DateTime(2018, 10, 01);
            var learningDeliveryFams = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var larsStandard = new LARSStandard()
            {
                StandardCode = stdCode,
                EffectiveTo = new DateTime(2018, 08, 01)
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.GetStandardFor(stdCode)).Returns(larsStandard);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES"))
                .Returns(false);

            NewRule(larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(progType, aimType, stdCode, dd18Date, learningDeliveryFams)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            var progType = 0;
            var aimType = 1;
            var stdCode = 1;
            var dd18Date = new DateTime(2018, 01, 01);
            var learningDeliveryFams = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnStartDateGreaterThanStandardsEffectiveTo(stdCode, dd18Date))
                .Returns(true);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES"))
                .Returns(false);

            NewRule(larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(progType, aimType, stdCode, dd18Date, learningDeliveryFams)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var progType = 25;
            var aimType = 0;
            var stdCode = 1;
            var dd18Date = new DateTime(2018, 01, 01);
            var learningDeliveryFams = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnStartDateGreaterThanStandardsEffectiveTo(stdCode, dd18Date))
                .Returns(true);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES"))
                .Returns(false);

            NewRule(larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(progType, aimType, stdCode, dd18Date, learningDeliveryFams)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_STDCode()
        {
            var progType = 25;
            var aimType = 1;
            var stdCode = 0;
            var dd18Date = new DateTime(2018, 01, 01);
            var learningDeliveryFams = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var larsStandard = new LARSStandard();

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.GetStandardFor(stdCode)).Returns(larsStandard);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES"))
                .Returns(false);

            NewRule(larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(progType, aimType, stdCode, dd18Date, learningDeliveryFams)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Excluded()
        {
            var progType = 25;
            var aimType = 1;
            var stdCode = 1;
            var dd18Date = new DateTime(2018, 01, 01);
            var learningDeliveryFams = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.LearnStartDateGreaterThanStandardsEffectiveTo(stdCode, dd18Date))
                .Returns(true);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES"))
                .Returns(true);

            NewRule(larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(progType, aimType, stdCode, dd18Date, learningDeliveryFams)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var stdCode = 1;
            var dd18Date = new DateTime(2018, 10, 01);
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 25,
                        AimType = 3,
                        StdCodeNullable = stdCode,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                        }
                    }
                }
            };

            var learningDeliveryFams = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var larsStandard = new LARSStandard()
            {
                StandardCode = stdCode,
                EffectiveTo = new DateTime(2018, 08, 01)
            };

            var dd18Mock = new Mock<IDerivedData_18Rule>();
            dd18Mock.Setup(dm => dm.GetApprenticeshipStandardProgrammeStartDateFor(It.IsAny<ILearningDelivery>(), It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(dd18Date);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.GetStandardFor(stdCode)).Returns(larsStandard);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES"))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, dd18Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var stdCode = 1;
            var dd18Date = new DateTime(2018, 01, 01);
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 25,
                        AimType = 3,
                        StdCodeNullable = stdCode,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "RES"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFams = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var larsStandard = new LARSStandard()
            {
                StandardCode = stdCode,
                EffectiveTo = new DateTime(2018, 08, 01)
            };

            var dd18Mock = new Mock<IDerivedData_18Rule>();
            dd18Mock.Setup(dm => dm.GetApprenticeshipStandardProgrammeStartDateFor(It.IsAny<ILearningDelivery>(), It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(dd18Date);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.GetStandardFor(stdCode)).Returns(larsStandard);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES"))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, dd18Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.StdCode, 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private LearnStartDate_14Rule NewRule(
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IDerivedData_18Rule derivedData18 = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnStartDate_14Rule(larsDataService, learningDeliveryFamQueryService, derivedData18, validationErrorHandler);
        }
    }
}
