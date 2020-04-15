using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    public class AFinType_10RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("AFinType_10", result);
        }

        [Fact]
        public void ConditionMetWithNullFinancialRecordReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null);

            Assert.True(result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, true)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel6, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(TypeOfLearningProgramme.IntermediateLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.Traineeship, false)]
        public void IsTargetApprenticeshipMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);

            var result = sut.IsTargetApprenticeship(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfAim.AimNotPartOfAProgramme, false)]
        [InlineData(TypeOfAim.CoreAim16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfAim.ProgrammeAim, true)]
        [InlineData(TypeOfAim.ComponentAimInAProgramme, false)]
        [InlineData(2, false)]
        public void IsInAProgrammeMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(candidate);

            var result = sut.IsInAProgramme(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, true)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, true)]
        [InlineData(TypeOfFunding.CommunityLearning, true)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, true)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.Other16To19, true)]
        [InlineData(TypeOfFunding.OtherAdult, true)]
        public void IsFundedMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            var result = sut.IsFunded(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, 1, false)]
        [InlineData(null, 2, false)]
        [InlineData(null, 3, false)]
        [InlineData(null, 4, false)]
        [InlineData("", 1, false)]
        [InlineData("", 2, false)]
        [InlineData("", 3, false)]
        [InlineData("", 4, false)]
        [InlineData(ApprenticeshipFinancialRecord.Types.PaymentRecord, 1, false)]
        [InlineData(ApprenticeshipFinancialRecord.Types.PaymentRecord, 2, false)]
        [InlineData(ApprenticeshipFinancialRecord.Types.PaymentRecord, 3, false)]
        [InlineData(ApprenticeshipFinancialRecord.Types.PaymentRecord, 4, false)]
        [InlineData(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, 1, false)]
        [InlineData(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, 2, true)]
        [InlineData(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, 3, false)]
        [InlineData(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, 4, true)]
        public void ConditionMetWithFinancialRecordMeetsExpectation(string candidateType, int candidateCode, bool expectation)
        {
            var sut = NewRule();
            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinType)
                .Returns(candidateType);
            mockFinRec
                .SetupGet(x => x.AFinCode)
                .Returns(candidateCode);

            var result = sut.ConditionMet(mockFinRec.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("PMR1", "PMR2")]
        [InlineData("TNP1", "TNP3")]
        [InlineData("PMR1", "TNP1", "PMR2", "TNP3")]
        [InlineData("PMR1", "TNP1", "TNP3", "PMR2", "PMR3")]
        public void InvalidItemRaisesValidationMessage(params string[] candidates)
        {
            const string LearnRefNumber = "123456789X";

            var records = new List<IAppFinRecord>();
            candidates.ForEach(x =>
            {
                var candidateType = x.Substring(0, 3);
                var candidateCode = int.Parse(x.Substring(3));

                var mockFinRec = new Mock<IAppFinRecord>();
                mockFinRec
                    .SetupGet(y => y.AFinType)
                    .Returns(candidateType);
                mockFinRec
                    .SetupGet(y => y.AFinCode)
                    .Returns(candidateCode);

                records.Add(mockFinRec.Object);
            });

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.ApprenticeshipStandard);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.ApprenticeshipsFrom1May2017);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == AFinType_10Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            var sut = new AFinType_10Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Theory]
        [InlineData("PMR1", "PMR2", "TNP2")]
        [InlineData("TNP1", "TNP3", "TNP4")]
        [InlineData("PMR1", "TNP4", "TNP1", "PMR2", "TNP3")]
        [InlineData("PMR1", "TNP1", "TNP3", "TNP2", "PMR2", "PMR3")]
        public void ValidItemDoesNotRaiseAValidationMessage(params string[] candidates)
        {
            const string LearnRefNumber = "123456789X";

            var records = new List<IAppFinRecord>();
            candidates.ForEach(x =>
            {
                var candidateType = x.Substring(0, 3);
                var candidateCode = int.Parse(x.Substring(3));

                var mockFinRec = new Mock<IAppFinRecord>();
                mockFinRec
                    .SetupGet(y => y.AFinType)
                    .Returns(candidateType);
                mockFinRec
                    .SetupGet(y => y.AFinCode)
                    .Returns(candidateCode);

                records.Add(mockFinRec.Object);
            });

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.ApprenticeshipStandard);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.ApprenticeshipsFrom1May2017);
            mockDelivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new AFinType_10Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public AFinType_10Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new AFinType_10Rule(mock.Object);
        }
    }
}
