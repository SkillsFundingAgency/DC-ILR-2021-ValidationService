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
    public class LearnDelFAMType_66Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly HashSet<string> _nvqLevels = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
             LARSNotionalNVQLevelV2.EntryLevel,
             LARSNotionalNVQLevelV2.Level1,
             LARSNotionalNVQLevelV2.Level2
        };

        private readonly ILARSDataService _larsData;
        private readonly IDerivedData_07Rule _derivedData07;
        private readonly IDerivedData_21Rule _derivedData21;
        private readonly IDerivedData_28Rule _derivedData28;
        private readonly IDerivedData_29Rule _derivedData29;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnDelFAMType_66Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_21Rule derivedData21,
            IDerivedData_28Rule derivedData28,
            IDerivedData_29Rule derivedData29,
            IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_66)
        {
            _larsData = larsData;
            _derivedData07 = derivedData07;
            _derivedData21 = derivedData21;
            _derivedData28 = derivedData28;
            _derivedData29 = derivedData29;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public static string FamTypeForError => Monitoring.Delivery.Types.FullOrCoFunding;

        public static string FamCodeForError => "1";

        public static DateTime LastInviableDate => new DateTime(2017, 07, 31);

        public static int MinimumViableAge => 24;

        public static int MaximumViableAge => 99;

        public bool WithinViableAgeGroup(DateTime candidate, DateTime reference) =>
            _dateTimeQueryService.YearsBetween(candidate, reference).IsBetween(MinimumViableAge, MaximumViableAge);

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.OLASSOffendersInCustody.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsReleasedOnTemporaryLicence(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.ReleasedOnTemporaryLicence.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            monitor.LearnDelFAMType.CaseInsensitiveEquals(Monitoring.Delivery.Types.Restart);

        public bool IsSteelWorkerRedundancyTraining(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.SteelIndustriesRedundancyTraining.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool InReceiptOfLowWages(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.InReceiptOfLowWages.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsBasicSkillsLearner(ILearningDelivery delivery)
        {
            var validities = _larsData.GetValiditiesFor(delivery.LearnAimRef);
            var annualValues = _larsData.GetAnnualValuesFor(delivery.LearnAimRef);

            return validities.Any(x => _larsData.IsCurrentAndNotWithdrawn(x, delivery.LearnStartDate))
                && annualValues.Any(IsBasicSkillsLearner);
        }

        public bool IsBasicSkillsLearner(ILARSAnnualValue monitor) =>
           monitor.BasicSkillsType.HasValue
           && TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills.Contains(monitor.BasicSkillsType.Value);

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

        public bool IsFullyFunded(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.FullyFundedLearningAim.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsEarlyStageNVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return _nvqLevels.Contains(larsDelivery.NotionalNVQLevelv2);
        }

        public bool IsExcluded(ILearningDelivery candidate)
        {
            return IsInflexibleElementOfTrainingAim(candidate)
                || IsApprenticeship(candidate)
                || IsBasicSkillsLearner(candidate)
                || CheckDeliveryFAMs(candidate, IsLearnerInCustody)
                || CheckDeliveryFAMs(candidate, IsReleasedOnTemporaryLicence)
                || CheckDeliveryFAMs(candidate, IsRestart)
                || CheckDeliveryFAMs(candidate, IsSteelWorkerRedundancyTraining)
                || CheckDeliveryFAMs(candidate, InReceiptOfLowWages);
        }

        public void Validate(ILearner objectToValidate)
        {
            ValidateDeliveries(objectToValidate);
        }

        public bool IsNotValid(ILearningDelivery delivery, ILearner learner) =>
            !IsExcluded(delivery)
            && !IsAdultFundedUnemployedWithBenefits(delivery, learner)
            && !IsAdultFundedUnemployedWithOtherStateBenefits(delivery, learner)
            && IsAdultFunding(delivery)
            && IsViableStart(delivery)
            && IsTargetAgeGroup(learner, delivery)
            && CheckDeliveryFAMs(delivery, IsFullyFunded)
            && IsEarlyStageNVQ(delivery);

        public void ValidateDeliveries(ILearner candidate)
        {
            candidate.LearningDeliveries
                .ForAny(x => IsNotValid(x, candidate), x => RaiseValidationMessage(x, candidate));
        }

        public void RaiseValidationMessage(ILearningDelivery thisDelivery, ILearner thisLearner)
        {
            HandleValidationError(thisLearner.LearnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery, thisLearner));
        }

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
