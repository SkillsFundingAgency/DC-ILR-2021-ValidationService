using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutCollDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.DestinationAndProgression.OutCollDate
{
    public class OutCollDate_01RuleTests : AbstractRuleTests<OutCollDate_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutCollDate_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var filePrepDate = new DateTime(2018, 08, 01);
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutCollDate = new DateTime(2018, 08, 01)
                    },
                        new TestDPOutcome
                    {
                        OutCollDate = new DateTime(2019, 08, 01)
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(f => f.FilePreparationDate()).Returns(filePrepDate);

            NewRule(fileDataServiceMock.Object).ConditionMet(learnerDP.DPOutcomes).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var filePrepDate = new DateTime(2018, 08, 01);
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutCollDate = new DateTime(2018, 08, 01)
                    },
                        new TestDPOutcome
                    {
                        OutCollDate = new DateTime(2017, 08, 01)
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(f => f.FilePreparationDate()).Returns(filePrepDate);

            NewRule(fileDataServiceMock.Object).ConditionMet(learnerDP.DPOutcomes).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var filePrepDate = new DateTime(2018, 08, 01);
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutCollDate = new DateTime(2018, 08, 01)
                    },
                        new TestDPOutcome
                    {
                        OutCollDate = new DateTime(2019, 08, 01)
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(f => f.FilePreparationDate()).Returns(filePrepDate);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(fileDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var filePrepDate = new DateTime(2018, 08, 01);
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutCollDate = new DateTime(2018, 08, 01)
                    },
                        new TestDPOutcome
                    {
                        OutCollDate = new DateTime(2017, 08, 01)
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(f => f.FilePreparationDate()).Returns(filePrepDate);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(fileDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutCollDate", "01/01/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private OutCollDate_01Rule NewRule(IFileDataService fileDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutCollDate_01Rule(fileDataService, validationErrorHandler);
        }
    }
}
