namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationContextFactory<in T>
    {
        IValidationContext Build(T context);
    }
}
