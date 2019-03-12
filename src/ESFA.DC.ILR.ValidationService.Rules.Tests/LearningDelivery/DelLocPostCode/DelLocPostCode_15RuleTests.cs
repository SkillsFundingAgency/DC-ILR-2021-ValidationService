using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_15RuleTests : AbstractRuleTests<DelLocPostCode_15Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DelLocPostCode_15");
        }

        [Fact]
        public void ValidationPasses_OutsideRuleDate()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var startDate = new DateTime(2017, 7, 31);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = startDate
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(35, "WEREW001", "CV1 1AB")] // irrelevant fundmodel
        [InlineData(70, "ZESF0001", "CV1 1AB")] // excluded aim ref
        [InlineData(70, "WEREW001", "ZZ99 9ZZ")] // excluded postcode
        public void ValidationPasses_IrrelevantLearningDeliveryProperties(int fundModel, string learnAimRef, string delLocPostCode)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var startDate = new DateTime(2017, 7, 30);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = startDate,
                        FundModel = fundModel,
                        LearnAimRef = learnAimRef,
                        DelLocPostCode = delLocPostCode
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_NoEnterprisePartnership()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var startDate = new DateTime(2017, 7, 30);
            var conRefNum = "12345";
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = startDate,
                        FundModel = 70,
                        LearnAimRef = "foo",
                        DelLocPostCode = "foo",
                        ConRefNumber = conRefNum
                    }
                }
            };

            var fcsServiceMock = new Mock<IFCSDataService>();
            fcsServiceMock
                    .Setup(m => m.GetEligibilityRuleEnterprisePartnershipsFor(conRefNum))
                    .Returns((IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership>)null);

            NewRule(validationErrorHandlerMock.Object, fcsServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_EPCodeMissing()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var startDate = new DateTime(2017, 7, 30);
            var conRefNum = "12345";
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = startDate,
                        FundModel = 70,
                        LearnAimRef = "foo",
                        DelLocPostCode = "foo",
                        ConRefNumber = conRefNum
                    }
                }
            };

            var fcsServiceMock = new Mock<IFCSDataService>();
            fcsServiceMock
                .Setup(m => m.GetEligibilityRuleEnterprisePartnershipsFor(conRefNum))
                .Returns(new List<EsfEligibilityRuleLocalEnterprisePartnership>
                {
                    new EsfEligibilityRuleLocalEnterprisePartnership
                    {
                        Code = null
                    },
                    new EsfEligibilityRuleLocalEnterprisePartnership
                    {
                        Code = string.Empty
                    }
                });

            NewRule(validationErrorHandlerMock.Object, fcsServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData("123", null)]
        [InlineData(null, "123")]
        [InlineData("123", "123")]
        public void ValidationPasses_PostCodeIsValid(string lep1, string lep2)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var startDate = new DateTime(2017, 7, 30);
            var conRefNum = "12345";
            var localAuthorityCode = "123";
            var delLocPostCode = "CV1 1AB";

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = startDate,
                        FundModel = 70,
                        LearnAimRef = "foo",
                        DelLocPostCode = delLocPostCode,
                        ConRefNumber = conRefNum
                    }
                }
            };

            var fcsServiceMock = new Mock<IFCSDataService>();
            fcsServiceMock
                    .Setup(m => m.GetEligibilityRuleEnterprisePartnershipsFor(conRefNum))
                    .Returns(new List<EsfEligibilityRuleLocalEnterprisePartnership>
                    {
                        new EsfEligibilityRuleLocalEnterprisePartnership
                        {
                            Code = localAuthorityCode
                        }
                    });

            var postcodeServiceMock = new Mock<IPostcodesDataService>();
            postcodeServiceMock
                .Setup(m => m.GetONSPostcodes(delLocPostCode))
                .Returns(new ONSPostcode[]
                {
                    new ONSPostcode()
                    {
                        Lep1 = lep1,
                        Lep2 = lep2,
                        Termination = null,
                        EffectiveFrom = startDate.AddMonths(-1),
                        EffectiveTo = startDate.AddYears(1)
                    }
                });

            var dd22Mock = new Mock<IDerivedData_22Rule>();
            dd22Mock
                .Setup(m => m.GetLatestLearningStartForESFContract(testLearner.LearningDeliveries.First(), testLearner.LearningDeliveries))
                .Returns(startDate);

            NewRule(validationErrorHandlerMock.Object, fcsServiceMock.Object, postcodeServiceMock.Object, dd22Mock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationFails_NoPostcodeFound()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var startDate = new DateTime(2017, 7, 30);
            var conRefNum = "12345";
            var localAuthorityCode = "123";
            var delLocPostCode = "CV1 1AB";

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = startDate,
                        FundModel = 70,
                        LearnAimRef = "foo",
                        DelLocPostCode = delLocPostCode,
                        ConRefNumber = conRefNum
                    }
                }
            };

            var fcsServiceMock = new Mock<IFCSDataService>();
            fcsServiceMock
                    .Setup(m => m.GetEligibilityRuleEnterprisePartnershipsFor(conRefNum))
                    .Returns(new List<EsfEligibilityRuleLocalEnterprisePartnership>
                    {
                        new EsfEligibilityRuleLocalEnterprisePartnership
                        {
                            Code = localAuthorityCode
                        }
                    });

            var postcodeServiceMock = new Mock<IPostcodesDataService>();
            postcodeServiceMock
                .Setup(m => m.GetONSPostcodes(delLocPostCode))
                .Returns((IONSPostcode[])null);

            var dd22Mock = new Mock<IDerivedData_22Rule>();
            dd22Mock
                .Setup(m => m.GetLatestLearningStartForESFContract(testLearner.LearningDeliveries.First(), testLearner.LearningDeliveries))
                .Returns(startDate);

            NewRule(validationErrorHandlerMock.Object, fcsServiceMock.Object, postcodeServiceMock.Object, dd22Mock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationFails_DD25ReturnsNull()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var startDate = new DateTime(2017, 7, 30);
            var conRefNum = "12345";
            var localAuthorityCode = "123";
            var delLocPostCode = "CV1 1AB";

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = startDate,
                        FundModel = 70,
                        LearnAimRef = "foo",
                        DelLocPostCode = delLocPostCode,
                        ConRefNumber = conRefNum
                    }
                }
            };

            var fcsServiceMock = new Mock<IFCSDataService>();
            fcsServiceMock
                .Setup(m => m.GetEligibilityRuleEnterprisePartnershipsFor(conRefNum))
                .Returns(new List<EsfEligibilityRuleLocalEnterprisePartnership>
                {
                    new EsfEligibilityRuleLocalEnterprisePartnership
                    {
                        Code = localAuthorityCode
                    }
                });

            var postcodeServiceMock = new Mock<IPostcodesDataService>();
            postcodeServiceMock
                .Setup(m => m.GetONSPostcodes(delLocPostCode))
                .Returns(new ONSPostcode[]
                {
                    new ONSPostcode()
                    {
                        LocalAuthority = localAuthorityCode,
                        Termination = null,
                        EffectiveFrom = startDate.AddMonths(-1),
                        EffectiveTo = startDate.AddYears(1)
                    }
                });

            var dd22Mock = new Mock<IDerivedData_22Rule>();
            dd22Mock
                .Setup(m => m.GetLatestLearningStartForESFContract(testLearner.LearningDeliveries.First(), testLearner.LearningDeliveries))
                .Returns((DateTime?)null);

            NewRule(validationErrorHandlerMock.Object, fcsServiceMock.Object, postcodeServiceMock.Object, dd22Mock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData("123", "2017-07-31", "2018-07-29", "2017-07-30")]
        [InlineData("123", "2017-08-30", "2018-08-31", "2099-01-01")]
        [InlineData("123", "2017-07-29", "2017-07-29", "2099-01-01")]
        [InlineData("123", "2017-07-29", "2018-08-31", "2017-07-29")]
        public void ValidationFails_ONSDataFails(string lep1, string effectiveFrom, string effectiveTo, string termination)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var startDate = new DateTime(2017, 7, 30);
            var conRefNum = "12345";
            var localAuthorityCode = "123";
            var delLocPostCode = "CV1 1AB";

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = startDate,
                        FundModel = 70,
                        LearnAimRef = "foo",
                        DelLocPostCode = delLocPostCode,
                        ConRefNumber = conRefNum
                    }
                }
            };

            var fcsServiceMock = new Mock<IFCSDataService>();
            fcsServiceMock
                .Setup(m => m.GetEligibilityRuleEnterprisePartnershipsFor(conRefNum))
                .Returns(new List<EsfEligibilityRuleLocalEnterprisePartnership>
                {
                    new EsfEligibilityRuleLocalEnterprisePartnership
                    {
                        Code = localAuthorityCode
                    }
                });

            var postcodeServiceMock = new Mock<IPostcodesDataService>();
            postcodeServiceMock
                .Setup(m => m.GetONSPostcodes(delLocPostCode))
                .Returns(new ONSPostcode[]
                {
                    new ONSPostcode()
                    {
                        Lep1 = lep1,
                        Termination = DateTime.Parse(termination),
                        EffectiveFrom = DateTime.Parse(effectiveFrom),
                        EffectiveTo = DateTime.Parse(effectiveTo)
                    }
                });

            var dd22Mock = new Mock<IDerivedData_22Rule>();
            dd22Mock
                .Setup(m => m.GetLatestLearningStartForESFContract(testLearner.LearningDeliveries.First(), testLearner.LearningDeliveries))
                .Returns(startDate);

            NewRule(validationErrorHandlerMock.Object, fcsServiceMock.Object, postcodeServiceMock.Object, dd22Mock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        [Theory]
        [InlineData("2018-09-01", "2018-08-01", "2018-10-01", "2018-10-01")]
        [InlineData("2018-09-01", "2018-08-01", null, null)]
        [InlineData("2018-09-01", "2018-08-01", "2018-10-01", null)]
        [InlineData("2018-09-01", "2018-08-01", null, "2018-10-01")]
        public void InQualifyingPeriod_False(string startDate, string effectiveFrom, string effectiveTo, string termination)
        {
            DateTime learnStartDate = DateTime.Parse(startDate);
            DateTime effectivefromDate = DateTime.Parse(effectiveFrom);
            DateTime? effectiveToDate = string.IsNullOrEmpty(effectiveTo) ? (DateTime?)null : DateTime.Parse(effectiveTo);
            DateTime? terminationDate = string.IsNullOrEmpty(termination) ? (DateTime?)null : DateTime.Parse(termination);

            var onsPostCode = new ONSPostcode()
            {
                EffectiveFrom = effectivefromDate,
                EffectiveTo = effectiveToDate,
                Termination = terminationDate
            };

            NewRule().CheckQualifyingPeriod(learnStartDate, onsPostCode).Should().BeFalse();
        }

        [Theory]
        [InlineData("2018-09-01", "2018-10-01", "2018-10-01", "2018-10-01")]
        [InlineData("2018-09-01", "2018-08-01", "2018-08-01", "2018-10-01")]
        [InlineData("2018-09-01", "2018-08-01", "2018-10-01", "2018-08-01")]
        [InlineData("2018-09-01", "2018-10-01", "2018-08-01", "2018-08-01")]
        [InlineData("2018-09-01", "2018-10-01", "2018-08-01", null)]
        [InlineData("2018-09-01", "2018-08-01", "2018-08-01", null)]
        [InlineData("2018-09-01", "2018-10-01", null, "2018-08-01")]
        [InlineData("2018-09-01", "2018-08-01", null, "2018-08-01")]
        [InlineData("2018-09-01", "2018-10-01", null, "2018-10-01")]
        [InlineData("2018-09-01", "2018-10-01", null, null)]
        public void InQualifyingPeriod_True(string startDate, string effectiveFrom, string effectiveTo, string termination)
        {
            DateTime learnStartDate = DateTime.Parse(startDate);
            DateTime effectivefromDate = DateTime.Parse(effectiveFrom);
            DateTime? effectiveToDate = string.IsNullOrEmpty(effectiveTo) ? (DateTime?)null : DateTime.Parse(effectiveTo);
            DateTime? terminationDate = string.IsNullOrEmpty(termination) ? (DateTime?)null : DateTime.Parse(termination);

            var onsPostCode = new ONSPostcode()
            {
                EffectiveFrom = effectivefromDate,
                EffectiveTo = effectiveToDate,
                Termination = terminationDate
            };

            NewRule().CheckQualifyingPeriod(learnStartDate, onsPostCode).Should().BeTrue();
        }

        [Fact]
        public void PostcodesContainValidPostcode_False()
        {
            DateTime learnStartDate = new DateTime(2018, 06, 01);

            var onsPostCodes = new List<ONSPostcode>()
            {
                new ONSPostcode()
                {
                    EffectiveFrom = new DateTime(2018, 01, 01),
                    EffectiveTo = new DateTime(2018, 05, 01),
                    Termination = new DateTime(2018, 05, 01)
                },
                new ONSPostcode()
                {
                    EffectiveFrom = new DateTime(2017, 01, 01),
                    EffectiveTo = new DateTime(2018, 01, 01),
                    Termination = new DateTime(2018, 01, 01)
                },
            };

            NewRule().PostcodesContainValidPostcode(learnStartDate, onsPostCodes).Should().BeFalse();
        }

        [Fact]
        public void PostcodesContainValidPostcode_True()
        {
            DateTime learnStartDate = new DateTime(2018, 06, 01);

            var onsPostCodes = new List<ONSPostcode>()
            {
                new ONSPostcode()
                {
                    EffectiveFrom = new DateTime(2018, 01, 01),
                    EffectiveTo = new DateTime(2018, 07, 01),
                    Termination = new DateTime(2018, 07, 01)
                },
                new ONSPostcode()
                {
                    EffectiveFrom = new DateTime(2017, 01, 01),
                    EffectiveTo = new DateTime(2018, 01, 01),
                    Termination = new DateTime(2018, 01, 01)
                },
            };

            NewRule().PostcodesContainValidPostcode(learnStartDate, onsPostCodes).Should().BeTrue();
        }

        private DelLocPostCode_15Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFCSDataService fcsDataService = null,
            IPostcodesDataService postcodesDataService = null,
            IDerivedData_22Rule derivedData22 = null)
        {
            return new DelLocPostCode_15Rule(fcsDataService, postcodesDataService, derivedData22, validationErrorHandler);
        }
    }
}
