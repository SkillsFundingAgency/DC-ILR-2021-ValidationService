using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R20RuleTests : AbstractRuleTests<R20Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R20");
        }

        [Fact]
        public void ComponentAimTypeConditionMet_False()
        {
            NewRule().ComponentAimTypeConditionMet(TypeOfAim.AimNotPartOfAProgramme).Should().BeFalse();
        }

        [Fact]
        public void ComponentAimTypeConditionMet_True()
        {
            NewRule().ComponentAimTypeConditionMet(TypeOfAim.ComponentAimInAProgramme).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(55)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(55).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(TypeOfLearningProgramme.HigherApprenticeshipLevel6)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(TypeOfLearningProgramme.HigherApprenticeshipLevel6).Should().BeTrue();
        }

        [Theory]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(null)]
        public void ApprenticeshipStandardsConditionMet_False(int? progType)
        {
            NewRule().ApprenticeshipStandardsConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardsConditionMet_True()
        {
            NewRule().ApprenticeshipStandardsConditionMet(TypeOfLearningProgramme.HigherApprenticeshipLevel4).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            var progType = 3;
            var fworkCode = 445;
            var pwayCode = 1;
            var learnStartDate = new DateTime(2019, 01, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };

            var larsMock = new Mock<ILARSDataService>();

            larsMock.Setup(e => e.FrameworkCodeExistsForFrameworkAimsAndFrameworkComponentTypes(learnAimRef, progType, fworkCode, pwayCode, frameWorkComponentTypes, learnStartDate)).Returns(false);

            NewRule(larsDataService: larsMock.Object).LARSConditionMet(learnAimRef, progType, fworkCode, pwayCode, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            string learnAimRef = "ZESF98765";
            var progType = 3;
            var fworkCode = 445;
            var pwayCode = 1;
            var learnStartDate = new DateTime(2019, 01, 01);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };

            var larsMock = new Mock<ILARSDataService>();

            larsMock.Setup(e => e.FrameworkCodeExistsForFrameworkAimsAndFrameworkComponentTypes(learnAimRef, progType, fworkCode, pwayCode, frameWorkComponentTypes, learnStartDate)).Returns(true);

            NewRule(larsDataService: larsMock.Object).LARSConditionMet(learnAimRef, progType, fworkCode, pwayCode, learnStartDate).Should().BeTrue();
        }

        [Theory]
        [InlineData("2015-07-02", "2015-12-01", true)]
        [InlineData("2015-12-01", "2015-07-02", false)]
        public void LearnStartDateConditionMet_False(string learnStartDateString, string learnActEndDateString, bool firstRecord)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            NewRule().LearnStartDateConditionMet(learnStartDate, learnActEndDate, firstRecord).Should().BeFalse();
        }

        [Theory]
        [InlineData("2015-07-02", null, true)]
        [InlineData("2015-07-02", "2015-12-01", false)]
        [InlineData("2015-12-01", null, false)]
        public void LearnStartDateConditionMet_True(string learnStartDateString, string learnActEndDateString, bool firstRecord)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            NewRule().LearnStartDateConditionMet(learnStartDate, learnActEndDate, firstRecord).Should().BeTrue();
        }

        // [InlineData(TypeOfAim.ComponentAimInAProgramme, TypeOfLearningProgramme.HigherApprenticeshipLevel4, "ZESF98765", "2015-07-02", "2015-01-01", false)]
        // [InlineData(TypeOfAim.AimNotPartOfAProgramme, TypeOfLearningProgramme.HigherApprenticeshipLevel4, "ZESF98765", "2015-07-02", "2015-01-01", false)]
        // [InlineData("2015-07-02", "2015-01-01", false)]
        [Theory]
        [InlineData("2015-07-02", "2015-01-01", false)]
        public void ConditionMet_False(string learnStartDateString, string learnActEndDateString, bool firstRecord)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            NewRule()
                .ConditionMet(learnStartDate, learnActEndDate, firstRecord)
                .Should().BeFalse();
        }

        // [InlineData("ZESF98765", TypeOfLearningProgramme.HigherApprenticeshipLevel4, "2015-07-02", null, true)]
        // [InlineData("ZESF98765", TypeOfLearningProgramme.HigherApprenticeshipLevel4, "2015-07-02", "2015-12-01", false)]
        [Theory]
        [InlineData("2015-07-02", null, true)]
        [InlineData("2015-07-02", "2015-12-01", false)]
        public void ConditionMet_True(string learnStartDateString, string learnActEndDateString, bool firstRecord)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            NewRule()
                .ConditionMet(
                    learnStartDate,
                    learnActEndDate,
                    firstRecord)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };
            var progType = TypeOfLearningProgramme.AdvancedLevelApprenticeship;
            var fworkCode = 445;
            var pwayCode = 1;
            var learnStartDate = new DateTime(2019, 01, 01);
            string learnAimRef = "ZESF98765";
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = TypeOfAim.AimNotPartOfAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        LearnStartDate = new DateTime(2017, 02, 01),
                        LearnActEndDateNullable = new DateTime(2017, 02, 28),
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        LearnStartDate = new DateTime(2017, 02, 24),
                        LearnActEndDateNullable = new DateTime(2017, 03, 01),
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = TypeOfAim.AimNotPartOfAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        LearnStartDate = new DateTime(2017, 02, 25),
                        LearnActEndDateNullable = new DateTime(2017, 10, 25),
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        LearnStartDate = new DateTime(2017, 02, 26),
                        LearnActEndDateNullable = new DateTime(2017, 11, 25),
                        FworkCodeNullable = fworkCode,
                        PwayCodeNullable = pwayCode
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(2)).Returns(true);
            larsDataServiceMock.Setup(e => e.FrameworkCodeExistsForFrameworkAimsAndFrameworkComponentTypes(learnAimRef, progType, fworkCode, pwayCode, frameWorkComponentTypes, It.IsAny<DateTime>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    larsDataService: larsDataServiceMock.Object,
                    dd07: dd07Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };
            string learnAimRef = "ZESF98765";
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2015, 07, 02),
                        LearnActEndDateNullable = null
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(2)).Returns(true);
            larsDataServiceMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    larsDataService: larsDataServiceMock.Object,
                    dd07: dd07Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(e => e.BuildErrorMessageParameter(PropertyNameConstants.AimType, TypeOfAim.ComponentAimInAProgramme)).Verifiable();
            validationErrorHandlerMock.Setup(e => e.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "02/07/2018")).Verifiable();
            validationErrorHandlerMock.Setup(e => e.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "25/10/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(
                TypeOfAim.ComponentAimInAProgramme,
                new DateTime(2018, 07, 02),
                new DateTime(2018, 10, 25));

            validationErrorHandlerMock.Verify();
        }

        public R20Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILARSDataService larsDataService = null,
            IDerivedData_07Rule dd07 = null)
        {
            return new R20Rule(
                validationErrorHandler: validationErrorHandler,
                lARSDataService: larsDataService,
                dd07: dd07);
        }
    }
}
