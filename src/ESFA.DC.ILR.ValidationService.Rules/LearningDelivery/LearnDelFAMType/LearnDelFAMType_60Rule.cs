using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_60Rule : AbstractRule, IRule<ILearner>
    {
        private const string _legalOrgType = "USDC";
        private readonly ILARSDataService _larsData;
        private readonly IDerivedData_07Rule _derivedData07;
        private readonly IDerivedData_21Rule _derivedData21;
        private readonly IDerivedData_28Rule _derivedData28;
        private readonly IDerivedData_29Rule _derivedData29;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IFileDataService _fileDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDelFAMType_60Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="derivedData07">The derived data07.</param>
        /// <param name="derivedData21">The derived data21.</param>
        /// <param name="derivedData28">The derived data28.</param>
        /// <param name="derivedData29">The derived data29.</param>
        /// <param name="organisationDataService">The organisation data service.</param>
        /// <param name="fileDataService">The file data service.</param>
        public LearnDelFAMType_60Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_21Rule derivedData21,
            IDerivedData_28Rule derivedData28,
            IDerivedData_29Rule derivedData29,
            IOrganisationDataService organisationDataService,
            IFileDataService fileDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_60)
        {
            _larsData = larsData;
            _derivedData07 = derivedData07;
            _derivedData21 = derivedData21;
            _derivedData28 = derivedData28;
            _derivedData29 = derivedData29;
            _organisationDataService = organisationDataService;
            _fileDataService = fileDataService;
        }

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public static DateTime FirstViableDate => new DateTime(2016, 08, 01);

        /// <summary>
        /// Gets the last viable date.
        /// </summary>
        public static DateTime LastViableDate => new DateTime(2017, 07, 31);

        /// <summary>
        /// Gets the minimum viable age.
        /// </summary>
        public static int MinimumViableAge => 24; // years

        /// <summary>
        /// Gets the maximum viable age.
        /// </summary>
        public static int MaximumViableAge => 99; // years (old..)

        /// <summary>
        /// Determines whether [is within viable age group] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="reference">The reference.</param>
        /// <returns>
        ///   <c>true</c> if [is within viable age group] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool WithinViableAgeGroup(DateTime candidate, DateTime reference) =>
            It.IsBetween(candidate, reference.AddYears(-MaximumViableAge), reference.AddYears(-MinimumViableAge));

        /// <summary>
        /// Checks the learning deliveries.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>returns true in any conditions match</returns>
        public bool CheckLearningDeliveries(ILearner candidate, Func<ILearningDelivery, bool> matchCondition) =>
            candidate.LearningDeliveries.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether the specified delivery is apprenticeship.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is apprenticeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        /// <summary>
        /// Determines whether [is inflexible element of training aim] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is inflexible element of training aim] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInflexibleElementOfTrainingAim(ILearner candidate) =>
            _derivedData29.IsInflexibleElementOfTrainingAim(candidate);

        /// <summary>
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>returns true in any conditions match</returns>
        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether [is learner in custody] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is learner in custody] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.OLASSOffendersInCustody);

        /// <summary>
        /// Determines whether [is released on temporary licence] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is released on temporary licence] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsReleasedOnTemporaryLicence(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.ReleasedOnTemporaryLicence);

        /// <summary>
        /// Determines whether [is adult funded unemployed with benefits] [this delivery for candidate].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="forCandidate">For candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is adult funded unemployed with benefits] [this delivery for candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultFundedUnemployedWithBenefits(ILearningDelivery thisDelivery, ILearner forCandidate) =>
            _derivedData28.IsAdultFundedUnemployedWithBenefits(thisDelivery, forCandidate);

        /// <summary>
        /// Determines whether [is adult funded unemployed with other state benefits] [this delivery for candidate].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="forCandidate">For candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is adult funded unemployed with other state benefits] [this delivery for candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultFundedUnemployedWithOtherStateBenefits(ILearningDelivery thisDelivery, ILearner forCandidate) =>
            _derivedData21.IsAdultFundedUnemployedWithOtherStateBenefits(thisDelivery, forCandidate);

        /// <summary>
        /// Determines whether the specified monitor is restart.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if the specified monitor is restart; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.Restart);

        /// <summary>
        /// Determines whether [is basic skills learner] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is basic skills learner] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBasicSkillsLearner(ILearningDelivery delivery)
        {
            var validities = _larsData.GetValiditiesFor(delivery.LearnAimRef);
            var annualValues = _larsData.GetAnnualValuesFor(delivery.LearnAimRef);

            return validities.Any(x => x.IsCurrent(delivery.LearnStartDate))
                && annualValues.Any(IsBasicSkillsLearner);
        }

        /// <summary>
        /// Determines whether [is basic skills learner] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is basic skills learner] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBasicSkillsLearner(ILARSAnnualValue monitor) =>
            It.IsInRange(monitor.BasicSkillsType, TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills);

        /// <summary>
        /// Determines whether [is steel worker redundancy training] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is steel worker redundancy training] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSteelWorkerRedundancyTraining(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.SteelIndustriesRedundancyTraining);

        public bool IsLegalOrgTypeMatchForUkprn()
        {
            var ukprn = _fileDataService.UKPRN();
            return _organisationDataService.LegalOrgTypeMatchForUkprn(ukprn, _legalOrgType);
        }

        /// <summary>
        /// Determines whether [is viable start] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is viable start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsViableStart(ILearningDelivery delivery) =>
            It.IsBetween(delivery.LearnStartDate, FirstViableDate, LastViableDate);

        /// <summary>
        /// Determines whether [is adult funding] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is adult funding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultFunding(ILearningDelivery delivery) =>
            delivery.FundModel == TypeOfFunding.AdultSkills;

        /// <summary>
        /// Determines whether [is target age group] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is target age group] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTargetAgeGroup(ILearner learner, ILearningDelivery delivery) =>
            It.Has(learner.DateOfBirthNullable)
                && WithinViableAgeGroup(learner.DateOfBirthNullable.Value, delivery.LearnStartDate);

        /// <summary>
        /// Determines whether [is fully funded learning aim] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is fully funded learning aim] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFullyFundedLearningAim(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.FullyFundedLearningAim);

        /// <summary>
        /// Determines whether [is early stage NVQ] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is early stage NVQ] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEarlyStageNVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return It.IsInRange(
                larsDelivery.NotionalNVQLevelv2,
                LARSNotionalNVQLevelV2.EntryLevel,
                LARSNotionalNVQLevelV2.Level1,
                LARSNotionalNVQLevelV2.Level2);
        }

        /// <summary>
        /// Runs the checks for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <param name="doAction">do action.</param>
        public void RunChecksFor(ILearningDelivery thisDelivery, ILearner learner, Action<ILearningDeliveryFAM> doAction)
        {
            if (!IsAdultFundedUnemployedWithBenefits(thisDelivery, learner)
                && !IsAdultFundedUnemployedWithOtherStateBenefits(thisDelivery, learner)
                && IsViableStart(thisDelivery)
                && IsAdultFunding(thisDelivery)
                && IsTargetAgeGroup(learner, thisDelivery)
                && IsEarlyStageNVQ(thisDelivery))
            {
                thisDelivery.LearningDeliveryFAMs.ForAny(IsFullyFundedLearningAim, doAction);
            }
        }

        /// <summary>
        /// Determines whether the specified candidate is excluded.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearner candidate)
        {
            /*
             This rule is not triggered by apprenticeships(DD07 = Y),                         <=IsApprenticeship
             traineeship non work prep/ experience(DD29 = Y),                                    <= IsInflexibleElementOfTrainingAim
             OLASS learners(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 034) or <= IsLearnerInCustody
             RoTL(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 328). <= IsReleasedOnTemporaryLicence
             It is also not triggered by learners on active benefits(DD28 = Y),  <= IsAdultFundedUnemployedWithBenefits
             Unemployed learners on other state benefits(DD21 = Y),               <= IsAdultFundedUnemployedWithOtherStateBenefits
             Restarts(LearningDeliveryFAM.LearnDelFAMType = RES),                   <= IsRestart
             English and maths basic skills(LARS_BasicSkillsType = 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35  <= IsBasicSkillsLearner
             (where LearningDelivery.LearnAimRef = LARS_LearnAimRef and LearningDelivery.LearnStartDate => LARS_EffectiveFrom
             (and =< LARS_EffectiveTo or LARS_EffectiveTo = null)),
             Providers with an organisation type of Specialist Designated College(LearningProvider.UKPRN = Org_Details.UKPRN and LegalOrgType = USDC),  <= IsLegalOrgTypeMatchForUkprn
             or
                 Steel Industries Redundancy Training(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 347)  <= IsSteelWorkerRedundancyTraining
            */

            return CheckLearningDeliveries(candidate, IsApprenticeship)
                || IsInflexibleElementOfTrainingAim(candidate)
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsLearnerInCustody))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsReleasedOnTemporaryLicence))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsRestart))
                || CheckLearningDeliveries(candidate, IsBasicSkillsLearner)
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsSteelWorkerRedundancyTraining) || IsLegalOrgTypeMatchForUkprn());
        }

        /// <summary>
        /// Validates the specified object to validate.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            if (IsExcluded(objectToValidate))
            {
                return;
            }

            ValidateDeliveries(objectToValidate);
        }

        /// <summary>
        /// Validates the deliveries.
        /// </summary>
        /// <param name="learner">The learner.</param>
        public void ValidateDeliveries(ILearner learner)
        {
            var learnRefNumber = learner.LearnRefNumber;

            /* For starts after 31 July 2016 and before 1 August 2017
             ( LearningDelivery.LearnStartDate > 2016 - 07 - 31 and LearningDelivery.LearnStartDate < 2017 - 08 - 01),  <= for a delivery after the given date
             and adult skills funded(LearningDelivery.FundModel = 35),                                                    <= that is adult skills
             for aims that are level 2 or below(LARS_NotionalNVQLevelv2 = E, 1, or 2 where LearningDelivery.LearnAimRef = LARS_LearnAimRef), <= is NVQ V2 level 2 or below
                by learners aged 24 or over at the start of the aim and((LearningDelivery.LearnStartDate - Learner.DateOfBirth) >= 24 years),  <= in target age group
                full funding cannot be claimed

                (error where LearningDeliveryFAM.LearnDelFAMType = FFI and LearningDeliveryFAM.LearnDelFAMCode = 1) <= IsFullyFundedLearningAim
                */

            learner.LearningDeliveries
                .ForEach(x => RunChecksFor(x, learner, y => RaiseValidationMessage(x, learner, y)));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="thisLearner">this learner.</param>
        /// <param name="andMonitor">and monitor.</param>
        public void RaiseValidationMessage(ILearningDelivery thisDelivery, ILearner thisLearner, ILearningDeliveryFAM andMonitor)
        {
            HandleValidationError(thisLearner.LearnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery, thisLearner, andMonitor));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <param name="thisLearner">The this learner.</param>
        /// <param name="andMonitor">The and monitor.</param>
        /// <returns>a collection of message parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery, ILearner thisLearner, ILearningDeliveryFAM andMonitor)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, andMonitor.LearnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, andMonitor.LearnDelFAMCode),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, thisLearner.DateOfBirthNullable)
            };
        }
    }
}
