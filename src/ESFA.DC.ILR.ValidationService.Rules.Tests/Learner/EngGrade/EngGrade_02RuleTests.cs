using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.EngGrade
{
    public class EngGrade_02RuleTests : AbstractRuleTests<EngGrade_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EngGrade_02");
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            string engGrade = Grades.AstarA;

            var validaionErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validaionErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.EngGrade, engGrade)).Verifiable();

            NewRule(validationErrorHandler: validaionErrorHandlerMock.Object).BuildErrorMessageParameters(engGrade);

            validaionErrorHandlerMock.Verify();
        }

        [Fact]
        public void EngGradeConditionMet_False()
        {
            string engGrade = "ABC";

            var provideLookupDetails = new Mock<IProvideLookupDetails>();

            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, engGrade)).Returns(true);

            NewRule(provideLookupDetails: provideLookupDetails.Object).EngGradeConditionMet(engGrade).Should().BeFalse();
        }

        [Theory]
        [InlineData(Grades.AstarA)]
        [InlineData("a*a*")]
        public void EngGradeConditionMet_True(string engGrade)
        {
            var provideLookupDetails = new Mock<IProvideLookupDetails>();

            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, engGrade)).Returns(false);

            NewRule(provideLookupDetails: provideLookupDetails.Object).EngGradeConditionMet(engGrade).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(Grades.NONE)]
        [InlineData("none")]
        public void EngGradeSuppliedAndNotNone_False(string engGrade)
        {
            NewRule().EngGradeSuppliedAndNotNone(engGrade).Should().BeFalse();
        }

        [Theory]
        [InlineData("a*a")]
        [InlineData(Grades.AstarAstar)]
        public void EngGradeSuppliedAndNotNone_True(string engGrade)
        {
            NewRule().EngGradeSuppliedAndNotNone(engGrade).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            string engGrade = "QQ";
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                EngGrade = engGrade,
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();

            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, engGrade)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    provideLookupDetails: provideLookupDetails.Object)
                    .Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(Grades.NONE)]
        [InlineData("")]
        [InlineData(Grades.AB)]
        [InlineData("ab")]
        public void Validate_NoError(string engGrade)
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                EngGrade = engGrade,
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();

            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, engGrade)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    provideLookupDetails: provideLookupDetails.Object)
                    .Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoGradeSupplied_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "1A234"
                    }
                }
            };

            var provideLookupDetails = new Mock<IProvideLookupDetails>();

            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, Grades.AstarA)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    provideLookupDetails: provideLookupDetails.Object)
                    .Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NullCheck()
        {
            TestLearner testLearner = null;

            var provideLookupDetails = new Mock<IProvideLookupDetails>();

            provideLookupDetails.Setup(p => p.Contains(TypeOfStringCodedLookup.GCSEGrade, Grades.AstarA)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    provideLookupDetails: provideLookupDetails.Object)
                    .Validate(testLearner);
            }
        }

        public EngGrade_02Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IProvideLookupDetails provideLookupDetails = null)
        {
            return new EngGrade_02Rule(
                validationErrorHandler: validationErrorHandler,
                provideLookupDetails: provideLookupDetails);
        }
    }
}
