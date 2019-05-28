namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRule<in T>
        where T : class
    {
        string RuleName { get; }

        void Validate(T objectToValidate);
    }
}