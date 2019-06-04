using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Moq;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterMock<T>(this ContainerBuilder containerBuilder) 
            where T : class
        {
            var mock = Mock.Of<T>();

            containerBuilder.RegisterInstance(mock).As<T>();
        }
    }
}
