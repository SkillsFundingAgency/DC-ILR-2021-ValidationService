using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
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
    public class LearnDelFAMType_79RuleTests : AbstractRuleTests<LearnDelFAMType_79Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_79");
        }

        [Fact]
        public void FundModelCondition_True()
        {
            NewRule().FundModelCondition(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelCondition_False()
        {
            NewRule().FundModelCondition(10).Should().BeFalse();
        }

        [Fact]
        public void StartDateCondition_True()
        {
            NewRule().StartDateCondition(new DateTime(2020, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void StartDateCondition_False()
        {
            NewRule().StartDateCondition(new DateTime(2020, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void AgeConditionMet_True()
        {
            var dob = new DateTime(2000, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dob, It.IsAny<DateTime>())).Returns(19);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).AgeConditionMet(new DateTime(2019, 10, 10), dob).Should().BeTrue();
        }

        [Theory]
        [InlineData("2000-11-10")]
        [InlineData("1994-10-10")]
        public void AgeConditionMet_False_Over23(string dateOfBirthString)
        {
            var dateOfBirth = DateTime.Parse(dateOfBirthString);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dateOfBirth, It.IsAny<DateTime>())).Returns(24);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).AgeConditionMet(new DateTime(2018, 10, 10), dateOfBirth).Should().BeFalse();
        }

        [Theory]
        [InlineData("2001-10-10")]
        public void AgeConditionMet_False_Under19(string dateOfBirthString)
        {
            var dateOfBirth = DateTime.Parse(dateOfBirthString);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dateOfBirth, It.IsAny<DateTime>())).Returns(18);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).AgeConditionMet(new DateTime(2018, 10, 10), dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void AgeConditionMet_False_NullDob()
        {
            NewRule().AgeConditionMet(It.IsAny<DateTime>(), null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsCondition_True()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.FFI,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.FFI_Fully
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                testLearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.FFI,
                LearningDeliveryFAMCodeConstants.FFI_Fully)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsCondition(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData("FFI", "2")]
        [InlineData("LDM", "1")]
        [InlineData("LDM", "2")]
        public void LearningDeliveryFAMsCondition_False(string famType, string famCode)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                testLearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.FFI,
                LearningDeliveryFAMCodeConstants.FFI_Fully)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsCondition(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsCondition_False_NullLDFams()
        {
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMCodeForType(
                null,
                LearningDeliveryFAMTypeConstants.FFI,
                LearningDeliveryFAMCodeConstants.FFI_Fully)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsCondition(null).Should().BeFalse();
        }

        [Theory]
        [InlineData("2", 37)]
        [InlineData("1", 35)]
        [InlineData("1", 37)]
        public void LarsCondition_False(string nvq, int categoryRef)
        {
            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                NotionalNVQLevelv2 = nvq,
                Categories = new List<Data.External.LARS.Model.LearningDeliveryCategory>
                {
                    new Data.External.LARS.Model.LearningDeliveryCategory
                    {
                        LearnAimRef = "LearnAimRef",
                        CategoryRef = categoryRef
                    }
                }
            };

            NewRule().LarsCondition(larsDelivery).Should().BeFalse();
        }

        [Fact]
        public void LarsCondition_True_NoCategories()
        {
            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                NotionalNVQLevelv2 = "2"
            };

            NewRule().LarsCondition(larsDelivery).Should().BeTrue();
        }

        [Fact]
        public void LarsCondition_True()
        {
            var larsDelivery = new Data.External.LARS.Model.LearningDelivery
            {
                LearnAimRef = "LearnAimRef",
                NotionalNVQLevelv2 = "2",
                Categories = new List<Data.External.LARS.Model.LearningDeliveryCategory>
                {
                    new Data.External.LARS.Model.LearningDeliveryCategory
                    {
                        LearnAimRef = "LearnAimRef",
                        CategoryRef = 35
                    }
                }
            };

            NewRule().LarsCondition(larsDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(37)]
        [InlineData(35)]
        [InlineData(null)]
        public void DD07Condition_False(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07Condition(progType).Should().BeFalse();
        }

        [Fact]
        public void DD07_True()
        {
            int? progType = 24;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07Condition(progType).Should().BeTrue();
        }

        [Fact]
        public void DD29Condition_False()
        {
            var learningDelivery = new TestLearningDelivery();

            var dd29Mock = new Mock<IDerivedData_29Rule>();
            dd29Mock.Setup(dd => dd.IsInflexibleElementOfTrainingAimLearningDelivery(learningDelivery)).Returns(false);

            NewRule(dd29: dd29Mock.Object).DD29Condition(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void DD29Condition_True()
        {
            var learningDelivery = new TestLearningDelivery();

            var dd29Mock = new Mock<IDerivedData_29Rule>();
            dd29Mock.Setup(dd => dd.IsInflexibleElementOfTrainingAimLearningDelivery(learningDelivery)).Returns(true);

            NewRule(dd29: dd29Mock.Object).DD29Condition(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.UKPRN, 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, "01/01/2000")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, "LearnAimRef")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2000")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 35)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProgType, 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.FFI)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, new DateTime(2000, 01, 01), "LearnAimRef", new DateTime(2000, 01, 01), 35, 1);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_79Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFileDataService fileDataService = null,
            IDateTimeQueryService dateTimeQueryService = null,
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IDerivedData_07Rule dd07 = null,
            IDerivedData_29Rule dd29 = null,
            IDerivedData_37Rule dd37 = null,
            IDerivedData_38Rule dd38 = null)
        {
            return new LearnDelFAMType_79Rule(
                validationErrorHandler,
                fileDataService,
                dateTimeQueryService,
                larsDataService,
                learningDeliveryFAMQueryService,
                dd07,
                dd29,
                dd37,
                dd38);
        }
    }
}
