namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface ICrossYearRule<in T>
        where T : class
    {
        string RuleName { get; }

        void Validate(T objectToValidate);
    }
}