using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_12RuleTests : AbstractRuleTests<ProgType_12Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ProgType_12");
        }

        [Fact]
        public void Excluded_False()
        {
            string learnAimRef = "ZESF98765";
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var larsMock = new Mock<ILARSDataService>();

            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);

            NewRule(larsDataService: larsMock.Object).Excluded(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void Excluded_True()
        {
            string learnAimRef = "ZESF98765";
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var larsMock = new Mock<ILARSDataService>();

            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(true);

            NewRule(larsDataService: larsMock.Object).Excluded(learnAimRef).Should().BeTrue();
        }

        [Theory]
        [InlineData(35, true)]
        [InlineData(36, true)]
        [InlineData(81, false)]
        [InlineData(0, false)]
        public void FundModelConditionMetMeetsExpectation(int fundModel, bool expectation)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(expectation);
        }

        [Theory]
        [InlineData(3, true)]
        [InlineData(21, false)]
        [InlineData(0, false)]
        public void AimTypeConditionMetMeetsExpectation(int aimType, bool expectation)
        {
            NewRule().AimTypeConditionMet(aimType).Should().Be(expectation);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(21, false)]
        [InlineData(null, false)]
        [InlineData(0, false)]
        public void ProgTypeConditionMetMeetsExpectation(int? progType, bool expectation)
        {
            NewRule().ProgTypeConditionMet(progType).Should().Be(expectation);
        }

        [Theory]
        [InlineData(445, true)]
        [InlineData(21, false)]
        [InlineData(null, false)]
        [InlineData(0, false)]
        public void FworkCodeConditionMetMeetsExpectation(int? fworkCode, bool expectation)
        {
            NewRule().FworkCodeConditionMet(fworkCode).Should().Be(expectation);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(12, false)]
        [InlineData(null, false)]
        [InlineData(0, false)]
        public void PwayCodeConditionMetMeetsExpectation(int? pwayCode, bool expectation)
        {
            NewRule().PwayCodeConditionMet(pwayCode).Should().Be(expectation);
        }

        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            NewRule().LastViableStartDate.Should().Be(DateTime.Parse("2014-09-01"));
        }

        [Fact]
        public void DD04ConditionMet_True()
        {
            NewRule().DD04ConditionMet(new DateTime(2014, 9, 1)).Should().BeTrue();
        }

        [Fact]
        public void DD04ConditionMet_False()
        {
            NewRule().DD04ConditionMet(new DateTime(2013, 9, 1)).Should().BeFalse();
        }

        [Fact]
        public void DD04ConditionMet_Null()
        {
            NewRule().DD04ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFamConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                    .Returns(false);

            NewRule(learningDeliveryFamQueryService: famQueryServiceMock.Object).LearningDeliveryFamConditionMet(learningDelivery.LearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFamConditionMet_False()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(true);

            NewRule(learningDeliveryFamQueryService: famQueryServiceMock.Object).LearningDeliveryFamConditionMet(learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void BasicSkillsTypeConditionMet_True()
        {
            string learnAimRef = "ZESF98765";
            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            NewRule(larsDataService: larsMock.Object).BasicSkillsTypeConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void BasicSkillsTypeConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(true);

            NewRule(larsDataService: larsMock.Object).BasicSkillsTypeConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 35;
            int progType = 2;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 36;
            int progType = 2;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(true);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(true);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModelConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 81;
            int progType = 2;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ProgTypConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 35;
            int progType = 0;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FworkCodeConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 35;
            int progType = 2;
            int? fworkCode = null;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_PwayCodeConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 35;
            int progType = 2;
            int fworkCode = 445;
            int? pwayCode = null;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimTypeConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 35;
            int progType = 2;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 0;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_IsExcluded()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 35;
            int progType = 2;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(true);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD04ConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 35;
            int progType = 2;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2013, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearningDeliveryFamConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 35;
            int progType = 2;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(true);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_BasicSkillsConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 35;
            int progType = 2;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = learnAimRef,
                FundModel = fundModel,
                AimType = aimType,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(true);

            NewRule(larsDataService: larsMock.Object, learningDeliveryFamQueryService: famQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 35;
            int progType = 2;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learner = new TestLearner()
            {
                LearnRefNumber = "123",
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        FundModel = fundModel,
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "RES"
                            }
                        }
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                            x.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                        .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            var dd04Mock = new Mock<IDerivedData_04Rule>();
            dd04Mock.Setup(x => x.GetEarliesStartDateFor(It.IsAny<ILearningDelivery>(), It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                                    .Returns(dd04Date);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                        dd04: dd04Mock.Object,
                        larsDataService: larsMock.Object,
                        learningDeliveryFamQueryService: famQueryServiceMock.Object,
                        validationErrorHandler: validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_No_Error()
        {
            string learnAimRef = "ZESF98765";
            int fundModel = 81;
            int progType = 2;
            int fworkCode = 445;
            int pwayCode = 1;
            int aimType = 3;
            DateTime dd04Date = new DateTime(2015, 09, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 2, 3 };

            var learner = new TestLearner()
            {
                LearnRefNumber = "123",
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        FundModel = fundModel,
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "RES"
                            }
                        }
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock.Setup(x =>
                            x.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                        .Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);
            larsMock.Setup(e => e.BasicSkillsTypeMatchForLearnAimRef(It.IsAny<IEnumerable<int>>(), learnAimRef)).Returns(false);

            var dd04Mock = new Mock<IDerivedData_04Rule>();
            dd04Mock.Setup(x => x.GetEarliesStartDateFor(It.IsAny<ILearningDelivery>(), It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                                    .Returns(dd04Date);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                        dd04: dd04Mock.Object,
                        larsDataService: larsMock.Object,
                        learningDeliveryFamQueryService: famQueryServiceMock.Object,
                        validationErrorHandler: validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoLearningDeliveries()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "123"
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProgType, 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FworkCode, 445)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.PwayCode, 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(TypeOfLearningProgramme.AdvancedLevelApprenticeship, 445, 1);

            validationErrorHandlerMock.Verify();
        }

        public ProgType_12Rule NewRule(
            IDerivedData_04Rule dd04 = null,
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new ProgType_12Rule(dd04, larsDataService, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
