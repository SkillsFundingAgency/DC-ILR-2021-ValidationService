using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinDate
{
    public class AFinDate_03RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("AFinDate_03", result);
        }

        [Theory]
        [InlineData("2016-04-01", "2016-04-02", false)]
        [InlineData("2016-04-01", "2016-03-31", true)]
        [InlineData("2016-04-30", "2016-05-01", false)]
        [InlineData("2016-05-01", "2016-05-01", false)]
        [InlineData("2016-05-02", "2016-05-01", true)]
        public void HasInvalidFinancialDateMeetsExpectation(string finDate, string fileDate, bool expectation)
        {
            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(DateTime.Parse(finDate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(DateTime.Parse(fileDate));

            var sut = new AFinDate_03Rule(handler.Object, fileData.Object);

            var result = sut.HasInvalidFinancialDate(mockFinRec.Object);

            Assert.Equal(expectation, result);
            handler.VerifyAll();
            fileData.VerifyAll();
        }

        [Theory]
        [InlineData("2016-04-01", "2016-03-31")]
        [InlineData("2016-05-02", "2016-05-01")]
        public void InvalidItemRaisesValidationMessage(string finDate, string fileDate)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(finDate);
            var preparationDate = DateTime.Parse(fileDate);

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(testDate);

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.AFinDate_03, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AFinDate", testDate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FilePrepDate", preparationDate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(preparationDate);

            var sut = new AFinDate_03Rule(handler.Object, fileData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            fileData.VerifyAll();
        }

        [Theory]
        [InlineData("2016-04-01", "2016-04-02")]
        [InlineData("2016-04-30", "2016-05-01")]
        [InlineData("2016-05-01", "2016-05-01")]
        public void ValidItemDoesNotRaiseAValidationMessage(string finDate, string fileDate)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(finDate);
            var preparationDate = DateTime.Parse(fileDate);

            var mockFinRec = new Mock<IAppFinRecord>();
            mockFinRec
                .SetupGet(x => x.AFinDate)
                .Returns(testDate);

            var records = new List<IAppFinRecord>();
            records.Add(mockFinRec.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(preparationDate);

            var sut = new AFinDate_03Rule(handler.Object, fileData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            fileData.VerifyAll();
        }

        public AFinDate_03Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);

            return new AFinDate_03Rule(handler.Object, fileData.Object);
        }
    }
}
