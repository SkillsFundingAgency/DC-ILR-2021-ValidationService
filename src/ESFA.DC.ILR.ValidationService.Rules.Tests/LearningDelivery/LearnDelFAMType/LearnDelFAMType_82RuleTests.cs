using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_82RuleTests : AbstractRuleTests<LearnDelFAMType_82Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_82");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(99).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void StartDateCondition_True()
        {
            var academicYearService = new Mock<IAcademicYearDataService>();
            academicYearService.Setup(dd => dd.Start()).Returns(new DateTime(2020, 8, 1));

            NewRule(academicYearDataService: academicYearService.Object).LearnStartDateConditionMet(new DateTime(2020, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void StartDateCondition_False()
        {
            var academicYearService = new Mock<IAcademicYearDataService>();
            academicYearService.Setup(dd => dd.Start()).Returns(new DateTime(2020, 8, 1));

            NewRule(academicYearDataService: academicYearService.Object).LearnStartDateConditionMet(new DateTime(2020, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void LarsCategoryConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";

            var categories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41
                }
            };

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataService: larsMock.Object).LarsCategoryConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LarsCategoryConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";
            var categories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    CategoryRef = 50
                }
            };

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetCategoriesFor(learnAimRef)).Returns(new List<ILARSLearningCategory>());

            NewRule(larsDataService: larsMock.Object).LarsCategoryConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void DD35ConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "120"
                    }
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            NewRule(dd35: dd35Mock.Object).DD35ConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void DD35ConditionMet_True_NoFamMatch()
        {
            var learningDelivery = new TestLearningDelivery();

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            NewRule(dd35: dd35Mock.Object).DD35ConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void DD35ConditionMet_False()
        {
            var learningDelivery = new TestLearningDelivery
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "117"
                    }
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(true);

            NewRule(dd35: dd35Mock.Object).DD35ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";

            var categories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "1"
                    }
                }
            };

            var academicYearService = new Mock<IAcademicYearDataService>();
            academicYearService.Setup(dd => dd.Start()).Returns(new DateTime(2020, 8, 1));

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(academicYearService.Object, dd35Mock.Object, larsMock.Object).ConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_StartDate()
        {
            var learnAimRef = "LearnAimRef";

            var categories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 8, 1),
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "1"
                    }
                }
            };

            var academicYearService = new Mock<IAcademicYearDataService>();
            academicYearService.Setup(dd => dd.Start()).Returns(new DateTime(2020, 8, 1));

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(academicYearService.Object, dd35Mock.Object, larsMock.Object).ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var learnAimRef = "LearnAimRef";

            var categories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 99,
                LearnStartDate = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "1"
                    }
                }
            };

            var academicYearService = new Mock<IAcademicYearDataService>();
            academicYearService.Setup(dd => dd.Start()).Returns(new DateTime(2020, 8, 1));

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(academicYearService.Object, dd35Mock.Object, larsMock.Object).ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD35()
        {
            var learnAimRef = "LearnAimRef";

            var categories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "1"
                    }
                }
            };

            var academicYearService = new Mock<IAcademicYearDataService>();
            academicYearService.Setup(dd => dd.Start()).Returns(new DateTime(2020, 8, 1));

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(true);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(academicYearService.Object, dd35Mock.Object, larsMock.Object).ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LARS()
        {
            var learnAimRef = "LearnAimRef";

            var categories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 50
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "1"
                    }
                }
            };

            var academicYearService = new Mock<IAcademicYearDataService>();
            academicYearService.Setup(dd => dd.Start()).Returns(new DateTime(2020, 8, 1));

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(academicYearService.Object, dd35Mock.Object, larsMock.Object).ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnAimRef = "LearnAimRef";

            var categories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41
                }
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "1"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery
                }
            };

            var academicYearService = new Mock<IAcademicYearDataService>();
            academicYearService.Setup(dd => dd.Start()).Returns(new DateTime(2020, 8, 1));

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetCategoriesFor(learnAimRef)).Returns(categories);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, "SOF")).Returns(learningDeliveryFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    academicYearService.Object,
                    dd35Mock.Object,
                    larsMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);

                VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
            }
        }

        [Fact]
        public void Validate_MultipleErrors()
        {
            var learnAimRef = "LearnAimRef";

            var categories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41
                }
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "2"
                }
            };

            var learningDeliveryOne = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                AimSeqNumber = 1,
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learningDeliveryTwo = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                AimSeqNumber = 1,
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDeliveryOne,
                    learningDeliveryTwo
                }
            };

            var academicYearService = new Mock<IAcademicYearDataService>();
            academicYearService.Setup(dd => dd.Start()).Returns(new DateTime(2020, 8, 1));

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDeliveryOne)).Returns(false);
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDeliveryTwo)).Returns(false);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetCategoriesFor(learnAimRef)).Returns(categories);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryOne.LearningDeliveryFAMs, "SOF")).Returns(learningDeliveryFams);
            learningDeliveryFamQueryServiceMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryTwo.LearningDeliveryFAMs, "SOF")).Returns(learningDeliveryFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    academicYearService.Object,
                    dd35Mock.Object,
                    larsMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);

                VerifyErrorHandlerMock(validationErrorHandlerMock, 4);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnAimRef = "LearnAimRef";

            var categories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41
                }
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "1"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery
                }
            };

            var academicYearService = new Mock<IAcademicYearDataService>();
            academicYearService.Setup(dd => dd.Start()).Returns(new DateTime(2020, 8, 1));

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(true);

            var larsMock = new Mock<ILARSDataService>();
            larsMock.Setup(x => x.GetCategoriesFor(learnAimRef)).Returns(categories);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, "SOF")).Returns(learningDeliveryFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    academicYearService.Object,
                    dd35Mock.Object,
                    larsMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void HandleErrors_MultipleSof()
        {
            var learnRefNumber = "LearnRef";
            var aimSeqNumber = 1;
            var learnStartDate = new DateTime(2020, 8, 1);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "2"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                AimSeqNumber = aimSeqNumber,
                FundModel = 35,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, "SOF")).Returns(learningDeliveryFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object, validationErrorHandler: validationErrorHandlerMock.Object).HandleErrors(learnRefNumber, learningDelivery);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 2);
            }
        }

        [Fact]
        public void HandleErrors_SingleSof()
        {
            var learnRefNumber = "LearnRef";
            var aimSeqNumber = 1;
            var learnStartDate = new DateTime(2020, 8, 1);

            var ldm = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LDM",
                LearnDelFAMCode = "2"
            };

            var sof = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "SOF",
                LearnDelFAMCode = "2"
            };

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                sof,
                ldm
            };

            var learningDelivery = new TestLearningDelivery
            {
                AimSeqNumber = aimSeqNumber,
                FundModel = 35,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, "SOF")).Returns(new List<ILearningDeliveryFAM> { sof });

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object, validationErrorHandler: validationErrorHandlerMock.Object).HandleErrors(learnRefNumber, learningDelivery);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
            }
        }

        [Fact]
        public void HandleErrors_NoSofs()
        {
            var learnRefNumber = "LearnRef";
            var aimSeqNumber = 1;
            var learnStartDate = new DateTime(2020, 8, 1);

            var learningDelivery = new TestLearningDelivery
            {
                AimSeqNumber = aimSeqNumber,
                FundModel = 35,
                LearnStartDate = learnStartDate,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "LDM",
                        LearnDelFAMCode = "1"
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, "SOF")).Returns(Enumerable.Empty<ILearningDeliveryFAM>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object, validationErrorHandler: validationErrorHandlerMock.Object).HandleErrors(learnRefNumber, learningDelivery);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
            }
        }

        [Fact]
        public void HandleErrors_NoFams()
        {
            var learnRefNumber = "LearnRef";
            var aimSeqNumber = 1;
            var learnStartDate = new DateTime(2020, 8, 1);

            var learningDelivery = new TestLearningDelivery
            {
                AimSeqNumber = aimSeqNumber,
                FundModel = 35,
                LearnStartDate = learnStartDate,
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, "SOF")).Returns(Enumerable.Empty<ILearningDeliveryFAM>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object, validationErrorHandler: validationErrorHandlerMock.Object).HandleErrors(learnRefNumber, learningDelivery);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
            }
        }

        [Theory]
        [InlineData("Code")]
        [InlineData(null)]
        public void BuildErrorMessageParameters(string famCode)
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2000")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, "SOF")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, famCode)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LarsCategoryRef, 41)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2000, 01, 01), famCode);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_82Rule NewRule(
            IAcademicYearDataService academicYearDataService = null,
            IDerivedData_35Rule dd35 = null,
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_82Rule(
                validationErrorHandler,
                academicYearDataService,
                dd35,
                larsDataService,
                learningDeliveryFAMQueryService);
        }
    }
}