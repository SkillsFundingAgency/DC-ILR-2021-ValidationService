using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_79RuleTests
    {
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_79Rule(null, service.Object, commonChecks.Object));
        }

        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_79Rule(handler.Object, null, commonChecks.Object));
        }

        [Fact]
        public void NewRuleWithNullDerivedData07Throws()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_79Rule(handler.Object, service.Object, null));
        }

        [Fact]
        public void RuleName1()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnAimRef_79", result);
        }

        [Fact]
        public void RuleName2()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal(LearnAimRef_79Rule.Name, result);
        }

        [Fact]
        public void RuleName3()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            var sut = NewRule();

            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            var result = LearnAimRef_79Rule.FirstViableDate;

            Assert.Equal(DateTime.Parse("2016-08-01"), result);
        }

        [Theory]
        [InlineData(LARSNotionalNVQLevelV2.EntryLevel, true)]
        [InlineData(LARSNotionalNVQLevelV2.HigherLevel, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level1, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level1_2, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level2, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level3, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level4, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level5, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level6, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level7, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level8, true)]
        [InlineData(LARSNotionalNVQLevelV2.MixedLevel, true)]
        [InlineData(LARSNotionalNVQLevelV2.NotKnown, true)]
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

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnAimRef_79Rule(handler.Object, service.Object, commonChecks.Object);

            var result = sut.HasQualifyingNotionalNVQ(mockDelivery.Object);

            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();

            Assert.True(result);
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
                .Returns(TypeOfFunding.AdultSkills);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("LearnAimRef_79", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", TypeOfFunding.AdultSkills))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockLars = new Mock<ILARSLearningDelivery>();
            mockLars
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level4);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLars.Object);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.IsSteelWorkerRedundancyTraining(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.AdultSkills))
                .Returns(true);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, LearnAimRef_79Rule.FirstViableDate, null))
                .Returns(true);

            var sut = new LearnAimRef_79Rule(handler.Object, service.Object, commonChecks.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
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
                .Returns(TypeOfFunding.AdultSkills);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockLars = new Mock<ILARSLearningDelivery>();
            mockLars
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level3);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLars.Object);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.IsSteelWorkerRedundancyTraining(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.AdultSkills))
                .Returns(true);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, LearnAimRef_79Rule.FirstViableDate, null))
                .Returns(true);

            var sut = new LearnAimRef_79Rule(handler.Object, service.Object, commonChecks.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
        }

        public LearnAimRef_79Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnAimRef_79Rule(handler.Object, service.Object, commonChecks.Object);
        }
    }
}
