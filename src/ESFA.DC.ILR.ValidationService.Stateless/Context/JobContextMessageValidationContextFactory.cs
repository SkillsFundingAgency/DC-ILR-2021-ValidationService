using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.Context
{
    public class JobContextMessageValidationContextFactory : IValidationContextFactory<IJobContextMessage>
    {
        public IValidationContext Build(IJobContextMessage context)
        {
            return new JobContextMessageValidationContext(context);
        }
    }
}
