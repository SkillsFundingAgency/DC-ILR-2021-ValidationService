using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearningDeliveryFAMQueryServiceTests
    {
        [Theory]
        [InlineData("TypeTwo", "CodeOne")]
        [InlineData("TypeTwo", "CodeThree")]
        [InlineData("TypeTWO", "CodeThree")]
        [InlineData("TypeTwo", "CodeTHREE")]
        public void HasAnyLearningDeliveryFAMCodesForType_True(string famType, string famCode)
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = famType, LearnDelFAMCode = famCode },
            };

            var codes = new string[] { "CodeOne", "CodeThree" };

            NewService().HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "TypeTwo", codes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMCodesForType_FalseNull()
        {
            var codes = new string[] { "CodeOne", "CodeThree" };

            NewService().HasAnyLearningDeliveryFAMCodesForType(null, "TypeTwo", codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMCodesForType_False_CodesNull()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeThree" },
            };

            NewService().HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "TypeTwo", null).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryFAMCodesForType_False_Mismatch()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeThree" },
            };

            var codes = new string[] { "CodeTwo", "CodeThree" };

            NewService().HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "TypeOne", codes).Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_True()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
            };

            NewService().HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "TypeTwo", "CodeTwo").Should().BeTrue();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_FalseNull()
        {
            NewService().HasLearningDeliveryFAMCodeForType(null, "TypeTwo", "CodeTwo").Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMCodeForType_FalseMismatch()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne", LearnDelFAMCode = "CodeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo", LearnDelFAMCode = "CodeTwo" },
            };

            NewService().HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "TypeOne", "CodeThree").Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMType_True()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo" }
            };

            NewService().HasLearningDeliveryFAMType(learningDeliveryFAMs, "TypeOne");
        }

        [Fact]
        public void HasLearningDeliveryFAMType_False()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeOne" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "TypeTwo" }
            };

            NewService().HasLearningDeliveryFAMType(learningDeliveryFAMs, "TypeThree");
        }

        [Fact]
        public void HasLearningDeliveryFAMType_False_Null()
        {
            NewService().HasLearningDeliveryFAMType(null, "Doesn't Matter");
        }

        [Fact]
        public void GetLearningDeliveryFAMsCountByFAMType_CountCheckForFound()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.HHS
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.HHS
                    }
                };

            NewService().GetLearningDeliveryFAMsCountByFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.HHS).Should().Be(2);
        }

        [Fact]
        public void GetLearningDeliveryFAMsCountByFAMType_CountCheckForNotFound()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.HHS
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.HHS
                    }
                };

            NewService().GetLearningDeliveryFAMsCountByFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT).Should().Be(0);
        }

        [Fact]
        public void GetLearningDeliveryFAMsCountByFAMType_NullCheck()
        {
            NewService().GetLearningDeliveryFAMsCountByFAMType(null, LearningDeliveryFAMTypeConstants.HHS).Should().Be(0);
        }

        [Fact]
        public void HasLearningDeliveryFAMTypeForDate_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "A", LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1) },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "B" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "C" }
            };

            NewService().HasLearningDeliveryFAMTypeForDate(learningDeliveryFAMs, "A", new DateTime(2018, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void HasLearningDeliveryFAMTypeForDate_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "A", LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1) },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "B" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "C" }
            };

            NewService().HasLearningDeliveryFAMTypeForDate(learningDeliveryFAMs, "A", new DateTime(2018, 9, 1)).Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMTypeForDate_False_NullLearningDeliveryFAMs()
        {
            NewService().HasLearningDeliveryFAMTypeForDate(null, "A", new DateTime(2018, 9, 1)).Should().BeFalse();
        }

        [Fact]
        public void HasLearningDeliveryFAMTypeForDate_False_NullFAMTypes()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "A", LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1) },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "B" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "C" }
            };

            NewService().HasLearningDeliveryFAMTypeForDate(learningDeliveryFAMs, null, new DateTime(2018, 9, 1)).Should().BeFalse();
        }

        [Fact]
        public void GetOverLappingLearningDeliveryFAMsForType_NullCheck()
        {
            NewService().GetOverLappingLearningDeliveryFAMsForType(null, "LSF").Should().BeEquivalentTo(new List<ILearningDeliveryFAM>());
        }

        [Fact]
        public void GetOverLappingLearningDeliveryFAMsForType_NullCheck_FamTypeMisMatch()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ACT", LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1) },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "SOF" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ALB" }
            };

            NewService().GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, "LSF").Should().BeEquivalentTo(new List<ILearningDeliveryFAM>());
        }

        [Fact]
        public void GetOverLappingLearningDeliveryFAMsForType_NullCheck_CountLessThanTwo()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "LSF", LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1) },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "SOF" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ALB" }
            };

            NewService().GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, "LSF").Should().BeEquivalentTo(new List<ILearningDeliveryFAM>());
        }

        [Fact]
        public void GetOverLappingLearningDeliveryFAMsForType_ReturnsEntity()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 9, 1)
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo
            };

            NewService().GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, "LSF").Should().BeEquivalentTo(new List<TestLearningDeliveryFAM> { learningDeliveryFamTwo });
        }

        [Fact]
        public void GetOverLappingLearningDeliveryFAMsForType_ReturnsEntity_Multiple()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 2),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
            };

            NewService().GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, "LSF").Should().BeEquivalentTo(new List<TestLearningDeliveryFAM> { learningDeliveryFamThree });
        }

        [Fact]
        public void GetOverLappingLearningDeliveryFAMsForType_ReturnsEntities_NoEndDates()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 3),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
            };

            NewService().GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, "LSF").Should().BeEquivalentTo(new List<TestLearningDeliveryFAM> { learningDeliveryFamTwo, learningDeliveryFamThree });
        }

        [Fact]
        public void GetOverLappingLearningDeliveryFAMsForType_ReturnsEntities_NoFromDates()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF"
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF"
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF"
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
            };

            NewService().GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, "LSF").Should().BeEquivalentTo(new List<TestLearningDeliveryFAM>());
        }

        [Fact]
        public void GetOverLappingLearningDeliveryFAMsForType_ReturnsEmptyCollection_NoOverlaps()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 3),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
            };

            NewService().GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, "LSF").Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData("2018-08-01", "2018-09-01", false)]
        [InlineData("2018-08-01", null, false)]
        [InlineData(null, "2018-09-01", true)]
        [InlineData("2018-08-01", "2018-08-01", true)]
        [InlineData("2018-08-01", "2018-07-01", true)]
        public void IsOverlappingLearningDeliveryFAM_False_NoOverlaps(string dateTo, string dateFrom, bool expectation)
        {
            DateTime? to = dateTo == null ? (DateTime?)null : DateTime.Parse(dateTo);
            DateTime? from = dateFrom == null ? (DateTime?)null : DateTime.Parse(dateFrom);

            NewService().IsOverlappingLearnDelFAMDates(to, from).Should().Be(expectation);
        }

        [Fact]
        public void HasFamType_True()
        {
            var learningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = "Type"
            };

            NewService().HasFamType(learningDeliveryFam, "Type").Should().BeTrue();
        }

        [Fact]
        public void HasFamType_True_CaseInsensitive()
        {
            var learningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = "Type"
            };

            NewService().HasFamType(learningDeliveryFam, "tYPE").Should().BeTrue();
        }

        [Fact]
        public void HasFamType_False()
        {
            var learningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = "Type"
            };

            NewService().HasFamType(learningDeliveryFam, "WrongType").Should().BeFalse();
        }

        [Fact]
        public void HasFamCode_True()
        {
            var learningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMCode = "Code"
            };

            NewService().HasFamCode(learningDeliveryFam, "Code").Should().BeTrue();
        }

        [Fact]
        public void HasFamCode_True_CaseInsensitive()
        {
            var learningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMCode = "Code"
            };

            NewService().HasFamCode(learningDeliveryFam, "cODE").Should().BeTrue();
        }

        [Fact]
        public void HasFamCode_False()
        {
            var learningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMCode = "Code"
            };

            NewService().HasFamType(learningDeliveryFam, "WrongCode").Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsForType_Match()
        {
            var matchOne = new TestLearningDeliveryFAM() { LearnDelFAMType = "One" };
            var matchTwo = new TestLearningDeliveryFAM() { LearnDelFAMType = "One" };
            var nonMatch = new TestLearningDeliveryFAM() { LearnDelFAMType = "Two" };

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                matchOne,
                matchTwo,
                nonMatch,
            };

            var result = NewService().GetLearningDeliveryFAMsForType(learningDeliveryFAMs, "One").ToList();

            result.Should().HaveCount(2);
            result.Should().Contain(matchOne);
            result.Should().Contain(matchTwo);
        }

        [Fact]
        public void LearningDeliveryFAMsForType_NoMatch()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>();

            NewService().GetLearningDeliveryFAMsForType(learningDeliveryFAMs, "anything").Should().BeEmpty();
        }

        [Fact]
        public void LearningDeliveryFAMsForType_Null()
        {
            NewService().GetLearningDeliveryFAMsForType(null, "anything").Should().BeNullOrEmpty();
        }

        private LearningDeliveryFAMQueryService NewService()
        {
            return new LearningDeliveryFAMQueryService();
        }
    }
}
