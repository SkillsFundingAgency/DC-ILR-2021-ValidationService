using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_78RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnAimRef_78", result);
        }

        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            var result = LearnAimRef_78Rule.FirstViableDate;

            Assert.Equal(DateTime.Parse("2016-08-01"), result);
        }

        [Fact]
        public void LastViableDateMeetsExpectation()
        {
            var result = LearnAimRef_78Rule.LastViableDate;

            Assert.Equal(DateTime.Parse("2017-07-31"), result);
        }

        [Theory]
        [InlineData(1004, false)]
        [InlineData(1005, true)]
        public void IsSpecialistDesignatedCollegeMeetsExpectation(int ukprn, bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(ukprn);

            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            orgData
                .Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, "USDC"))
                .Returns(expectation);

            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, learningDeliveryFAMQS.Object, fileData.Object, orgData.Object, dd07.Object, dateTimeQS.Object);

            var result = sut.IsSpecialistDesignatedCollege();

            handler.VerifyAll();
            service.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();
            dd07.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(LARSNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.HigherLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level1, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level1_2, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level2, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level3, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level4, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level5, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level6, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level7, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level8, false)]
        [InlineData(LARSNotionalNVQLevelV2.MixedLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.NotKnown, false)]
        public void IsQualifyingNotionalNVQMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery
                .SetupGet(y => y.NotionalNVQLevelv2)
                .Returns(candidate);

            var result = sut.IsQualifyingNotionalNVQ(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("testAim1")]
        [InlineData("testAim2")]
        public void HasQualifyingNotionalNVQMeetsExpectation(string candidate)
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(candidate))
                .Returns((ILARSLearningDelivery)null);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, learningDeliveryFAMQS.Object, fileData.Object, orgData.Object, dd07.Object, dateTimeQS.Object);

            var result = sut.HasQualifyingNotionalNVQ(mockDelivery.Object);

            handler.VerifyAll();
            service.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();
            dd07.VerifyAll();

            Assert.False(result);
        }

        [Theory]
        [InlineData(LARSCategories.LegalEntitlementLevel2, false)]
        [InlineData(LARSCategories.OnlyForLegalEntitlementAtLevel3, true)]
        [InlineData(LARSCategories.WorkPlacementSFAFunded, false)]
        [InlineData(LARSCategories.WorkPreparationSFATraineeships, false)]
        [InlineData(36, false)]
        [InlineData(39, false)]
        public void IsQualifyingCategoryMeetsExpectation(int category, bool expectation)
        {
            var sut = NewRule();

            var mockValidity = new Mock<ILARSLearningCategory>();
            mockValidity
                .SetupGet(x => x.CategoryRef)
                .Returns(category);

            var result = sut.IsQualifyingCategory(mockValidity.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("testAim1")]
        [InlineData("testAim2")]
        public void HasQualifyingCategoryMeetsExpectation(string candidate)
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetCategoriesFor(candidate))
                .Returns(new List<ILARSLearningCategory>());

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, learningDeliveryFAMQS.Object, fileData.Object, orgData.Object, dd07.Object, dateTimeQS.Object);

            var result = sut.HasQualifyingCategory(mockDelivery.Object);

            handler.VerifyAll();
            service.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();
            dd07.VerifyAll();

            Assert.False(result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse("2016-08-01");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("LearnAimRef_78", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", FundModels.AdultSkills))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockCategory = new Mock<ILARSLearningCategory>();
            mockCategory
                .SetupGet(x => x.CategoryRef)
                .Returns(LARSCategories.LegalEntitlementLevel2);

            var larsCategories = new List<ILARSLearningCategory>();
            larsCategories.Add(mockCategory.Object);

            var mockLars = new Mock<ILARSLearningDelivery>();
            mockLars
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level3);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLars.Object);
            service
                .Setup(x => x.GetCategoriesFor(learnAimRef))
                .Returns(larsCategories);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
               .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                   mockDelivery.Object.LearningDeliveryFAMs,
                   "LDM",
                   "347"))
               .Returns(false);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                  mockDelivery.Object.LearningDeliveryFAMs,
                  "LDM",
                  "034"))
                .Returns(false);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMType(
                    mockDelivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(1004);

            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            orgData
                .Setup(x => x.LegalOrgTypeMatchForUkprn(1004, "USDC"))
                .Returns(false);

            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            dd07
                .Setup(dd => dd.IsApprenticeship(mockDelivery.Object.ProgTypeNullable)).Returns(false);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, LearnAimRef_78Rule.FirstViableDate, LearnAimRef_78Rule.LastViableDate, true))
                .Returns(true);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, learningDeliveryFAMQS.Object, fileData.Object, orgData.Object, dd07.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();
            dd07.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse("2016-08-01");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockCategory = new Mock<ILARSLearningCategory>();
            mockCategory
                .SetupGet(x => x.CategoryRef)
                .Returns(LARSCategories.OnlyForLegalEntitlementAtLevel3);

            var larsCategories = new List<ILARSLearningCategory>();
            larsCategories.Add(mockCategory.Object);

            var mockLars = new Mock<ILARSLearningDelivery>();
            mockLars
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level3);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLars.Object);
            service
                .Setup(x => x.GetCategoriesFor(learnAimRef))
                .Returns(larsCategories);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
               .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                   mockDelivery.Object.LearningDeliveryFAMs,
                   "LDM",
                   "347"))
               .Returns(false);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                  mockDelivery.Object.LearningDeliveryFAMs,
                  "LDM",
                  "034"))
                .Returns(false);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMType(
                    mockDelivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(1004);

            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            orgData
                .Setup(x => x.LegalOrgTypeMatchForUkprn(1004, "USDC"))
                .Returns(false);

            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            dd07
                .Setup(dd => dd.IsApprenticeship(mockDelivery.Object.ProgTypeNullable)).Returns(false);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, LearnAimRef_78Rule.FirstViableDate, LearnAimRef_78Rule.LastViableDate, true))
                .Returns(true);

            var sut = new LearnAimRef_78Rule(handler.Object, service.Object, learningDeliveryFAMQS.Object, fileData.Object, orgData.Object, dd07.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            fileData.VerifyAll();
            orgData.VerifyAll();
            dd07.VerifyAll();
        }

        public LearnAimRef_78Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var orgData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            return new LearnAimRef_78Rule(handler.Object, service.Object, learningDeliveryFAMQS.Object, fileData.Object, orgData.Object, dd07.Object, dateTimeQS.Object);
        }
    }
}
