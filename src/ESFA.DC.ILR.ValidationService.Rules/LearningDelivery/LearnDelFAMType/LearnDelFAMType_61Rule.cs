using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_61Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly HashSet<string> _ldmLookups = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Monitoring.Delivery.SteelIndustriesRedundancyTraining,
            Monitoring.Delivery.InReceiptOfLowWages
        };

        private readonly ILARSDataService _larsData;
        private readonly IDerivedData_07Rule _derivedData07;
        private readonly IDerivedData_21Rule _derivedData21;
        private readonly IDerivedData_28Rule _derivedData28;
        private readonly IDerivedData_29Rule _derivedData29;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnDelFAMType_61Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_21Rule derivedData21,
            IDerivedData_28Rule derivedData28,
            IDerivedData_29Rule derivedData29,
            IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_61)
        {
            _larsData = larsData;
            _derivedData07 = derivedData07;
            _derivedData21 = derivedData21;
            _derivedData28 = derivedData28;
            _derivedData29 = derivedData29;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public static DateTime LastInviableDate => new DateTime(2017, 07, 31);

        public static int MinimumViableAge => 19;

        public static int MaximumViableAge => 23;

        public bool WithinViableAgeGroup(DateTime candidate, DateTime reference) =>
            _dateTimeQueryService.YearsBetween(candidate, reference).IsBetween(MinimumViableAge, MaximumViableAge);

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.OLASSOffendersInCustody.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsReleasedOnTemporaryLicence(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.ReleasedOnTemporaryLicence.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.Types.Restart.CaseInsensitiveEquals(monitor.LearnDelFAMType);

        public bool IsSteelWorkerRedundancyTrainingOrIsInReceiptOfLowWages(ILearningDeliveryFAM monitor) =>
            _ldmLookups.Contains($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsBasicSkillsLearner(ILearningDelivery delivery, ILARSLearningDelivery larsLearningDelivery)
        {
            var annualValues = _larsData.GetAnnualValuesFor(delivery.LearnAimRef);

            return _dateTimeQueryService.IsDateBetween(delivery.LearnStartDate, larsLearningDelivery.EffectiveFrom, larsLearningDelivery.EffectiveTo ?? DateTime.MaxValue)
                && annualValues.Any(IsBasicSkillsLearner);
        }

        public bool IsBasicSkillsLearner(ILARSAnnualValue monitor) =>
            monitor.BasicSkillsType.HasValue
            && LARSConstants.BasicSkills.EnglishAndMathsList.Contains(monitor.BasicSkillsType.Value);

        public bool IsAdultFundedUnemployedWithOtherStateBenefits(ILearningDelivery thisDelivery, ILearner forCandidate) =>
            _derivedData21.IsAdultFundedUnemployedWithOtherStateBenefits(thisDelivery, forCandidate);

        public bool IsAdultFundedUnemployedWithBenefits(ILearningDelivery thisDelivery, ILearner forCandidate) =>
            _derivedData28.IsAdultFundedUnemployedWithBenefits(thisDelivery, forCandidate);

        public bool IsInflexibleElementOfTrainingAim(ILearningDelivery candidate) =>
            _derivedData29.IsInflexibleElementOfTrainingAimLearningDelivery(candidate);

        public bool IsApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.NullSafeAny(matchCondition);

        public bool CheckLearningDeliveries(ILearner candidate, Func<ILearningDelivery, bool> matchCondition) =>
            candidate.LearningDeliveries.NullSafeAny(matchCondition);

        public bool IsAdultFunding(ILearningDelivery delivery) =>
            delivery.FundModel == FundModels.AdultSkills;

        public bool IsViableStart(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        public bool IsTargetAgeGroup(ILearner learner, ILearningDelivery delivery) =>
            learner.DateOfBirthNullable.HasValue
            && WithinViableAgeGroup(learner.DateOfBirthNullable.Value, delivery.LearnStartDate);

        public bool IsFullyFundedLearningAim(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.FullyFundedLearningAim.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        //public bool IsLevel2Nvq(ILearningDelivery delivery)
        //{
        //    var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

        //    return IsV2NotionalLevel2(larsDelivery);
        //}

        public bool IsNotEntitled(ILARSLearningDelivery larsLearningDelivery)
        {
            return !larsLearningDelivery.Categories.NullSafeAny(category => category.CategoryRef == LARSConstants.Categories.LegalEntitlementLevel2);
        }

        public bool IsV2NotionalLevel2(ILARSLearningDelivery delivery) =>
            delivery?.NotionalNVQLevelv2 == LARSConstants.NotionalNVQLevelV2Strings.Level2;

        public void RunChecksFor(ILearningDelivery thisDelivery, ILearner learner, Action<ILearningDeliveryFAM> doAction)
        {
            var larsLearningDelivery = _larsData.GetDeliveryFor(thisDelivery.LearnAimRef);

            if (!IsExcluded(thisDelivery, larsLearningDelivery)
                && !IsAdultFundedUnemployedWithBenefits(thisDelivery, learner)
                && !IsAdultFundedUnemployedWithOtherStateBenefits(thisDelivery, learner)
                && IsViableStart(thisDelivery)
                && IsAdultFunding(thisDelivery)
                && IsTargetAgeGroup(learner, thisDelivery)
                && IsV2NotionalLevel2(larsLearningDelivery)
                && IsNotEntitled(larsLearningDelivery))
            {
                thisDelivery.LearningDeliveryFAMs.ForAny(IsFullyFundedLearningAim, doAction);
            }
        }

        public bool IsExcluded(ILearningDelivery candidate, ILARSLearningDelivery larsLearningDelivery)
        {
            return IsInflexibleElementOfTrainingAim(candidate)
                || IsApprenticeship(candidate)
                || IsBasicSkillsLearner(candidate, larsLearningDelivery)
                || CheckDeliveryFAMs(candidate, IsLearnerInCustody)
                || CheckDeliveryFAMs(candidate, IsReleasedOnTemporaryLicence)
                || CheckDeliveryFAMs(candidate, IsRestart)
                || CheckDeliveryFAMs(candidate, IsSteelWorkerRedundancyTrainingOrIsInReceiptOfLowWages);
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            ValidateDeliveries(objectToValidate);
        }

        public void ValidateDeliveries(ILearner learner)
        {
            learner.LearningDeliveries
                .ForEach(x => RunChecksFor(x, learner, y => RaiseValidationMessage(x, learner, y)));
        }

        public void RaiseValidationMessage(ILearningDelivery thisDelivery, ILearner thisLearner, ILearningDeliveryFAM andMonitor)
        {
            HandleValidationError(thisLearner.LearnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery, thisLearner, andMonitor));
        }

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
