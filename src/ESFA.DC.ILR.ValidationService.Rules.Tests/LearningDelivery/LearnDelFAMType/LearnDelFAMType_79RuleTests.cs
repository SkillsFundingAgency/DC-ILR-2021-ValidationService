using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_79RuleTests : AbstractRuleTests<LearnDelFAMType_79Rule>
    {
        private readonly HashSet<int?> _basicSkillTypes = new HashSet<int?>() { 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35 };
        private readonly HashSet<string> _ldmExclusions = new HashSet<string>
        {
            LearningDeliveryFAMCodeConstants.LDM_OLASS,
            LearningDeliveryFAMCodeConstants.LDM_RoTL,
            LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy,
            LearningDeliveryFAMCodeConstants.LDM_LowWages,
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_79");
        }

        [Fact]
        public void FundModelCondition_True()
        {
            NewRule().FundModelCondition(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelCondition_False()
        {
            NewRule().FundModelCondition(10).Should().BeFalse();
        }

        [Fact]
        public void StartDateCondition_True()
        {
            NewRule().StartDateCondition(new DateTime(2020, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void StartDateCondition_False()
        {
            NewRule().StartDateCondition(new DateTime(2020, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void AgeConditionMet_True()
        {
            var dob = new DateTime(2000, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dob, It.IsAny<DateTime>())).Returns(19);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).AgeConditionMet(new DateTime(2019, 10, 10), dob).Should().BeTrue();
        }

        [Theory]
        [InlineData("2000-11-10")]
        [InlineData("1994-10-10")]
        public void AgeConditionMet_False_Over23(string dateOfBirthString)
        {
            var dateOfBirth = DateTime.Parse(dateOfBirthString);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dateOfBirth, It.IsAny<DateTime>())).Returns(24);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).AgeConditionMet(new DateTime(2018, 10, 10), dateOfBirth).Should().BeFalse();
        }

        [Theory]
        [InlineData("2001-10-10")]
        public void AgeConditionMet_False_Under19(string dateOfBirthString)
        {
            var dateOfBirth = DateTime.Parse(dateOfBirthString);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dateOfBirth, It.IsAny<DateTime>())).Returns(18);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).AgeConditionMet(new DateTime(2018, 10, 10), dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void AgeConditionMet_False_NullDob()
        {
            NewRule().AgeConditionMet(It.IsAny<DateTime>(), null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsCondition_True()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.FFI,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.FFI_Fully
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                testLearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.FFI,
                LearningDeliveryFAMCodeConstants.FFI_Fully)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsCondition(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData("FFI", "2")]
        [InlineData("LDM", "1")]
        [InlineData("LDM", "2")]
        public void LearningDeliveryFAMsCondition_False(string famType, string famCode)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                testLearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.FFI,
                LearningDeliveryFAMCodeConstants.FFI_Fully)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsCondition(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsCondition_False_NullLDFams()
        {
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                null,
                LearningDeliveryFAMTypeConstants.FFI,
                LearningDeliveryFAMCodeConstants.FFI_Fully)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsCondition(null).Should().BeFalse();
        }

        [Theory]
        [InlineData("2", 37)]
        [InlineData("1", 35)]
        [InlineData("1", 37)]
        public void LarsCondition_False(string nvq, int categoryRef)
        {
            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                NotionalNVQLevelv2 = nvq,
                Categories = new List<Data.External.LARS.Model.LearningDeliveryCategory>
                {
                    new Data.External.LARS.Model.LearningDeliveryCategory
                    {
                        LearnAimRef = "LearnAimRef",
                        CategoryRef = categoryRef
                    }
                }
            };

            NewRule().LarsCondition(larsDelivery).Should().BeFalse();
        }

        [Fact]
        public void LarsCondition_True_NoCategories()
        {
            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                NotionalNVQLevelv2 = "2"
            };

            NewRule().LarsCondition(larsDelivery).Should().BeTrue();
        }

        [Fact]
        public void LarsCondition_True()
        {
            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                NotionalNVQLevelv2 = "2",
                Categories = new List<Data.External.LARS.Model.LearningDeliveryCategory>
                {
                    new Data.External.LARS.Model.LearningDeliveryCategory
                    {
                        LearnAimRef = "LearnAimRef",
                        CategoryRef = 35
                    }
                }
            };

            NewRule().LarsCondition(larsDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(37)]
        [InlineData(35)]
        [InlineData(null)]
        public void DD07Condition_False(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07Condition(progType).Should().BeFalse();
        }

        [Fact]
        public void DD07_True()
        {
            int? progType = 24;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07Condition(progType).Should().BeTrue();
        }

        [Fact]
        public void DD29Condition_False()
        {
            var learningDelivery = new TestLearningDelivery();

            var dd29Mock = new Mock<IDerivedData_29Rule>();
            dd29Mock.Setup(dd => dd.IsInflexibleElementOfTrainingAimLearningDelivery(learningDelivery)).Returns(false);

            NewRule(dd29: dd29Mock.Object).DD29Condition(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void DD29Condition_True()
        {
            var learningDelivery = new TestLearningDelivery();

            var dd29Mock = new Mock<IDerivedData_29Rule>();
            dd29Mock.Setup(dd => dd.IsInflexibleElementOfTrainingAimLearningDelivery(learningDelivery)).Returns(true);

            NewRule(dd29: dd29Mock.Object).DD29Condition(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void DD37Condition_True()
        {
            var learningDelivery = new TestLearningDelivery();

            var dd37Mock = new Mock<IDerivedData_37Rule>();
            dd37Mock.Setup(dd => dd.Derive(35, new DateTime(2019, 8, 1), It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<IEnumerable<ILearningDeliveryFAM>>())).Returns(true);

            NewRule(dd37: dd37Mock.Object).DD37Condition(35, new DateTime(2019, 8, 1), It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<IEnumerable<ILearningDeliveryFAM>>()).Should().BeTrue();
        }

        [Fact]
        public void DD37Condition_False()
        {
            var learningDelivery = new TestLearningDelivery();

            var dd37Mock = new Mock<IDerivedData_37Rule>();
            dd37Mock.Setup(dd => dd.Derive(35, new DateTime(2019, 8, 1), It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<IEnumerable<ILearningDeliveryFAM>>())).Returns(false);

            NewRule(dd37: dd37Mock.Object).DD37Condition(35, new DateTime(2019, 8, 1), It.IsAny<IEnumerable<ILearnerEmploymentStatus>>(), It.IsAny<IEnumerable<ILearningDeliveryFAM>>()).Should().BeFalse();
        }

        [Fact]
        public void DD38Condition_True()
        {
            var learningDelivery = new TestLearningDelivery();

            var dd38Mock = new Mock<IDerivedData_38Rule>();
            dd38Mock.Setup(dd => dd.Derive(35, new DateTime(2019, 8, 1), It.IsAny<IEnumerable<ILearnerEmploymentStatus>>())).Returns(true);

            NewRule(dd38: dd38Mock.Object).DD38Condition(35, new DateTime(2019, 8, 1), It.IsAny<IEnumerable<ILearnerEmploymentStatus>>()).Should().BeTrue();
        }

        [Fact]
        public void DD38Condition_False()
        {
            var learningDelivery = new TestLearningDelivery();

            var dd38Mock = new Mock<IDerivedData_38Rule>();
            dd38Mock.Setup(dd => dd.Derive(35, new DateTime(2019, 8, 1), It.IsAny<IEnumerable<ILearnerEmploymentStatus>>())).Returns(false);

            NewRule(dd38: dd38Mock.Object).DD38Condition(35, new DateTime(2019, 8, 1), It.IsAny<IEnumerable<ILearnerEmploymentStatus>>()).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMExclusion_False()
        {
            var testLearningDeliveryFAMs = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, "RES")).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasAnyLearningDeliveryFAMCodesForType(testLearningDeliveryFAMs, "LDM", _ldmExclusions)).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "DAM", "023")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMExclusion(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMExclusion_False_NullFams()
        {
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMType(null, "RES")).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasAnyLearningDeliveryFAMCodesForType(null, "LDM", _ldmExclusions)).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(null, "DAM", "023")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMExclusion(null).Should().BeFalse();
        }

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        [InlineData(false, true, true)]
        public void LearningDeliveryFAMExclusion_True(bool mockOne, bool mockTwo, bool mockThree)
        {
            var testLearningDeliveryFAMs = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, "RES")).Returns(mockOne);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasAnyLearningDeliveryFAMCodesForType(testLearningDeliveryFAMs, "LDM", _ldmExclusions)).Returns(mockTwo);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "DAM", "023")).Returns(mockThree);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMExclusion(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LARSExclusionCondition_True()
        {
            var learnStartDate = new DateTime(2020, 8, 1);
            var larsLearningDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "AimRef",
                EffectiveFrom = new DateTime(2020, 8, 1)
            };

            var annualValues = new List<ILARSAnnualValue>
            {
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 20
                },
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 105
                }
            };

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetAnnualValuesFor(larsLearningDelivery.LearnAimRef)).Returns(annualValues);

            var dateTimeQSMock = new Mock<IDateTimeQueryService>();
            dateTimeQSMock.Setup(x => x.IsDateBetween(learnStartDate, larsLearningDelivery.EffectiveFrom, larsLearningDelivery.EffectiveTo ?? DateTime.MaxValue, true)).Returns(true);

            NewRule(larsDataService: larsMock.Object, dateTimeQueryService: dateTimeQSMock.Object).LARSExclusionCondition(larsLearningDelivery, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void LARSExclusionCondition_False_NullAnnualValue()
        {
            var learnStartDate = new DateTime(2020, 8, 1);
            var larsLearningDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "AimRef",
                EffectiveFrom = new DateTime(2020, 8, 1)
            };

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetAnnualValuesFor(larsLearningDelivery.LearnAimRef)).Returns(Array.Empty<ILARSAnnualValue>());

            var dateTimeQSMock = new Mock<IDateTimeQueryService>();
            dateTimeQSMock.Setup(x => x.IsDateBetween(learnStartDate, larsLearningDelivery.EffectiveFrom, larsLearningDelivery.EffectiveTo ?? DateTime.MaxValue, true)).Returns(true);

            NewRule(larsDataService: larsMock.Object, dateTimeQueryService: dateTimeQSMock.Object).LARSExclusionCondition(larsLearningDelivery, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void LARSExclusionCondition_False_AnnualValueMismatch()
        {
            var learnStartDate = new DateTime(2020, 8, 1);
            var larsLearningDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "AimRef",
                EffectiveFrom = new DateTime(2020, 8, 1)
            };

            var annualValues = new List<ILARSAnnualValue>
            {
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 105
                }
            };

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetAnnualValuesFor(larsLearningDelivery.LearnAimRef)).Returns(annualValues);

            var dateTimeQSMock = new Mock<IDateTimeQueryService>();
            dateTimeQSMock.Setup(x => x.IsDateBetween(learnStartDate, larsLearningDelivery.EffectiveFrom, larsLearningDelivery.EffectiveTo ?? DateTime.MaxValue, true)).Returns(true);

            NewRule(larsDataService: larsMock.Object, dateTimeQueryService: dateTimeQSMock.Object).LARSExclusionCondition(larsLearningDelivery, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void LARSExclusionCondition_False_StartDateMisMatch()
        {
            var learnStartDate = new DateTime(2020, 8, 1);
            var larsLearningDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "AimRef",
                EffectiveFrom = new DateTime(2020, 9, 1)
            };

            var annualValues = new List<ILARSAnnualValue>
            {
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 20
                },
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 105
                }
            };

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetAnnualValuesFor(larsLearningDelivery.LearnAimRef)).Returns(annualValues);

            var dateTimeQSMock = new Mock<IDateTimeQueryService>();
            dateTimeQSMock.Setup(x => x.IsDateBetween(learnStartDate, larsLearningDelivery.EffectiveFrom, larsLearningDelivery.EffectiveTo ?? DateTime.MaxValue, true)).Returns(false);

            NewRule(larsDataService: larsMock.Object, dateTimeQueryService: dateTimeQSMock.Object).LARSExclusionCondition(larsLearningDelivery, learnStartDate).Should().BeFalse();
        }

        [Theory]
        [InlineData(true, false, false, false, false, false)]
        [InlineData(false, true, false, false, false, false)]
        [InlineData(false, false, true, false, false, false)]
        [InlineData(false, false, false, true, false, false)]
        [InlineData(false, false, false, false, true, false)]
        [InlineData(false, false, false, false, false, true)]
        [InlineData(false, false, false, true, false, true)]
        [InlineData(false, true, false, false, false, true)]
        [InlineData(false, true, false, true, false, true)]
        [InlineData(false, true, true, true, false, true)]
        [InlineData(true, true, false, false, false, true)]
        [InlineData(false, true, false, false, true, true)]
        [InlineData(true, true, true, true, true, true)]
        public void Excluded_True(bool dd07, bool dd29, bool dd37, bool dd38, bool deliveryFAMS, bool lars)
        {
            var learnStartDate = new DateTime(2020, 8, 1);
            var fundModel = 35;
            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = 24,
                FundModel = fundModel,
                LearnStartDate = learnStartDate
            };

            var employmentStatuses = It.IsAny<IEnumerable<ILearnerEmploymentStatus>>();
            var learningDeliveryFAMs = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                NotionalNVQLevelv2 = "2",
                EffectiveFrom = new DateTime(2020, 8, 1),
                Categories = new List<Data.External.LARS.Model.LearningDeliveryCategory>
                {
                    new Data.External.LARS.Model.LearningDeliveryCategory
                    {
                        LearnAimRef = "LearnAimRef",
                        CategoryRef = 35
                    }
                }
            };

            var annualValues = new List<ILARSAnnualValue>
            {
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 20
                },
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 105
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(learningDelivery.ProgTypeNullable)).Returns(dd07);

            var dd29Mock = new Mock<IDerivedData_29Rule>();
            dd29Mock.Setup(dd => dd.IsInflexibleElementOfTrainingAimLearningDelivery(learningDelivery)).Returns(dd29);

            var dd37Mock = new Mock<IDerivedData_37Rule>();
            dd37Mock.Setup(dd => dd.Derive(fundModel, learnStartDate, employmentStatuses, learningDeliveryFAMs)).Returns(dd37);

            var dd38Mock = new Mock<IDerivedData_38Rule>();
            dd38Mock.Setup(dd => dd.Derive(fundModel, learnStartDate, employmentStatuses)).Returns(dd38);

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(deliveryFAMS);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", _ldmExclusions)).Returns(deliveryFAMS);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "DAM", "023")).Returns(deliveryFAMS);

            var dateTimeQSMock = new Mock<IDateTimeQueryService>();
            dateTimeQSMock.Setup(x => x.IsDateBetween(learnStartDate, larsDelivery.EffectiveFrom, larsDelivery.EffectiveTo ?? DateTime.MaxValue, true)).Returns(lars);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetAnnualValuesFor(larsDelivery.LearnAimRef)).Returns(annualValues);

            NewRule(
                dd07: dd07Mock.Object,
                dd29: dd29Mock.Object,
                dd37: dd37Mock.Object,
                dd38: dd38Mock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                larsDataService: larsMock.Object,
                dateTimeQueryService: dateTimeQSMock.Object)
                .Excluded(learningDelivery, employmentStatuses, larsDelivery).Should().BeTrue();
        }

        public void Excluded_False()
        {
            var learnStartDate = new DateTime(2020, 8, 1);
            var fundModel = 35;
            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = 24,
                FundModel = fundModel,
                LearnStartDate = learnStartDate
            };

            var employmentStatuses = It.IsAny<IEnumerable<ILearnerEmploymentStatus>>();
            var learningDeliveryFAMs = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                NotionalNVQLevelv2 = "2",
                EffectiveFrom = new DateTime(2020, 8, 1),
                Categories = new List<Data.External.LARS.Model.LearningDeliveryCategory>
                {
                    new Data.External.LARS.Model.LearningDeliveryCategory
                    {
                        LearnAimRef = "LearnAimRef",
                        CategoryRef = 35
                    }
                }
            };

            var annualValues = new List<ILARSAnnualValue>
            {
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 20
                },
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 105
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(learningDelivery.ProgTypeNullable)).Returns(false);

            var dd29Mock = new Mock<IDerivedData_29Rule>();
            dd29Mock.Setup(dd => dd.IsInflexibleElementOfTrainingAimLearningDelivery(learningDelivery)).Returns(false);

            var dd37Mock = new Mock<IDerivedData_37Rule>();
            dd37Mock.Setup(dd => dd.Derive(fundModel, learnStartDate, employmentStatuses, learningDeliveryFAMs)).Returns(false);

            var dd38Mock = new Mock<IDerivedData_38Rule>();
            dd38Mock.Setup(dd => dd.Derive(fundModel, learnStartDate, employmentStatuses)).Returns(false);

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", _ldmExclusions)).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "DAM", "023")).Returns(false);

            var dateTimeQSMock = new Mock<IDateTimeQueryService>();
            dateTimeQSMock.Setup(x => x.IsDateBetween(learnStartDate, larsDelivery.EffectiveFrom, larsDelivery.EffectiveTo ?? DateTime.MaxValue, true)).Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetAnnualValuesFor(larsDelivery.LearnAimRef)).Returns(annualValues);

            NewRule(
                dd07: dd07Mock.Object,
                dd29: dd29Mock.Object,
                dd37: dd37Mock.Object,
                dd38: dd38Mock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                larsDataService: larsMock.Object,
                dateTimeQueryService: dateTimeQSMock.Object)
                .Excluded(learningDelivery, employmentStatuses, larsDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullLars()
        {
            var learnAimRef = "LearnAimRef";
            var dateOfBirth = new DateTime(2000, 8, 1);
            var learnStartDate = new DateTime(2020, 8, 1);
            var fundModel = 35;
            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = 24,
                FundModel = fundModel,
                LearnStartDate = learnStartDate
            };

            var employmentStatuses = It.IsAny<IEnumerable<ILearnerEmploymentStatus>>();
            var learningDeliveryFAMs = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = learnAimRef,
                NotionalNVQLevelv2 = "2",
                EffectiveFrom = new DateTime(2020, 8, 1),
                Categories = new List<Data.External.LARS.Model.LearningDeliveryCategory>
                {
                    new Data.External.LARS.Model.LearningDeliveryCategory
                    {
                        LearnAimRef = learnAimRef,
                        CategoryRef = 35
                    }
                }
            };

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetDeliveryFor(learnAimRef)).Returns((ILARSLearningDelivery)null);

            NewRule(larsDataService: larsMock.Object).ConditionMet(dateOfBirth, employmentStatuses, learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoError_NoDeliveries()
        {
            var learner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_Exclusion()
        {
            var dateOfBirth = new DateTime(2000, 8, 1);
            var learnStartDate = new DateTime(2020, 8, 1);
            var fundModel = 35;
            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = 24,
                FundModel = fundModel,
                LearnStartDate = learnStartDate
            };

            var employmentStatuses = It.IsAny<IEnumerable<ILearnerEmploymentStatus>>();
            var learningDeliveryFAMs = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery
                }
            };

            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                NotionalNVQLevelv2 = "2",
                EffectiveFrom = new DateTime(2020, 8, 1),
                Categories = new List<Data.External.LARS.Model.LearningDeliveryCategory>
                {
                    new Data.External.LARS.Model.LearningDeliveryCategory
                    {
                        LearnAimRef = "LearnAimRef",
                        CategoryRef = 35
                    }
                }
            };

            var annualValues = new List<ILARSAnnualValue>
            {
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 20
                },
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 105
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(x => x.UKPRN()).Returns(1);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(learningDelivery.ProgTypeNullable)).Returns(false);

            var dd29Mock = new Mock<IDerivedData_29Rule>();
            dd29Mock.Setup(dd => dd.IsInflexibleElementOfTrainingAimLearningDelivery(learningDelivery)).Returns(false);

            var dd37Mock = new Mock<IDerivedData_37Rule>();
            dd37Mock.Setup(dd => dd.Derive(fundModel, learnStartDate, employmentStatuses, learningDeliveryFAMs)).Returns(false);

            var dd38Mock = new Mock<IDerivedData_38Rule>();
            dd38Mock.Setup(dd => dd.Derive(fundModel, learnStartDate, employmentStatuses)).Returns(false);

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", _ldmExclusions)).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "DAM", "023")).Returns(true);

            var dateTimeQSMock = new Mock<IDateTimeQueryService>();
            dateTimeQSMock.Setup(x => x.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQSMock.Setup(x => x.IsDateBetween(learnStartDate, larsDelivery.EffectiveFrom, larsDelivery.EffectiveTo ?? DateTime.MaxValue, true)).Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetAnnualValuesFor(larsDelivery.LearnAimRef)).Returns(annualValues);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandlerMock.Object,
                    fileDataServiceMock.Object,
                    dateTimeQSMock.Object,
                    larsMock.Object,
                    learningDeliveryFAMsQueryServiceMock.Object,
                    dd07Mock.Object,
                    dd29Mock.Object,
                    dd37Mock.Object,
                    dd38Mock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(2000, 8, 1);
            var learnStartDate = new DateTime(2020, 8, 1);
            var fundModel = 35;
            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = 24,
                FundModel = fundModel,
                LearnStartDate = learnStartDate
            };

            var employmentStatuses = It.IsAny<IEnumerable<ILearnerEmploymentStatus>>();
            var learningDeliveryFAMs = It.IsAny<IEnumerable<ILearningDeliveryFAM>>();

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery
                }
            };

            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                NotionalNVQLevelv2 = "2",
                EffectiveFrom = new DateTime(2020, 8, 1),
                Categories = new List<Data.External.LARS.Model.LearningDeliveryCategory>
                {
                    new Data.External.LARS.Model.LearningDeliveryCategory
                    {
                        LearnAimRef = "LearnAimRef",
                        CategoryRef = 35
                    }
                }
            };

            var annualValues = new List<ILARSAnnualValue>
            {
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 20
                },
                new Data.External.LARS.Model.AnnualValue
                {
                    LearnAimRef = "AimRef",
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    BasicSkillsType = 105
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(x => x.UKPRN()).Returns(1);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(learningDelivery.ProgTypeNullable)).Returns(false);

            var dd29Mock = new Mock<IDerivedData_29Rule>();
            dd29Mock.Setup(dd => dd.IsInflexibleElementOfTrainingAimLearningDelivery(learningDelivery)).Returns(false);

            var dd37Mock = new Mock<IDerivedData_37Rule>();
            dd37Mock.Setup(dd => dd.Derive(fundModel, learnStartDate, employmentStatuses, learningDeliveryFAMs)).Returns(false);

            var dd38Mock = new Mock<IDerivedData_38Rule>();
            dd38Mock.Setup(dd => dd.Derive(fundModel, learnStartDate, employmentStatuses)).Returns(false);

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", _ldmExclusions)).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "DAM", "023")).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "FFI", "1")).Returns(true);

            var dateTimeQSMock = new Mock<IDateTimeQueryService>();
            dateTimeQSMock.Setup(x => x.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQSMock.Setup(x => x.IsDateBetween(learnStartDate, larsDelivery.EffectiveFrom, larsDelivery.EffectiveTo ?? DateTime.MaxValue, true)).Returns(true);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetAnnualValuesFor(larsDelivery.LearnAimRef)).Returns(annualValues);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandlerMock.Object,
                    fileDataServiceMock.Object,
                    dateTimeQSMock.Object,
                    larsMock.Object,
                    learningDeliveryFAMsQueryServiceMock.Object,
                    dd07Mock.Object,
                    dd29Mock.Object,
                    dd37Mock.Object,
                    dd38Mock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.UKPRN, 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, "01/01/2000")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, "LearnAimRef")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2000")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 35)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProgType, 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.FFI)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, new DateTime(2000, 01, 01), "LearnAimRef", new DateTime(2000, 01, 01), 35, 1);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_79Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFileDataService fileDataService = null,
            IDateTimeQueryService dateTimeQueryService = null,
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IDerivedData_07Rule dd07 = null,
            IDerivedData_29Rule dd29 = null,
            IDerivedData_37Rule dd37 = null,
            IDerivedData_38Rule dd38 = null)
        {
            return new LearnDelFAMType_79Rule(
                validationErrorHandler,
                fileDataService,
                dateTimeQueryService,
                larsDataService,
                learningDeliveryFAMQueryService,
                dd07,
                dd29,
                dd37,
                dd38);
        }
    }
}
