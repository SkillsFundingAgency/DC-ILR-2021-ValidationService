using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
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
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_56RuleTests : AbstractRuleTests<LearnDelFAMType_56Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_56");
        }

        [Fact]
        public void Validate_Null_LearningDeliveries()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(new TestLearner());
            }
        }

        [Fact]
        public void IsProviderExcluded_True()
        {
            var organisationDataService = new Mock<IOrganisationDataService>();
            organisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(It.IsAny<long>(), "USDC")).Returns(true);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(x => x.UKPRN()).Returns(1000);

            NewRule(organisationDataService: organisationDataService.Object, fileDataService: fileDataServiceMock.Object).IsProviderExcluded().Should().BeTrue();
        }

        [Fact]
        public void IsProviderExcluded_False()
        {
            var organisationDataService = new Mock<IOrganisationDataService>();
            organisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(It.IsAny<long>(), "USDC")).Returns(false);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(x => x.UKPRN()).Returns(1000);

            NewRule(
                organisationDataService: organisationDataService.Object,
                fileDataService: fileDataServiceMock.Object).IsProviderExcluded().Should().BeFalse();
        }

        [Fact]
        public void StartDateConditionMet_True()
        {
            NewRule().StartDateConditionMet(new DateTime(2016, 07, 31)).Should().BeTrue();
        }

        [Fact]
        public void StartDateConditionMet_False()
        {
            NewRule().StartDateConditionMet(new DateTime(2016, 08, 01)).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(25).Should().BeFalse();
        }

        [Theory]
        [InlineData("1999-10-10")]
        [InlineData("1999-10-09")]
        [InlineData("1995-10-09")]
        [InlineData("1995-10-10")]
        [InlineData("1996-10-10")]
        public void AgeConditionMet_True(string dateOfBirth)
        {
            NewRule().AgeConditionMet(new DateTime(2018, 10, 10), DateTime.Parse(dateOfBirth)).Should().BeTrue();
        }

        [Theory]
        [InlineData("2000-11-10")]
        [InlineData("1994-10-10")]
        public void AgeConditionMet_False_Over23(string dateOfBirthString)
        {
            var dateOfBirth = DateTime.Parse(dateOfBirthString);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.AgeAtGivenDate(dateOfBirth, It.IsAny<DateTime>())).Returns(24);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).AgeConditionMet(new DateTime(2018, 10, 10), dateOfBirth).Should().BeFalse();
        }

        [Theory]
        [InlineData("2001-10-10")]
        public void AgeConditionMet_False_Under19(string dateOfBirthString)
        {
            var dateOfBirth = DateTime.Parse(dateOfBirthString);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.AgeAtGivenDate(dateOfBirth, It.IsAny<DateTime>())).Returns(18);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).AgeConditionMet(new DateTime(2018, 10, 10), dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void AgeConditionMet_False_NullDob()
        {
            NewRule().AgeConditionMet(It.IsAny<DateTime>(), null).Should().BeFalse();
        }

        [Fact]
        public void FamConditionMet_True()
        {
            var famqueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "FFI", "1"))
                .Returns(true);

            var fams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "FFI",
                    LearnDelFAMType = "1"
                }
            };
            NewRule(famQueryService: famqueryServiceMock.Object).FamConditionMet(fams).Should().BeTrue();
        }

        [Fact]
        public void FamConditionMet_False()
        {
            var famqueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "FFI", "2113"))
                .Returns(false);

            var fams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "FFI",
                    LearnDelFAMType = "23"
                }
            };
            NewRule(famQueryService: famqueryServiceMock.Object).FamConditionMet(fams).Should().BeFalse();
        }

        [Theory]
        [InlineData("E", 2)]
        [InlineData("1", 3)]
        [InlineData("2", 4)]
        [InlineData("E", 5)]
        [InlineData("2", 10)]
        [InlineData("2", 11)]
        [InlineData("1", 12)]
        [InlineData("E", 13)]
        [InlineData("2", 97)]
        [InlineData("2", 98)]

        [InlineData("3", 3)]
        [InlineData("4", 4)]
        [InlineData("3", 5)]
        [InlineData("4", 10)]
        [InlineData("3", 11)]
        [InlineData("4", 12)]
        [InlineData("4", 13)]
        [InlineData("3", 97)]
        [InlineData("3", 98)]
        public void NvQLevelConditionMet_True(string nvqLevel, int priorAttain)
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x =>
                    x.GetNotionalNVQLevelv2ForLearnAimRef(It.IsAny<string>()))
                .Returns(nvqLevel);

            NewRule(larsDataService: larsDataServiceMock.Object).NvQLevelConditionMet(It.IsAny<string>(), priorAttain).Should().BeTrue();
        }

        [Theory]
        [InlineData("3", 2)]
        [InlineData("4", 2)]

        [InlineData("80", 3)]
        [InlineData("20", 4)]
        [InlineData("X", 5)]
        [InlineData("7", 10)]
        [InlineData("9", 11)]
        [InlineData("20", 12)]
        [InlineData("E", 22)]
        [InlineData("1", 99)]
        [InlineData("2", 100)]
        public void NvQLevelConditionMet_False(string nvqLevel, int priorAttain)
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x =>
                    x.GetNotionalNVQLevelv2ForLearnAimRef(It.IsAny<string>()))
                .Returns(nvqLevel);

            NewRule(larsDataService: larsDataServiceMock.Object).NvQLevelConditionMet(It.IsAny<string>(), priorAttain).Should().BeFalse();
        }

        [Fact]
        public void NvQLevelConditionMet_PriorAttain_Null_False()
        {
            NewRule().NvQLevelConditionMet(It.IsAny<string>(), null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x =>
                    x.GetNotionalNVQLevelv2ForLearnAimRef(It.IsAny<string>()))
                .Returns("1");

            var famqueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "FFI", "1"))
                .Returns(true);

            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2016, 07, 29),
                FundModel = 35,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMCode = "FFI",
                        LearnDelFAMType = "1"
                    }
                }
            };

            NewRule(
                    larsDataService: larsDataServiceMock.Object,
                    famQueryService: famqueryServiceMock.Object).
                ConditionMet(learningDelivery, new DateTime(1997, 10, 11), 3).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x =>
                    x.GetNotionalNVQLevelv2ForLearnAimRef(It.IsAny<string>()))
                .Returns("123");

            var famqueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "FFI", "1"))
                .Returns(true);

            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2018, 10, 10),
                FundModel = 35,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMCode = "FFI",
                        LearnDelFAMType = "1"
                    }
                }
            };

            NewRule(
                    larsDataService: larsDataServiceMock.Object,
                    famQueryService: famqueryServiceMock.Object).
                ConditionMet(learningDelivery, new DateTime(2018, 10, 11), 3).Should().BeFalse();
        }

        [Fact]
        public void IsLearningDeliveryExcluded_ProgType_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24
            };

            NewRule().IsLearningDeliveryExcluded(It.IsAny<ILearner>(), learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void IsLearningDeliveryExcluded_IsApprenticeship_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24
            };

            var ddo07Mock = new Mock<IDerivedData_07Rule>();
            ddo07Mock.Setup(x => x.IsApprenticeship(It.IsAny<int?>())).Returns(true);

            NewRule(dd07: ddo07Mock.Object).IsLearningDeliveryExcluded(It.IsAny<ILearner>(), learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData("034")]
        [InlineData("328")]
        [InlineData("347")]
        public void IsLearningDeliveryExcluded_FamCode_True(string famCode)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "LDM",
                        LearnDelFAMCode = famCode
                    }
                }
            };

            var famqueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "LDM", famCode))
                .Returns(true);

            NewRule().IsLearningDeliveryExcluded(It.IsAny<ILearner>(), learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void IsLearningDeliveryExcluded_DD12_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24
            };

            var ddo07Mock = new Mock<IDerivedData_07Rule>();
            ddo07Mock.Setup(x => x.IsApprenticeship(It.IsAny<int?>())).Returns(true);

            var famqueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var dd12Mock = new Mock<IDerivedData_12Rule>();
            dd12Mock.Setup(x => x.IsAdultSkillsFundedOnBenefits(It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), It.IsAny<ILearningDelivery>())).Returns(true);

            NewRule(dd07: ddo07Mock.Object, famQueryService: famqueryServiceMock.Object, dd12: dd12Mock.Object).IsLearningDeliveryExcluded(It.IsAny<ILearner>(), learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void IsLearningDeliveryExcluded_DD21_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "LDm"
                    }
                }
            };

            var dd12Mock = new Mock<IDerivedData_12Rule>();
            dd12Mock.Setup(x => x.IsAdultSkillsFundedOnBenefits(It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), It.IsAny<ILearningDelivery>())).Returns(true);

            var dd21Mock = new Mock<IDerivedData_21Rule>();
            dd21Mock.Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(It.IsAny<ILearningDelivery>(), It.IsAny<ILearner>())).Returns(true);

            var ddo07Mock = new Mock<IDerivedData_07Rule>();
            ddo07Mock.Setup(x => x.IsApprenticeship(It.IsAny<int?>())).Returns(true);

            var famqueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            NewRule(dd07: ddo07Mock.Object, famQueryService: famqueryServiceMock.Object, dd12: dd12Mock.Object, dd21: dd21Mock.Object).
            IsLearningDeliveryExcluded(It.IsAny<ILearner>(), learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void IsLearningDeliveryExcluded_FamType_True()
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

            var famqueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, "RES"))
                .Returns(true);

            NewRule().IsLearningDeliveryExcluded(It.IsAny<ILearner>(), learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(01)]
        [InlineData(11)]
        [InlineData(13)]
        [InlineData(20)]
        [InlineData(23)]
        [InlineData(24)]
        [InlineData(29)]
        [InlineData(31)]
        [InlineData(02)]
        [InlineData(12)]
        [InlineData(14)]
        [InlineData(19)]
        [InlineData(21)]
        [InlineData(25)]
        [InlineData(30)]
        [InlineData(32)]
        [InlineData(33)]
        [InlineData(34)]
        [InlineData(35)]
        public void IsLearningDeliveryExcluded_BasicSkillTypes_True(int basicSkillType)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24,
                LearnAimRef = "test11",
                LearnStartDate = new DateTime(2018, 10, 10),
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x =>
                    x.BasicSkillsMatchForLearnAimRefAndStartDate(new List<int>() { basicSkillType }, It.IsAny<string>(), new DateTime(2018, 10, 10)))
                .Returns(true);

            NewRule().IsLearningDeliveryExcluded(It.IsAny<ILearner>(), learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void IsLearningDeliveryExcluded_TradeUnionCategoryRef_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24,
                LearnAimRef = "test11",
                LearnStartDate = new DateTime(2018, 10, 10),
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "RES"
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x =>
                    x.LearnAimRefExistsForLearningDeliveryCategoryRef(It.IsAny<string>(), 19)).Returns(true);

            NewRule().IsLearningDeliveryExcluded(It.IsAny<ILearner>(), learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void IsLearningDeliveryExcluded_False()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 25,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "LDm"
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = new List<ILearnerEmploymentStatus>(),
            };

            var dd12Mock = new Mock<IDerivedData_12Rule>();
            dd12Mock.Setup(x => x.IsAdultSkillsFundedOnBenefits(It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), It.IsAny<ILearningDelivery>())).Returns(false);

            var dd21Mock = new Mock<IDerivedData_21Rule>();
            dd21Mock.Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(It.IsAny<ILearningDelivery>(), It.IsAny<ILearner>())).Returns(false);

            var ddo07Mock = new Mock<IDerivedData_07Rule>();
            ddo07Mock.Setup(x => x.IsApprenticeship(It.IsAny<int?>())).Returns(false);

            var famqueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x =>
                    x.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(false);

            larsDataServiceMock.Setup(x =>
                x.LearnAimRefExistsForLearningDeliveryCategoryRef(It.IsAny<string>(), 19)).Returns(false);

            NewRule(dd07: ddo07Mock.Object, famQueryService: famqueryServiceMock.Object, dd12: dd12Mock.Object, dd21: dd21Mock.Object, larsDataService: larsDataServiceMock.Object).
                IsLearningDeliveryExcluded(learner, learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x =>
                    x.GetNotionalNVQLevelv2ForLearnAimRef(It.IsAny<string>()))
                .Returns("1");
            larsDataServiceMock.Setup(x =>
                    x.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(false);

            var famqueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "FFI", "1"))
                .Returns(true);
            famqueryServiceMock.Setup(x =>
                    x.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", It.IsAny<IEnumerable<string>>()))
                .Returns(false);

            famqueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(),  It.IsAny<string>()))
                .Returns(false);

            var organisationDataService = new Mock<IOrganisationDataService>();
            organisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(It.IsAny<long>(), "USDC")).Returns(false);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(x => x.UKPRN()).Returns(1000);

            var dd12Mock = new Mock<IDerivedData_12Rule>();
            dd12Mock.Setup(x => x.IsAdultSkillsFundedOnBenefits(It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), It.IsAny<ILearningDelivery>())).Returns(false);

            var dd21Mock = new Mock<IDerivedData_21Rule>();
            dd21Mock.Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(It.IsAny<ILearningDelivery>(), It.IsAny<ILearner>())).Returns(false);

            var ddo07Mock = new Mock<IDerivedData_07Rule>();
            ddo07Mock.Setup(x => x.IsApprenticeship(It.IsAny<int?>())).Returns(false);

            larsDataServiceMock.Setup(x =>
                x.LearnAimRefExistsForLearningDeliveryCategoryRef(It.IsAny<string>(), 19)).Returns(false);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(1994, 10, 10),
                PriorAttainNullable = 4,
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2016, 07, 29),
                        FundModel = 35,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = "FFI",
                                LearnDelFAMType = "1"
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandlerMock.Object,
                    larsDataServiceMock.Object,
                    fileDataService: fileDataServiceMock.Object,
                    organisationDataService: organisationDataService.Object,
                    famQueryService: famqueryServiceMock.Object,
                    dd07: ddo07Mock.Object,
                    dd12: dd12Mock.Object,
                    dd21: dd21Mock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Pass()
        {
            var organisationDataService = new Mock<IOrganisationDataService>();
            organisationDataService.Setup(x => x.LegalOrgTypeMatchForUkprn(It.IsAny<long>(), "USDC")).Returns(false);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(x => x.UKPRN()).Returns(1000);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(1994, 10, 10),
                PriorAttainNullable = 4,
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 07, 29),
                        FundModel = 35,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = "FFI",
                                LearnDelFAMType = "1"
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, organisationDataService: organisationDataService.Object, fileDataService: fileDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.UKPRN, 99999999999)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, "01/01/1999")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, 4)).Verifiable();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, "testRef")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProgType, 24)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 25)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, "FFI")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2017")).Verifiable();

            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2017, 01, 01),
                ProgTypeNullable = 24,
                FundModel = 25,
                LearnAimRef = "testRef",
            };
            var learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(1999, 01, 01),
                PriorAttainNullable = 4
            };

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(99999999999, learner, learningDelivery);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_56Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILARSDataService larsDataService = null,
            IDerivedData_07Rule dd07 = null,
            IDerivedData_12Rule dd12 = null,
            IDerivedData_21Rule dd21 = null,
            ILearningDeliveryFAMQueryService famQueryService = null,
            IFileDataService fileDataService = null,
            IOrganisationDataService organisationDataService = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(20);

            return new LearnDelFAMType_56Rule(
                validationErrorHandler,
                larsDataService,
                dd07,
                dd12,
                dd21,
                famQueryService,
                fileDataService,
                organisationDataService,
                dateTimeQueryService ?? dateTimeQueryServiceMock.Object);
        }
    }
}
