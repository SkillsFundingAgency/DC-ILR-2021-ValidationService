using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.ValidationDPActor.Context;
using ESFA.DC.ILR.ValidationService.ValidationDPActor.Interfaces.Models;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.ValidationActor.Context
{
    public class ValidationDPActorModelValidationContextFactory : IValidationContextFactory<ValidationDPActorModel>
    {
        private readonly IJsonSerializationService _jsonSerializationService;

        public ValidationDPActorModelValidationContextFactory(IJsonSerializationService jsonSerializationService)
        {
            _jsonSerializationService = jsonSerializationService;
        }

        public IValidationContext Build(ValidationDPActorModel context)
        {
            return new ValidationDPActorModelValidationContext();
        }
    }
}
