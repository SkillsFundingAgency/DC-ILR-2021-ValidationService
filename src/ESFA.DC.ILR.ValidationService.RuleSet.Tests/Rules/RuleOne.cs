using ESFA.DC.ILR.ValidationService.Interface;
using Moq;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules
{
    public class RuleOne : IRule<string>
    {
        private readonly IValidationErrorCache _validationErrorCache;

        public RuleOne(IValidationErrorCache validationErrorCache)
        {
            _validationErrorCache = validationErrorCache;
        }

        public string RuleName => "RuleOne";

        public void Validate(string objectToValidate)
        {
            var validationErrorMock = new Mock<IValidationError>();
            validationErrorMock.SetupGet(e => e.RuleName).Returns("1");

            _validationErrorCache.Add(validationErrorMock.Object);
        }
    }
}
