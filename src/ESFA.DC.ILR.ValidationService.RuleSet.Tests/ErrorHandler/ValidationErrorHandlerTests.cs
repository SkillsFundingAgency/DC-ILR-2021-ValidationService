using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests.ErrorHandler
{
    public class ValidationErrorHandlerTests
    {
        [Fact]
        public void Handle()
        {
            var validationErrorCacheMock = new Mock<IValidationErrorCache>();
            var validationErrorsDataService = new Mock<IValidationErrorsDataService>();

            var validationErrorHandler = NewHandler(validationErrorCacheMock.Object, validationErrorsDataService.Object);

            var ruleName = "RuleName";

            validationErrorHandler.Handle(ruleName);

            validationErrorsDataService.Verify(c => c.SeverityForRuleName(ruleName), Times.Once);
            validationErrorCacheMock.Verify(c => c.Add(It.IsAny<IValidationError>()), Times.Once);
        }

        [Fact]
        public void BuildValidationError_RuleName()
        {
            var validationErrorHandler = NewHandler();

            var validationError = validationErrorHandler.BuildValidationError("RuleName", null, null, null, null);

            validationError.RuleName.Should().Be("RuleName");
            validationError.LearnerReferenceNumber.Should().BeNull();
            validationError.AimSequenceNumber.Should().BeNull();
            validationError.Severity.Should().BeNull();
            validationError.ErrorMessageParameters.Should().BeNull();
        }

        [Fact]
        public void BuildValidationError_LearnRefNumber()
        {
            var validationErrorHandler = NewHandler();

            var validationError = validationErrorHandler.BuildValidationError("RuleName", "LearnRefNumber", null, null, null);

            validationError.RuleName.Should().Be("RuleName");
            validationError.LearnerReferenceNumber.Should().Be("LearnRefNumber");
            validationError.AimSequenceNumber.Should().BeNull();
            validationError.Severity.Should().BeNull();
            validationError.ErrorMessageParameters.Should().BeNull();
        }

        [Fact]
        public void BuildValidationError_AimSequenceNumber()
        {
            var validationErrorHandler = NewHandler();

            var validationError = validationErrorHandler.BuildValidationError("RuleName", "LearnRefNumber", 1234, null, null);

            validationError.RuleName.Should().Be("RuleName");
            validationError.LearnerReferenceNumber.Should().Be("LearnRefNumber");
            validationError.AimSequenceNumber.Should().Be(1234);
            validationError.Severity.Should().BeNull();
            validationError.ErrorMessageParameters.Should().BeNull();
        }

        [Fact]
        public void BuildValidationError_Severity()
        {
            var validationErrorHandler = NewHandler();

            var validationError = validationErrorHandler.BuildValidationError("RuleName", "LearnRefNumber", 1234, Severity.Error, null);

            validationError.RuleName.Should().Be("RuleName");
            validationError.LearnerReferenceNumber.Should().Be("LearnRefNumber");
            validationError.AimSequenceNumber.Should().Be(1234);
            validationError.Severity.Should().Be(Severity.Error);
            validationError.ErrorMessageParameters.Should().BeNull();
        }

        [Fact]
        public void BuildvalidationError_ErrorMessageParameters()
        {
            var validationErrorHandler = NewHandler();

            var validationError = validationErrorHandler.BuildValidationError("RuleName", "LearnRefNumber", 1234, Severity.Error, new List<IErrorMessageParameter>() { new ErrorMessageParameter("A", "error"), new ErrorMessageParameter("B", "message"), new ErrorMessageParameter("C", "parameters") });

            validationError.RuleName.Should().Be("RuleName");
            validationError.LearnerReferenceNumber.Should().Be("LearnRefNumber");
            validationError.AimSequenceNumber.Should().Be(1234);
            validationError.ErrorMessageParameters.Should().HaveCount(3);
            validationError.Severity.Should().Be(Severity.Error);
            validationError.ErrorMessageParameters.Should().Equal(new List<IErrorMessageParameter>() { new ErrorMessageParameter("A", "error"), new ErrorMessageParameter("B", "message"), new ErrorMessageParameter("C", "parameters") });
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var errorMessageParameter = NewHandler().BuildErrorMessageParameter("Property", "Value");

            errorMessageParameter.PropertyName.Should().Be("Property");
            errorMessageParameter.Value.Should().Be("Value");
        }

        [Fact]
        public void BuildErrorMessageParameters_ToString()
        {
            var errorMessageParameter = NewHandler().BuildErrorMessageParameter("Property", 123);

            errorMessageParameter.PropertyName.Should().Be("Property");
            errorMessageParameter.Value.Should().Be("123");
        }

        [Fact]
        public void BuildErrorMessageParameters_Null()
        {
            var errorMessageParameter = NewHandler().BuildErrorMessageParameter("Property", default(int?));

            errorMessageParameter.PropertyName.Should().Be("Property");
            errorMessageParameter.Value.Should().Be(null);
        }

        private ValidationErrorHandler NewHandler(IValidationErrorCache validationErrorCache = null, IValidationErrorsDataService validationErrorsDataService = null)
        {
            return new ValidationErrorHandler(validationErrorCache, validationErrorsDataService);
        }
    }
}
