using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R64RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R64Rule(null, commonOps.Object, larsData.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R64Rule(handler.Object, null, larsData.Object));
        }

        /// <summary>
        /// New rule with null lars data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLarsDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R64Rule(handler.Object, commonOps.Object, null));
        }

        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("R64", result);
        }

        /// <summary>
        /// Rule name 2, matches the constant.
        /// </summary>
        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(RuleNameConstants.R64, result);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        /// <summary>
        /// Validate with null learner throws.
        /// </summary>
        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsTraineeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsTraineeship(delivery.Object))
                .Returns(expectation);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new R64Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            var result = sut.IsTraineeship(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void HasQualifyingModelExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 35, 36))
                .Returns(expectation);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new R64Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            var result = sut.HasQualifyingModel(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsComponentAimModelExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(delivery.Object))
                .Returns(expectation);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new R64Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            var result = sut.IsComponentAim(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        [Fact]
        public void GetFrameworkAimsForMeetsExpectation()
        {
            // arrange
            const string learnAimRef = "shonkyRef";
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { });

            var sut = new R64Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            var result = sut.GetFrameworkAimsFor(delivery.Object);

            // assert
            Assert.Empty(result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        [Fact]
        public void HasQualifyingFrameworkAimReturnsFalseWithNullAims()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingFrameworkAim(null, x => true);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void HasQualifyingFrameworkAimReturnsFalseWithEmptyAims()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingFrameworkAim(new ILARSFrameworkAim[] { }, x => true);

            // assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(23, null, false)]
        [InlineData(23, 22, false)]
        [InlineData(23, 23, true)]
        [InlineData(23, 24, false)]
        [InlineData(24, 24, true)]
        [InlineData(25, 24, false)]
        public void HasMatchingProgrammeMeetsExpectation(int frameworkItem, int? deliveryItem, bool expectation)
        {
            // arrange
            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim
                .SetupGet(x => x.ProgType)
                .Returns(frameworkItem);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(deliveryItem);

            var sut = NewRule();

            // act
            var result = sut.HasMatchingProgramme(frameworkAim.Object, delivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(23, null, false)]
        [InlineData(23, 22, false)]
        [InlineData(23, 23, true)]
        [InlineData(23, 24, false)]
        [InlineData(24, 24, true)]
        [InlineData(25, 24, false)]
        public void HasMatchingFrameworkCodeMeetsExpectation(int frameworkItem, int? deliveryItem, bool expectation)
        {
            // arrange
            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim
                .SetupGet(x => x.FworkCode)
                .Returns(frameworkItem);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.FworkCodeNullable)
                .Returns(deliveryItem);

            var sut = NewRule();

            // act
            var result = sut.HasMatchingFrameworkCode(frameworkAim.Object, delivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(23, null, false)]
        [InlineData(23, 22, false)]
        [InlineData(23, 23, true)]
        [InlineData(23, 24, false)]
        [InlineData(24, 24, true)]
        [InlineData(25, 24, false)]
        public void HasMatchingPathwayCodeMeetsExpectation(int frameworkItem, int? deliveryItem, bool expectation)
        {
            // arrange
            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim
                .SetupGet(x => x.PwayCode)
                .Returns(frameworkItem);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.PwayCodeNullable)
                .Returns(deliveryItem);

            var sut = NewRule();

            // act
            var result = sut.HasMatchingPathwayCode(frameworkAim.Object, delivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(1, TypeOfLARSCommonComponent.Apprenticeship.CompetencyElement)]
        [InlineData(3, TypeOfLARSCommonComponent.Apprenticeship.MainAimOrTechnicalCertificate)]
        public void ComponentTypesMeetExpectation(int candidate, int expectation)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, false)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        public void HasQualifyingComponentTypesMeetsExpectation(int? frameworkItem, bool expectation)
        {
            // arrange
            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim
                .SetupGet(x => x.FrameworkComponentType)
                .Returns(frameworkItem);

            var sut = NewRule();

            // act
            var result = sut.HasQualifyingComponentTypes(frameworkAim.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2018-04-01", null, "2018-03-31", false)]
        [InlineData("2018-04-01", null, "2018-04-01", true)]
        [InlineData("2018-04-01", null, "2019-11-14", true)]
        [InlineData("2018-04-01", "2018-04-10", "2018-03-31", false)]
        [InlineData("2018-04-01", "2018-04-10", "2018-04-01", true)]
        [InlineData("2018-04-01", "2018-04-10", "2018-04-10", true)]
        [InlineData("2018-04-01", "2018-04-10", "2018-04-11", false)]
        public void IsCurrentAimMeetsExpectation(string start, string end, string candidate, bool expectation)
        {
            // arrange
            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(start));
            frameworkAim
                .SetupGet(x => x.EndDate)
                .Returns(GetNullableDate(end));

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var sut = NewRule();

            // act
            var result = sut.IsCurrentAim(frameworkAim.Object, delivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(1, OutcomeConstants.Achieved)]
        [InlineData(2, CompletionState.HasCompleted)]
        public void StatesMeetsExpectation(int candidate, int expectation)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(146, false)]
        public void HasCompletedMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.CompStatus)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.HasCompleted(delivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(1, true)]
        [InlineData(7, false)]
        [InlineData(8, false)]
        [InlineData(19, false)]
        [InlineData(312, false)]
        public void HasAchievementMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.OutcomeNullable)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.HasAchievement(delivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// date ranges that generate one error item
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("2016-09-16")]
        [InlineData("2017-04-01")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "shonkyRef";
            const int pwayCode = 123;
            const int fworkCode = 345;
            const int progType = 789;

            var testDate = DateTime.Parse(candidate);
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            delivery
                .SetupGet(x => x.AimType)
                .Returns(100);
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(101);
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(102);
            delivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(1);
            delivery
                .SetupGet(x => x.CompStatus)
                .Returns(2);
            delivery
                .SetupGet(x => x.OutcomeNullable)
                .Returns(1);
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);
            delivery
                .SetupGet(x => x.PwayCodeNullable)
                .Returns(pwayCode);
            delivery
                .SetupGet(x => x.FworkCodeNullable)
                .Returns(fworkCode);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            var testStart = testDate.AddDays(-1);

            var temp = new Mock<ILearningDelivery>(MockBehavior.Strict);
            temp
                .SetupGet(x => x.CompStatus)
                .Returns(0);
            temp
                .SetupGet(x => x.OutcomeNullable)
                .Returns((int?)null);
            temp
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            temp
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);
            temp
                .SetupGet(x => x.PwayCodeNullable)
                .Returns(pwayCode);
            temp
                .SetupGet(x => x.FworkCodeNullable)
                .Returns(fworkCode);
            temp
                .Setup(x => x.LearnStartDate)
                .Returns(testStart);

            var deliveries = new ILearningDelivery[] { delivery.Object, temp.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.R64, learnRefNumber, 1, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AimType", 100))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 101))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("StdCode", 102))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", progType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                 .Setup(x => x.BuildErrorMessageParameter("FworkCode", fworkCode))
                 .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                 .Setup(x => x.BuildErrorMessageParameter("PwayCode", pwayCode))
                 .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                 .Setup(x => x.BuildErrorMessageParameter("Outcome", 1))
                 .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                 .Setup(x => x.BuildErrorMessageParameter("CompStatus", 2))
                 .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsTraineeship(It.IsAny<ILearningDelivery>()))
                .Returns(false);
            commonOps
                .Setup(x => x.HasQualifyingFunding(It.IsAny<ILearningDelivery>(), 35, 36))
                .Returns(true);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(It.IsAny<ILearningDelivery>()))
                .Returns(true);

            var larsAim = new Mock<ILARSFrameworkAim>();
            larsAim.SetupGet(x => x.FrameworkComponentType).Returns(3);
            larsAim.SetupGet(x => x.ProgType).Returns(progType);
            larsAim.SetupGet(x => x.PwayCode).Returns(pwayCode);
            larsAim.SetupGet(x => x.FworkCode).Returns(fworkCode);
            larsAim.SetupGet(x => x.StartDate).Returns(testDate.AddDays(-2));

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { larsAim.Object });

            var sut = new R64Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("2016-04-01")]
        [InlineData("2019-06-09")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "shonkyRef";
            int pwayCode = 123;
            const int fworkCode = 345;
            const int progType = 789;

            var testDate = DateTime.Parse(candidate);
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            delivery
                .SetupGet(x => x.CompStatus)
                .Returns(2);
            delivery
                .SetupGet(x => x.OutcomeNullable)
                .Returns(1);
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);
            delivery
                .SetupGet(x => x.PwayCodeNullable)
                .Returns(pwayCode);
            delivery
                .SetupGet(x => x.FworkCodeNullable)
                .Returns(fworkCode);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            var testStart = testDate.AddDays(-1);

            var temp = new Mock<ILearningDelivery>(MockBehavior.Strict);
            temp
                .SetupGet(x => x.CompStatus)
                .Returns(0);
            temp
                .SetupGet(x => x.OutcomeNullable)
                .Returns((int?)null);
            temp
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            temp
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);
            temp
                .SetupGet(x => x.PwayCodeNullable)
                .Returns(pwayCode++);
            temp
                .SetupGet(x => x.FworkCodeNullable)
                .Returns(fworkCode);
            temp
                .Setup(x => x.LearnStartDate)
                .Returns(testStart);

            var deliveries = new ILearningDelivery[] { delivery.Object, temp.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsTraineeship(It.IsAny<ILearningDelivery>()))
                .Returns(false);
            commonOps
                .Setup(x => x.HasQualifyingFunding(It.IsAny<ILearningDelivery>(), 35, 36))
                .Returns(true);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(It.IsAny<ILearningDelivery>()))
                .Returns(true);

            var larsAim = new Mock<ILARSFrameworkAim>();
            larsAim.SetupGet(x => x.FrameworkComponentType).Returns(3);
            larsAim.SetupGet(x => x.ProgType).Returns(progType);
            larsAim.SetupGet(x => x.PwayCode).Returns(pwayCode);
            larsAim.SetupGet(x => x.FworkCode).Returns(fworkCode);
            larsAim.SetupGet(x => x.StartDate).Returns(testDate.AddDays(-2));

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { larsAim.Object });

            var sut = new R64Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Gets the nullable date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a nullable date</returns>
        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public R64Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new R64Rule(handler.Object, commonOps.Object, larsData.Object);
        }

        /*
        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveries = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = 3,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 2,
                    StdCodeNullable = 3,
                    LearnStartDate = new DateTime(2018, 10, 10)
                },
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = 3,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 5,
                    StdCodeNullable = 3,
                    LearnStartDate = new DateTime(2017, 10, 10)
                }
            };

            var completedLearningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = "101",
                AimType = 3,
                FworkCodeNullable = 1,
                PwayCodeNullable = 2,
                StdCodeNullable = 3,
                LearnStartDate = new DateTime(2018, 10, 09)
            };

            //NewRule().ConditionMet(learningDeliveries, completedLearningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(1, null, null, null, null)]
        [InlineData(3, 99, null, null, null)]
        [InlineData(3, 99, 100, 200, null)]
        [InlineData(3, 80, 0, 0, 100)]
        [InlineData(3, 0, 0, 0, 0)]
        [InlineData(3, 1, null, 20, null)]
        [InlineData(3, 1, null, 2, 9999)]
        [InlineData(3, 1, 2, 2, 9999)]
        [InlineData(3, 2, null, 3, 3)]
        public void ConditionMet_False(int aimType, int? frameworkCode, int? standardCode, int? pwayCode, int? progType)
        {
            var learningDeliveries = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = aimType,
                    FworkCodeNullable = frameworkCode,
                    PwayCodeNullable = pwayCode,
                    StdCodeNullable = standardCode,
                    ProgTypeNullable = progType,
                    LearnStartDate = new DateTime(2018, 10, 10)
                },
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = aimType,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 5,
                    StdCodeNullable = 3,
                    ProgTypeNullable = progType,
                    LearnStartDate = new DateTime(2017, 10, 10)
                }
            };

            var completedLearningDelivery = new TestLearningDelivery()
            {
                LearnAimRef = "101",
                AimType = aimType,
                FworkCodeNullable = 1,
                PwayCodeNullable = 2,
                StdCodeNullable = null,
                ProgTypeNullable = 3,
                LearnStartDate = new DateTime(2018, 10, 09)
            };

            //NewRule().ConditionMet(learningDeliveries, completedLearningDelivery).Should().BeFalse();
        }
        */
    }
}
