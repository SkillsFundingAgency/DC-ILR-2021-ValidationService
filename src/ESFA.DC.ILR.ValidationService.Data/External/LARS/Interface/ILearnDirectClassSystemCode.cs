using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    public interface ILearnDirectClassSystemCode
    {
        string Code { get; }
    }

    public static class LearnDirectClassSystemCodeHelper
    {
        public static bool IsKnown(this ILearnDirectClassSystemCode source) =>
            source != null
                && !string.IsNullOrWhiteSpace(source.Code)
                && !source.Code.CaseInsensitiveEquals("NUL");
    }
}
