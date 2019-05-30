using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Extensions
{
    public class IEnumerableExtensionsTests
    {
        [Fact]
        public void ToCaseInsensitiveHashSet()
        {
            var enumerable = new List<string>() { "abc", "def", "abc" };

            var hashset = enumerable.ToCaseInsensitiveHashSet();

            hashset.Should().HaveCount(2);
            hashset.Should().Contain("ABC");
        }

        [Fact]
        public void ToCaseInsensitiveHashSet_Null()
        {
            IEnumerable<string> enumerable = null;

            Action action = () => enumerable.ToCaseInsensitiveHashSet();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SplitList()
        {
            var enumerable = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var splitEnumerables = enumerable.SplitList(3).ToList();

            splitEnumerables.Should().HaveCount(4);

            splitEnumerables[0].Should().HaveCount(3);
            splitEnumerables[0].Should().ContainInOrder(0, 1, 2);

            splitEnumerables[1].Should().HaveCount(3);
            splitEnumerables[1].Should().ContainInOrder(3, 4, 5);

            splitEnumerables[2].Should().HaveCount(3);
            splitEnumerables[2].Should().ContainInOrder(6, 7, 8);

            splitEnumerables[3].Should().HaveCount(1);
            splitEnumerables[3].Should().ContainInOrder(9);
        }

        [Fact]
        public void SplitList_Empty()
        {
            var enumerable = new List<int>() { };

            var splitEnumerables = enumerable.SplitList(3).ToList();

            splitEnumerables.Should().HaveCount(0);
        }

        [Fact]
        public void SplitList_NoRemainder()
        {
            var enumerable = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

            var splitEnumerables = enumerable.SplitList(3).ToList();

            splitEnumerables.Should().HaveCount(3);

            splitEnumerables[0].Should().HaveCount(3);
            splitEnumerables[0].Should().ContainInOrder(0, 1, 2);

            splitEnumerables[1].Should().HaveCount(3);
            splitEnumerables[1].Should().ContainInOrder(3, 4, 5);

            splitEnumerables[2].Should().HaveCount(3);
            splitEnumerables[2].Should().ContainInOrder(6, 7, 8);
        }

        [Fact]
        public void SplitList_Null()
        {
            IEnumerable<string> enumerable = null;

            Action action = () => enumerable.SplitList(10).ToList();

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
