using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutCollDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.DestinationAndProgression.OutCollDate
{
    public class OutCollDate_02RuleTests : AbstractRuleTests<OutCollDate_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutCollDate_02");
        }

        [Fact]
        public void ValidateError()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutCollDate = new DateTime(2008, 07, 01)
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
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutCollDate = new DateTime(2008, 08, 02)
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

        private OutCollDate_02Rule NewRule(
            IAcademicYearDataService academicYearDataService = null,
            IDateTimeQueryService dateTimeQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutCollDate_02Rule(academicYearDataService, dateTimeQueryService, validationErrorHandler);
        }
    }
}
