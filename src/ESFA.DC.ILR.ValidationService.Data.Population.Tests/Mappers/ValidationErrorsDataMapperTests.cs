using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using FluentAssertions;
using Xunit;
using SeverityLevel = ESFA.DC.ILR.ReferenceDataService.Model.MetaData.ValidationError.SeverityLevel;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.Mappers
{
    public class ValidationErrorsDataMapperTests
    {
        [Fact]
        public void MapValidationErrors()
        {
            var validationErrors = TestValidationErrors();

            var expectedValidationErrors = new Dictionary<string, ValidationError>
            {
                { "Rule1", new ValidationError { RuleName = "Rule1", Severity = Severity.Error, Message = "Message1" } },
                { "Rule2", new ValidationError { RuleName = "Rule2", Severity = Severity.Warning, Message = "Message2" } },
                { "Rule3", new ValidationError { RuleName = "Rule3", Severity = Severity.Error, Message = "Message3" } },
                { "Rule4", new ValidationError { RuleName = "Rule4", Severity = Severity.Error, Message = "Message4" } }
            };

            NewMapper().MapValidationErrors(validationErrors).Should().BeEquivalentTo(expectedValidationErrors);
        }

        private IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> TestValidationErrors()
        {
            return new List<ReferenceDataService.Model.MetaData.ValidationError>
            {
                new ReferenceDataService.Model.MetaData.ValidationError { RuleName = "Rule1", Severity = SeverityLevel.Error, Message = "Message1" },
                new ReferenceDataService.Model.MetaData.ValidationError { RuleName = "Rule2", Severity = SeverityLevel.Warning, Message = "Message2" },
                new ReferenceDataService.Model.MetaData.ValidationError { RuleName = "Rule3", Severity = SeverityLevel.Error, Message = "Message3" },
                new ReferenceDataService.Model.MetaData.ValidationError { RuleName = "Rule4", Severity = SeverityLevel.Error, Message = "Message4" },
            };
        }

        private ValidationErrorsDataMapper NewMapper()
        {
            return new ValidationErrorsDataMapper();
        }
    }
}
