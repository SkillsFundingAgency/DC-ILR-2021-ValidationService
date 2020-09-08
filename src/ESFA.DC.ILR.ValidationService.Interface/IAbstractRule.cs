namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IAbstractRule<T>
    {
        string RuleName { get; }

        void Validate(T objectToValidate);
    }
}