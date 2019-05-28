using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IPopulationService
    {
        Task PopulateAsync(IValidationContext validationContext,  CancellationToken cancellationToken);
    }
}
