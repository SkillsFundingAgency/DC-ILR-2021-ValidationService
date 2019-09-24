﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    /// <summary>
    /// learn start date rule 07 tests
    /// </summary>
    public class LearnStartDate_07RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_07Rule(null, ddRule04.Object, larsData.Object, commonOps.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 04 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule04Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_07Rule(handler.Object, null, larsData.Object, commonOps.Object));
        }

        /// <summary>
        /// New rule with null lars data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_07Rule(handler.Object, ddRule04.Object, null, commonOps.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_07Rule(handler.Object, ddRule04.Object, larsData.Object, null));
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
            Assert.Equal("LearnStartDate_07", result);
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
            Assert.Equal(RuleNameConstants.LearnStartDate_07, result);
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

        /// <summary>
        /// Gets the earliest start date for, meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("2018-06-04")]
        [InlineData("2018-08-06")]
        public void GetEarliestStartDateForMeetsExpectation(string candidate)
        {
            // arrange
            var testDate = GetNullableDate(candidate);
            var delivery = new Mock<ILearningDelivery>();
            var deliveries = new ILearningDelivery[] { };

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            ddRule04
                .Setup(x => x.GetEarliesStartDateFor(delivery.Object, deliveries))
                .Returns(testDate);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnStartDate_07Rule(handler.Object, ddRule04.Object, larsData.Object, commonOps.Object);

            // act
            var result = sut.GetEarliestStartDateFor(delivery.Object, deliveries);

            // assert
            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();

            Assert.Equal(testDate, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsStandardApprenticeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(delivery.Object))
                .Returns(expectation);

            var sut = new LearnStartDate_07Rule(handler.Object, ddRule04.Object, larsData.Object, commonOps.Object);

            // act
            var result = sut.IsStandardApprenticeship(delivery.Object);

            // assert
            handler.VerifyAll();
            ddRule04.VerifyAll();
            commonOps.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsRestartMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(expectation);

            var sut = new LearnStartDate_07Rule(handler.Object, ddRule04.Object, larsData.Object, commonOps.Object);

            // act
            var result = sut.IsRestart(delivery.Object);

            // assert
            handler.VerifyAll();
            ddRule04.VerifyAll();
            commonOps.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void IsCommonComponentWithNullLARSAimReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.IsCommonComponent(null);

            // assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(0, false)]
        [InlineData(null, false)]
        public void IsCommonComponentMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILARSLearningDelivery>();
            delivery
                .SetupGet(x => x.FrameworkCommonComponent)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.IsCommonComponent(delivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get framework aims for, meets expectation.
        /// </summary>
        /// <param name="learnAimRef">The learn aim reference.</param>
        [Theory]
        [InlineData("shonkyRefCode1")]
        [InlineData("shonkyRefCode2")]
        [InlineData("shonkyRefCode3")]
        public void GetFrameworkAimsForMeetsExpectation(string learnAimRef)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns((IReadOnlyCollection<ILARSFrameworkAim>)null);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnStartDate_07Rule(handler.Object, ddRule04.Object, larsData.Object, commonOps.Object);

            // act
            var result = sut.GetQualifyingFrameworksFor(delivery.Object);

            // assert
            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();

            Assert.Null(result);
        }

        /// <summary>
        /// Filtered framework aims for null frameworks meets expectation.
        /// </summary>
        [Fact]
        public void FilteredFrameworkAimsForNullFrameworksMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.FilteredFrameworkAimsFor(null, null);

            // assert
            Assert.Empty(result);
        }

        /// <summary>
        /// Filtered framework aims for empty frameworks meets expectation.
        /// </summary>
        [Fact]
        public void FilteredFrameworkAimsForEmptyFrameworksMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.FilteredFrameworkAimsFor(null, new ILARSFrameworkAim[] { });

            // assert
            Assert.Empty(result);
        }

        /// <summary>
        /// Filtered framework aims for vanilla delivery and empty frameworks meets expectation.
        /// </summary>
        [Fact]
        public void FilteredFrameworkAimsForVanillaDeliveryAndEmptyFrameworksMeetsExpectation()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            var sut = NewRule();

            // act
            var result = sut.FilteredFrameworkAimsFor(delivery.Object, new ILARSFrameworkAim[] { });

            // assert
            Assert.Empty(result);
        }

        /// <summary>
        /// Filtered framework aims for, meets expectation.
        /// </summary>
        [Fact]
        public void FilteredFrameworkAimsForMeetsExpectation()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery.SetupGet(x => x.ProgTypeNullable).Returns(2);
            delivery.SetupGet(x => x.FworkCodeNullable).Returns(3);
            delivery.SetupGet(x => x.PwayCodeNullable).Returns(4);

            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim.SetupGet(x => x.ProgType).Returns(2);
            frameworkAim.SetupGet(x => x.FworkCode).Returns(3);
            frameworkAim.SetupGet(x => x.PwayCode).Returns(4);

            var sut = NewRule();

            // act
            var result = sut.FilteredFrameworkAimsFor(delivery.Object, new ILARSFrameworkAim[] { frameworkAim.Object });

            // assert
            Assert.Contains(result, x => x == frameworkAim.Object);
        }

        [Fact]
        public void IsOutOfScopeWithEmptyFrameworksTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.IsOutOfScope(new ILARSFrameworkAim[] { });

            // assert
            Assert.True(result);
        }

        [Fact]
        public void IsCurrentWithEmptyFrameworksFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.IsCurrent(new ILARSFrameworkAim[] { }, DateTime.Today);

            // assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("2016-08-02", "2016-04-01", "2017-04-01", true)] // inside
        [InlineData("2016-04-01", "2016-04-01", "2017-04-01", true)] // lower limit
        [InlineData("2017-04-01", "2016-04-01", "2017-04-01", true)] // upper limit
        [InlineData("2017-04-02", "2016-04-01", "2017-04-01", false)] // outside upper limit
        [InlineData("2016-04-01", "2016-04-01", "2016-03-31", false)] // withdrawn lower limit
        [InlineData("2017-04-01", "2016-04-01", "2016-03-31", false)] // withdrawn upper limit
        public void IsCurrentMeetsExpectation(string candidate, string start, string end, bool expectation)
        {
            // arrange
            var testDate = DateTime.Parse(candidate);
            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim.SetupGet(x => x.StartDate).Returns(DateTime.Parse(start));
            frameworkAim.SetupGet(x => x.EndDate).Returns(DateTime.Parse(end));

            var sut = NewRule();

            // act
            var result = sut.IsCurrent(frameworkAim.Object, testDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// this isn't going to happen
        /// [InlineData("2016-03-31", "2016-04-01", "2017-04-01", Skip = "no lower range checks till next year (19/20)")] // outside lower limit
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [Theory]
        [InlineData("2017-04-02", "2016-04-01", "2017-04-01")] // outside upper limit
        [InlineData("2016-04-01", "2016-04-01", "2016-03-31")] // withdrawn lower limit
        [InlineData("2017-04-01", "2016-04-01", "2016-03-31")] // withdrawn upper limit
        public void InvalidItemRaisesValidationMessage(string candidate, string start, string end)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "shonkyRefCode"; // <= any old code for the purpose of the test...

            var testDate = DateTime.Parse(candidate);
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);

            // these are random and meaningless values
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(2);
            delivery
                .SetupGet(x => x.FworkCodeNullable)
                .Returns(3);
            delivery
                .SetupGet(x => x.PwayCodeNullable)
                .Returns(4);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnStartDate_07, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("PwayCode", 4))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", 2))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FworkCode", 3))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            ddRule04
                .Setup(x => x.GetEarliesStartDateFor(delivery.Object, deliveries))
                .Returns(testDate);

            var startDate = DateTime.Parse(start);
            var endDate = GetNullableDate(end);

            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim
                .SetupGet(x => x.ProgType)
                .Returns(2);
            frameworkAim
                .SetupGet(x => x.FworkCode)
                .Returns(3);
            frameworkAim
                .SetupGet(x => x.PwayCode)
                .Returns(4);
            frameworkAim
                .SetupGet(x => x.StartDate)
                .Returns(startDate);
            frameworkAim
                .SetupGet(x => x.EndDate)
                .Returns(endDate);

            var larsDelivery = new Mock<ILARSLearningDelivery>();
            larsDelivery
                .SetupGet(x => x.FrameworkCommonComponent)
                .Returns(TypeOfLARSCommonComponent.Unknown);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { frameworkAim.Object });
            larsData
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(larsDelivery.Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.InApprenticeship(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(delivery.Object))
                .Returns(false);

            var sut = new LearnStartDate_07Rule(handler.Object, ddRule04.Object, larsData.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [Theory]
        [InlineData("2016-08-02", "2016-04-01", "2017-04-01")] // inside
        [InlineData("2016-04-01", "2016-04-01", "2017-04-01")] // lower limit
        [InlineData("2017-04-01", "2016-04-01", "2017-04-01")] // upper limit
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, string start, string end)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "shonkyRefCode"; // <= any old code for the purpose of the test...

            var testDate = DateTime.Parse(candidate);
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);

            // these are random and meaningless values
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(2);
            delivery
                .SetupGet(x => x.FworkCodeNullable)
                .Returns(3);
            delivery
                .SetupGet(x => x.PwayCodeNullable)
                .Returns(4);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            ddRule04
                .Setup(x => x.GetEarliesStartDateFor(delivery.Object, deliveries))
                .Returns(testDate);

            var startDate = DateTime.Parse(start);
            var endDate = GetNullableDate(end);

            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim
                .SetupGet(x => x.ProgType)
                .Returns(2);
            frameworkAim
                .SetupGet(x => x.FworkCode)
                .Returns(3);
            frameworkAim
                .SetupGet(x => x.PwayCode)
                .Returns(4);
            frameworkAim
                .SetupGet(x => x.StartDate)
                .Returns(startDate);
            frameworkAim
                .SetupGet(x => x.EndDate)
                .Returns(endDate);

            var larsDelivery = new Mock<ILARSLearningDelivery>();
            larsDelivery
                .SetupGet(x => x.FrameworkCommonComponent)
                .Returns(TypeOfLARSCommonComponent.Unknown);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { frameworkAim.Object });
            larsData
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(larsDelivery.Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.InApprenticeship(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(delivery.Object))
                .Returns(false);

            var sut = new LearnStartDate_07Rule(handler.Object, ddRule04.Object, larsData.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();
        }

        [Fact]
        public void Bug84886_RaisesValidationMessage()
        {
            // arrange
            const string learnRefNumber = "0LstartDt07";
            const string learnAimRef = "50104767";
            const int pwayCode = 0;
            const int progType = 2;
            const int fworkCode = 420;

            var testDate = DateTime.Parse("2018-10-14");
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            delivery
                .SetupGet(x => x.AimType)
                .Returns(3);
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);
            delivery
                .SetupGet(x => x.FworkCodeNullable)
                .Returns(fworkCode);
            delivery
                .SetupGet(x => x.PwayCodeNullable)
                .Returns(pwayCode);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnStartDate_07, learnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("PwayCode", pwayCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", progType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FworkCode", fworkCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            ddRule04
                .Setup(x => x.GetEarliesStartDateFor(delivery.Object, deliveries))
                .Returns(testDate);

            var startDate = DateTime.Parse("2011-06-06");
            var endDate = DateTime.Parse("2013-03-06");

            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim
                .SetupGet(x => x.ProgType)
                .Returns(progType);
            frameworkAim
                .SetupGet(x => x.FworkCode)
                .Returns(fworkCode);
            frameworkAim
                .SetupGet(x => x.PwayCode)
                .Returns(pwayCode);
            frameworkAim
                .SetupGet(x => x.StartDate)
                .Returns(startDate);
            frameworkAim
                .SetupGet(x => x.EndDate)
                .Returns(endDate);

            var larsDelivery = new Mock<ILARSLearningDelivery>();
            larsDelivery
                .SetupGet(x => x.FrameworkCommonComponent)
                .Returns(TypeOfLARSCommonComponent.NotApplicable);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { frameworkAim.Object });
            larsData
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(larsDelivery.Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.InApprenticeship(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(delivery.Object))
                .Returns(false);

            var sut = new LearnStartDate_07Rule(handler.Object, ddRule04.Object, larsData.Object, commonOps.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Shonky learn aim reference not crash.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        [Theory]
        [InlineData("2016-08-02", "2016-04-01", "2017-04-01")] // inside
        public void ShonkyLearnAimRefNotCrash(string candidate, string start, string end)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "shonkyRefCode"; // <= any old code for the purpose of the test...
            var testDate = DateTime.Parse(candidate);
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);

            // these are random and meaningless values
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(2);
            delivery
                .SetupGet(x => x.FworkCodeNullable)
                .Returns(3);
            delivery
                .SetupGet(x => x.PwayCodeNullable)
                .Returns(4);
            var deliveries = new ILearningDelivery[] { delivery.Object };
            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            ddRule04
                .Setup(x => x.GetEarliesStartDateFor(delivery.Object, deliveries))
                .Returns(testDate);

            var startDate = DateTime.Parse(start);
            var endDate = GetNullableDate(end);

            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim
                .SetupGet(x => x.ProgType)
                .Returns(2);
            frameworkAim
                .SetupGet(x => x.FworkCode)
                .Returns(3);
            frameworkAim
                .SetupGet(x => x.PwayCode)
                .Returns(4);
            frameworkAim
                .SetupGet(x => x.StartDate)
                .Returns(startDate);
            frameworkAim
                .SetupGet(x => x.EndDate)
                .Returns(endDate);

            var larsDelivery = new Mock<ILARSLearningDelivery>();
            larsDelivery
                .SetupGet(x => x.FrameworkCommonComponent)
                .Returns(TypeOfLARSCommonComponent.Unknown);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns((ILARSLearningDelivery)null);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { frameworkAim.Object });

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.InApprenticeship(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(delivery.Object))
                .Returns(false);

            var sut = new LearnStartDate_07Rule(handler.Object, ddRule04.Object, larsData.Object, commonOps.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();
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
        public LearnStartDate_07Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnStartDate_07Rule(handler.Object, ddRule04.Object, larsData.Object, commonOps.Object);
        }
    }
}
