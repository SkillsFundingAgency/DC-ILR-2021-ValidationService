using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Internal
{
    /// <summary>
    /// the lookups details provider test fixture
    /// </summary>
    public class LookupDetailsProviderTests
    {
        /// <summary>
        /// Provider 'is current' values match expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="testCaseDate">The test case date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(1, "2013-06-14", true)]
        [InlineData(1, "2013-06-13", false)]
        [InlineData(2, "2015-09-03", true)]
        [InlineData(2, "2007-09-03", false)]
        [InlineData(3, "2012-06-18", false)]
        [InlineData(5, "2013-06-14", true)]
        [InlineData(5, "2010-10-14", false)]
        [InlineData(9, "2004-05-01", true)]
        [InlineData(9, "2008-08-27", false)]
        public void ProviderIsCurrentValuesMatchExpectation(int candidate, string testCaseDate, bool expectation)
        {
            // arrange
            var sut = NewService();
            var testDate = DateTime.Parse(testCaseDate);

            // act
            var result = sut.IsCurrent(TypeOfLimitedLifeLookup.TTAccom, candidate, testDate);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("BSI", "1", "2013/01/01", true)]
        [InlineData("BSI", "10", "2013/01/01", false)]
        [InlineData("LOE", "1", "2013/01/01", true)]
        [InlineData("LOE", "4", "2013/01/01", false)]
        [InlineData("XXX", "1", "2013/01/01", false)]
        public void ProviderIsCurrentValuesMatchForESMType(string esmType, string esmCode, string dateToCheckString, bool expectedResult)
        {
            var dateToCheck = DateTime.Parse(dateToCheckString);

            var t = NewService().IsCurrent(TypeOfLimitedLifeLookup.ESMType, $"{esmType}{esmCode}", dateToCheck);

            t.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("ECF", "5", "2019/09/09", true)]
        [InlineData("ECF", "5", "2017/09/09", false)]
        public void ProviderIsExpredValuesMatchForFAMType(string famType, string famCode, string dateToCheckString, bool expectedResult)
        {
            var dateToCheck = DateTime.Parse(dateToCheckString);

            var t = NewService().IsExpired(TypeOfLimitedLifeLookup.LearnFAMType, $"{famType}{famCode}", dateToCheck);

            t.Should().Be(expectedResult);
        }

        /// <summary>
        /// Provider contains value matches expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(1, true)]
        [InlineData(3, false)]
        [InlineData(9, true)]
        [InlineData(26, false)]
        public void ProviderContainsSimpleValueMatchesExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewService();

            // act
            var result = sut.Contains(TypeOfIntegerCodedLookup.AimType, candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Provider contains coded value matches expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("A", true)]
        [InlineData("A*", true)]
        [InlineData("B*", false)]
        public void ProviderContainsCodedValueMatchesExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewService();

            // act
            var result = sut.Contains(TypeOfStringCodedLookup.OutGrade, candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Provider contains limited life value matches expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(1, true)]
        [InlineData(3, false)]
        [InlineData(9, true)]
        [InlineData(26, false)]
        public void ProviderContainsLimitedLifeValueMatchesExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewService();

            // act
            var result = sut.Contains(TypeOfLimitedLifeLookup.TTAccom, candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Provider contains Dictionary Lookup value matches expectation.
        /// </summary>
        /// <param name="keyCandidate">The dictionary key candidate.</param>
        /// <param name="valueCandidate">The dictionary value candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("TNP", 1, true)]
        [InlineData("TNP", 2, true)]
        [InlineData("TNP", 5, false)]
        [InlineData("PMR", 1, true)]
        [InlineData("PMR", 2, true)]
        [InlineData("PMR", 5, false)]
        [InlineData("TXX", 1, false)]
        public void ProviderContainsCodedKeyDictionaryMatchesExpectation_AppFinType(string keyCandidate, int? valueCandidate, bool expectation)
        {
            // arrange
            var sut = NewService();

            // act
            var result = sut.Contains(TypeOfStringCodedLookup.AppFinType, $"{keyCandidate}{valueCandidate}");

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// New service.
        /// </summary>
        /// <returns>a <seealso cref="LookupDetailsProvider"/></returns>
        public LookupDetailsProvider NewService()
        {
            var aimTypes = new DistinctKeySet<int> { 1, 2, 4, 5, 6, 9, 24, 25, 29, 45 };

            var tTAccomItems = new Dictionary<string, ValidityPeriods>()
            {
                ["1"] = new ValidityPeriods(DateTime.Parse("2013-06-14"), DateTime.Parse("2020-06-14")),
                ["2"] = new ValidityPeriods(DateTime.Parse("2009-04-28"), DateTime.Parse("2020-06-14")),
                ["4"] = new ValidityPeriods(DateTime.Parse("2012-09-06"), DateTime.Parse("2015-02-28")),
                ["5"] = new ValidityPeriods(DateTime.Parse("2010-11-21"), DateTime.Parse("2020-06-14")),
                ["6"] = new ValidityPeriods(DateTime.Parse("2018-07-02"), DateTime.Parse("2020-06-14")),
                ["9"] = new ValidityPeriods(DateTime.Parse("2000-02-01"), DateTime.Parse("2008-08-26")),
            };

            var esmTypes = new Dictionary<string, ValidityPeriods>()
            {
                ["BSI1"] = new ValidityPeriods(DateTime.Parse("2000-06-14"), DateTime.Parse("2020-06-14")),
                ["BSI2"] = new ValidityPeriods(DateTime.Parse("2000-04-28"), DateTime.Parse("2020-06-14")),
                ["LOE1"] = new ValidityPeriods(DateTime.Parse("2000-09-06"), DateTime.Parse("2015-02-28")),
                ["LOE2"] = new ValidityPeriods(DateTime.Parse("2000-11-21"), DateTime.Parse("2020-06-14")),
                ["LOE3"] = new ValidityPeriods(DateTime.Parse("2000-07-02"), DateTime.Parse("2020-06-14")),
                ["LOE4"] = new ValidityPeriods(DateTime.Parse("2000-02-01"), DateTime.Parse("2008-08-26")),
            };

            var famTypes = new Dictionary<string, ValidityPeriods>()
            {
                ["ECF5"] = new ValidityPeriods(DateTime.Parse("2000-06-14"), DateTime.Parse("2018-06-14")),
            };

            var cache = new InternalDataCache
            {
                IntegerLookups = new Dictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>>
                {
                    { TypeOfIntegerCodedLookup.AimType, aimTypes }
                },
                StringLookups = new Dictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>>
                {
                    { TypeOfStringCodedLookup.AppFinType, new List<string> { "PMR1", "PMR2", "PMR3", "TNP1", "TNP2", "TNP3", "TNP4" } },
                    { TypeOfStringCodedLookup.OutGrade, new List<string> { "A", "A*" } }
                },
                LimitedLifeLookups = new Dictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>>
                {
                    { TypeOfLimitedLifeLookup.TTAccom, tTAccomItems },
                    { TypeOfLimitedLifeLookup.ESMType, esmTypes },
                    { TypeOfLimitedLifeLookup.LearnFAMType, famTypes },
                },
                ListItemLookups = new Dictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IReadOnlyCollection<string>>>()
            };

            return new LookupDetailsProvider(cache);
        }
    }
}
