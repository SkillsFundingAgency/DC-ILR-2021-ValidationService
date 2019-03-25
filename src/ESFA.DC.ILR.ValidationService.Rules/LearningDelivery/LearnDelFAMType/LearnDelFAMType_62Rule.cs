using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
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
    public class LearnDelFAMType_62Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// The derived data 07 (rule)
        /// </summary>
        private readonly IDerivedData_07Rule _derivedData07;

        /// <summary>
        /// The derived data 21 (rule)
        /// </summary>
        private readonly IDerivedData_21Rule _derivedData21;

        /// <summary>
        /// The derived data 28 (rule)
        /// </summary>
        private readonly IDerivedData_28Rule _derivedData28;

        /// <summary>
        /// The derived data 29 (rule)
        /// </summary>
        private readonly IDerivedData_29Rule _derivedData29;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDelFAMType_62Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="derivedData07">The derived data 07 rule.</param>
        /// <param name="derivedData21">The derived data 21 rule.</param>
        /// <param name="derivedData28">The derived data 28 rule.</param>
        /// <param name="derivedData29">The derived data 29 rule.</param>
        public LearnDelFAMType_62Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_21Rule derivedData21,
            IDerivedData_28Rule derivedData28,
            IDerivedData_29Rule derivedData29)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_62)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));
            It.IsNull(derivedData07)
                .AsGuard<ArgumentNullException>(nameof(derivedData07));
            It.IsNull(derivedData21)
                .AsGuard<ArgumentNullException>(nameof(derivedData21));
            It.IsNull(derivedData28)
                .AsGuard<ArgumentNullException>(nameof(derivedData28));
            It.IsNull(derivedData29)
                .AsGuard<ArgumentNullException>(nameof(derivedData29));

            _larsData = larsData;
            _derivedData07 = derivedData07;
            _derivedData21 = derivedData21;
            _derivedData28 = derivedData28;
            _derivedData29 = derivedData29;
        }

        /// <summary>
        /// Gets the FAM Type for Error parameter.
        /// </summary>
        public static string FamTypeForError => Monitoring.Delivery.Types.FullOrCoFunding;

        /// <summary>
        /// Gets the FAM Code for Error parameter.
        /// </summary>
        public static string FamCodeForError => "2";

        /// <summary>
        /// Gets the last inviable date.
        /// </summary>
        public static DateTime LastInviableDate => new DateTime(2017, 07, 31);

        /// <summary>
        /// Gets the minimum viable age.
        /// </summary>
        public static int MinimumViableAge => 19; // years

        /// <summary>
        /// Gets the maximum viable age.
        /// </summary>
        public static int MaximumViableAge => 23; // years

        public static bool WithinViableAgeGroup(DateTime candidate, DateTime reference) =>
            It.IsBetween(candidate, reference.AddYears(-MaximumViableAge), reference.AddYears(-MinimumViableAge));

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
        /// Determines whether the specified monitor is restart.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if the specified monitor is restart; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.Restart);

        /// <summary>
        /// Determines whether [is steel worker] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is steel worker] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSteelWorkerRedundancyTraining(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.SteelIndustriesRedundancyTraining);

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
                && annualValues.Any(x => IsBasicSkillsLearner(x) || IsESOLBasicSkillsLearner(x));
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
        /// Determines whether [is esol basic skills learner] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is esol basic skills learner] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsESOLBasicSkillsLearner(ILARSAnnualValue monitor) =>
            It.IsInRange(monitor.BasicSkillsType, TypeOfLARSBasicSkill.AsESOLBasicSkills);

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
        /// Determines whether [is inflexible element of training aim] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is inflexible element of training aim] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInflexibleElementOfTrainingAim(ILearner candidate) =>
            _derivedData29.IsInflexibleElementOfTrainingAim(candidate);

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
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if any of the delivery fams match the condition</returns>
        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.SafeAny(matchCondition);

        /// <summary>
        /// Checks the learning deliveries.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="matchCondition">The matching condition.</param>
        /// <returns>true if any of the deliveries match the condition</returns>
        public bool CheckLearningDeliveries(ILearner candidate, Func<ILearningDelivery, bool> matchCondition) =>
            candidate.LearningDeliveries.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether the specified delivery is funded.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is funded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultFunding(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.AdultSkills);

        /// <summary>
        /// Determines whether [is viable start] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is viable start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsViableStart(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

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
        /// Determines whether [is co funded] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is fully funded] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCoFunded(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.CoFundedLearningAim);

        /// <summary>
        /// Determines whether [is entitled level 2 NVQ] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is entitled level 2 NVQ] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEntitledLevel2NVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return IsV2NotionalLevel2(larsDelivery)
                && larsDelivery.Categories.SafeAny(IsLegallyEntitled);
        }

        /// <summary>
        /// Determines whether [is v2 notional level2] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is v2 notional level2] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsV2NotionalLevel2(ILARSLearningDelivery delivery) =>
            It.IsInRange(delivery.NotionalNVQLevelv2, LARSNotionalNVQLevelV2.Level2);

        /// <summary>
        /// Determines whether [is legally entitled] [the specified category].
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>
        ///   <c>true</c> if [is legally entitled] [the specified category]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLegallyEntitled(ILARSLearningCategory category) =>
            It.IsInRange(category.CategoryRef, TypeOfLARSCategory.LegalEntitlementLevel2);

        /// <summary>
        /// Determines whether [is higher achiever] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is higher achiever] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsHigherAchiever(ILearner candidate) =>
            It.Has(candidate.PriorAttainNullable)
                && It.IsInRange(candidate.PriorAttainNullable.Value, TypeOfPriorAttainment.AsHigherLevelAchievements);

        /// <summary>
        /// Determines whether [is not valid] [this delivery for learner].
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <param name="forLearner">For learner.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [this delivery for learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisDelivery, ILearner forLearner) =>
            !IsAdultFundedUnemployedWithBenefits(thisDelivery, forLearner)
                && !IsAdultFundedUnemployedWithOtherStateBenefits(thisDelivery, forLearner)
                && IsViableStart(thisDelivery)
                && IsAdultFunding(thisDelivery)
                && IsTargetAgeGroup(forLearner, thisDelivery)
                && CheckDeliveryFAMs(thisDelivery, IsCoFunded)
                && !IsEntitledLevel2NVQ(thisDelivery);

        /// <summary>
        /// Determines whether the specified candidate is excluded.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearner candidate) =>
            IsInflexibleElementOfTrainingAim(candidate)
                || IsHigherAchiever(candidate)
                || CheckLearningDeliveries(candidate, IsApprenticeship)
                || CheckLearningDeliveries(candidate, IsBasicSkillsLearner)
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsLearnerInCustody))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsReleasedOnTemporaryLicence))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsRestart))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsSteelWorkerRedundancyTraining));

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            if (IsExcluded(objectToValidate))
            {
                return;
            }

            ValidateDeliveries(objectToValidate);
        }

        /// <summary>
        /// Validates the deliveries.
        /// a breakout routine to simplify testing
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        public void ValidateDeliveries(ILearner candidate)
        {
            /*
            LearningDelivery.LearnStartDate > 2017-07-31                                                        <= for a delivery after the given date
            and LearningDelivery.FundModel = 35                                                                 <= that is adult skills
            and ((LearningDelivery.LearnStartDate - Learner.DateOfBirth) >= 19 years and <=23 years)            <= in target age group
                and ((LearningDeliveryFAM.LearnDelFAMType = FFI and LearningDeliveryFAM.LearnDelFAMCode = 2)    <= is co funded
                    and (LearningDelivery.LearnAimRef = LARS_LearnAimRef and LARS_NotionalNVQLevelv2 = 2)       <= is NVQ V2 level 2
                    and LARS_CategoryRef  <> 37)                                                                <= is not legal entitlement level 2
                and Learner.PriorAttain <> 2, 3, 4, 5, 10, 11, 12, 13)                                          <= and is not a higher level of attainment
            */

            candidate.LearningDeliveries
                .ForAny(x => IsNotValid(x, candidate), x => RaiseValidationMessage(x, candidate));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="thisLearner">this learner.</param>
        public void RaiseValidationMessage(ILearningDelivery thisDelivery, ILearner thisLearner)
        {
            HandleValidationError(thisLearner.LearnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery, thisLearner));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="thisLearner">this learner.</param>
        /// <returns>a collection of message parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery, ILearner thisLearner)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, FamTypeForError),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, FamCodeForError),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, thisLearner.DateOfBirthNullable)
            };
        }
    }
}
