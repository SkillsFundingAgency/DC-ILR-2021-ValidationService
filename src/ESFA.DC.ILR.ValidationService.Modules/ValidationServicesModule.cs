using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ValidationServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMock<IRuleSetOrchestrationService<IMessage, IValidationError>>();
        }
    }
}
