using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules.Console;
using ESFA.DC.ILR.ValidationService.Stubs;

namespace ESFA.DC.ILR.ValidationService.Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var argsList = args.ToList();

            if (!argsList.Any())
            {
                argsList.Add(@"Files/ILR.xml");
            }

            RunValidation(argsList.First()).GetAwaiter().GetResult();
        }

        private static async Task RunValidation(string filePath)
        {
            var preValidationContext = new PreValidationContextStub
            {
                Filename = filePath,
                Output = filePath + ".vs.csv",
                ValidLearnRefNumbersKey = "ValidLearnRefNumbers",
                InvalidLearnRefNumbersKey = "InvalidLearnRefNumbers",
                ValidationErrorsKey = "ValidationErrors",
                ValidationErrorMessageLookupKey = "ValidationErrorMessageLookups",
            };

            var container = BuildContainer();

            using (var scope = container.BeginLifetimeScope(c => RegisterContext(c, preValidationContext)))
            {
                var preValidationOrchestrationService = scope.Resolve<IPreValidationOrchestrationService<IValidationError>>();

                await preValidationOrchestrationService.ExecuteAsync(preValidationContext, CancellationToken.None);

                // Todo: Get errors from elsewhere
                // OutputResultsToFile(errors, $"{preValidationContext.Output}"); -
            }

            System.Console.WriteLine($"{preValidationContext.Output}");
        }

        private static void OutputResultsToFile(IEnumerable<IValidationError> errors, string path)
        {
            StringBuilder contents = new StringBuilder();
            contents.AppendLine(@"Error\Warning,Learner Ref,Rule Name,Field Values,Error Message,Aim Sequence Number,Aim Reference Number,Software Supplier Aim ID,Funding Model,Subcontracted UKPRN,Provider Specified Learner Monitoring A,Provider Specified Learner Monitoring B,Provider Specified Learning Delivery Monitoring A,Provider Specified Learning Delivery Monitoring B,Provider Specified Learning Delivery Monitoring C,Provider Specified Learning Delivery Monitoring D,OFFICIAL-SENSITIVE");
            foreach (var error in errors)
            {
                StringBuilder errorFirst = new StringBuilder();
                if (error.ErrorMessageParameters != null)
                {
                    foreach (IErrorMessageParameter s in error.ErrorMessageParameters)
                    {
                        errorFirst.Append(s.PropertyName);
                        errorFirst.Append(":");
                        errorFirst.Append(s.Value);
                    }
                }

                contents.AppendLine($"E,{error.LearnerReferenceNumber},{error.RuleName},{errorFirst},,{error?.AimSequenceNumber},,,,");
            }

            System.IO.File.WriteAllText(path, contents.ToString());
        }

        private static void RegisterContext(ContainerBuilder containerBuilder, IValidationContext preValidationContext)
        {
            containerBuilder.RegisterInstance(preValidationContext).As<IValidationContext>();
        }

        private static IContainer BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<ConsoleValidationServiceModule>();

            return containerBuilder.Build();
        }
    }
}
