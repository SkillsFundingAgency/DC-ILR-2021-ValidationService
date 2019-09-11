using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LSDPostcode
{
    /// <summary>
    /// learn start date postcode rule 02 tests
    /// </summary>
    public class LSDPostcode_02RuleTests
    {
        private const int TestProviderID = 10001973;

        private const string TestLegalOrgType = "PLBG"; // not specialist designated college

        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LSDPostcode_02Rule(null, commonOps.Object, organisationData.Object, fileData.Object, postcodeData.Object));
        }

        /// <summary>
        /// New rule with null common ops throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOpsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LSDPostcode_02Rule(handler.Object, null, organisationData.Object, fileData.Object, postcodeData.Object));
        }

        [Fact]
        public void NewRuleWithNullOrganisationDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LSDPostcode_02Rule(handler.Object, commonOps.Object, null, fileData.Object, postcodeData.Object));
        }

        [Fact]
        public void NewRuleWithNullFileDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, null, postcodeData.Object));
        }

        /// <summary>
        /// New rule with null file data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullPostcodeDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, fileData.Object, null));
        }

        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("LSDPostcode_02", result);
        }

        /// <summary>
        /// Rule name 2, matches the constant.
        /// </summary>
        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(RuleNameConstants.LSDPostcode_02, result);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        /// <summary>
        /// First viable start meets expectation.
        /// </summary>
        [Fact]
        public void FirstViableStartMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Equal(DateTime.Parse("2019-08-01"), sut.FirstViableStart);
        }

        /// <summary>
        /// Organisation type meets expectation.
        /// </summary>
        [Fact]
        public void OrganisationTypeMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Equal(TestLegalOrgType, sut.OrganisationType);
        }

        /// <summary>
        /// Validate with null learner throws.
        /// </summary>
        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        [Theory]
        [InlineData(1003, "PBLG", false)]
        [InlineData(1005, "PBLG", false)]
        [InlineData(1006, "USDC", true)]
        [InlineData(1007, "USDC", true)]
        public void IsSpecialistDesignatedCollegeMeetsExpectation(int ukprn, string candidate, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationData
                .Setup(x => x.GetLegalOrgTypeForUkprn(ukprn))
                .Returns(candidate);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(ukprn);

            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            var sut = new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, fileData.Object, postcodeData.Object);

            // act
            var result = sut.IsSpecialistDesignatedCollege();

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            organisationData.VerifyAll();
            fileData.VerifyAll();
            postcodeData.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(21, true)]
        [InlineData(24, true)]
        [InlineData(25, true)]
        [InlineData(22, true)]
        [InlineData(1, true)]
        [InlineData(0, true)]
        public void HasProgrammeDefinedMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.HasProgrammeDefined(delivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsRestartMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(expectation);

            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationData
                .Setup(x => x.GetLegalOrgTypeForUkprn(TestProviderID))
                .Returns(TestLegalOrgType);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);
            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            var sut = new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, fileData.Object, postcodeData.Object);

            // act
            var result = sut.IsRestart(delivery.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            organisationData.VerifyAll();
            fileData.VerifyAll();
            postcodeData.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("DAM001", Monitoring.Delivery.PostcodeValidationExclusion)]
        public void MonitoringDeliveryMeetsExpectation(string expectation, string candidate)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData("DAM", "012", false)]
        [InlineData("DAM", "002", false)]
        [InlineData("LDM", "034", false)]
        [InlineData("DAM", "001", true)]
        public void IsPostcodeValidationExclusionMeetsExpectation(string candidateType, string candidateCode, bool expectation)
        {
            // arrange
            var monitor = new Mock<ILearningDeliveryFAM>();
            monitor
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(candidateType);
            monitor
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns(candidateCode);

            var sut = NewRule();

            // act
            var result = sut.IsPostcodeValidationExclusion(monitor.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Type of funding meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(35, TypeOfFunding.AdultSkills)]
        [InlineData(10, TypeOfFunding.CommunityLearning)]
        public void TypeOfFundingMeetsExpectation(int expectation, int candidate)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Has qualifying model meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void HasQualifyingModelMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 35, 10))
                .Returns(expectation);

            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationData
                .Setup(x => x.GetLegalOrgTypeForUkprn(TestProviderID))
                .Returns(TestLegalOrgType);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            var sut = new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, fileData.Object, postcodeData.Object);

            // act
            var result = sut.HasQualifyingModel(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            organisationData.VerifyAll();
            fileData.VerifyAll();
            postcodeData.VerifyAll();
        }

        /// <summary>
        /// Has qualifying start meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void HasQualifyingStartMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, DateTime.Parse("2019-08-01"), null))
                .Returns(expectation);

            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationData
                .Setup(x => x.GetLegalOrgTypeForUkprn(TestProviderID))
                .Returns(TestLegalOrgType);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            var sut = new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, fileData.Object, postcodeData.Object);

            // act
            var result = sut.HasQualifyingStart(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            organisationData.VerifyAll();
            fileData.VerifyAll();
            postcodeData.VerifyAll();
        }

        [Theory]
        [InlineData("blah")]
        [InlineData("blah blah")]
        public void GetDevolvedPostcodesMeetsExpectation(string candidate)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .Setup(x => x.LSDPostcode)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationData
                .Setup(x => x.GetLegalOrgTypeForUkprn(TestProviderID))
                .Returns(TestLegalOrgType);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            postcodeData
                .Setup(x => x.GetDevolvedPostcodes(candidate))
                .Returns(new IDevolvedPostcode[] { });

            var sut = new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, fileData.Object, postcodeData.Object);

            // act
            var result = sut.GetDevolvedPostcodes(delivery.Object);

            // assert
            Assert.Empty(result);

            delivery.VerifyAll();

            handler.VerifyAll();
            commonOps.VerifyAll();
            organisationData.VerifyAll();
            fileData.VerifyAll();
            postcodeData.VerifyAll();
        }

        /// <summary>
        /// Get delivery funding codes returns empty set by default.
        /// </summary>
        [Fact]
        public void GetDeliveryFundingCodesReturnsEmptySetByDefault()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns((IReadOnlyCollection<ILearningDeliveryFAM>)null);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationData
                .Setup(x => x.GetLegalOrgTypeForUkprn(TestProviderID))
                .Returns(TestLegalOrgType);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            var sut = new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, fileData.Object, postcodeData.Object);

            // act
            var result = sut.GetDeliveryFundingCodes(delivery.Object);

            // assert
            Assert.Empty(result);

            delivery.VerifyAll();

            handler.VerifyAll();
            commonOps.VerifyAll();
            organisationData.VerifyAll();
            fileData.VerifyAll();
            postcodeData.VerifyAll();
        }

        /// <summary>
        /// Has qualifying funding type meets expectaton
        /// this is a 'random' selection of funding and monitoring types
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("SOF", true)] // Monitoring.Delivery.Types.SourceOfFunding
        [InlineData("DAM", false)] // Monitoring.Delivery.Types.DevolvedAreaMonitoring
        [InlineData("ADL", false)] // Monitoring.Delivery.Types.AdvancedLearnerLoan
        [InlineData("ALB", false)] // Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding
        [InlineData("LDM", false)] // Monitoring.Delivery.Types.Learner
        [InlineData("ACT", false)] // Monitoring.Delivery.Types.ApprenticeshipContract
        [InlineData("ASL", false)] // Monitoring.Delivery.Types.CommunityLearningProvision
        [InlineData("EEF", false)] // Monitoring.Delivery.Types.CommunityLearningProvision
        [InlineData("FLN", false)] // Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage
        [InlineData("FFI", false)] // Monitoring.Delivery.Types.FullOrCoFunding
        public void HasQualifyingFundingTypeMeetsExpectaton(string candidate, bool expectation)
        {
            // arrange
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .Setup(x => x.LearnDelFAMType)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.HasQualifyingFundingType(fam.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("blah")]
        [InlineData("blah blah")]
        public void GetFundingCodeMeetsExpectaton(string expectation)
        {
            // arrange
            var fam = new Mock<ILearningDeliveryFAM>(MockBehavior.Strict);
            fam
                .Setup(x => x.LearnDelFAMCode)
                .Returns(expectation);

            var sut = NewRule();

            // act
            var result = sut.GetFundingCode(fam.Object);

            // assert
            Assert.Equal(expectation, result);
            fam.VerifyAll();
        }

        /// <summary>
        /// Get devolved postcodes for sof returns empty set using duff parameters.
        /// </summary>
        [Fact]
        public void GetDevolvedPostcodesForSoFReturnsEmptySetUsingDuffParameters()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.GetDevolvedPostcodesForSoF(null, x => true);

            // assert
            Assert.Empty(result);
        }

        /// <summary>
        /// Get devolved postcodes for sof returns empty set using empty parameters.
        /// </summary>
        [Fact]
        public void GetDevolvedPostcodesForSoFReturnsEmptySetUsingEmptyParameters()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.GetDevolvedPostcodesForSoF(new IDevolvedPostcode[] { }, x => true);

            // assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("blah blah", true)]
        [InlineData("blah blah blah", false)]
        public void HasQualifyingFundingCodeMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var devolvedCode = new Mock<IDevolvedPostcode>(MockBehavior.Strict);
            devolvedCode
                .SetupGet(x => x.SourceOfFunding)
                .Returns(candidate);

            var sofCodes = new Mock<IContainThis<string>>(MockBehavior.Strict);
            sofCodes
                .Setup(x => x.Contains(candidate))
                .Returns(expectation);

            var sut = NewRule();

            // act
            var result = sut.HasQualifyingFundingCode(devolvedCode.Object, sofCodes.Object);

            // assert
            Assert.Equal(expectation, result);

            devolvedCode.VerifyAll();
            sofCodes.VerifyAll();
        }

        /// <summary>
        /// Has valid source of funding with null devolved postcodes returns false
        /// </summary>
        [Fact]
        public void HasValidSourceOfFundingWithNullDevolvedPostcodesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasValidSourceOfFunding(null, x => true);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has valid source of funding with empty devolved postcodes returns false
        /// </summary>
        [Fact]
        public void HasValidSourceOfFundingWithEmptyDevolvedPostcodesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasValidSourceOfFunding(new IDevolvedPostcode[] { }, x => true);

            // assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("2018-06-04", "2018-06-03", false)]
        [InlineData("2018-06-04", "2018-06-04", true)]
        [InlineData("2018-06-04", "2018-06-05", true)]
        public void HasQualifyingEffectiveStartMeetsExpectation(string effective, string start, bool expectation)
        {
            // arrange
            var devolvedCode = new Mock<IDevolvedPostcode>(MockBehavior.Strict);
            devolvedCode
                .SetupGet(x => x.EffectiveFrom)
                .Returns(DateTime.Parse(effective));

            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .Setup(x => x.LearnStartDate)
                .Returns(DateTime.Parse(start));

            var sut = NewRule();

            // act
            var result = sut.HasQualifyingEffectiveStart(devolvedCode.Object, delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            devolvedCode.VerifyAll();
            delivery.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string testSofCode = "112";

            var testStart = DateTime.Parse("2016-05-01");
            var testCode = "blah blah";

            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(x => x.LearnDelFAMType)
                .Returns("SOF"); // Monitoring.Delivery.Types.SourceOfFunding
            fam
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns(testSofCode);

            var fams = new ILearningDeliveryFAM[] { fam.Object };

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(35);
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testStart);
            delivery
                .SetupGet(y => y.LSDPostcode)
                .Returns(testCode);
            delivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.ULN)
                .Returns(ValidationConstants.TemporaryULN);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LSDPostcode_02, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 35))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testStart)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LSDPostcode", testCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "SOF"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 35, 10))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, DateTime.Parse("2019-08-01"), null))
                .Returns(true);

            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationData
                .Setup(x => x.GetLegalOrgTypeForUkprn(TestProviderID))
                .Returns(TestLegalOrgType);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var devolvedCode = new Mock<IDevolvedPostcode>();
            devolvedCode
                .SetupGet(x => x.SourceOfFunding)
                .Returns(testSofCode);
            devolvedCode
                .SetupGet(x => x.EffectiveFrom)
                .Returns(testStart.AddDays(1)); // to push it out of range
            var devolvedCodes = new IDevolvedPostcode[] { devolvedCode.Object };

            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            postcodeData
                .Setup(x => x.GetDevolvedPostcodes(testCode))
                .Returns(devolvedCodes);

            var sut = new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, fileData.Object, postcodeData.Object);

            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsPostcodeValidationExclusion))
                .Returns(false);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            organisationData.VerifyAll();
            fileData.VerifyAll();
            postcodeData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string testSofCode = "112";

            var testStart = DateTime.Parse("2016-05-01");
            var testCode = "blah blah";

            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(x => x.LearnDelFAMType)
                .Returns("SOF"); // Monitoring.Delivery.Types.SourceOfFunding
            fam
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns(testSofCode);

            var fams = new ILearningDeliveryFAM[] { fam.Object };

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(35);
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testStart);
            delivery
                .SetupGet(y => y.LSDPostcode)
                .Returns(testCode);
            delivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.ULN)
                .Returns(ValidationConstants.TemporaryULN);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 35, 10))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, DateTime.Parse("2019-08-01"), null))
                .Returns(true);

            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationData
                .Setup(x => x.GetLegalOrgTypeForUkprn(TestProviderID))
                .Returns(TestLegalOrgType);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var devolvedCode = new Mock<IDevolvedPostcode>();
            devolvedCode
                .SetupGet(x => x.SourceOfFunding)
                .Returns(testSofCode);
            devolvedCode
                .SetupGet(x => x.EffectiveFrom)
                .Returns(testStart);
            var devolvedCodes = new IDevolvedPostcode[] { devolvedCode.Object };

            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);
            postcodeData
                .Setup(x => x.GetDevolvedPostcodes(testCode))
                .Returns(devolvedCodes);

            var sut = new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, fileData.Object, postcodeData.Object);

            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsPostcodeValidationExclusion))
                .Returns(false);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            organisationData.VerifyAll();
            fileData.VerifyAll();
            postcodeData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LSDPostcode_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var organisationData = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationData
                .Setup(x => x.GetLegalOrgTypeForUkprn(TestProviderID))
                .Returns(TestLegalOrgType);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var postcodeData = new Mock<IPostcodesDataService>(MockBehavior.Strict);

            return new LSDPostcode_02Rule(handler.Object, commonOps.Object, organisationData.Object, fileData.Object, postcodeData.Object);
        }
    }
}
