using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpId_10RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpId_10", result);
        }

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, true, true)]
        public void IsApprenticeshipMeetsExpectation(bool isApprenctice, bool isProgramAim, bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.InApprenticeship(mockItem.Object))
                .Returns(isApprenctice);

            if (isApprenctice)
            {
                commonOps
                    .Setup(x => x.InAProgramme(mockItem.Object))
                    .Returns(isProgramAim);
            }

            var sut = new EmpId_10Rule(handler.Object, commonOps.Object);

            var result = sut.IsPrimaryLearningAim(mockItem.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        [Theory]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, true)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, false)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, false)]
        [InlineData(TypeOfEmploymentStatus.NotKnownProvided, false)]
        public void HasQualifyingEmploymentStatusMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            var result = sut.HasQualifyingEmploymentStatus(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(1004, false)]
        public void HasQualifyingEmployerMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(y => y.EmpIdNullable)
                .Returns(candidate);

            var result = sut.HasDisqualifyingEmployerID(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2013-08-01");

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.DateEmpStatApp)
                .Returns(testDate);
            status
                .SetupGet(x => x.EmpStat)
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);

            var statii = new ILearnerEmploymentStatus[] { status.Object };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("EmpId_10", LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("EmpId", "(missing)"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("EmpStat", TypeOfEmploymentStatus.InPaidEmployment))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(testDate, statii))
                .Returns(status.Object);
            commonOps
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.InAProgramme(mockDelivery.Object))
                .Returns(true);

            var sut = new EmpId_10Rule(handler.Object, commonOps.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2013-08-01");

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.EmpIdNullable)
                .Returns(10004);
            status
                .SetupGet(x => x.EmpStat)
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);

            var statii = new ILearnerEmploymentStatus[] { status.Object };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(testDate, statii))
                .Returns(status.Object);
            commonOps
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.InAProgramme(mockDelivery.Object))
                .Returns(true);

            var sut = new EmpId_10Rule(handler.Object, commonOps.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        public EmpId_10Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new EmpId_10Rule(handler.Object, commonOps.Object);
        }
    }
}