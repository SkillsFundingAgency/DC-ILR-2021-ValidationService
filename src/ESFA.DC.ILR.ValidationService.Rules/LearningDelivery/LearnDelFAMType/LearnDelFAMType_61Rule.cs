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
    public class LearnDelFAMType_61Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly ILARSDataService _larsData;
        private readonly IDerivedData_07Rule _derivedData07;
        private readonly IDerivedData_21Rule _derivedData21;
        private readonly IDerivedData_28Rule _derivedData28;
        private readonly IDerivedData_29Rule _derivedData29;

        public LearnDelFAMType_61Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_21Rule derivedData21,
            IDerivedData_28Rule derivedData28,
            IDerivedData_29Rule derivedData29)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_61)
        {
            _larsData = larsData;
            _derivedData07 = derivedData07;
            _derivedData21 = derivedData21;
            _derivedData28 = derivedData28;
            _derivedData29 = derivedData29;
        }

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

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.OLASSOffendersInCustody);

        public bool IsReleasedOnTemporaryLicence(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.ReleasedOnTemporaryLicence);

        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.Restart);

        public bool IsSteelWorkerRedundancyTrainingOrIsInReceiptOfLowWages(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.SteelIndustriesRedundancyTraining, Monitoring.Delivery.InReceiptOfLowWages);

        public bool IsBasicSkillsLearner(ILearningDelivery delivery)
        {
            var validities = _larsData.GetValiditiesFor(delivery.LearnAimRef);
            var annualValues = _larsData.GetAnnualValuesFor(delivery.LearnAimRef);

            return validities.Any(x => x.IsCurrent(delivery.LearnStartDate))
                && annualValues.Any(IsBasicSkillsLearner);
        }

        public bool IsBasicSkillsLearner(ILARSAnnualValue monitor) =>
            It.IsInRange(monitor.BasicSkillsType, TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills);

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

        public bool IsInflexibleElementOfTrainingAim(ILearner candidate) =>
            _derivedData29.IsInflexibleElementOfTrainingAim(candidate);

        public bool IsApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.SafeAny(matchCondition);

        public bool CheckLearningDeliveries(ILearner candidate, Func<ILearningDelivery, bool> matchCondition) =>
            candidate.LearningDeliveries.SafeAny(matchCondition);

        public bool IsAdultFunding(ILearningDelivery delivery) =>
            delivery.FundModel == TypeOfFunding.AdultSkills;

        public bool IsViableStart(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        public bool IsTargetAgeGroup(ILearner learner, ILearningDelivery delivery) =>
            It.Has(learner.DateOfBirthNullable)
            && WithinViableAgeGroup(learner.DateOfBirthNullable.Value, delivery.LearnStartDate);

        public bool IsFullyFundedLearningAim(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.FullyFundedLearningAim);

        public bool IsLevel2Nvq(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return IsV2NotionalLevel2(larsDelivery);
        }

        public bool IsNotEntitled(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return !larsDelivery.Categories.SafeAny(category => category.CategoryRef == TypeOfLARSCategory.LegalEntitlementLevel2);
        }

        public bool IsV2NotionalLevel2(ILARSLearningDelivery delivery) =>
            delivery.NotionalNVQLevelv2 == LARSNotionalNVQLevelV2.Level2;

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
                && IsLevel2Nvq(thisDelivery)
                && IsNotEntitled(thisDelivery))
            {
                thisDelivery.LearningDeliveryFAMs.ForAny(IsFullyFundedLearningAim, doAction);
            }
        }

        public bool IsExcluded(ILearner candidate)
        {
            /* This rule is not triggered by apprenticeships(DD07 = Y),               <=IsApprenticeship
             traineeship non work prep/ experience(DD29 = Y),                         <= IsInflexibleElementOfTrainingAim
             OLASS learners(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 034) or  <= IsLearnerInCustody
             RoTL(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 328).  <= IsReleasedOnTemporaryLicence
             It is also not triggered by learners on active benefits(DD28 = Y),       <= IsAdultFundedUnemployedWithBenefits
             Unemployed learners on other state benefits(DD21 = Y),                   <= IsAdultFundedUnemployedWithOtherStateBenefits
             Restarts(LearningDeliveryFAM.LearnDelFAMType = RES),                     <= IsRestart
             English and maths basic skills(LARS_BasicSkillsType = 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35), <= IsBasicSkillsLearner
            (where LearningDelivery.LearnAimRef = LARS_LearnAimRef and LearningDelivery.LearnStartDate => LARS_EffectiveFrom
            (and =< LARS_EffectiveTo or LARS_EffectiveTo = null)),
            Providers with
            Steel Industries Redundancy Training(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 347),   <=IsSteelWorkerRedundancyTrainingOrIsInReceiptOfLowWages
            Learners in receipt of low wages(LearningDeliveryFAM.LearnDelFAMType = LDM and LearningDeliveryFAM.LearnDelFAMCode = 363)
            */

            return IsInflexibleElementOfTrainingAim(candidate)
                || CheckLearningDeliveries(candidate, IsApprenticeship)
                || CheckLearningDeliveries(candidate, IsBasicSkillsLearner)
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsLearnerInCustody))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsReleasedOnTemporaryLicence))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsRestart))
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsSteelWorkerRedundancyTrainingOrIsInReceiptOfLowWages));
        }

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

        public void ValidateDeliveries(ILearner learner)
        {
            /*
              LearningDelivery.LearnStartDate > 2017 - 07 - 31                                                  <= for a delivery after the given date
              and LearningDelivery.FundModel = 35                                                               <= that is adult skills
              and  ((LearningDelivery.LearnStartDate - Learner.DateOfBirth) >= 19 years and <= 23 years)        <= in target age group
                        and ((LearningDelivery.LearnAimRef = LARS_LearnAimRef and LARS_NotionalNVQLevelv2 = 2)  <= is NVQ V2 level 2
                            and (LearningDeliveryFAM.LearnDelFAMType = FFI and LearningDeliveryFAM.LearnDelFAMCode = 1)  <= IsFullyFundedLearningAim
                            and LARS_CategoryRef<> 37)

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
