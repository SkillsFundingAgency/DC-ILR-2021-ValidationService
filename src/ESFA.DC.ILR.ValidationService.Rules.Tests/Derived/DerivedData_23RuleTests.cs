using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_23RuleTests
    {
        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsDefaultIntForNullLearner()
        {
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(20);

            var rule = NewRule(dateTimeServiceMock.Object);
            TestLearner learner = null;

            var result = rule.GetLearnersAgeAtStartOfESFContract(learner, null);

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsDefaultIntForNullLearningDeliveries()
        {
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(20);

            var rule = NewRule(dateTimeServiceMock.Object);
            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1999, 9, 1),
                LearningDeliveries = null
            };

            var result = rule.GetLearnersAgeAtStartOfESFContract(learner, null);

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsDefaultIntForIrrelevantConRefNumber()
        {
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(20);

            var rule = NewRule(dateTimeServiceMock.Object);

            var testConRefNumber = "123456";
            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1999, 9, 1),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNumber,
                        LearnStartDate = new DateTime(2017, 9, 1),
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        CompStatus = CompletionState.HasCompleted
                    }
                }
            };

            var result = rule.GetLearnersAgeAtStartOfESFContract(learner, "99999");

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsDefaultIntForNullDateOfBirth()
        {
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(20);

            var rule = NewRule(dateTimeServiceMock.Object);
            var testConRefNumber = "123456";
            var learner = new TestLearner
            {
                DateOfBirthNullable = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNumber,
                        LearnStartDate = new DateTime(2017, 9, 1),
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        CompStatus = CompletionState.HasCompleted
                    }
                }
            };

            var result = rule.GetLearnersAgeAtStartOfESFContract(learner, testConRefNumber);

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsDefaultIntForNoZESF0001Deliverable()
        {
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(20);

            var rule = NewRule(dateTimeServiceMock.Object);

            var testConRefNumber = "123456";
            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1999, 9, 1),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNumber,
                        LearnStartDate = new DateTime(2017, 9, 1),
                        LearnAimRef = TypeOfAim.References.WorkExperience,
                        CompStatus = CompletionState.HasCompleted
                    }
                }
            };

            var result = rule.GetLearnersAgeAtStartOfESFContract(learner, testConRefNumber);

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsDefaultIntForNoZESF0001DeliverableCompleted()
        {
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(20);

            var rule = NewRule(dateTimeServiceMock.Object);

            var testConRefNumber = "123456";
            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1999, 9, 1),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNumber,
                        LearnStartDate = new DateTime(2017, 9, 1),
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        CompStatus = CompletionState.IsOngoing
                    }
                }
            };

            var result = rule.GetLearnersAgeAtStartOfESFContract(learner, testConRefNumber);

            Assert.Null(result);
        }

        [Fact]
        public void GetLearnersAgeAtStartOfESFContract_ReturnsCorrectAge()
        {
            var testConRefNumber = "123456";
            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1999, 9, 1),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ConRefNumber = testConRefNumber,
                        LearnStartDate = new DateTime(2017, 9, 1),
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        CompStatus = CompletionState.HasCompleted
                    }
                }
            };

            var mockAge = 20;
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.YearsBetween(learner.DateOfBirthNullable.Value, It.IsAny<DateTime>())).Returns(mockAge);

            var rule = NewRule(dateTimeServiceMock.Object);

            var result = rule.GetLearnersAgeAtStartOfESFContract(learner, testConRefNumber);

            Assert.Equal(mockAge, result);
        }

        public DerivedData_23Rule NewRule(IDateTimeQueryService dateTimeQueryService)
        {
            return new DerivedData_23Rule(dateTimeQueryService);
        }
    }
}
