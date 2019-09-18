using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_04RuleTests : AbstractRuleTests<EmpStat_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpStat_04");
        }

        [Fact]
        public void GetQualifyingEmploymentStatus_NoApplicableEmploymentRecord()
        {
            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
                {
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 08, 02),
                        EmpStat = 1
                    }
                }
            };

            NewRule().GetQualifyingEmploymentStatus(learner, new DateTime(2019, 08, 01)).Should().BeNull();
        }

        [Fact]
        public void GetQualifyingEmploymentStatus_ApplicableEmploymentRecordOn()
        {
            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
                {
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2017, 06, 15),
                        EmpStat = 1
                    },
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 08, 01),
                        EmpStat = 2
                    },
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 10, 02),
                        EmpStat = 3
                    }
                }
            };

            NewRule().GetQualifyingEmploymentStatus(learner, new DateTime(2019, 08, 01)).Should().Be(2);
        }

        [Fact]
        public void GetQualifyingEmploymentStatus_ApplicableEmploymentRecord_BeforeStartDate()
        {
            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
                {
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2017, 06, 15),
                        EmpStat = 90
                    },
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 07, 15),
                        EmpStat = 91
                    },
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 10, 02),
                        EmpStat = 93
                    }
                }
            };

            NewRule().GetQualifyingEmploymentStatus(learner, new DateTime(2019, 08, 01)).Should().Be(91);
        }

        [Fact]
        public void GetQualifyingEmploymentStatus_NoApplicableEmploymentRecordMultiple()
        {
            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
                {
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 08, 02),
                        EmpStat = 90
                    },
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 09, 15),
                        EmpStat = 91
                    }
                }
            };

            NewRule().GetQualifyingEmploymentStatus(learner, new DateTime(2019, 08, 01)).Should().BeNull();
        }

        [Fact]
        public void Validate_Error_When_Condition_Met()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2019, 08, 01),
                FundModel = 70,
                LearnAimRef = "ZESF0001",
                ConRefNumber = "1",
                CompStatus = 2,
                LearnActEndDateNullable = new DateTime(2020, 08, 01)
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
                {
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 08, 01),
                        EmpStat = 98
                    },
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 09, 15),
                        EmpStat = 91
                    }
                },

                LearningDeliveries = new List<TestLearningDelivery>() { learningDelivery },
            };

            var dd22Mock = new Mock<IDerivedData_22Rule>();

            dd22Mock.Setup(dd => dd.GetLatestLearningStartForESFContract(learningDelivery, learner.LearningDeliveries)).Returns(new DateTime(2019, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd22Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_No_Error()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2019, 08, 01),
                FundModel = 70,
                LearnAimRef = "ZESF0001",
                ConRefNumber = "1",
                CompStatus = 2,
                LearnActEndDateNullable = new DateTime(2020, 08, 01)
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
                {
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 08, 01),
                        EmpStat = 10
                    },
                    new TestLearnerEmploymentStatus()
                    {
                        DateEmpStatApp = new DateTime(2019, 09, 15),
                        EmpStat = 91
                    }
                },

                LearningDeliveries = new List<TestLearningDelivery>() { learningDelivery },
            };

            var dd22Mock = new Mock<IDerivedData_22Rule>();

            dd22Mock.Setup(dd => dd.GetLatestLearningStartForESFContract(learningDelivery, learner.LearningDeliveries)).Returns(new DateTime(2019, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd22Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_No_Error_No_Employment_Record()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2019, 08, 01),
                FundModel = 70,
                LearnAimRef = "ZESF0001",
                ConRefNumber = "1",
                CompStatus = 2,
                LearnActEndDateNullable = new DateTime(2020, 08, 01)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>() { learningDelivery },
            };

            var dd22Mock = new Mock<IDerivedData_22Rule>();

            dd22Mock.Setup(dd => dd.GetLatestLearningStartForESFContract(learningDelivery, learner.LearningDeliveries)).Returns((DateTime?)null);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd22Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private EmpStat_04Rule NewRule(
            IDerivedData_22Rule dd22 = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpStat_04Rule(dd22, validationErrorHandler);
        }

        private DerivedData_22Rule NewDD22()
        {
            return new DerivedData_22Rule();
        }
    }
}
