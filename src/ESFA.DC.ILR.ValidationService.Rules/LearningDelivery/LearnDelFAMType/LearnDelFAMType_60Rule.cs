using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_60Rule : AbstractRule, IRule<ILearner>
    {
        private const string _legalOrgType = "USDC";
        private readonly HashSet<string> _nvqLevels = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
             LARSConstants.NotionalNVQLevelV2Strings.EntryLevel,
             LARSConstants.NotionalNVQLevelV2Strings.Level1,
             LARSConstants.NotionalNVQLevelV2Strings.Level2
        };

        private readonly ILARSDataService _larsData;
        private readonly IDerivedData_07Rule _derivedData07;
        private readonly IDerivedData_21Rule _derivedData21;
        private readonly IDerivedData_28Rule _derivedData28;
        private readonly IDerivedData_29Rule _derivedData29;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IFileDataService _fileDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnDelFAMType_60Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_21Rule derivedData21,
            IDerivedData_28Rule derivedData28,
            IDerivedData_29Rule derivedData29,
            IOrganisationDataService organisationDataService,
            IFileDataService fileDataService,
            IDateTimeQueryService datetimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_60)
        {
            _larsData = larsData;
            _derivedData07 = derivedData07;
            _derivedData21 = derivedData21;
            _derivedData28 = derivedData28;
            _derivedData29 = derivedData29;
            _organisationDataService = organisationDataService;
            _fileDataService = fileDataService;
            _dateTimeQueryService = datetimeQueryService;
        }

        public static DateTime FirstViableDate => new DateTime(2016, 08, 01);

        public static DateTime LastViableDate => new DateTime(2017, 07, 31);

        public static int MinimumViableAge => 24;

        public static int MaximumViableAge => 99;

        public bool WithinViableAgeGroup(DateTime candidate, DateTime reference) =>
            _dateTimeQueryService.YearsBetween(candidate, reference).IsBetween(MinimumViableAge, MaximumViableAge);

        public bool CheckLearningDeliveries(ILearner candidate, Func<ILearningDelivery, bool> matchCondition) =>
            candidate.LearningDeliveries.NullSafeAny(matchCondition);

        public bool IsApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool IsInflexibleElementOfTrainingAim(ILearningDelivery candidate) =>
            _derivedData29.IsInflexibleElementOfTrainingAimLearningDelivery(candidate);

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.NullSafeAny(matchCondition);

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.OLASSOffendersInCustody.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsReleasedOnTemporaryLicence(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.ReleasedOnTemporaryLicence.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsAdultFundedUnemployedWithBenefits(ILearningDelivery thisDelivery, ILearner forCandidate) =>
            _derivedData28.IsAdultFundedUnemployedWithBenefits(thisDelivery, forCandidate);

        public bool IsAdultFundedUnemployedWithOtherStateBenefits(ILearningDelivery thisDelivery, ILearner forCandidate) =>
            _derivedData21.IsAdultFundedUnemployedWithOtherStateBenefits(thisDelivery, forCandidate);

        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            monitor.LearnDelFAMType.CaseInsensitiveEquals(Monitoring.Delivery.Types.Restart);

        public bool IsBasicSkillsLearner(ILearningDelivery delivery)
        {
            var validities = _larsData.GetValiditiesFor(delivery.LearnAimRef);
            var annualValues = _larsData.GetAnnualValuesFor(delivery.LearnAimRef);

            return validities.Any(x => _larsData.IsCurrentAndNotWithdrawn(x, delivery.LearnStartDate))
                && annualValues.Any(IsBasicSkillsLearner);
        }

        public bool IsBasicSkillsLearner(ILARSAnnualValue monitor) =>
            monitor.BasicSkillsType.HasValue
            && LARSConstants.BasicSkills.EnglishAndMathsList.Contains(monitor.BasicSkillsType.Value);

        public bool IsSteelWorkerRedundancyTraining(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.SteelIndustriesRedundancyTraining.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsLegalOrgTypeMatchForUkprn()
        {
            var ukprn = _fileDataService.UKPRN();
            return _organisationDataService.LegalOrgTypeMatchForUkprn(ukprn, _legalOrgType);
        }

        public bool IsViableStart(ILearningDelivery delivery) =>
        _dateTimeQueryService.IsDateBetween(delivery.LearnStartDate, FirstViableDate, LastViableDate);

        public bool IsAdultFunding(ILearningDelivery delivery) =>
            delivery.FundModel == FundModels.AdultSkills;

        public bool IsTargetAgeGroup(ILearner learner, ILearningDelivery delivery) =>
            learner.DateOfBirthNullable.HasValue
                && WithinViableAgeGroup(learner.DateOfBirthNullable.Value, delivery.LearnStartDate);

        public bool IsFullyFundedLearningAim(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.FullyFundedLearningAim.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsEarlyStageNVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            if (larsDelivery == null)
            {
                return false;
            }

            return _nvqLevels.Contains(larsDelivery.NotionalNVQLevelv2);
        }

        public void RunChecksFor(ILearningDelivery thisDelivery, ILearner learner, Action<ILearningDeliveryFAM> doAction)
        {
            if (!IsExcluded(thisDelivery)
                && !IsAdultFundedUnemployedWithBenefits(thisDelivery, learner)
                && !IsAdultFundedUnemployedWithOtherStateBenefits(thisDelivery, learner)
                && IsViableStart(thisDelivery)
                && IsAdultFunding(thisDelivery)
                && IsTargetAgeGroup(learner, thisDelivery)
                && IsEarlyStageNVQ(thisDelivery))
            {
                thisDelivery.LearningDeliveryFAMs.ForAny(IsFullyFundedLearningAim, doAction);
            }
        }

        public bool IsExcluded(ILearningDelivery candidate)
        {
            return IsApprenticeship(candidate)
                || IsInflexibleElementOfTrainingAim(candidate)
                || CheckDeliveryFAMs(candidate, IsLearnerInCustody)
                || CheckDeliveryFAMs(candidate, IsReleasedOnTemporaryLicence)
                || CheckDeliveryFAMs(candidate, IsRestart)
                || IsBasicSkillsLearner(candidate)
                || CheckDeliveryFAMs(candidate, IsSteelWorkerRedundancyTraining) || IsLegalOrgTypeMatchForUkprn();
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
            var learnRefNumber = learner.LearnRefNumber;

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
