using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_58RuleTests : AbstractRuleTests<LearnDelFAMType_58Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_58");
        }

        [Fact]
        public void RuleName_CheckWithConstantValue()
        {
            NewRule().RuleName.Should().Be(RuleNameConstants.LearnDelFAMType_58);
        }

        [Theory]
        [InlineData("2016-08-01", false)]
        [InlineData("2016-07-31", true)]
        [InlineData("2016-08-02", false)]
        public void LearnStartDateConditionMet(string learningStarted, bool result)
        {
            NewRule().LearnStartDateConditionMet(DateTime.Parse(learningStarted)).Should().Be(result);
        }

        [Theory]
        [InlineData(35, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        public void FundModelConditionMet(int fundModel, bool result)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(result);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(25, false)]
        public void FundModelConditionMet_CheckWithConstantValue(int fundModel, bool result)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(result);
        }

        [Theory]
        [InlineData("2017-07-31", "1994-07-01", 23, false)]
        [InlineData("2017-07-31", "2000-07-01", 16, false)]
        [InlineData("2018-12-31", "1993-01-01", 25, true)]
        [InlineData("2018-12-31", "1994-12-31", 24, true)]
        public void AgeLimitConditionMet(string learningStarted, string dateOfBirth, int age, bool result)
        {
            DateTime learnStartDate = DateTime.Parse(learningStarted);
            DateTime birthDate = DateTime.Parse(dateOfBirth);

            var dateTimeQuerySericeMock = new Mock<IDateTimeQueryService>();
            dateTimeQuerySericeMock.Setup(x => x.YearsBetween(learnStartDate, birthDate)).Returns(age);

            NewRule(datetimeQueryService: dateTimeQuerySericeMock.Object)
                .AgeLimitConditionMet(learnStartDate, birthDate)
                .Should().Be(result);
        }

        [Theory]
        [InlineData("FFI", "1", "LDM", "346", true)]
        [InlineData("ADL", "5", "SPP", "107", false)]
        [InlineData("ADL", "1", "SPP", "346", false)]
        [InlineData("ADL", "1", "LDM", "107", false)]
        [InlineData("FFI", "1", "LDM", "346", false)]
        public void LearnDelFAMsConditionMet(string learnDelFAMType1, string learnDelFAMCode1, string learnDelFAMType2, string learnDelFAMCode2, bool result)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType1,
                    LearnDelFAMCode = learnDelFAMCode1
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType2,
                    LearnDelFAMCode = learnDelFAMCode2
                }
            };

            var learnDelFAMsMock = new Mock<ILearningDeliveryFAMQueryService>();
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType1, learnDelFAMCode1)).Returns(result);
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType2, learnDelFAMCode2)).Returns(result);

            NewRule(learningDeliveryFAMQueryService: learnDelFAMsMock.Object).LearnDelFAMsConditionMet(testLearningDeliveryFAMs).Should().Be(result);
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.FFI, LearningDeliveryFAMCodeConstants.FFI_Fully, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_Military, true)]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL, LearningDeliveryFAMCodeConstants.ADL_Code, LearningDeliveryFAMTypeConstants.SPP, LearningDeliveryFAMCodeConstants.NSA_SportAndActiveLeisure, false)]
        public void LearnDelFAMsConditionMet_CheckWithConstantValue(string learnDelFAMType1, string learnDelFAMCode1, string learnDelFAMType2, string learnDelFAMCode2, bool result)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType1,
                    LearnDelFAMCode = learnDelFAMCode1
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType2,
                    LearnDelFAMCode = learnDelFAMCode2
                }
            };

            var learnDelFAMsMock = new Mock<ILearningDeliveryFAMQueryService>();
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType1, learnDelFAMCode1)).Returns(result);
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType2, learnDelFAMCode2)).Returns(result);

            NewRule(learningDeliveryFAMQueryService: learnDelFAMsMock.Object).LearnDelFAMsConditionMet(testLearningDeliveryFAMs).Should().Be(result);
        }

        [Theory]
        [InlineData("10001234", "0", false)]
        [InlineData("10001234", "1", false)]
        [InlineData("10001234", "2", true)]
        [InlineData("10001234", "3", false)]
        [InlineData("10001234", "4", false)]
        [InlineData("10001234", "5", false)]
        public void LARSConditionMet(string learnAimRef, string notionalNVQLeveV2, bool result)
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, notionalNVQLeveV2)).Returns(result);

            NewRule(lARSDataService: larsDataServiceMock.Object).LARSConditionMet(learnAimRef).Should().Be(result);
        }

        [Theory]
        [InlineData("10001456", "2015-08-01", 2, 100.00, true, true, false, true)]
        [InlineData("10001456", "2015-08-01", 2, 100.00, true, true, true, true)]
        [InlineData("10001456", "2018-08-01", 2, 100.00, false, true, true, false)]
        [InlineData("10001456", "2018-08-01", 2, 100.00, true, false, true, false)]
        [InlineData("10001456", "2018-08-01", 2, 100.00, false, false, true, false)]
        [InlineData("10001456", "2018-08-01", 2, 80.00, false, false, false, false)]
        [InlineData("10001456", "2018-08-01", 6, 80.00, false, false, false, false)]
        public void LARSPercentageLevelConditionMet(string learnAimRef, string effectiveDate, int priorAttain, decimal fullLevel2Percent, bool validityResult, bool level2PercentResult, bool percentNotMatchedResult, bool ruleResult)
        {
            DateTime effectiveFrom = DateTime.Parse(effectiveDate);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(d => d.EffectiveDatesValidforLearnAimRef(learnAimRef, effectiveFrom)).Returns(validityResult);
            larsDataServiceMock.Setup(d => d.FullLevel2PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, effectiveFrom, fullLevel2Percent)).Returns(level2PercentResult);
            larsDataServiceMock.Setup(d => d.FullLevel2PercentForLearnAimRefNotMatchPercentValue(learnAimRef, effectiveFrom, fullLevel2Percent)).Returns(percentNotMatchedResult);

            NewRule(lARSDataService: larsDataServiceMock.Object)
                .LARSPercentageLevelConditionMet(learnAimRef, priorAttain)
                .Should().Be(ruleResult);
        }

        [Theory]
        [InlineData("1000456", TypeOfLARSCategory.TradeUnionAims, true, false)]
        [InlineData("1000456", TypeOfLARSCategory.WorkPreparationSFATraineeships, false, true)]
        [InlineData("1000456", TypeOfLARSCategory.WorkPlacementSFAFunded, false, true)]
        [InlineData("1000456", TypeOfLARSCategory.LegalEntitlementLevel2, false, true)]
        [InlineData("1000456", TypeOfLARSCategory.OnlyForLegalEntitlementAtLevel3, false, true)]
        [InlineData("1000456", TypeOfLARSCategory.LicenseToPractice, false, true)]
        public void LARSConditionExcluded(string learnAimRef, int categoryRef, bool result, bool ruleresult)
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(l => l.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, categoryRef)).Returns(result);

            NewRule(lARSDataService: larsDataServiceMock.Object).LARSConditionExcluded(learnAimRef).Should().Be(ruleresult);
        }

        [Theory]
        [InlineData(2, true, false)]
        [InlineData(3, true, false)]
        [InlineData(20, true, false)]
        [InlineData(21, true, false)]
        [InlineData(22, true, false)]
        [InlineData(23, true, false)]
        [InlineData(25, true, false)]
        [InlineData(24, false, true)]
        public void DerivedData07ConditionExcluded(int? progType, bool result, bool ruleResult)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(result);

            NewRule(derivedData_07Rule: dd07Mock.Object)
                .DerivedData07ConditionExcluded(progType).Should().Be(ruleResult);
        }

        [Theory]
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship, true, false)]
        [InlineData(TypeOfLearningProgramme.IntermediateLevelApprenticeship, true, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4, true, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5, true, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel6, true, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, true, false)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, true, false)]
        [InlineData(TypeOfLearningProgramme.Traineeship, false, true)]
        public void DerivedData07ConditionExcluded_CheckWithConstantValue(int? progType, bool result, bool ruleResult)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(result);

            NewRule(derivedData_07Rule: dd07Mock.Object)
                .DerivedData07ConditionExcluded(progType).Should().Be(ruleResult);
        }

        [Theory]
        [InlineData("LDM", "034", "RES", true, true, false)]
        [InlineData("LDM", "5", "RES", false, true, false)]
        [InlineData("ADL", "1", "SPP", false, false, true)]
        [InlineData("ADL", "1", "RES", false, true, false)]
        [InlineData("LDM", "034", "SPP", true, false, false)]
        public void LearnDelFAMsConditionExcluded(string learnDelFAMType1, string learnDelFAMCode1, string learnDelFAMType2, bool result1, bool result2, bool ruleResult)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType1,
                    LearnDelFAMCode = learnDelFAMCode1
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType2
                }
            };

            var learnDelFAMsMock = new Mock<ILearningDeliveryFAMQueryService>();
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType1, learnDelFAMCode1)).Returns(result1);
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, learnDelFAMType2)).Returns(result2);

            NewRule(learningDeliveryFAMQueryService: learnDelFAMsMock.Object).LearnDelFAMsConditionExcluded(testLearningDeliveryFAMs).Should().Be(ruleResult);
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS, LearningDeliveryFAMTypeConstants.RES, true, true, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL, LearningDeliveryFAMCodeConstants.ADL_Code, LearningDeliveryFAMTypeConstants.SPP, false, false, true)]
        public void LearnDelFAMsConditionExcluded_CheckWithConstantValue(string learnDelFAMType1, string learnDelFAMCode1, string learnDelFAMType2, bool result1, bool result2, bool ruleResult)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType1,
                    LearnDelFAMCode = learnDelFAMCode1
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType2
                }
            };

            var learnDelFAMsMock = new Mock<ILearningDeliveryFAMQueryService>();
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType1, learnDelFAMCode1)).Returns(result1);
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, learnDelFAMType2)).Returns(result2);

            NewRule(learningDeliveryFAMQueryService: learnDelFAMsMock.Object).LearnDelFAMsConditionExcluded(testLearningDeliveryFAMs).Should().Be(ruleResult);
        }

        [Theory]
        [InlineData(35, "2018-05-01", "2017-07-31", "BSI", 1, true, false)]
        [InlineData(35, "2018-05-01", "2017-07-31", "BSI", 2, true, false)]
        [InlineData(35, "2018-05-01", "2017-07-31", "BSI", 4, true, false)]
        [InlineData(35, "2018-05-01", "2018-07-31", "PEI", 4, false, true)]
        [InlineData(30, "2018-05-01", "2018-07-31", "BSI", 5, false, true)]
        [InlineData(10, "2018-05-01", "2018-07-31", "BSI", 2, false, true)]
        [InlineData(10, "2018-05-01", "2018-07-31", "PEI", 7, false, true)]
        public void DerivedData12ConditionExcluded(int fundModel, string learningStarted, string employmentDate, string esmType, int esmCode, bool result, bool ruleResult)
        {
            DateTime learnStartDate = DateTime.Parse(learningStarted);

            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = DateTime.Parse(employmentDate),
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMType = esmType,
                            ESMCode = esmCode
                        }
                    }
                }
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "LDM",
                        LearnDelFAMCode = "107"
                    }
                }
            };

            var dd12Mock = new Mock<IDerivedData_12Rule>();
            dd12Mock.Setup(d => d.IsAdultSkillsFundedOnBenefits(learnerEmploymentStatuses, learningDelivery)).Returns(result);

            NewRule(derivedData_12Rule: dd12Mock.Object)
                .DerivedData12ConditionExcluded(learnerEmploymentStatuses, learningDelivery)
                .Should().Be(ruleResult);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, "2018-05-01", "2017-07-31", Monitoring.EmploymentStatus.Types.BenefitStatusIndicator, 1, true, false)]
        [InlineData(TypeOfFunding.AdultSkills, "2018-05-01", "2017-07-31", Monitoring.EmploymentStatus.Types.BenefitStatusIndicator, 2, true, false)]
        [InlineData(TypeOfFunding.AdultSkills, "2018-05-01", "2017-07-31", Monitoring.EmploymentStatus.Types.BenefitStatusIndicator, 4, true, false)]
        [InlineData(TypeOfFunding.AdultSkills, "2018-05-01", "2018-07-31", Monitoring.EmploymentStatus.Types.PreviousEducationIndicator, 4, false, true)]
        [InlineData(TypeOfFunding.AdultSkills, "2018-05-01", "2018-07-31", Monitoring.EmploymentStatus.Types.BenefitStatusIndicator, 5, false, true)]
        [InlineData(TypeOfFunding.CommunityLearning, "2018-05-01", "2018-07-31", Monitoring.EmploymentStatus.Types.BenefitStatusIndicator, 2, false, true)]
        [InlineData(TypeOfFunding.CommunityLearning, "2018-05-01", "2018-07-31", Monitoring.EmploymentStatus.Types.PreviousEducationIndicator, 7, false, true)]
        public void DerivedData12ConditionExcluded_CheckWithConstantValue(int fundModel, string learningStarted, string employmentDate, string esmType, int esmCode, bool result, bool ruleResult)
        {
            DateTime learnStartDate = DateTime.Parse(learningStarted);

            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = DateTime.Parse(employmentDate),
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMType = esmType,
                            ESMCode = esmCode
                        }
                    }
                }
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "LDM",
                        LearnDelFAMCode = "107"
                    }
                }
            };

            var dd12Mock = new Mock<IDerivedData_12Rule>();
            dd12Mock.Setup(d => d.IsAdultSkillsFundedOnBenefits(learnerEmploymentStatuses, learningDelivery)).Returns(result);

            NewRule(derivedData_12Rule: dd12Mock.Object)
                .DerivedData12ConditionExcluded(learnerEmploymentStatuses, learningDelivery)
                .Should().Be(ruleResult);
        }

        [Theory]
        [InlineData(35, "2018-05-01", "2017-07-31", 11, "BSI", 3, "LDM", "107", true, false)]
        [InlineData(35, "2018-05-01", "2017-07-31", 12, "BSI", 3, "LDM", "107", true, false)]
        [InlineData(35, "2018-05-01", "2017-07-31", 11, "BSI", 4, "LDM", "107", true, false)]
        [InlineData(35, "2018-05-01", "2018-07-31", 13, "PEI", 4, "LDM", "318", false, true)]
        [InlineData(35, "2018-05-01", "2018-07-31", 14, "BSI", 5, "ADL", "318", false, true)]
        [InlineData(10, "2018-05-01", "2018-07-31", 15, "BSI", 2, "SPP", "318", false, true)]
        [InlineData(10, "2018-05-01", "2018-07-31", 16, "PEI", 7, "LDM", "318", false, true)]
        public void DerivedData21ConditionExcluded(int fundModel, string learningStarted, string employmentDate, int empStat, string esmType, int esmCode, string learnDelFAMType, string learnDelFAMCode, bool result, bool ruleResult)
        {
            DateTime learnStartDate = DateTime.Parse(learningStarted);

            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = DateTime.Parse(employmentDate),
                    EmpStat = empStat,
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMType = esmType,
                            ESMCode = esmCode
                        }
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMType,
                        LearnDelFAMCode = learnDelFAMCode
                    }
                }
            };

            var dd21Mock = new Mock<IDerivedData_21Rule>();
            dd21Mock.Setup(d => d.IsAdultFundedUnemployedWithOtherStateBenefits(learningDelivery, learner)).Returns(result);

            NewRule(derivedData_21Rule: dd21Mock.Object)
                .DerivedData21ConditionExcluded(learner, learningDelivery)
                .Should().Be(ruleResult);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, "2018-05-01", "2017-07-31", 11, Monitoring.EmploymentStatus.Types.BenefitStatusIndicator, 3, "LDM", "107", true, false)]
        [InlineData(TypeOfFunding.AdultSkills, "2018-05-01", "2017-07-31", 12, Monitoring.EmploymentStatus.Types.BenefitStatusIndicator, 3, "LDM", "107", true, false)]
        [InlineData(TypeOfFunding.AdultSkills, "2018-05-01", "2017-07-31", 11, Monitoring.EmploymentStatus.Types.BenefitStatusIndicator, 4, "LDM", "107", true, false)]
        [InlineData(TypeOfFunding.AdultSkills, "2018-05-01", "2018-07-31", 13, Monitoring.EmploymentStatus.Types.PreviousEducationIndicator, 4, "LDM", "318", false, true)]
        [InlineData(TypeOfFunding.AdultSkills, "2018-05-01", "2018-07-31", 14, Monitoring.EmploymentStatus.Types.BenefitStatusIndicator, 5, "ADL", "318", false, true)]
        [InlineData(TypeOfFunding.CommunityLearning, "2018-05-01", "2018-07-31", 15, Monitoring.EmploymentStatus.Types.BenefitStatusIndicator, 2, "SPP", "318", false, true)]
        [InlineData(TypeOfFunding.CommunityLearning, "2018-05-01", "2018-07-31", 16, Monitoring.EmploymentStatus.Types.PreviousEducationIndicator, 7, "LDM", "318", false, true)]
        public void DerivedData21ConditionExcluded_CheckWithConstantValues(int fundModel, string learningStarted, string employmentDate, int empStat, string esmType, int esmCode, string learnDelFAMType, string learnDelFAMCode, bool result, bool ruleResult)
        {
            DateTime learnStartDate = DateTime.Parse(learningStarted);

            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = DateTime.Parse(employmentDate),
                    EmpStat = empStat,
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMType = esmType,
                            ESMCode = esmCode
                        }
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMType,
                        LearnDelFAMCode = learnDelFAMCode
                    }
                }
            };

            var dd21Mock = new Mock<IDerivedData_21Rule>();
            dd21Mock.Setup(d => d.IsAdultFundedUnemployedWithOtherStateBenefits(learningDelivery, learner)).Returns(result);

            NewRule(derivedData_21Rule: dd21Mock.Object)
                .DerivedData21ConditionExcluded(learner, learningDelivery)
                .Should().Be(ruleResult);
        }

        [Theory]
        [InlineData(1000123, "USDC", true, false)]
        [InlineData(1000123, "SSPS", false, true)]
        [InlineData(1000123, "LAFB", false, true)]
        [InlineData(1000123, "UFES", false, true)]
        [InlineData(1000123, "UGFE", false, true)]
        [InlineData(1000123, "UHEO", false, true)]
        [InlineData(1000123, "ULAD", false, true)]
        [InlineData(1000123, "ULEA", false, true)]
        [InlineData(1000123, "USAH", false, true)]
        [InlineData(1000123, "USCL", false, true)]
        [InlineData(1000123, "USDC", false, true)]
        [InlineData(1000123, "USFC", false, true)]
        public void OrgDetailsConditionExcluded(int ukprn, string legalOrgType, bool result, bool ruleResult)
        {
            var fileDataServiceMock = new Mock<IFileDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            fileDataServiceMock.Setup(f => f.UKPRN()).Returns(ukprn);
            organisationDataServiceMock.Setup(o => o.LegalOrgTypeMatchForUkprn(ukprn, legalOrgType)).Returns(result);

            NewRule(
                fileDataService: fileDataServiceMock.Object,
                organisationDataService: organisationDataServiceMock.Object)
                .OrgDetailsConditionExcluded().Should().Be(ruleResult);
        }

        [Theory]
        [InlineData(35, 24, "1992-04-30", "2016-05-01", "2017-07-31", 11, "PEI", 7, "FFI", "1", "LDM", "346")]
        [InlineData(35, 25, "1992-04-30", "2016-05-01", "2017-07-31", 11, "PEI", 7, "FFI", "1", "LDM", "346")]
        public void ConditionMet_True(
            int fundModel,
            int progType,
            string dateOfBirth,
            string learningStarted,
            string employmentDate,
            int empStat,
            string esmType,
            int esmCode,
            string learnDelFAMType1,
            string learnDelFAMCode1,
            string learnDelFAMType2,
            string learnDelFAMCode2)
        {
            DateTime learnStartDate = DateTime.Parse(learningStarted);
            DateTime birthDate = DateTime.Parse(dateOfBirth);
            DateTime effectiveFromDate = new DateTime(2015, 05, 01);
            string learnAimRef = "10001234";

            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = DateTime.Parse(employmentDate),
                    EmpStat = empStat,
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMType = esmType,
                            ESMCode = esmCode
                        }
                    }
                }
            };

            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType1,
                    LearnDelFAMCode = learnDelFAMCode1
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType2,
                    LearnDelFAMCode = learnDelFAMCode2
                }
            };

            var learner = new TestLearner()
            {
                DateOfBirthNullable = birthDate,
                PriorAttainNullable = 2,
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearnAimRef = learnAimRef,
                ProgTypeNullable = progType,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = testLearningDeliveryFAMs
            };

            var dateTimeQuerySericeMock = new Mock<IDateTimeQueryService>();
            var learnDelFAMsMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dd12Mock = new Mock<IDerivedData_12Rule>();
            var dd21Mock = new Mock<IDerivedData_21Rule>();
            var fileDataServiceMock = new Mock<IFileDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            dateTimeQuerySericeMock.Setup(x => x.YearsBetween(learnStartDate, birthDate)).Returns(24);

            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType1, learnDelFAMCode1)).Returns(true);
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType2, learnDelFAMCode2)).Returns(true);

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(false);
            dd12Mock.Setup(d => d.IsAdultSkillsFundedOnBenefits(learnerEmploymentStatuses, learningDelivery)).Returns(false);
            dd21Mock.Setup(d => d.IsAdultFundedUnemployedWithOtherStateBenefits(learningDelivery, learner)).Returns(false);
            fileDataServiceMock.Setup(f => f.UKPRN()).Returns(1000123);
            organisationDataServiceMock.Setup(o => o.LegalOrgTypeMatchForUkprn(1000123, "SSPS")).Returns(false);
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, "2")).Returns(true);
            larsDataServiceMock.Setup(l => l.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, 2)).Returns(false);
            larsDataServiceMock.Setup(d => d.EffectiveDatesValidforLearnAimRef(learnAimRef, effectiveFromDate)).Returns(true);
            larsDataServiceMock.Setup(d => d.FullLevel2PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, effectiveFromDate, 100.00M)).Returns(true);
            larsDataServiceMock.Setup(d => d.FullLevel2PercentForLearnAimRefNotMatchPercentValue(learnAimRef, effectiveFromDate, 80.00M)).Returns(false);

            NewRule(
                datetimeQueryService: dateTimeQuerySericeMock.Object,
                learningDeliveryFAMQueryService: learnDelFAMsMock.Object,
                derivedData_07Rule: dd07Mock.Object,
                derivedData_12Rule: dd12Mock.Object,
                derivedData_21Rule: dd21Mock.Object,
                fileDataService: fileDataServiceMock.Object,
                organisationDataService: organisationDataServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object)
                .ConditionMet(learner, learningDelivery)
                .Should().Be(true);
        }

        [Theory]
        [InlineData(35, 24, "1992-04-30", "2018-05-01", "2017-07-31", 11, "PEI", 7, "FFI", "1", "LDM", "346")]
        [InlineData(25, 25, "1992-04-30", "2016-05-01", "2017-07-31", 11, "PEI", 7, "FFI", "1", "LDM", "346")]
        [InlineData(35, 25, "1998-04-30", "2016-05-01", "2017-07-31", 11, "PEI", 7, "FFI", "1", "LDM", "346")]
        [InlineData(35, 25, "1992-04-30", "2016-05-01", "2017-07-31", 11, "PEI", 7, "FFI", "2", "LDM", "346")]
        [InlineData(35, 25, "1992-04-30", "2016-05-01", "2017-07-31", 11, "PEI", 7, "ADL", "1", "LDM", "346")]
        [InlineData(35, 25, "1992-04-30", "2016-05-01", "2017-07-31", 11, "PEI", 7, "FFI", "1", "LDM", "034")]
        [InlineData(35, 25, "1992-04-30", "2016-05-01", "2017-07-31", 11, "PEI", 7, "FFI", "2", "ADL", "346")]
        [InlineData(35, 25, "1992-04-30", "2016-05-01", "2017-07-31", 13, "PEI", 7, "FFI", "2", "LDM", "346")]
        [InlineData(35, 2, "1992-04-30", "2016-05-01", "2017-07-31", 11, "PEI", 7, "FFI", "2", "LDM", "346")]
        [InlineData(35, 25, "1992-04-30", "2016-05-01", "2017-07-31", 11, "BSI", 3, "FFI", "2", "LDM", "346")]
        public void ConditionMet_False(
            int fundModel,
            int progType,
            string dateOfBirth,
            string learningStarted,
            string employmentDate,
            int empStat,
            string esmType,
            int esmCode,
            string learnDelFAMType1,
            string learnDelFAMCode1,
            string learnDelFAMType2,
            string learnDelFAMCode2)
        {
            DateTime learnStartDate = DateTime.Parse(learningStarted);
            DateTime birthDate = DateTime.Parse(dateOfBirth);
            DateTime effectiveFromDate = new DateTime(2015, 05, 01);
            string learnAimRef = "10001234";

            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = DateTime.Parse(employmentDate),
                    EmpStat = empStat,
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMType = esmType,
                            ESMCode = esmCode
                        }
                    }
                }
            };

            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType1,
                    LearnDelFAMCode = learnDelFAMCode1
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType2,
                    LearnDelFAMCode = learnDelFAMCode2
                }
            };

            var learner = new TestLearner()
            {
                PriorAttainNullable = 6,
                DateOfBirthNullable = birthDate,
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearnAimRef = learnAimRef,
                ProgTypeNullable = progType,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = testLearningDeliveryFAMs
            };

            var dateTimeQuerySericeMock = new Mock<IDateTimeQueryService>();
            var learnDelFAMsMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dd12Mock = new Mock<IDerivedData_12Rule>();
            var dd21Mock = new Mock<IDerivedData_21Rule>();
            var fileDataServiceMock = new Mock<IFileDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            dateTimeQuerySericeMock.Setup(x => x.YearsBetween(learnStartDate, birthDate)).Returns(24);

            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType1, learnDelFAMCode1)).Returns(false);
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType2, learnDelFAMCode2)).Returns(false);

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);
            dd12Mock.Setup(d => d.IsAdultSkillsFundedOnBenefits(learnerEmploymentStatuses, learningDelivery)).Returns(true);
            dd21Mock.Setup(d => d.IsAdultFundedUnemployedWithOtherStateBenefits(learningDelivery, learner)).Returns(true);
            fileDataServiceMock.Setup(f => f.UKPRN()).Returns(1000123);
            organisationDataServiceMock.Setup(o => o.LegalOrgTypeMatchForUkprn(1000123, "SSPS")).Returns(true);
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, "1")).Returns(false);
            larsDataServiceMock.Setup(l => l.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, 19)).Returns(true);
            larsDataServiceMock.Setup(d => d.EffectiveDatesValidforLearnAimRef(learnAimRef, effectiveFromDate)).Returns(false);
            larsDataServiceMock.Setup(d => d.FullLevel2PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, effectiveFromDate, 100.00M)).Returns(false);
            larsDataServiceMock.Setup(d => d.FullLevel2PercentForLearnAimRefNotMatchPercentValue(learnAimRef, effectiveFromDate, 80.00M)).Returns(true);

            NewRule(
                datetimeQueryService: dateTimeQuerySericeMock.Object,
                learningDeliveryFAMQueryService: learnDelFAMsMock.Object,
                derivedData_07Rule: dd07Mock.Object,
                derivedData_12Rule: dd12Mock.Object,
                derivedData_21Rule: dd21Mock.Object,
                fileDataService: fileDataServiceMock.Object,
                organisationDataService: organisationDataServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object)
                .ConditionMet(learner, learningDelivery)
                .Should().Be(false);
        }

        [Fact]
        public void Validate_Error()
        {
            DateTime birthDate = new DateTime(1992, 04, 30);
            DateTime learnStartDate = new DateTime(2016, 05, 01);
            DateTime effectiveFromDate = new DateTime(2015, 05, 01);
            string learnAimRef = "10001234";
            string learnDelFAMType1 = "FFI";
            string learnDelFAMCode1 = "1";
            string learnDelFAMType2 = "LDM";
            string learnDelFAMCode2 = "346";
            int fundModel = 35;
            int progType = 24;

            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = new DateTime(2017, 07, 31),
                    EmpStat = 11,
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMType = "PEI",
                            ESMCode = 7
                        }
                    }
                }
            };

            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType1,
                    LearnDelFAMCode = learnDelFAMCode1
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType2,
                    LearnDelFAMCode = learnDelFAMCode2
                }
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearnAimRef = learnAimRef,
                ProgTypeNullable = progType,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = testLearningDeliveryFAMs
            };

            var learner = new TestLearner()
            {
                PriorAttainNullable = 2,
                DateOfBirthNullable = birthDate,
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    learningDelivery
                }
            };

            var dateTimeQuerySericeMock = new Mock<IDateTimeQueryService>();
            var learnDelFAMsMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dd12Mock = new Mock<IDerivedData_12Rule>();
            var dd21Mock = new Mock<IDerivedData_21Rule>();
            var fileDataServiceMock = new Mock<IFileDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            dateTimeQuerySericeMock.Setup(x => x.YearsBetween(learnStartDate, birthDate)).Returns(24);

            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType1, learnDelFAMCode1)).Returns(true);
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType2, learnDelFAMCode2)).Returns(true);

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(false);
            dd12Mock.Setup(d => d.IsAdultSkillsFundedOnBenefits(learnerEmploymentStatuses, learningDelivery)).Returns(false);
            dd21Mock.Setup(d => d.IsAdultFundedUnemployedWithOtherStateBenefits(learningDelivery, learner)).Returns(false);
            fileDataServiceMock.Setup(f => f.UKPRN()).Returns(1000123);
            organisationDataServiceMock.Setup(o => o.LegalOrgTypeMatchForUkprn(1000123, "SSPS")).Returns(false);
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, "2")).Returns(true);
            larsDataServiceMock.Setup(l => l.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, 19)).Returns(false);
            larsDataServiceMock.Setup(d => d.EffectiveDatesValidforLearnAimRef(learnAimRef, effectiveFromDate)).Returns(true);
            larsDataServiceMock.Setup(d => d.FullLevel2PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, effectiveFromDate, 100.00M)).Returns(true);
            larsDataServiceMock.Setup(d => d.FullLevel2PercentForLearnAimRefNotMatchPercentValue(learnAimRef, effectiveFromDate, 80.00M)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    datetimeQueryService: dateTimeQuerySericeMock.Object,
                    learningDeliveryFAMQueryService: learnDelFAMsMock.Object,
                    derivedData_07Rule: dd07Mock.Object,
                    derivedData_12Rule: dd12Mock.Object,
                    derivedData_21Rule: dd21Mock.Object,
                    lARSDataService: larsDataServiceMock.Object,
                    fileDataService: fileDataServiceMock.Object,
                    organisationDataService: organisationDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            DateTime birthDate = new DateTime(1998, 04, 30);
            DateTime learnStartDate = new DateTime(2018, 05, 01);
            DateTime effectiveFromDate = new DateTime(201, 05, 01);
            string learnAimRef = "10001234";
            string learnDelFAMType1 = "SPP";
            string learnDelFAMCode1 = "154";
            string learnDelFAMType2 = "ADL";
            string learnDelFAMCode2 = "034";
            int fundModel = 25;
            int progType = 2;

            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = new DateTime(2017, 07, 31),
                    EmpStat = 11,
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMType = "PEI",
                            ESMCode = 7
                        }
                    }
                }
            };

            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType1,
                    LearnDelFAMCode = learnDelFAMCode1
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType2,
                    LearnDelFAMCode = learnDelFAMCode2
                }
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearnAimRef = learnAimRef,
                ProgTypeNullable = progType,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = testLearningDeliveryFAMs
            };

            var learner = new TestLearner()
            {
                PriorAttainNullable = 6,
                DateOfBirthNullable = birthDate,
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    learningDelivery
                }
            };

            var dateTimeQuerySericeMock = new Mock<IDateTimeQueryService>();
            var learnDelFAMsMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dd12Mock = new Mock<IDerivedData_12Rule>();
            var dd21Mock = new Mock<IDerivedData_21Rule>();
            var fileDataServiceMock = new Mock<IFileDataService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            dateTimeQuerySericeMock.Setup(x => x.YearsBetween(learnStartDate, birthDate)).Returns(24);

            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType1, learnDelFAMCode1)).Returns(false);
            learnDelFAMsMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMType2, learnDelFAMCode2)).Returns(false);

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);
            dd12Mock.Setup(d => d.IsAdultSkillsFundedOnBenefits(learnerEmploymentStatuses, learningDelivery)).Returns(true);
            dd21Mock.Setup(d => d.IsAdultFundedUnemployedWithOtherStateBenefits(learningDelivery, learner)).Returns(true);
            fileDataServiceMock.Setup(f => f.UKPRN()).Returns(1000123);
            organisationDataServiceMock.Setup(o => o.LegalOrgTypeMatchForUkprn(1000123, "SSPS")).Returns(true);
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, "2")).Returns(false);
            larsDataServiceMock.Setup(l => l.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, 2)).Returns(true);
            larsDataServiceMock.Setup(d => d.EffectiveDatesValidforLearnAimRef(learnAimRef, effectiveFromDate)).Returns(true);
            larsDataServiceMock.Setup(d => d.FullLevel2PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, effectiveFromDate, 100.00M)).Returns(true);
            larsDataServiceMock.Setup(d => d.FullLevel2PercentForLearnAimRefNotMatchPercentValue(learnAimRef, effectiveFromDate, 80.00M)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    datetimeQueryService: dateTimeQuerySericeMock.Object,
                    learningDeliveryFAMQueryService: learnDelFAMsMock.Object,
                    derivedData_07Rule: dd07Mock.Object,
                    derivedData_12Rule: dd12Mock.Object,
                    derivedData_21Rule: dd21Mock.Object,
                    lARSDataService: larsDataServiceMock.Object,
                    fileDataService: fileDataServiceMock.Object,
                    organisationDataService: organisationDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameter()
        {
            DateTime dateOfBirth = new DateTime(2019, 05, 21);
            int priorAttain = 2;
            string learnDelFAMType = "FFI";
            string learnDelFAMCode = "1";
            var learningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = "AB12340056",
                LearnStartDate = new DateTime(2018, 07, 01),
                FundModel = 35
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter("DateOfBirth", "21/05/2019")).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter("PriorAttain", priorAttain)).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learningDelivery.LearnAimRef)).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter("LearnStartDate", "01/07/2018")).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter("FundModel", learningDelivery.FundModel)).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", learnDelFAMType)).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter("LearnFAMCode", learnDelFAMCode)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameter(
                dateOfBirth,
                priorAttain,
                learningDelivery,
                learnDelFAMType,
                learnDelFAMCode);
            validationErrorHandlerMock.VerifyAll();
        }

        private LearnDelFAMType_58Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDateTimeQueryService datetimeQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IDerivedData_07Rule derivedData_07Rule = null,
            IDerivedData_12Rule derivedData_12Rule = null,
            IDerivedData_21Rule derivedData_21Rule = null,
            IFileDataService fileDataService = null,
            IOrganisationDataService organisationDataService = null,
            ILARSDataService lARSDataService = null)
        {
            return new LearnDelFAMType_58Rule(
                validationErrorHandler: validationErrorHandler,
                datetimeQueryService: datetimeQueryService,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService,
                derivedData_07Rule: derivedData_07Rule,
                derivedData_12Rule: derivedData_12Rule,
                derivedData_21Rule: derivedData_21Rule,
                fileDataService: fileDataService,
                organisationDataService: organisationDataService,
                lARSDataService: lARSDataService);
        }
    }
}
