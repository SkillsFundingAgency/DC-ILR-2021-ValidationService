using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Learner;
using ESFA.DC.ILR.ValidationService.Data.Learner.Interface;
using ESFA.DC.ILR.ValidationService.Data.Learner.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_39RuleTests
    {
        [Theory]
        [InlineData("LearnAimRef", 2020, 35, 1, 1, 1, 1)]
        [InlineData("learnaimref", 2020, 35, 1, 1, 1, 1)]
        [InlineData("LEARNAIMREF", 2020, 35, 1, 1, 1, 1)]
        [InlineData("LearnAimRef", 2020, 25, 1, 1, 1, 1)]
        [InlineData("LearnAimRef", 2020, 35, 1, 2, 3, 4)]
        [InlineData("LearnAimRef", 2020, 35, null, 1, 1, 1)]
        [InlineData("LearnAimRef", 2020, 35, 1, null, 1, 1)]
        [InlineData("LearnAimRef", 2020, 35, 1, 1, null, 1)]
        [InlineData("LearnAimRef", 2020, 35, 1, 1, 1, null)]
        [InlineData("LearnAimRef", 2020, 35, 1, 1, null, null)]
        [InlineData("LearnAimRef", 2020, 35, 1, null, 1, null)]
        [InlineData("LearnAimRef", 2020, 35, 1, null, null, 1)]
        [InlineData("LearnAimRef", 2020, 35, null, null, 1, 1)]
        [InlineData("LearnAimRef", 2020, 35, null, 1, null, 1)]
        [InlineData("LearnAimRef", 2020, 35, null, null, null, null)]
        public void HasLearningDeliveryMatch_True(string learnAimRef, int yearStart, int fundModel, int? progType, int? fworkCode, int? pwayCode, int? stdCode)
        {
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                LearnStartDate = new DateTime(yearStart, 8, 1),
                FundModel = fundModel,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                StdCodeNullable = stdCode
            };

            var previousLearnerData = new LearnerReferenceData
            {
                LearnAimRef = learnAimRef,
                LearnStartDate = new DateTime(yearStart, 8, 1),
                FundModel = fundModel,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                StdCodeNullable = stdCode
            };

            NewDD().HasLearningDeliveryMatch(learningDelivery, previousLearnerData).Should().BeTrue();
        }

        [Theory]
        [InlineData("NOTAIMREF", 2020, 35, 1, 1, 1, 1)]
        [InlineData("LearnAimRef", 2019, 35, 1, 1, 1, 1)]
        [InlineData("LearnAimRef", 2020, 25, 1, 1, 1, 1)]
        [InlineData("LearnAimRef", 2020, 35, 2, 1, 1, 1)]
        [InlineData("LearnAimRef", 2020, 35, null, 1, 2, 1)]
        [InlineData("LearnAimRef", 2020, 35, null, 1, 1, 3)]
        [InlineData("LearnAimRef", 2020, 35, null, 1, 2, 3)]
        [InlineData("LearnAimRef", 2020, 35, null, null, 2, 2)]
        [InlineData("LearnAimRef", 2020, 35, 2, 2, 2, 2)]
        [InlineData("LearnAimRef", 2020, 35, 1, 2, 1, 1)]
        [InlineData("LearnAimRef", 2020, 35, 1, 1, 2, 1)]
        [InlineData("LearnAimRef", 2020, 35, 1, 1, 1, 2)]
        public void HasLearningDeliveryMatch_False(string learnAimRef, int yearStart, int fundModel, int? progType, int? fworkCode, int? pwayCode, int? stdCode)
        {
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                LearnStartDate = new DateTime(yearStart, 8, 1),
                FundModel = fundModel,
                ProgTypeNullable = progType,
                FworkCodeNullable = fworkCode,
                PwayCodeNullable = pwayCode,
                StdCodeNullable = stdCode
            };

            var previousLearnerData = new LearnerReferenceData
            {
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                StdCodeNullable = 1
            };

            NewDD().HasLearningDeliveryMatch(learningDelivery, previousLearnerData).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2, 1, 2)]
        [InlineData(5, 1, 2)]
        [InlineData(1, 5, 2)]
        [InlineData(1, null, 1)]
        [InlineData(1, 1, null)]
        [InlineData(1, null, null)]
        public void HasUKPRNMatch_True(long ukprn, long? prevUkprn, long? pmUkprn)
        {
            var ukprnList = new List<long> { 1, 2, 3 };

            NewDD().HasUKPRNMatch(ukprnList, ukprn, prevUkprn, pmUkprn).Should().BeTrue();
        }

        [Theory]
        [InlineData(5, 5, 5)]
        [InlineData(5, null, 5)]
        [InlineData(5, 5, null)]
        [InlineData(5, null, null)]
        public void HasUKPRNMatch_False(long ukprn, long? prevUkprn, long? pmUkprn)
        {
            var ukprnList = new List<long> { 1, 2, 3 };

            NewDD().HasUKPRNMatch(ukprnList, ukprn, prevUkprn, pmUkprn).Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void HasMatch_True(int ukprn)
        {
            var learner = new TestLearner
            {
                LearnRefNumber = "LearnRefNumber1",
                PrevLearnRefNumber = "LearnRefNumber2",
                PMUKPRNNullable = ukprn,
                PrevUKPRNNullable = ukprn,
                ULN = 1,
            };
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
            };

            var previousLearnerData = new LearnerReferenceData
            {
                LearnRefNumber = "LearnRefNumber1",
                UKPRN = 1,
                PrevUKPRN = 2,
                PMUKPRN = 2222,
                ULN = 1,
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
            };

            var ukprnList = new List<long> { 1, 2, 3 };

            NewDD().HasMatch(ukprnList, 1, learningDelivery, previousLearnerData).Should().BeTrue();
        }

        [Theory]
        [InlineData(1111)]
        [InlineData(null)]
        public void HasMatch_False(int? ukprn)
        {
            var learner = new TestLearner
            {
                LearnRefNumber = "LearnRefNumber1",
                PrevLearnRefNumber = "LearnRefNumber2",
                PMUKPRNNullable = ukprn,
                ULN = 1,
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
            };

            var previousLearnerData = new LearnerReferenceData
            {
                LearnRefNumber = "LearnRefNumber1",
                PrevUKPRN = 5555,
                PMUKPRN = 5555,
                ULN = 1,
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
            };

            var ukprnList = new List<long> { 1, 2, 3 };

            NewDD().HasMatch(ukprnList, 1, learningDelivery, previousLearnerData).Should().BeFalse();
        }

        [Fact]
        public void GetMatchingLearningAimFromPreviousYear_MatchFound()
        {
            var learner = new TestLearner
            {
                LearnRefNumber = "LearnRefNumber1",
                PrevLearnRefNumber = "LearnRefNumber2",
                PrevUKPRNNullable = 5555,
                ULN = 1,
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
            };

            var previousLearnerData1 = new LearnerReferenceData
            {
                LearnRefNumber = "LearnRefNumber1",
                PrevUKPRN = 5555,
                PMUKPRN = 5555,
                ULN = 1,
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
            };

            var previousLearnerData2 = new LearnerReferenceData
            {
                LearnRefNumber = "LearnRefNumber2",
                PrevUKPRN = 5555,
                PMUKPRN = 5555,
                ULN = 1,
                LearnAimRef = "LearnAimRef2",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
            };

            var previousLearnerData = new List<LearnerReferenceData>
            {
                previousLearnerData1,
                previousLearnerData2
            };

            var refDataServiceMock = new Mock<ILearnerReferenceDataService>();
            refDataServiceMock.Setup(x => x.GetLearnerDataForPreviousYear(new List<string> { learner.LearnRefNumber, learner.PrevLearnRefNumber }))
                .Returns(previousLearnerData);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(x => x.UKPRN()).Returns(12345);

            NewDD(refDataServiceMock.Object, fileDataServiceMock.Object).GetMatchingLearningAimFromPreviousYear(learner, learningDelivery).Should().BeEquivalentTo(previousLearnerData1);
        }

        [Fact]
        public void GetMatchingLearningAimFromPreviousYear_NoMatchFound()
        {
            var learner = new TestLearner
            {
                LearnRefNumber = "LearnRefNumber1",
                PrevLearnRefNumber = "LearnRefNumber2",
                PrevUKPRNNullable = 5555,
                ULN = 1,
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
            };

            var previousLearnerData1 = new LearnerReferenceData
            {
                LearnRefNumber = "LearnRefNumber1",
                PrevUKPRN = 5555,
                PMUKPRN = 5555,
                ULN = 1,
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
            };

            var previousLearnerData2 = new LearnerReferenceData
            {
                LearnRefNumber = "LearnRefNumber2",
                PrevUKPRN = 5555,
                PMUKPRN = 5555,
                ULN = 1,
                LearnAimRef = "LearnAimRef2",
                LearnStartDate = new DateTime(2020, 8, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
            };

            var previousLearnerData = new List<LearnerReferenceData>
            {
                previousLearnerData1,
                previousLearnerData2
            };

            var refDataServiceMock = new Mock<ILearnerReferenceDataService>();
            refDataServiceMock.Setup(x => x.GetLearnerDataForPreviousYear(new List<string> { learner.LearnRefNumber, learner.PrevLearnRefNumber }))
                .Returns(previousLearnerData);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(x => x.UKPRN()).Returns(12345);

            NewDD(refDataServiceMock.Object, fileDataServiceMock.Object).GetMatchingLearningAimFromPreviousYear(learner, learningDelivery).Should().BeEquivalentTo(previousLearnerData1);
        }

        private DerivedData_39Rule NewDD(ILearnerReferenceDataService learnerReferenceDataService = null, IFileDataService fileDataService = null)
        {
            return new DerivedData_39Rule(learnerReferenceDataService, fileDataService);
        }
    }
}
