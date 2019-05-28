using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.IO.Model.Validation;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.IO.Model;
using ESFA.DC.ILR.ValidationService.Providers.Output;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class ValidationOutputServiceTests
    {
        [Fact]
        public void SeverityToString_Warning()
        {
            NewService().SeverityToString(Severity.Warning).Should().Be("W");
        }

        [Fact]
        public void SeverityToString_Error()
        {
            NewService().SeverityToString(Severity.Error).Should().Be("E");
        }

        [Fact]
        public void SeverityToString_Fail()
        {
            NewService().SeverityToString(Severity.Fail).Should().Be("F");
        }

        [Fact]
        public void SeverityToString_Null()
        {
            NewService().SeverityToString(null).Should().BeNull();
        }

        [Fact]
        public void BuildInvalidLearnRefNumbers()
        {
            var validationErrors = new List<ValidationError>()
            {
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "a", Severity = "E" },
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "a", Severity = "E" },
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "b", Severity = "E" },
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "b", Severity = "W" },
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "c", Severity = "W" },
                new ValidationError() { RuleName = string.Empty, LearnerReferenceNumber = "c", Severity = "W" },
            };
            
            NewService().BuildInvalidLearnRefNumbers(validationErrors).Should().BeEquivalentTo("a", "b");
        }

        [Fact]
        public void BuildValidLearnRefNumbers()
        {
            var invalidLearnRefNumbers = new List<string>() { "a", "b" };

            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner() { LearnRefNumber = "a" },
                    new TestLearner() { LearnRefNumber = "b" },
                    new TestLearner() { LearnRefNumber = "c" },
                    new TestLearner() { LearnRefNumber = "d" },
                    new TestLearner() { LearnRefNumber = "e" },
                }
            };

            NewService().BuildValidLearnRefNumbers(message, invalidLearnRefNumbers).Should().BeEquivalentTo("c", "d", "e");
        }

        [Fact]
        public void BuildValidLearnRefNumbers_No_InvalidLearners()
        {
            var invalidLearnRefNumbers = new List<string>();

            var message = new TestMessage()
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner() { LearnRefNumber = "a" },
                    new TestLearner() { LearnRefNumber = "b" },
                }
            };

            NewService().BuildValidLearnRefNumbers(message, invalidLearnRefNumbers).Should().HaveCount(2);
        }

        [Fact]
        public void BuildValidLearnRefNumbers_NullMessage()
        {
            NewService().BuildValidLearnRefNumbers(null, new List<string>()).Should().HaveCount(0);
        }

        [Fact]
        public void BuildValidLearnRefNumbers_NullLearners()
        {
            NewService().BuildValidLearnRefNumbers(new TestMessage(), new List<string>()).Should().HaveCount(0);
        }


        [Fact]
        public async Task SaveAsync()
        {
            var serializedValidLearners = "Serialized Valid Learners";
            var serializedInvalidLearners = "Serialized Invalid Learners";
            var serializedValidationErrors = "Serialized Validation Errors";
            var serializedValidationErrorMessageLookups = "Serialized Validation Error Message Lookups";

            var validLearnRefNumbersKey = "Valid Learn Ref Numbers Key";
            var invalidLearnRefNumbersKey = "Invalid Learn Ref Numbers Key";
            var validationErrorsKey = "Validation Errors Key";
            var validationErrorMessageLookupsKey = "Validation Error Message Lookups Key";
            var container = "Container";

            IEnumerable<string> validLearnerRefNumbers = new List<string>() { "a", "b", "c" };
            IEnumerable<string> invalidLearnerRefNumbers = new List<string>() { "d", "e", "f" };
            IEnumerable<ValidationError> validationErrors = new List<ValidationError>() { new ValidationError(), new ValidationError(), new ValidationError() };
            IEnumerable<ValidationErrorMessageLookup> validationErrorMessageLookups = new List<ValidationErrorMessageLookup> { new ValidationErrorMessageLookup(), new ValidationErrorMessageLookup(), new ValidationErrorMessageLookup() };

            var serializationServiceMock = new Mock<IJsonSerializationService>();
            var preValidationContextMock = new Mock<IValidationContext>();
            var fileServiceMock = new Mock<IFileService>();

            serializationServiceMock.Setup(s => s.Serialize(validLearnerRefNumbers)).Returns(serializedValidLearners);
            serializationServiceMock.Setup(s => s.Serialize(invalidLearnerRefNumbers)).Returns(serializedInvalidLearners);
            serializationServiceMock.Setup(s => s.Serialize(validationErrors)).Returns(serializedValidationErrors);
            serializationServiceMock.Setup(s => s.Serialize(validationErrorMessageLookups)).Returns(serializedValidationErrorMessageLookups);

            preValidationContextMock.SetupGet(c => c.ValidLearnRefNumbersKey).Returns(validLearnRefNumbersKey);
            preValidationContextMock.SetupGet(c => c.InvalidLearnRefNumbersKey).Returns(invalidLearnRefNumbersKey);
            preValidationContextMock.SetupGet(c => c.ValidationErrorsKey).Returns(validationErrorsKey);
            preValidationContextMock.SetupGet(c => c.ValidationErrorMessageLookupKey).Returns(validationErrorMessageLookupsKey);
            preValidationContextMock.SetupGet(c => c.Container).Returns(container);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("Stream")))
            {
                fileServiceMock.Setup(s => s.OpenWriteStreamAsync(validLearnRefNumbersKey, container, default(CancellationToken))).ReturnsAsync(stream).Verifiable();
                fileServiceMock.Setup(s => s.OpenWriteStreamAsync(invalidLearnRefNumbersKey, container, default(CancellationToken))).ReturnsAsync(stream).Verifiable();
                fileServiceMock.Setup(s => s.OpenWriteStreamAsync(validationErrorsKey, container, default(CancellationToken))).ReturnsAsync(stream).Verifiable();
                fileServiceMock.Setup(s => s.OpenWriteStreamAsync(validationErrorMessageLookupsKey, container, default(CancellationToken))).ReturnsAsync(stream).Verifiable();

                var service = NewService(
                    fileService: fileServiceMock.Object,
                    preValidationContext: preValidationContextMock.Object,
                    jsonSerializationService: serializationServiceMock.Object);

                await service.SaveAsync(validLearnerRefNumbers, invalidLearnerRefNumbers, validationErrors, validationErrorMessageLookups, CancellationToken.None);

                fileServiceMock.VerifyAll();
            }

         
        }

        private ValidationOutputService NewService(
            IValidationErrorCache<IValidationError> validationErrorCache = null,
            ICache<IMessage> messageCache = null,
            IFileService fileService = null,
            IValidationContext preValidationContext = null,
            IJsonSerializationService jsonSerializationService = null,
            IValidationErrorsDataService validationErrorsDataService = null
            )
        {
            return new ValidationOutputService(
                validationErrorCache,
                messageCache,
                fileService,
                preValidationContext,
                jsonSerializationService,
                validationErrorsDataService,
                new Mock<ILogger>().Object);
        }
    }
}
