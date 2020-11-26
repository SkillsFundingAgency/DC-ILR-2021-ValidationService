namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationRule<T>
    {
        string RuleName { get; }

        void Validate(T objectToValidate);
    }
}