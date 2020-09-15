using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.Learner;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Logging.Interfaces;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class PreValidationOrchestrationSfServiceTests
    {
        [Fact]
        public async Task ExecuteAsync()
        {
            var fileName = "Filename.xml";
            var container = "Container";
            var ilrReferenceDataKey = "IlrReferenceDataKey";
            var learnerReferenceDataKey = "LearnerReferenceDataKey";
            var message = new Message();
            var ilrRefData = new ReferenceDataRoot();
            var learnerRefData = new LearnerReferenceData();

            var cancellationToken = CancellationToken.None;
            var context = new Mock<IValidationContext>();
            context.Setup(x => x.Filename).Returns(fileName);
            context.Setup(x => x.Container).Returns(container);
            context.Setup(x => x.IlrReferenceDataKey).Returns(ilrReferenceDataKey);
            context.Setup(x => x.LearnerReferenceDataKey).Returns(learnerReferenceDataKey);

            var messageProvider = new Mock<IFileProvider<Message>>();
            messageProvider.Setup(x => x.ProvideAsync(fileName, container, cancellationToken)).ReturnsAsync(message);
            var referenceDataRootProvider = new Mock<IFileProvider<ReferenceDataRoot>>();
            referenceDataRootProvider.Setup(x => x.ProvideAsync(ilrReferenceDataKey, container, cancellationToken)).ReturnsAsync(ilrRefData);
            var learnerReferenceDataProvider = new Mock<IFileProvider<LearnerReferenceData>>();
            learnerReferenceDataProvider.Setup(x => x.ProvideAsync(learnerReferenceDataKey, container, cancellationToken)).ReturnsAsync(learnerRefData);

            var preValidationPopulationService = new Mock<IPopulationService>();
          
            var validationErrorCache = new Mock<IValidationErrorCache>();
            validationErrorCache.Setup(x => x.ValidationErrors).Returns(new List<IValidationError>());

            var validationOutputService = new Mock<IValidationOutputService>();
            validationOutputService.Setup(x => x.ProcessAsync(context.Object, message, validationErrorCache.Object.ValidationErrors, cancellationToken)).Returns(Task.CompletedTask);

            var validIlrFileOutputService = new Mock<IValidIlrFileOutputService>();
            validIlrFileOutputService.Setup(x => x.ProcessAsync(context.Object, message, cancellationToken)).Returns(Task.CompletedTask);

            var messageRuleSetOrchestratonService = new Mock<IRuleSetOrchestrationService<IRule<IMessage>, IMessage>>();
            messageRuleSetOrchestratonService.Setup(x => x.ExecuteAsync(message, cancellationToken)).ReturnsAsync(new List<IValidationError>());

            var crossYearRuleSetOrchestrationService = new Mock<IRuleSetOrchestrationService<ICrossYearRule<ILearner>, ILearner>>();
            crossYearRuleSetOrchestrationService.Setup(x => x.ExecuteAsync(message.Learners, cancellationToken)).ReturnsAsync(new List<IValidationError>());

            var validationExecutionProvider = new Mock<IValidationExecutionProvider>();
            validationExecutionProvider.Setup(x => x.ExecuteAsync(context.Object, message, cancellationToken)).Returns(Task.CompletedTask);

            await NewService(
                preValidationPopulationService.Object,
                messageProvider.Object,
                referenceDataRootProvider.Object,
                learnerReferenceDataProvider.Object,
                validationErrorCache.Object,
                validationOutputService.Object,
                validIlrFileOutputService.Object,
                messageRuleSetOrchestratonService.Object,
                crossYearRuleSetOrchestrationService.Object,
                validationExecutionProvider.Object
                ).ExecuteAsync(context.Object, cancellationToken);

            preValidationPopulationService.VerifyAll();
            messageProvider.VerifyAll();
            referenceDataRootProvider.VerifyAll();
            learnerReferenceDataProvider.VerifyAll();
            validationErrorCache.VerifyAll();
            validationOutputService.VerifyAll();
            validIlrFileOutputService.VerifyAll();
            messageRuleSetOrchestratonService.VerifyAll();
            crossYearRuleSetOrchestrationService.VerifyAll();
            validationExecutionProvider.VerifyAll();
        }

        private PreValidationOrchestrationSfService NewService(
            IPopulationService preValidationPopulationService,
            IFileProvider<Message> messageProvider,
            IFileProvider<ReferenceDataRoot> referenceDataRootProvider,
            IFileProvider<LearnerReferenceData> learnerReferenceDataProvider,
            IValidationErrorCache validationErrorCache,
            IValidationOutputService validationOutputService,
            IValidIlrFileOutputService validIlrFileOutputService,
            IRuleSetOrchestrationService<IRule<IMessage>, IMessage> messageRuleSetOrchestrationService,
            IRuleSetOrchestrationService<ICrossYearRule<ILearner>, ILearner> crossYearRuleSetOrchestrationService,
            IValidationExecutionProvider validationExecutionProvider)
        {
            return new PreValidationOrchestrationSfService(
                preValidationPopulationService,
                messageProvider,
                referenceDataRootProvider,
                learnerReferenceDataProvider,
                validationErrorCache,
                validationOutputService,
                validIlrFileOutputService,
                messageRuleSetOrchestrationService,
                crossYearRuleSetOrchestrationService,
                validationExecutionProvider,
                Mock.Of<ILogger>());
        }
    }
}
