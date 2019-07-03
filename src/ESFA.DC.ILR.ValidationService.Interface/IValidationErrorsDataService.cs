using ESFA.DC.ILR.ValidationService.Interface.Enum;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationErrorsDataService : IDataService
    {
        Severity? SeverityForRuleName(string ruleName);

        string MessageforRuleName(string ruleName);
    }
}
