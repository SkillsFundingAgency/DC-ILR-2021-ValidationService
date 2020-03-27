using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearningDeliveryAppFinRecordQueryServiceTests
    {
        [Fact]
        public void HasAnyLearningDeliveryAFinCodesForType_True()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
            };

            var codes = new int[] { 1, 3 };

            NewService().HasAnyLearningDeliveryAFinCodesForType(learningDeliveryAppFinRecords, "TypeTwo", codes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodesForType_FalseNull()
        {
            var codes = new int[] { 1, 3 };

            NewService().HasAnyLearningDeliveryAFinCodesForType(null, "TypeTwo", codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodesForType_False_CodesNull()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
           {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
           };

            NewService().HasAnyLearningDeliveryAFinCodesForType(learningDeliveryAppFinRecords, "TypeTwo", null).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodesForType_False_Mismatch()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
           {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
           };

            var codes = new int[] { 2, 3 };

            NewService().HasAnyLearningDeliveryAFinCodesForType(learningDeliveryAppFinRecords, "TypeOne", codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodeForType_True()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
            };

            NewService().HasAnyLearningDeliveryAFinCodeForType(learningDeliveryAppFinRecords, "TypeTwo", 2).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodeForType_FalseNull()
        {
            NewService().HasAnyLearningDeliveryAFinCodeForType(null, "TypeTwo", 1).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodeForType_False_CodeNull()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
           {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
           };

            NewService().HasAnyLearningDeliveryAFinCodeForType(learningDeliveryAppFinRecords, "TypeTwo", null).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodeForType_False_Mismatch()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
           {
                new TestAppFinRecord() { AFinType = "TypeOne", AFinCode = 1 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 2 },
                new TestAppFinRecord() { AFinType = "TypeTwo", AFinCode = 3 },
           };

            NewService().HasAnyLearningDeliveryAFinCodeForType(learningDeliveryAppFinRecords, "TypeOne", 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodes_True()
        {
            var appFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord() { AFinCode = 1 },
                new TestAppFinRecord() { AFinCode = 2 },
                new TestAppFinRecord() { AFinCode = 3 },
            };

            var codes = new int[] { 1, 3 };

            NewService().HasAnyLearningDeliveryAFinCodes(appFinRecords, codes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodes_False()
        {
            var appFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord() { AFinCode = 1 },
                new TestAppFinRecord() { AFinCode = 2 },
                new TestAppFinRecord() { AFinCode = 3 },
            };

            var codes = new int[] { 4, 5 };

            NewService().HasAnyLearningDeliveryAFinCodes(appFinRecords, codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodes_FalseNull()
        {
            var codes = new int[] { 4, 5 };

            NewService().HasAnyLearningDeliveryAFinCodes(null, codes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyLearningDeliveryAFinCodes_FalseCodeNull()
        {
            var appFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord() { AFinCode = 1 },
                new TestAppFinRecord() { AFinCode = 2 },
                new TestAppFinRecord() { AFinCode = 3 },
            };

            NewService().HasAnyLearningDeliveryAFinCodes(appFinRecords, null).Should().BeFalse();
        }

        [Fact]
        public void GetLatestAppFinRecord_Success()
        {
            var learningDeliveryAppFinRecords = new TestAppFinRecord[]
            {
                new TestAppFinRecord()
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2017, 10, 10),
                    AFinAmount = 10
                },
                new TestAppFinRecord()
                {
                    AFinType = "tNp",
                    AFinCode = 1,
                    AFinDate = new DateTime(2017, 10, 12),
                    AFinAmount = 20
                },
                new TestAppFinRecord()
                {
                    AFinType = "TN111",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 10),
                    AFinAmount = 30
                },
                new TestAppFinRecord()
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2019, 01, 01),
                    AFinAmount = 40
                },
            };

            var result = NewService().GetLatestAppFinRecord(learningDeliveryAppFinRecords, "tnp", 1);
            result.Should().NotBeNull();
            result.AFinAmount.Should().Be(20);
            result.AFinDate.Should().Be(new DateTime(2017, 10, 12));
        }

        [Theory]
        [InlineData(null, 1)]
        [InlineData("XYZ", 0)]
        public void GetLatestAppFinRecord_Null_AppFinType(string finType, int finCode)
        {
            NewService().GetLatestAppFinRecord(new List<IAppFinRecord>(), finType, finCode).Should().BeNull();
        }

        [Fact]
        public void GetAFinTotalValues_NUll_LearningDeliveries()
        {
            NewService().GetTotalTNPPriceForLatestAppFinRecordsForLearning(null).Should().Be(0);
        }

        [Fact]
        public void GetAFinTotalValues_Success()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord
                        {
                            AFinCode = 1,
                            AFinType = "TNP",
                            AFinAmount = 5,
                            AFinDate = new DateTime(2017, 10, 12)
                        },
                        new TestAppFinRecord
                        {
                            AFinCode = 1,
                            AFinType = "tnp",
                            AFinAmount = 10,
                            AFinDate = new DateTime(2017, 10, 11)
                        },
                        new TestAppFinRecord
                        {
                            AFinCode = 2,
                            AFinType = "tnp",
                            AFinAmount = 20,
                            AFinDate = new DateTime(2016, 10, 14)
                        }
                    }
                },
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord
                        {
                            AFinCode = 2,
                            AFinType = "TNP",
                            AFinAmount = 5,
                            AFinDate = new DateTime(2017, 10, 10)
                        },
                        new TestAppFinRecord
                        {
                            AFinCode = 2,
                            AFinType = "tnp",
                            AFinAmount = 10,
                            AFinDate = new DateTime(2017, 10, 12)
                        },
                    }
                }
            };

            NewService().GetTotalTNPPriceForLatestAppFinRecordsForLearning(learningDeliveries).Should().Be(35);
        }

        [Fact]
        public void GetLatestAppFinRecord_Null_AppFinRecords()
        {
            NewService().GetLatestAppFinRecord(null, "xyz", 1).Should().BeNull();
        }

        [Fact]
        public void GetAppFinRecordsForType_NullRecords()
        {
            NewService().GetAppFinRecordsForType(null, "TNP").Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetAppFinRecordsForType_AFinType_MisMatch()
        {
            var appFinRecords = new List<IAppFinRecord>()
            {
                new TestAppFinRecord
                {
                    AFinCode = 2,
                    AFinType = "TNP",
                    AFinAmount = 5,
                    AFinDate = new DateTime(2017, 10, 10)
                },
                new TestAppFinRecord
                {
                    AFinCode = 2,
                    AFinType = "tnp",
                    AFinAmount = 10,
                    AFinDate = new DateTime(2017, 10, 12)
                },
            };

            NewService().GetAppFinRecordsForType(appFinRecords, "xxx").Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetAppFinRecordsForType()
        {
            var appFinRecords = new List<IAppFinRecord>()
            {
                new TestAppFinRecord
                {
                    AFinCode = 2,
                    AFinType = "TNP",
                    AFinAmount = 5,
                    AFinDate = new DateTime(2017, 10, 10)
                },
                new TestAppFinRecord
                {
                    AFinCode = 2,
                    AFinType = "tnp",
                    AFinAmount = 10,
                    AFinDate = new DateTime(2017, 10, 12)
                },
            };

            NewService().GetAppFinRecordsForType(appFinRecords, "TNP").Should().HaveCount(2);
        }

        private LearningDeliveryAppFinRecordQueryService NewService()
        {
            return new LearningDeliveryAppFinRecordQueryService();
        }
    }
}
