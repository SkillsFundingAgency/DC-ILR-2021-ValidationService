using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.DestinationAndProgression.OutStartDate
{
    public class OutStartDate_02RuleTests : AbstractRuleTests<OutStartDate_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutStartDate_02");
        }

        [Fact]
        public void ConditionMet_False()
        {
            DateTime outStartDate = new DateTime(2018, 07, 01);
            DateTime endOfAcademicYear = DateTime.Now.AddYears(1);

            NewRule().ConditionMet(outStartDate, endOfAcademicYear).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            DateTime outStartDate = DateTime.Now.AddYears(2);
            DateTime endOfAcademicYear = DateTime.Now.AddYears(1);
            NewRule().ConditionMet(outStartDate, endOfAcademicYear).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearningDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                DPOutcomes = new TestDPOutcome[]
                {
                    new TestDPOutcome()
                    {
                        OutStartDate = DateTime.Now.AddYears(2)
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(a => a.End()).Returns(DateTime.Now);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    academicYearDataService: academicYearDataServiceMock.Object)
                    .Validate(testLearningDestinationAndProgression);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearningDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                DPOutcomes = new TestDPOutcome[]
                {
                    new TestDPOutcome()
                    {
                        OutStartDate = DateTime.Now
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(a => a.End()).Returns(DateTime.Now);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    academicYearDataService: academicYearDataServiceMock.Object)
                    .Validate(testLearningDestinationAndProgression);
            }
        }

        [Fact]
        public void Validate_NoError_NullCheck()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object)
                    .Validate(null);
            }
        }

        [Fact]
        public void Validate_NoError_DBOutComeNullCheck()
        {
            var testLearningDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                DPOutcomes = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object)
                    .Validate(testLearningDestinationAndProgression);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHanldeMock = new Mock<IValidationErrorHandler>();

            validationErrorHanldeMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.OutStartDate, "10/12/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHanldeMock.Object).BuildErrorMessageParameters(new DateTime(2018, 12, 10));

            validationErrorHanldeMock.Verify();
        }

        private OutStartDate_02Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IAcademicYearDataService academicYearDataService = null)
        {
            return new OutStartDate_02Rule(
                validationErrorHandler: validationErrorHandler,
                academicYearDataService: academicYearDataService);
        }
    }
}
