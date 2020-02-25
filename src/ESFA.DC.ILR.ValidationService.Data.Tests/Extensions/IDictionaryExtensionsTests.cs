using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Extensions
{
    public class IDictionaryExtensionsTests
    {
        [Fact]
        public void ToCaseInsensitiveDictionary()
        {
            IReadOnlyDictionary<string, int> dictionary = new Dictionary<string, int>()
            {
                { "One", 1 },
                { "Two", 2 },
                { "Three", 3 }
            };

            var caseInsensitiveDictionary = dictionary.ToCaseInsensitiveDictionary();

            caseInsensitiveDictionary.Should().NotBeSameAs(dictionary);

            caseInsensitiveDictionary.ContainsKey("ONE").Should().BeTrue();
            caseInsensitiveDictionary.ContainsKey("two").Should().BeTrue();
            caseInsensitiveDictionary.ContainsKey("ThReE").Should().BeTrue();
            caseInsensitiveDictionary["one"].Should().Be(1);
        }

        [Fact]
        public void ToCaseInsensitiveDictionary_Null()
        {
            IReadOnlyDictionary<string, int> dictionary = null;

            // returns a new Dictionary, note different behaviour to .net ToDictionary() method.
            dictionary.ToCaseInsensitiveDictionary().Should().HaveCount(0);
        }
    }
}
