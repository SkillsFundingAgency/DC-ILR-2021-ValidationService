using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IPostcodeQueryService : IQueryService
    {
        bool RegexValid(string postcode);
    }
}
