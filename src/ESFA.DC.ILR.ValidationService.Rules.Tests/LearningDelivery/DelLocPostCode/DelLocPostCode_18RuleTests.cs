using System;
using System.Collections.Generic;
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
    public class DelLocPostCode_18RuleTests : AbstractRuleTests<DelLocPostCode_18Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DelLocPostCode_18");
        }

        [Fact]
        public void ConditionMetDD22Exists_True()
        {
            NewRule().ConditionMetDD22Exists(new DateTime(2017, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMetDD22Exists_False()
        {
            NewRule().ConditionMetDD22Exists(null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMetStartDate_True()
        {
            NewRule().ConditionMetStartDate(new DateTime(2017, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMetStartDate_False()
        {
            NewRule().ConditionMetStartDate(new DateTime(2017, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMetFundModel_True()
        {
            NewRule().ConditionMetFundModel(70).Should().BeTrue();
        }

        [Fact]
        public void ConditionMetFundModel_False()
        {
            NewRule().ConditionMetFundModel(36).Should().BeFalse();
        }

        [Fact]
        public void ConditionMetONSPostcode_False_Null()
        {
            NewRule().ConditionMetONSPostcode(null, null).Should().BeFalse();
        }

        [Theory]
        [InlineData("2017-08-01", "2017-08-02", null, null)]
        [InlineData("2017-08-01", "2016-08-01", "2016-12-31", null)]
        [InlineData("2017-08-01", "2017-08-01", "2020-01-01", "2017-01-01")]
        public void ConditionMetONSPostcode_True(string dd22, string effectiveFrom, string effectiveTo, string termination)
        {
            DateTime? dateDD22 = string.IsNullOrEmpty(dd22) ? (DateTime?)null : DateTime.Parse(dd22);
            DateTime effectiveFromDate = DateTime.Parse(effectiveFrom);
            DateTime? effectiveToDate = string.IsNullOrEmpty(effectiveTo) ? (DateTime?)null : DateTime.Parse(effectiveTo);
            DateTime? dateOfTermination = string.IsNullOrEmpty(termination) ? (DateTime?)null : DateTime.Parse(termination);

            var onsPostCodes = new ONSPostcode[]
                {
                    new ONSPostcode()
                    {
                        EffectiveFrom = effectiveFromDate,
                        EffectiveTo = effectiveToDate,
                        Termination = dateOfTermination
                    }
                };

            NewRule().ConditionMetONSPostcode(dateDD22, onsPostCodes).Should().BeTrue();
        }

        [Theory]
        [InlineData("2017-08-01", "2016-08-01", null, null)]
        [InlineData("2017-08-01", "2016-08-01", "2019-01-01", null)]
        [InlineData("2017-08-01", "2017-01-01", null, "2017-08-02")]
        public void ConditionMetONSPostcode_False(string dd22, string effectiveFrom, string effectiveTo, string termination)
        {
            DateTime? dateDD22 = string.IsNullOrEmpty(dd22) ? (DateTime?)null : DateTime.Parse(dd22);
            DateTime effectiveFromDate = DateTime.Parse(effectiveFrom);
            DateTime? effectiveToDate = string.IsNullOrEmpty(effectiveTo) ? (DateTime?)null : DateTime.Parse(effectiveTo);
            DateTime? dateOfTermination = string.IsNullOrEmpty(termination) ? (DateTime?)null : DateTime.Parse(termination);

            var onsPostCodes = new ONSPostcode[]
            {
                new ONSPostcode()
                {
                    EffectiveFrom = effectiveFromDate,
                    EffectiveTo = effectiveToDate,
                    Termination = dateOfTermination
                }
            };

            NewRule().ConditionMetONSPostcode(dateDD22, onsPostCodes).Should().BeFalse();
        }

        [Theory]
        [InlineData("abc", "efg", "hij")]
        [InlineData(null, "efg", "hij")]
        [InlineData("abc", null, "hij")]
        [InlineData("abc", "efg", null)]
        [InlineData("abc", null, null)]
        public void ConditionMetPartnership_True(string code, string lep1, string lep2)
        {
            var partnershipEligibility = new List<EsfEligibilityRuleLocalEnterprisePartnership>
            {
                new EsfEligibilityRuleLocalEnterprisePartnership
                {
                    Code = code
                }
            };

            var onsPostCodes = new ONSPostcode[]
            {
                new ONSPostcode()
                {
                      Lep1 = lep1,
                      Lep2 = lep2
                }
            };

            NewRule().ConditionMetPartnership(partnershipEligibility, onsPostCodes).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("abc", "abc", "abc")]
        [InlineData("abc", "efg", "abc")]
        [InlineData("abc", "abc", "efg")]
        [InlineData(null, "abc", null)]
        [InlineData(null, null, "abc")]
        public void ConditionMetPartnership_False(string code, string lep1, string lep2)
        {
            var partnershipEligibility = new List<EsfEligibilityRuleLocalEnterprisePartnership>
            {
                new EsfEligibilityRuleLocalEnterprisePartnership
                {
                    Code = code
                }
            };

            var onsPostCodes = new ONSPostcode[]
            {
                new ONSPostcode()
                {
                      Lep1 = lep1,
                      Lep2 = lep2
                }
            };

            NewRule().ConditionMetPartnership(partnershipEligibility, onsPostCodes).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error_NonMatchingPartnership()
        {
            var fcsServiceMock = new Mock<IFCSDataService>();
            fcsServiceMock
                .Setup(m => m.GetEligibilityRuleEnterprisePartnershipsFor(It.IsAny<string>()))
                .Returns(new List<EsfEligibilityRuleLocalEnterprisePartnership>
                {
                        new EsfEligibilityRuleLocalEnterprisePartnership
                        {
                            Code = "123"
                        },
                });

            var postcodeServiceMock = new Mock<IPostcodesDataService>();
            postcodeServiceMock
                .Setup(m => m.GetONSPostcodes(It.IsAny<string>()))
                .Returns(new ONSPostcode[]
                {
                        new ONSPostcode()
                        {
                            Lep1 = "ABC",
                            Lep2 = "XYZ",
                            EffectiveFrom = new DateTime(2016, 1, 1)
                        }
                });

            var learningDeliveries = new List<TestLearningDelivery> { };
            var dd22Mock = new Mock<IDerivedData_22Rule>();
            dd22Mock
                .Setup(m => m.GetLatestLearningStartForESFContract(It.IsAny<TestLearningDelivery>(), It.IsAny<List<TestLearningDelivery>>()))
                        .Returns(new DateTime(2017, 8, 1));

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                    {
                        new TestLearningDelivery
                        {
                            LearnStartDate = new DateTime(2017, 8, 1),
                            FundModel = 70,
                            LearnAimRef = "12345678",
                            DelLocPostCode = "CV1 2WT"
                        },
                    }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsServiceMock.Object, postcodeServiceMock.Object, dd22Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_Error_LearnStartBeforeEffectiveDate()
        {
            var fcsServiceMock = new Mock<IFCSDataService>();
            fcsServiceMock
                .Setup(m => m.GetEligibilityRuleEnterprisePartnershipsFor(It.IsAny<string>()))
                .Returns(new List<EsfEligibilityRuleLocalEnterprisePartnership>
                {
                        new EsfEligibilityRuleLocalEnterprisePartnership
                        {
                            Code = "ABC"
                        },
                });

            var postcodeServiceMock = new Mock<IPostcodesDataService>();
            postcodeServiceMock
                .Setup(m => m.GetONSPostcodes(It.IsAny<string>()))
                .Returns(new ONSPostcode[]
                {
                        new ONSPostcode()
                        {
                            Lep1 = "ABC",
                            EffectiveFrom = new DateTime(2019, 1, 1)
                        }
                });

            var learningDeliveries = new List<TestLearningDelivery> { };
            var dd22Mock = new Mock<IDerivedData_22Rule>();
            dd22Mock
                .Setup(m => m.GetLatestLearningStartForESFContract(It.IsAny<TestLearningDelivery>(), It.IsAny<List<TestLearningDelivery>>()))
                        .Returns(new DateTime(2017, 8, 1));

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                    {
                        new TestLearningDelivery
                        {
                            LearnStartDate = new DateTime(2017, 8, 1),
                            FundModel = 70,
                            LearnAimRef = "12345678",
                            DelLocPostCode = "CV1 2WT"
                        },
                    }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsServiceMock.Object, postcodeServiceMock.Object, dd22Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_No_Error()
        {
            var fcsServiceMock = new Mock<IFCSDataService>();
            fcsServiceMock
                .Setup(m => m.GetEligibilityRuleEnterprisePartnershipsFor(It.IsAny<string>()))
                .Returns(new List<EsfEligibilityRuleLocalEnterprisePartnership>
                {
                        new EsfEligibilityRuleLocalEnterprisePartnership
                        {
                            Code = "ABC"
                        },
                });

            var postcodeServiceMock = new Mock<IPostcodesDataService>();
            postcodeServiceMock
                .Setup(m => m.GetONSPostcodes(It.IsAny<string>()))
                .Returns(new ONSPostcode[]
                {
                        new ONSPostcode()
                        {
                            Lep1 = "ABC",
                            Lep2 = "XYZ",
                            EffectiveFrom = new DateTime(2016, 1, 1)
                        }
                });

            var learningDeliveries = new List<TestLearningDelivery> { };
            var dd22Mock = new Mock<IDerivedData_22Rule>();
            dd22Mock
                .Setup(m => m.GetLatestLearningStartForESFContract(It.IsAny<TestLearningDelivery>(), It.IsAny<List<TestLearningDelivery>>()))
                        .Returns(new DateTime(2017, 8, 1));

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                    {
                        new TestLearningDelivery
                        {
                            LearnStartDate = new DateTime(2017, 8, 1),
                            FundModel = 70,
                            LearnAimRef = "12345678",
                            DelLocPostCode = "CV1 2WT"
                        },
                    }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsServiceMock.Object, postcodeServiceMock.Object, dd22Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_No_Error_NoEligibility()
        {
            var fcsServiceMock = new Mock<IFCSDataService>();
            fcsServiceMock
                .Setup(m => m.GetEligibilityRuleEnterprisePartnershipsFor(It.IsAny<string>()))
                .Returns(Utility.Collection.EmptyAndReadOnly<IEsfEligibilityRuleLocalEnterprisePartnership>());

            var postcodeServiceMock = new Mock<IPostcodesDataService>();
            postcodeServiceMock
                .Setup(m => m.GetONSPostcodes(It.IsAny<string>()))
                .Returns(new ONSPostcode[]
                {
                        new ONSPostcode()
                        {
                            Lep1 = "ABC",
                            Lep2 = "XYZ",
                            EffectiveFrom = new DateTime(2016, 1, 1)
                        }
                });

            var learningDeliveries = new List<TestLearningDelivery> { };
            var dd22Mock = new Mock<IDerivedData_22Rule>();
            dd22Mock
                .Setup(m => m.GetLatestLearningStartForESFContract(It.IsAny<TestLearningDelivery>(), It.IsAny<List<TestLearningDelivery>>()))
                        .Returns(new DateTime(2017, 8, 1));

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                    {
                        new TestLearningDelivery
                        {
                            LearnStartDate = new DateTime(2017, 8, 1),
                            FundModel = 70,
                            LearnAimRef = "12345678",
                            DelLocPostCode = "CV1 2WT"
                        },
                    }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsServiceMock.Object, postcodeServiceMock.Object, dd22Mock.Object).Validate(testLearner);
            }
        }

        private DelLocPostCode_18Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFCSDataService fcsDataService = null,
            IPostcodesDataService postcodesDataService = null,
            IDerivedData_22Rule derivedData22 = null)
        {
            return new DelLocPostCode_18Rule(fcsDataService, postcodesDataService, derivedData22, validationErrorHandler);
        }
    }
}
