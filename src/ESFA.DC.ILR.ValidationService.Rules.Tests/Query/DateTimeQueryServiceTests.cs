﻿using System;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class DateTimeQueryServiceTests
    {
        [Theory]
        [InlineData("1988-3-10", "2018-2-18", 29)]
        [InlineData("1988-3-10", "2018-3-10", 30)]
        [InlineData("1988-3-10", "1988-3-10", 0)]
        [InlineData("1988-3-10", "1989-3-10", 1)]
        [InlineData("1988-3-10", "1987-3-10", -1)]
        [InlineData("1988-3-10", "1987-4-10", -1)]
        public void YearsBetween(string start, string end, int years)
        {
            new DateTimeQueryService().YearsBetween(DateTime.Parse(start), DateTime.Parse(end)).Should().Be(years);
        }

        [Theory]
        [InlineData("2018-1-10", "2018-2-18", 1)]
        [InlineData("2018-1-10", "2018-3-10", 2)]
        [InlineData("2018-1-10", "2024-6-10", 77)]
        [InlineData("2018-1-10", "2018-1-12", 0)]
        [InlineData("2018-1-10", "2017-10-10", 3)]
        [InlineData("2018-1-10", "2020-1-10", 24)]
        [InlineData("2018-8-01", "2019-7-31", 11)]
        public void MonthsBetween(string start, string end, int months)
        {
            new DateTimeQueryService().MonthsBetween(DateTime.Parse(start), DateTime.Parse(end)).Should().Be(months);
        }

        [Theory]
        [InlineData("2018-8-01", "2019-7-31", 12)]
        [InlineData("2018-8-01", "2020-7-31", 24)]
        [InlineData("2018-8-01", "2018-8-31", 0)]
        [InlineData("2018-8-01", "2018-8-01", 0)]
        [InlineData("2018-8-01", "2018-7-01", 1)]
        [InlineData("2018-01-01", "2018-12-01", 11)]
        public void WholeMonthsBetween(string start, string end, int months)
        {
            new DateTimeQueryService().WholeMonthsBetween(DateTime.Parse(start), DateTime.Parse(end)).Should().Be(months);
        }

        [Theory]
        [InlineData("2018-3-10", "2018-3-18", 8)]
        [InlineData("2018-3-10", "2018-3-10", 0)]
        [InlineData("2018-3-10", "2018-3-11", 1)]
        [InlineData("2018-3-10", "2018-3-29", 19)]
        [InlineData("2018-3-11", "2018-3-10", -1)]
        [InlineData("2017-9-4", "2018-9-3", 364)]
        public void DaysBetween(string start, string end, double days)
        {
            new DateTimeQueryService().DaysBetween(DateTime.Parse(start), DateTime.Parse(end)).Should().Be(days);
        }

        [Theory]
        [InlineData("2018-3-10", "2018-3-18", 9)]
        [InlineData("2018-3-10", "2018-3-10", 1)]
        [InlineData("2018-3-10", "2018-3-11", 2)]
        [InlineData("2018-3-10", "2018-3-29", 20)]
        [InlineData("2018-3-10", "2018-3-09", 2)]
        [InlineData("2017-9-4", "2018-9-3", 365)]
        public void WholeDaysBetween(string start, string end, double days)
        {
            new DateTimeQueryService().WholeDaysBetween(DateTime.Parse(start), DateTime.Parse(end)).Should().Be(days);
        }

        [Theory]
        [InlineData("1988-3-10", "2018-2-18", 29)]
        [InlineData("1988-3-10", "2018-3-10", 30)]
        [InlineData("1988-3-10", "1988-3-10", 0)]
        [InlineData("1988-3-10", "1989-3-10", 1)]
        [InlineData("1988-3-10", "1987-3-10", -1)]
        [InlineData("1988-3-10", "1987-4-10", -1)]
        public void AgeAtGivenDate(string start, string end, int years)
        {
            new DateTimeQueryService().YearsBetween(DateTime.Parse(start), DateTime.Parse(end)).Should().Be(years);
        }

        [Theory]
        [InlineData("2019-3-10", "2020-3-10", 1)]
        [InlineData("2019-3-10", "2049-3-10", 30)]
        [InlineData("2019-3-10", "2019-3-10", 0)]
        [InlineData("2019-3-10", "2018-3-10", -1)]
        [InlineData("2019-3-10", "2009-3-10", -10)]
        [InlineData("0001-1-1", "0001-1-1", -1)]
        [InlineData("0001-1-1", "0001-1-1", -10)]
        [InlineData("9999-12-31", "9999-12-31", 1)]
        [InlineData("9999-12-31", "9999-12-31", 10)]
        public void AddYearsToDate(string date, string result, int years)
        {
            new DateTimeQueryService().AddYearsToDate(DateTime.Parse(date), years).Should().Be(DateTime.Parse(result));
        }
    }
}
