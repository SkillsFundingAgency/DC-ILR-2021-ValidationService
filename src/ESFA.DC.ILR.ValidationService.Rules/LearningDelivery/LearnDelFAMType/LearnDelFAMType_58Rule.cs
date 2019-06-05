using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_58Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augustFirst2016 = new DateTime(2016, 08, 01);
        private readonly DateTime _larsAimEffectiveFrom = new DateTime(2015, 08, 01);
        private readonly HashSet<int> _priorAttains = new HashSet<int>() { 2, 3, 4, 5, 10, 11, 12, 13, 97, 98 };
        private readonly decimal _percentValue = 100M;

        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDerivedData_07Rule _derivedData_07Rule;
        private readonly IDerivedData_12Rule _derivedData_12Rule;
        private readonly IDerivedData_21Rule _derivedData_21Rule;
        private readonly IFileDataService _fileDataService;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly ILARSDataService _lARSDataService;

        public LearnDelFAMType_58Rule(
            IValidationErrorHandler validationErrorHandler,
            IDateTimeQueryService datetimeQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IDerivedData_07Rule derivedData_07Rule,
            IDerivedData_12Rule derivedData_12Rule,
            IDerivedData_21Rule derivedData_21Rule,
            IFileDataService fileDataService,
            IOrganisationDataService organisationDataService,
            ILARSDataService lARSDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_58)
        {
            _dateTimeQueryService = datetimeQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _derivedData_07Rule = derivedData_07Rule;
            _derivedData_12Rule = derivedData_12Rule;
            _derivedData_21Rule = derivedData_21Rule;
            _fileDataService = fileDataService;
            _organisationDataService = organisationDataService;
            _lARSDataService = lARSDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null
                || objectToValidate.DateOfBirthNullable == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    objectToValidate,
                    learningDelivery))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameter(
                            objectToValidate.DateOfBirthNullable,
                            objectToValidate.PriorAttainNullable,
                            learningDelivery,
                            "FFI",
                            "1"));
                }
            }
        }

        public bool ConditionMet(
            ILearner learner,
            ILearningDelivery learningDelivery)
        {
            return LearnStartDateConditionMet(learningDelivery.LearnStartDate)
                && FundModelConditionMet(learningDelivery.FundModel)
                && AgeLimitConditionMet(learningDelivery.LearnStartDate, learner.DateOfBirthNullable)
                && LearnDelFAMsConditionMet(learningDelivery.LearningDeliveryFAMs)
                && LARSConditionMet(learningDelivery.LearnAimRef)
                && LARSConditionExcluded(learningDelivery.LearnAimRef)
                && (learner.PriorAttainNullable.HasValue
                    || LARSPercentageLevelConditionMet(learningDelivery.LearnAimRef, learner.PriorAttainNullable.Value))
                && DerivedData07ConditionExcluded(learningDelivery.ProgTypeNullable)
                && LearnDelFAMsConditionExcluded(learningDelivery.LearningDeliveryFAMs)
                && DerivedData12ConditionExcluded(learner.LearnerEmploymentStatuses, learningDelivery)
                && DerivedData21ConditionExcluded(learner, learningDelivery)
                && OrgDetailsConditionExcluded();
        }

        public bool OrgDetailsConditionExcluded() =>
            !_organisationDataService.LegalOrgTypeMatchForUkprn(
                _fileDataService.UKPRN(),
                LegalOrgTypeConstants.USDC);

        public bool DerivedData21ConditionExcluded(ILearner learner, ILearningDelivery learningDelivery) =>
            !_derivedData_21Rule.IsAdultFundedUnemployedWithOtherStateBenefits(learningDelivery, learner);

        public bool DerivedData12ConditionExcluded(
            IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses,
            ILearningDelivery learningDelivery) =>
            !_derivedData_12Rule.IsAdultSkillsFundedOnBenefits(learnerEmploymentStatuses, learningDelivery);

        public bool DerivedData07ConditionExcluded(int? progType) =>
            !_derivedData_07Rule.IsApprenticeship(progType);

        public bool LARSConditionExcluded(string learnAimRef) =>
            !_lARSDataService.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, TypeOfLARSCategory.TradeUnionAims);

        public bool LearnDelFAMsConditionExcluded(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs) =>
            !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                    learningDeliveryFAMs,
                    LearningDeliveryFAMTypeConstants.LDM,
                    LearningDeliveryFAMCodeConstants.LDM_OLASS)
                && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(
                    learningDeliveryFAMs,
                    LearningDeliveryFAMTypeConstants.RES);

        public bool LARSConditionMet(string learnAimRef) =>
            _lARSDataService.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, LARSNotionalNVQLevelV2.Level2);

        public virtual bool LARSPercentageLevelConditionMet(string learnAimRef, int priorAttain)
        {
            return _lARSDataService.EffectiveDatesValidforLearnAimRef(learnAimRef, _larsAimEffectiveFrom)
                && ((_priorAttains.Contains(priorAttain)
                    && _lARSDataService.FullLevel2PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, _larsAimEffectiveFrom, _percentValue))
                    || !_lARSDataService.FullLevel2PercentForLearnAimRefNotMatchPercentValue(learnAimRef, _larsAimEffectiveFrom, _percentValue));
        }

        public bool LearnDelFAMsConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs) =>
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.FFI,
                LearningDeliveryFAMCodeConstants.FFI_Fully)
                && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                    learningDeliveryFAMs,
                    LearningDeliveryFAMTypeConstants.LDM,
                    LearningDeliveryFAMCodeConstants.LDM_Military);

        public bool AgeLimitConditionMet(DateTime learnStartDate, DateTime? dateOfBirthNullable) =>
            _dateTimeQueryService.YearsBetween(learnStartDate, dateOfBirthNullable.Value) >= 24;

        public bool FundModelConditionMet(int fundModel) => fundModel == TypeOfFunding.AdultSkills;

        public bool LearnStartDateConditionMet(DateTime learnStartDate) => learnStartDate < _augustFirst2016;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameter(
            DateTime? dateOfBirthNullable,
            int? priorAttainNullable,
            ILearningDelivery learningDelivery,
            string learnDelFAMType,
            string learnDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirthNullable),
                BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, priorAttainNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learningDelivery.LearnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, learnDelFAMCode)
            };
        }
    }
}
