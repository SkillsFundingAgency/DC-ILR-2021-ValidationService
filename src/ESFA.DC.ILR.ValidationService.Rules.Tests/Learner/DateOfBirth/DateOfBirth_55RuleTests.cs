using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_55RuleTests : AbstractRuleTests<DateOfBirth_55Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_55");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = FundModels.AdultSkills;
            var progType = ProgTypes.AdvancedLevelApprenticeship;
            var learnStartDate = new DateTime(2017, 09, 01);
            var dateOfBirth = new DateTime(1992, 09, 01);
            var learnAimRef = "123456789";

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock
                .Setup(ldsm => ldsm.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, It.IsAny<string[]>()))
                .Returns(true);

            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<string[]>()))
                .Returns(false);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, It.IsAny<string[]>()))
                .Returns(false);

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_DevolvedLevelTwoOrThreeExclusion))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, dd07Mock.Object)
                .ConditionMet(fundModel, progType, learnStartDate, dateOfBirth, learnAimRef, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData(true, false, false, false, false)]
        [InlineData(false, true, false, false, false)]
        [InlineData(false, false, true, false, false)]
        [InlineData(false, false, false, true, false)]
        [InlineData(false, false, false, false, true)]
        [InlineData(true, true, true, true, true)]
        public void ConditionMet_False_Excluded(bool dd07MockResult, bool ldFamTypeMockResult, bool ldFamTypeAndCodesMockResult, bool larsDataServiceMockResult, bool devolvedLevelTwoOrThreeExclusion)
        {
            var fundModel = FundModels.AdultSkills;
            var progType = ProgTypes.AdvancedLevelApprenticeship;
            var learnStartDate = new DateTime(2017, 09, 01);
            var dateOfBirth = new DateTime(1992, 09, 01);
            var learnAimRef = "123456789";

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock
                .Setup(ldsm => ldsm.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, It.IsAny<string[]>()))
                .Returns(true);

            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<string[]>()))
                .Returns(larsDataServiceMockResult);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(dd07MockResult);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(ldFamTypeMockResult);

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, It.IsAny<string[]>()))
                .Returns(ldFamTypeAndCodesMockResult);

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_DevolvedLevelTwoOrThreeExclusion))
                .Returns(devolvedLevelTwoOrThreeExclusion);

            NewRule(dateTimeQueryServiceMock.Object, larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, dd07Mock.Object)
                .ConditionMet(fundModel, progType, learnStartDate, dateOfBirth, learnAimRef, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var fundModel = 0;
            var progType = ProgTypes.AdvancedLevelApprenticeship;
            var learnStartDate = new DateTime(2017, 09, 01);
            var dateOfBirth = new DateTime(1992, 09, 01);
            var learnAimRef = "123456789";

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock
                .Setup(ldsm => ldsm.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, It.IsAny<string[]>()))
                .Returns(true);

            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<string[]>()))
                .Returns(false);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, It.IsAny<string[]>()))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, dd07Mock.Object)
                .ConditionMet(fundModel, progType, learnStartDate, dateOfBirth, learnAimRef, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDateLessThanFirstAugust2017()
        {
            var fundModel = FundModels.AdultSkills;
            var progType = ProgTypes.AdvancedLevelApprenticeship;
            var learnStartDate = new DateTime(2016, 09, 01);
            var dateOfBirth = new DateTime(1992, 09, 01);
            var learnAimRef = "123456789";

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock
                .Setup(ldsm => ldsm.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, It.IsAny<string[]>()))
                .Returns(true);

            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<string[]>()))
                .Returns(false);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, It.IsAny<string[]>()))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, dd07Mock.Object)
                .ConditionMet(fundModel, progType, learnStartDate, dateOfBirth, learnAimRef, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_YearsBetweenDoBAndLearnStartDateLessThan24()
        {
            var fundModel = FundModels.AdultSkills;
            var progType = ProgTypes.AdvancedLevelApprenticeship;
            var learnStartDate = new DateTime(2017, 09, 01);
            var dateOfBirth = new DateTime(1994, 09, 01);
            var learnAimRef = "123456789";

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(23);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock
                .Setup(ldsm => ldsm.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, It.IsAny<string[]>()))
                .Returns(true);

            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<string[]>()))
                .Returns(false);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, It.IsAny<string[]>()))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, dd07Mock.Object)
                .ConditionMet(fundModel, progType, learnStartDate, dateOfBirth, learnAimRef, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LARSNVQConditionFalse()
        {
            var fundModel = FundModels.AdultSkills;
            var progType = ProgTypes.AdvancedLevelApprenticeship;
            var learnStartDate = new DateTime(2017, 09, 01);
            var dateOfBirth = new DateTime(1992, 09, 01);
            var learnAimRef = "123456789";

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock
                .Setup(ldsm => ldsm.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, It.IsAny<string[]>()))
                .Returns(false);

            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<string[]>()))
                .Returns(false);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, It.IsAny<string[]>()))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, dd07Mock.Object)
                .ConditionMet(fundModel, progType, learnStartDate, dateOfBirth, learnAimRef, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var fundModel = FundModels.AdultSkills;
            var progType = ProgTypes.AdvancedLevelApprenticeship;
            var learnStartDate = new DateTime(2017, 09, 01);
            var dateOfBirth = new DateTime(1992, 09, 01);
            var learnAimRef = "123456789";

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearnStartDate = learnStartDate,
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock
                .Setup(ldsm => ldsm.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, It.IsAny<string[]>()))
                .Returns(true);

            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<string[]>()))
                .Returns(false);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, It.IsAny<string[]>()))
                .Returns(false);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(x => x.UKPRN()).Returns(99999999);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dateTimeQueryServiceMock.Object,
                    larsDataServiceMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    dd07Mock.Object,
                    fileDataServiceMock.Object,
                    validationErrorHandlerMock.Object)
                .Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var fundModel = FundModels.AdultSkills;
            var progType = ProgTypes.AdvancedLevelApprenticeship;
            var learnStartDate = new DateTime(2017, 09, 01);
            var dateOfBirth = new DateTime(1992, 09, 01);
            var learnAimRef = "123456789";

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearnStartDate = learnStartDate,
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(25);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock
                .Setup(ldsm => ldsm.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, It.IsAny<string[]>()))
                .Returns(false);

            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<string[]>()))
                .Returns(false);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.LDM, It.IsAny<string[]>()))
                .Returns(false);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(x => x.UKPRN()).Returns(99999999);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dateTimeQueryServiceMock.Object,
                    larsDataServiceMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    dd07Mock.Object,
                    fileDataServiceMock.Object,
                    validationErrorHandlerMock.Object)
                .Validate(learner);
            }
        }

        private DateOfBirth_55Rule NewRule(
           IDateTimeQueryService dateTimeQueryService = null,
           ILARSDataService larsDataService = null,
           ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
           IDerivedData_07Rule derivedData07 = null,
           IFileDataService fileDataService = null,
           IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_55Rule(
                dateTimeQueryService,
                larsDataService,
                learningDeliveryFamQueryService,
                derivedData07,
                fileDataService,
                validationErrorHandler);
        }
    }
}
