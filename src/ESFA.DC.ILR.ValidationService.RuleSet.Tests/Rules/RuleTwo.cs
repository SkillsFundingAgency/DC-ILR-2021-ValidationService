using ESFA.DC.ILR.ValidationService.Interface;
using Moq;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules
{
    public class RuleTwo : IRule<string>
    {
        private readonly IValidationErrorCache _validationErrorCache;

        public RuleTwo(IValidationErrorCache validationErrorCache)
        {
            _validationErrorCache = validationErrorCache;
        }

        public string RuleName => "RuleTwo";

        public void Validate(string objectToValidate)
        {
            var validationErrorOneMock = new Mock<IValidationError>();

            validationErrorOneMock.SetupGet(e => e.RuleName).Returns("2");

            var validationErrorTwoMock = new Mock<IValidationError>();
            validationErrorTwoMock.SetupGet(e => e.RuleName).Returns("3");

            _validationErrorCache.Add(validationErrorOneMock.Object);
            _validationErrorCache.Add(validationErrorTwoMock.Object);
        }
    }
}
