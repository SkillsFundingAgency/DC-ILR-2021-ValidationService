using ESFA.DC.ILR.ValidationService.Rules.Derived;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_05RuleTests
    {
        /// <summary>
        /// Get employer identifier checksum meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData(100000002, '2')]
        [InlineData(100000010, '0')]
        [InlineData(100000029, '9')]
        [InlineData(100000045, '5')]
        [InlineData(100000053, '3')]
        [InlineData(100000061, '1')]
        [InlineData(100000088, '8')]
        [InlineData(100000096, '6')]
        [InlineData(100000126, '6')]
        [InlineData(100000134, '4')]
        [InlineData(100000142, '2')]
        [InlineData(100000150, '0')]
        [InlineData(100000169, '9')]
        [InlineData(100000177, '7')]
        [InlineData(100000185, '5')]
        [InlineData(100000193, '3')]
        [InlineData(100000207, '7')]
        [InlineData(10000020, 'X')]
        [InlineData(1000002, 'X')]
        [InlineData(100000, 'X')]
        [InlineData(0, 'X')]
        [InlineData(-1, 'X')]
        public void GetEmployerIDChecksumMeetsExpectation(int candidate, char expectation)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.GetEmployerIDChecksum(candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up derived data rule</returns>
        public DerivedData_05Rule NewRule()
        {
            return new DerivedData_05Rule();
        }
    }
}
