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
        [InlineData(TypeOfQualEnt3.CertificateOfHigherEducation, "2013/01/01", true)]
        [InlineData(TypeOfQualEnt3.ProfessionalQualificationAtLevel3, "2013/07/31", true)]
        [InlineData(TypeOfQualEnt3.CambridgePreUDiploma31072013, "2014/07/31", false)]
        [InlineData("Z12345", "2018/10/28", false)]
        public void ProviderIsCurrentValuesMatchForQualent3(string qualent3, string dateToCheckString, bool expectedResult)
        {
            var dateToCheck = DateTime.Parse(dateToCheckString);

            NewService().IsCurrent(TypeOfLimitedLifeLookup.QualEnt3, qualent3, dateToCheck).Should().Be(expectedResult);
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
        [InlineData("TNP", true)]
        [InlineData("AMP", false)]
        [InlineData("PMR", true)]
        [InlineData("UOY", false)]
        public void ProviderContainsCodedValueMatchesExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewService();

            // act
            var result = sut.Contains(TypeOfStringCodedLookup.AppFinType, candidate);

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
        public void ProviderContainsCodedKeyDictionaryMatchesExpectation(string keyCandidate, int valueCandidate, bool expectation)
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

            var apprenticeshipFinancialRecords = new CaseInsensitiveDistinctKeySet { "PMR", "TNP" };

            var cache = new InternalDataCache
            {
                IntegerLookups = new Dictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>>
                {
                    { TypeOfIntegerCodedLookup.AimType, aimTypes }
                },
                StringLookups = new Dictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>>
                {
                    { TypeOfStringCodedLookup.AppFinType, apprenticeshipFinancialRecords }
                },
                LimitedLifeLookups = new Dictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>>
                {
                    { TypeOfLimitedLifeLookup.TTAccom, tTAccomItems }
                },
                ListItemLookups = new Dictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IReadOnlyCollection<string>>>()
            };

            return new LookupDetailsProvider(cache);
        }
    }
}
