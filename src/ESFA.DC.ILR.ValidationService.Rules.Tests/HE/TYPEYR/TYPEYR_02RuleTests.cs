using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.TYPEYR;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.TYPEYR
{
    public class TYPEYR_02RuleTests : AbstractRuleTests<TYPEYR_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("TYPEYR_02");
        }

        [Fact]
        public void ConditionMetLearningDelivery_True()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            DateTime? learnActEndDate = new DateTime(2018, 01, 06);
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnStartDate, AcademicYearDates.Commencement))
                .Returns(new DateTime(2018, 08, 01));

            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnActEndDate.Value, AcademicYearDates.Commencement))
                .Returns(new DateTime(2018, 08, 01));

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).ConditionMetLearningDelivery(learnStartDate, learnActEndDate).Should().BeTrue();
        }

        [Fact]
        public void ConditionMetLearningDelivery_False()
        {
            var learnStartDate = new DateTime(2017, 01, 01);
            DateTime? learnActEndDate = new DateTime(2018, 01, 06);
            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnStartDate, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnActEndDate.Value, AcademicYearDates.Commencement))
                .Returns(new DateTime(2018, 08, 01));

            NewRule(academicYearQueryService: academicYearQueryServiceMock.Object).ConditionMetLearningDelivery(learnStartDate, learnActEndDate).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 2, true)]
        [InlineData(1, 3, true)]
        [InlineData(1, 1, false)]
        public void LearningDeliveryHEConditionMeetsExpectation(int fundComp, int typeYr, bool expectation)
        {
            NewRule().ConditionMetLearningDeliveryHE(fundComp, typeYr).Should().Be(expectation);
        }

        [Fact]
        public void Validate_AcademicYearMismatching_NoError()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            DateTime? learnActEndDate = new DateTime(2017, 12, 01);
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = 1,
                            TYPEYR = 1
                        }
                    }
                }
            };

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnStartDate, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnActEndDate.Value, AcademicYearDates.Commencement))
                .Returns(new DateTime(2018, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, academicYearQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            DateTime? learnActEndDate = new DateTime(2017, 12, 01);
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = 1,
                            TYPEYR = 2
                        }
                    }
                }
            };

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnStartDate, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnActEndDate.Value, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, academicYearQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_MultipleDeliveries_Error()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            DateTime? learnActEndDate = new DateTime(2017, 12, 01);
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = 1,
                            TYPEYR = 1
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = 2,
                            TYPEYR = 1
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = 1,
                            TYPEYR = 2
                        }
                    }
                }
            };

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnStartDate, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnActEndDate.Value, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, academicYearQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            DateTime? learnActEndDate = new DateTime(2017, 12, 01);
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = 1,
                            TYPEYR = 1
                        }
                    }
                }
            };

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnStartDate, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnActEndDate.Value, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, academicYearQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_MultipleDeliveries_NoError()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            DateTime? learnActEndDate = new DateTime(2017, 12, 01);
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = 1,
                            TYPEYR = 1
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = 2,
                            TYPEYR = 1
                        }
                    },
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = 2,
                            TYPEYR = 2
                        }
                    }
                }
            };

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnStartDate, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnActEndDate.Value, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, academicYearQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NullLearningDeliveryHE_NoError()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            DateTime? learnActEndDate = new DateTime(2017, 12, 01);
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryHEEntity = null
                    }
                }
            };

            var academicYearQueryServiceMock = new Mock<IAcademicYearQueryService>();
            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnStartDate, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            academicYearQueryServiceMock
                .Setup(ds => ds.GetAcademicYearOfLearningDate(learnActEndDate.Value, AcademicYearDates.Commencement))
                .Returns(new DateTime(2017, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, academicYearQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NullLearningDelivery_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.TYPEYR, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FUNDCOMP, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2008")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "01/04/2008")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, 1, new DateTime(2008, 01, 01), new DateTime(2008, 04, 01));

            validationErrorHandlerMock.Verify();
        }

        private TYPEYR_02Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IAcademicYearQueryService academicYearQueryService = null)
        {
            return new TYPEYR_02Rule(validationErrorHandler, academicYearQueryService);
        }
    }
}
