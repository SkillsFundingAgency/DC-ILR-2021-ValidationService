using ESFA.DC.ILR.Model.Interface;
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
    public class LearnStartDate_07RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnStartDate_07", result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("2018-06-04")]
        [InlineData("2018-08-06")]
        public void GetEarliestStartDateForMeetsExpectation(string candidate)
        {
            var testDate = GetNullableDate(candidate);
            var delivery = new Mock<ILearningDelivery>();
            var deliveries = new ILearningDelivery[] { };

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            ddRule04
                .Setup(x => x.GetEarliesStartDateFor(delivery.Object, deliveries))
                .Returns(testDate);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = NewRule(handler.Object, ddRule04.Object, larsData.Object, learningDeliveryFAMQS.Object);

            var result = sut.GetEarliestStartDateFor(delivery.Object, deliveries);

            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();

            Assert.Equal(testDate, result);
        }

        [Theory]
        [InlineData(25, true)]
        [InlineData(1, false)]
        public void IsStandardApprenticeshipMeetsExpectation(int? progType, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var sut = NewRule(handler.Object, ddRule04.Object, larsData.Object, learningDeliveryFAMQS.Object);

            var result = sut.IsStandardApprenticeship(delivery.Object);

            handler.VerifyAll();
            ddRule04.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsRestartMeetsExpectation(bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule04 = new Mock<IDerivedData_04Rule>(MockBehavior.Strict);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMType(
                    delivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(expectation);

            var sut = NewRule(handler.Object, ddRule04.Object, larsData.Object, learningDeliveryFAMQS.Object);

            var result = sut.IsRestart(delivery.Object);

            handler.VerifyAll();
            ddRule04.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void IsCommonComponentWithNullLARSAimReturnsFalse()
        {
            var sut = NewRule();

            var result = sut.IsCommonComponent(null);

            Assert.False(result);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(0, false)]
        [InlineData(null, false)]
        public void IsCommonComponentMeetsExpectation(int? candidate, bool expectation)
        {
            var delivery = new Mock<ILARSLearningDelivery>();
            delivery
                .SetupGet(x => x.FrameworkCommonComponent)
                .Returns(candidate);

            var sut = NewRule();

            var result = sut.IsCommonComponent(delivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("shonkyRefCode1")]
        [InlineData("shonkyRefCode2")]
        [InlineData("shonkyRefCode3")]
        public void GetFrameworkAimsForMeetsExpectation(string learnAimRef)
        {
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

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var sut = NewRule(handler.Object, ddRule04.Object, larsData.Object, learningDeliveryFAMQS.Object);

            var result = sut.GetQualifyingFrameworksFor(delivery.Object);

            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();

            Assert.Null(result);
        }

        [Fact]
        public void FilteredFrameworkAimsForNullFrameworksMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.FilteredFrameworkAimsFor(null, null);

            Assert.Empty(result);
        }

        [Fact]
        public void FilteredFrameworkAimsForEmptyFrameworksMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.FilteredFrameworkAimsFor(null, new ILARSFrameworkAim[] { });

            Assert.Empty(result);
        }

        [Fact]
        public void FilteredFrameworkAimsForVanillaDeliveryAndEmptyFrameworksMeetsExpectation()
        {
            var delivery = new Mock<ILearningDelivery>();
            var sut = NewRule();

            var result = sut.FilteredFrameworkAimsFor(delivery.Object, new ILARSFrameworkAim[] { });

            Assert.Empty(result);
        }

        [Fact]
        public void FilteredFrameworkAimsForMeetsExpectation()
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery.SetupGet(x => x.ProgTypeNullable).Returns(2);
            delivery.SetupGet(x => x.FworkCodeNullable).Returns(3);
            delivery.SetupGet(x => x.PwayCodeNullable).Returns(4);

            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim.SetupGet(x => x.ProgType).Returns(2);
            frameworkAim.SetupGet(x => x.FworkCode).Returns(3);
            frameworkAim.SetupGet(x => x.PwayCode).Returns(4);

            var sut = NewRule();

            var result = sut.FilteredFrameworkAimsFor(delivery.Object, new ILARSFrameworkAim[] { frameworkAim.Object });

            Assert.Contains(result, x => x == frameworkAim.Object);
        }

        [Fact]
        public void IsOutOfScopeWithEmptyFrameworksTrue()
        {
            var sut = NewRule();

            var result = sut.IsOutOfScope(new ILARSFrameworkAim[] { });

            Assert.True(result);
        }

        [Fact]
        public void IsCurrentWithEmptyFrameworksFalse()
        {
            var sut = NewRule();

            var result = sut.IsCurrent(new ILARSFrameworkAim[] { }, DateTime.Today);

            Assert.False(result);
        }

        [Theory]
        [InlineData("2016-08-02", "2016-04-01", "2017-04-01", true)]
        [InlineData("2016-04-01", "2016-04-01", "2017-04-01", true)]
        [InlineData("2017-04-01", "2016-04-01", "2017-04-01", true)]
        [InlineData("2017-04-02", "2016-04-01", "2017-04-01", false)]
        [InlineData("2016-04-01", "2016-04-01", "2016-03-31", false)]
        [InlineData("2017-04-01", "2016-04-01", "2016-03-31", false)]
        public void IsCurrentMeetsExpectation(string candidate, string start, string end, bool expectation)
        {
            var testDate = DateTime.Parse(candidate);
            var frameworkAim = new Mock<ILARSFrameworkAim>();
            frameworkAim.SetupGet(x => x.StartDate).Returns(DateTime.Parse(start));
            frameworkAim.SetupGet(x => x.EndDate).Returns(DateTime.Parse(end));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.IsDateBetween(testDate, DateTime.MinValue, frameworkAim.Object.EndDate.Value, true)).Returns(expectation);

            var sut = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            var result = sut.IsCurrent(frameworkAim.Object, testDate);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2017-04-02", "2016-04-01", "2017-04-01")]
        [InlineData("2016-04-01", "2016-04-01", "2016-03-31")]
        [InlineData("2017-04-01", "2016-04-01", "2016-03-31")]
        public void InvalidItemRaisesValidationMessage(string candidate, string start, string end)
        {
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "shonkyRefCode";

            var testDate = DateTime.Parse(candidate);
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
                .Returns(LARSConstants.CommonComponents.Unknown);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { frameworkAim.Object });
            larsData
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(larsDelivery.Object);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMType(
                    delivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.IsDateBetween(testDate, DateTime.MinValue, frameworkAim.Object.EndDate.Value, true)).Returns(false);

            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            dd07
                .Setup(dd => dd.IsApprenticeship(delivery.Object.ProgTypeNullable)).Returns(true);

            var sut = NewRule(handler.Object, ddRule04.Object, larsData.Object, learningDeliveryFAMQS.Object, dateTimeQueryServiceMock.Object, dd07.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            dd07.VerifyAll();
        }

        [Theory]
        [InlineData("2016-08-02", "2016-04-01", "2017-04-01")]
        [InlineData("2016-04-01", "2016-04-01", "2017-04-01")]
        [InlineData("2017-04-01", "2016-04-01", "2017-04-01")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, string start, string end)
        {
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "shonkyRefCode";

            var testDate = DateTime.Parse(candidate);
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
                .Returns(LARSConstants.CommonComponents.Unknown);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { frameworkAim.Object });
            larsData
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(larsDelivery.Object);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMType(
                    delivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.IsDateBetween(testDate, DateTime.MinValue, frameworkAim.Object.EndDate.Value, true)).Returns(true);

            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            dd07
                .Setup(dd => dd.IsApprenticeship(delivery.Object.ProgTypeNullable)).Returns(true);

            var sut = NewRule(handler.Object, ddRule04.Object, larsData.Object, learningDeliveryFAMQS.Object, dateTimeQueryServiceMock.Object, dd07.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            dd07.VerifyAll();
        }

        [Fact]
        public void Bug84886_RaisesValidationMessage()
        {
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
                .Returns(LARSConstants.CommonComponents.NotApplicable);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { frameworkAim.Object });
            larsData
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(larsDelivery.Object);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMType(
                    delivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.IsDateBetween(testDate, DateTime.MinValue, frameworkAim.Object.EndDate.Value, true)).Returns(false);

            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            dd07
                .Setup(dd => dd.IsApprenticeship(delivery.Object.ProgTypeNullable)).Returns(true);

            var sut = NewRule(handler.Object, ddRule04.Object, larsData.Object, learningDeliveryFAMQS.Object, dateTimeQueryServiceMock.Object, dd07.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            dd07.VerifyAll();
        }

        [Theory]
        [InlineData("2016-08-02", "2016-04-01", "2017-04-01")]
        public void ShonkyLearnAimRefNotCrash(string candidate, string start, string end)
        {
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "shonkyRefCode";
            var testDate = DateTime.Parse(candidate);
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
                .Returns(LARSConstants.CommonComponents.Unknown);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns((ILARSLearningDelivery)null);
            larsData
                .Setup(x => x.GetFrameworkAimsFor(learnAimRef))
                .Returns(new ILARSFrameworkAim[] { frameworkAim.Object });

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                 .Setup(x => x.HasLearningDeliveryFAMType(
                     delivery.Object.LearningDeliveryFAMs,
                     "RES"))
                 .Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.IsDateBetween(testDate, DateTime.MinValue, frameworkAim.Object.EndDate.Value, true)).Returns(true);

            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            dd07
                .Setup(dd => dd.IsApprenticeship(delivery.Object.ProgTypeNullable)).Returns(true);

            var sut = NewRule(handler.Object, ddRule04.Object, larsData.Object, learningDeliveryFAMQS.Object, dateTimeQueryServiceMock.Object, dd07.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            ddRule04.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            dd07.VerifyAll();
        }

        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        public LearnStartDate_07Rule NewRule(
            IValidationErrorHandler handler = null,
            IDerivedData_04Rule ddRule04 = null,
            ILARSDataService larsData = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQS = null,
            IDateTimeQueryService dateTimeQueryService = null,
            IDerivedData_07Rule dd07 = null)
        {
            return new LearnStartDate_07Rule(
                handler,
                ddRule04,
                larsData,
                learningDeliveryFAMQS,
                dateTimeQueryService,
                dd07);
        }
    }
}
