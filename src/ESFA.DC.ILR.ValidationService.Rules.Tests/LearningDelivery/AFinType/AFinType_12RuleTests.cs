using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// </summary>
    public class AFinType_12RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new AFinType_12Rule(null));
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
            Assert.Equal("AFinType_12", result);
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
            Assert.Equal(AFinType_12Rule.Name, result);
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
        /// Condition met with null learning delivery returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with learning delivery and null financial records returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryAndNullFinancialRecordsReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mock = new Mock<ILearningDelivery>();

            // act
            var result = sut.ConditionMet(mock.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with learning delivery and no financial records returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryAndNoFinancialRecordsReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(new List<IAppFinRecord>());

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with learning delivery and no matching financial records returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryAndNoMatchingFinancialRecordsReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            var mockFinRec = new Mock<IAppFinRecord>();

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with learning delivery and matching financial record returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryAndMatchingFinancialRecordReturnsTrue()
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.ApprenticeshipsFrom1May2017);

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinType)
                .Returns(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice);

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
               .SetupGet(x => x.AimSeqNumber)
               .Returns(AimSeqNumber);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.ApprenticeshipsFrom1May2017);

            var mockFinRec = new Mock<IAppFinRecord>();
            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mockLearner.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == AFinType_12Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                AimSeqNumber,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "AFinType"),
                    Moq.It.Is<string>(y => y == "TNP")))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new AFinType_12Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.ApprenticeshipsFrom1May2017);

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinType)
                .Returns(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice);

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mockLearner.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new AFinType_12Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public AFinType_12Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new AFinType_12Rule(mock.Object);
        }
    }
}
