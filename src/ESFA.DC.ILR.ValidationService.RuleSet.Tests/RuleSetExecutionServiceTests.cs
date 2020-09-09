using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.ValidationService.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests
{
    public class RuleSetExecutionServiceTests
    {
        [Fact]
        public void ExecuteEmptyRules()
        {
            var service = NewService();

            service.Execute(new IRule<string>[] { }, "irrelevant");
        }

        [Fact]
        public void Execute_Single()
        {
            var service = NewService();

            const string input = "input";

            var ruleMock = new Mock<IRule<string>>();

            ruleMock.Setup(r => r.Validate(input)).Verifiable();

            service.Execute(new List<IRule<string>> { ruleMock.Object }, input);

            ruleMock.Verify();
        }

        [Fact]
        public void Execute_MultipleDuplicate()
        {
            var service = NewService();

            const string input = "input";

            var ruleMock = new Mock<IRule<string>>();

            Expression<Action<IRule<string>>> validate = r => r.Validate(input);

            ruleMock.Setup(validate).Verifiable();

            service.Execute(new List<IRule<string>> { ruleMock.Object, ruleMock.Object, ruleMock.Object }, input);

            ruleMock.Verify(validate, Times.Exactly(3));
        }

        [Fact]
        public void Execute_MultipleDifferent()
        {
            var service = NewService();

            const string input = "input";

            var ruleOneMock = new Mock<IRule<string>>();
            var ruleTwoMock = new Mock<IRule<string>>();
            var ruleThreeMock = new Mock<IRule<string>>();

            ruleOneMock.Setup(r => r.Validate(input)).Verifiable();
            ruleTwoMock.Setup(r => r.Validate(input)).Verifiable();
            ruleThreeMock.Setup(r => r.Validate(input)).Verifiable();

            service.Execute(new List<IRule<string>> { ruleOneMock.Object, ruleTwoMock.Object, ruleThreeMock.Object }, input);

            ruleOneMock.Verify();
            ruleTwoMock.Verify();
            ruleThreeMock.Verify();
        }

        private RuleSetExecutionService<IRule<string>, string> NewService()
        {
            return new RuleSetExecutionService<IRule<string>, string>();
        }
    }
}
