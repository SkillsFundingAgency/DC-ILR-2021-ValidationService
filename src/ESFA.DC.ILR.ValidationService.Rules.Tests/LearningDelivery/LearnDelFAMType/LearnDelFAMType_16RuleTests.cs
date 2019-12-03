using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_16RuleTests : AbstractRuleTests<LearnDelFAMType_16Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_16");
        }

        [Fact]
        public void ValidationPasses_SubmissionBeforeYearEnd()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.ReturnPeriod()).Returns(10);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = "118"
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object).Validate(testLearner);
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPassesIrrelevantFamType()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.ReturnPeriod()).Returns(10);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                                LearnDelFAMCode = "118"
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object).Validate(testLearner);
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPassesIrrelevantFamCode()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.ReturnPeriod()).Returns(10);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnActEndDateNullable = new DateTime(2018, 11, 2),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = "119"
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object).Validate(testLearner);
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_NoFAMs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.ReturnPeriod()).Returns(13);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object).Validate(testLearner);
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 12)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 13)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 14)]
        public void ValidationFails(string learDelFamType, int returnPeriod)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.ReturnPeriod()).Returns(returnPeriod);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnActEndDateNullable = new DateTime(2017, 10, 31),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = learDelFamType,
                                LearnDelFAMCode = "118"
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object).Validate(testLearner);
            VerifyHandleCalled(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 1)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 2)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 3)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 4)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 5)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 6)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 7)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 8)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 9)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 10)]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, 11)]
        public void ValidationNotCalled(string learDelFamType, int returnPeriod)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.ReturnPeriod()).Returns(returnPeriod);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnActEndDateNullable = new DateTime(2017, 10, 31),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = learDelFamType,
                                LearnDelFAMCode = "118"
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object).Validate(testLearner);
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        private LearnDelFAMType_16Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IAcademicYearDataService academicYearDataService = null)
        {
            return new LearnDelFAMType_16Rule(academicYearDataService, validationErrorHandler);
        }

        private void VerifyHandleNotCalled(ValidationErrorHandlerMock errorHandlerMock)
        {
            errorHandlerMock.Verify(
                m => m.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()),
                Times.Never);
        }

        private void VerifyHandleCalled(ValidationErrorHandlerMock errorHandlerMock)
        {
            errorHandlerMock.Verify(
                m => m.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()),
                Times.Once);
        }
    }
}
