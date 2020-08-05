using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_84RuleTests : AbstractRuleTests<LearnDelFAMType_84Rule>
    {
        private DateTime academicYear = new DateTime(2020, 08, 01);
        private HashSet<string> ldmCodes = new HashSet<string>()
        {
            "376"
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_84");
        }

        [Fact]
        public void Validate_Error()
        {
            //arrange
            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "376"
                }
            };

            var learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(2000, 07, 31),
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        LearnStartDate = new DateTime(2019, 6, 1),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "ALB",
                                LearnDelFAMCode = "012"
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "376"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(ldm => ldm.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", ldmCodes))
                .Returns(ldmLearnDelFams);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(x => x.Start()).Returns(new DateTime(2020, 08, 01));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock
               .Setup(ds => ds.AddYearsToDate(new DateTime(2020, 08, 01), -19))
               .Returns(new DateTime(2001, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            //arrange
            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "376"
                }
            };

            var learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(2001, 08, 31),
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                        LearnStartDate = new DateTime(2019, 6, 1),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "ALB",
                                LearnDelFAMCode = "012"
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "376"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(ldm => ldm.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", ldmCodes))
                .Returns(ldmLearnDelFams);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(x => x.Start()).Returns(new DateTime(2020, 08, 01));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock
               .Setup(ds => ds.AddYearsToDate(new DateTime(2020, 08, 01), -19))
               .Returns(new DateTime(2001, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, academicYearDataServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ConditionMet_True()
        {
            var dateOfBirth = new DateTime(2001, 07, 31);
            var fundModel = 35;

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(x => x.Start()).Returns(new DateTime(2020, 08, 01));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock
               .Setup(ds => ds.AddYearsToDate(new DateTime(2020, 08, 01), -19))
               .Returns(new DateTime(2001, 08, 01));

            NewRule(academicYearDataService: academicYearDataServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object).ConditionMet(dateOfBirth, fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData("2001-08-02", 35)] // incorrect dob correct FundModel
        [InlineData("2001-07-31", 25)] // correct dob incorrect FundModel
        public void ConditionMet_False(string dob, int fm)
        {
            var dateOfBirth = DateTime.Parse(dob);
            var fundModel = fm;

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(x => x.Start()).Returns(new DateTime(2020, 08, 01));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock
               .Setup(ds => ds.AddYearsToDate(new DateTime(2020, 08, 01), -19))
               .Returns(new DateTime(2001, 08, 01));

            NewRule(academicYearDataService: academicYearDataServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object).ConditionMet(dateOfBirth, fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData("2001-08-02")] // Is younger than 19 before current teaching year but not LDM 376
        [InlineData("2001-08-01")] // Is younger than 19 before current teaching year but is LDM 376
        public void IsOutsideDateOfBirthRange_False(string dob)
        {
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(x => x.Start()).Returns(new DateTime(2020, 08, 01));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock
               .Setup(ds => ds.AddYearsToDate(new DateTime(2020, 08, 01), -19))
               .Returns(new DateTime(2001, 08, 01));

            NewRule(academicYearDataService: academicYearDataServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object).IsOutsideDateOfBirthRange(DateTime.Parse(dob)).Should().BeFalse();
        }

        [Fact]
        public void IsOutsideDateOfBirthRange_True()
        {
            var dob = new DateTime(2001, 07, 31);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(x => x.Start()).Returns(new DateTime(2020, 08, 01));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock
               .Setup(ds => ds.AddYearsToDate(new DateTime(2020, 08, 01), -19))
               .Returns(new DateTime(2001, 08, 01));

            NewRule(academicYearDataService: academicYearDataServiceMock.Object, dateTimeQueryService: dateTimeQueryServiceMock.Object).IsOutsideDateOfBirthRange(dob).Should().BeTrue();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(36)]
        [InlineData(70)]
        public void IsAdultSkillsFundingModel_False(int fundModel)
        {
            NewRule().IsAdultSkillsFundingModel(fundModel).Should().BeFalse();
        }

        [Fact]
        public void IsAdultSkillsFundingModel_True()
        {
            int fundModel = 35;
            NewRule().IsAdultSkillsFundingModel(fundModel).Should().BeTrue();
        }

        private LearnDelFAMType_84Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IAcademicYearDataService academicYearDataService = null,
            IDateTimeQueryService dateTimeQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_84Rule(
                learningDeliveryFAMQueryService,
                academicYearDataService,
                dateTimeQueryService,
                validationErrorHandler);
        }
    }
}
