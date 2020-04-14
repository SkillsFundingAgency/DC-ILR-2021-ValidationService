using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Utility;
using FluentAssertions;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_06RuleTests
    {
        [Fact]
        public void Derive()
        {
            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2015, 1, 1)
                },
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2015, 10, 10)
                },
            };

            NewRule().Derive(learningDeliveries).Should().Be(new DateTime(2015, 1, 1));
        }

        [Fact]
        public void DeriveFor_NullLearningDelivery()
        {
            Action action = () => NewRule().Derive(null);

            action.Should().Throw<ArgumentNullException>();
        }

        /// <summary>
        /// Derive using empty collection thows invalid operation exception.
        /// i don't think it should be allowed to do this... so, what's the policy?
        /// </summary>
        [Fact]
        public void DeriveUsingEmptyCollectionThrowsInvalidOperationException()
        {
            // arrange
            var rule = NewRule();

            // act / assert
            Assert.Throws<InvalidOperationException>(() => rule.Derive(Array.Empty<ILearningDelivery>()));
        }

        private DerivedData_06Rule NewRule()
        {
            return new DerivedData_06Rule();
        }
    }
}
