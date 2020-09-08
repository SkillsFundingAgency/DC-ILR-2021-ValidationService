namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRule<T> : IValidationRule<T>
        where T : class
    {
    }
}