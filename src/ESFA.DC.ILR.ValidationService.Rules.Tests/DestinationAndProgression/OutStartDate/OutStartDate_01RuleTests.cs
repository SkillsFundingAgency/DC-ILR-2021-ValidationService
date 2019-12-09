using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.DestinationAndProgression.OutStartDate
{
    public class OutStartDate_01RuleTests : AbstractRuleTests<OutStartDate_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutStartDate_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var outStartDate = new DateTime(2008, 07, 01);
            var academicStartMinus10Years = new DateTime(2008, 08, 01);

            NewRule().ConditionMet(outStartDate, academicStartMinus10Years).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var outStartDate = new DateTime(2008, 08, 02);
            var academicStartMinus10Years = new DateTime(2008, 08, 01);

            NewRule().ConditionMet(outStartDate, academicStartMinus10Years).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>()
                {
                    new TestDPOutcome()
                    {
                        OutStartDate = new DateTime(2008, 07, 01)
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(new DateTime(2018, 08, 01));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(ds => ds.AddYearsToDate(academicYearDataServiceMock.Object.Start(), -10)).Returns(new DateTime(2008, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>()
                {
                    new TestDPOutcome()
                    {
                        OutStartDate = new DateTime(2008, 08, 02)
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(new DateTime(2018, 08, 01));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(ds => ds.AddYearsToDate(academicYearDataServiceMock.Object.Start(), -10)).Returns(new DateTime(2008, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void ValidateNoErrorNull()
        {
            var learnerDP = new TestLearnerDestinationAndProgression();

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.Start()).Returns(new DateTime(2018, 08, 01));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(ds => ds.AddYearsToDate(academicYearDataServiceMock.Object.Start(), -10)).Returns(new DateTime(2008, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutStartDate", "01/01/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private OutStartDate_01Rule NewRule(
            IAcademicYearDataService academicYearDataService = null,
            IDateTimeQueryService dateTimeQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutStartDate_01Rule(academicYearDataService, dateTimeQueryService, validationErrorHandler);
        }
    }
}
