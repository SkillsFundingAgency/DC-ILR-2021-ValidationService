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
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_56Rule : AbstractRule, IRule<ILearner>
    {
        private const int MinAge = 19;
        private const int MaxAge = 23;
        private const int TradeUnionAimsCategoryRef = 19;
        private readonly DateTime minimumStartDate = new DateTime(2016, 08, 01);

        private readonly HashSet<int> priorAttainList1 = new HashSet<int>() { 2, 3, 4, 5, 10, 11, 12, 13, 97, 98 };
        private readonly HashSet<int> priorAttainList2 = new HashSet<int>() { 3, 4, 5, 10, 11, 12, 13, 97, 98 };

        private readonly List<string> famCodesForExclusion = new List<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_OLASS,
            LearningDeliveryFAMCodeConstants.LDM_RoTL,
            LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy
        };

        private readonly HashSet<string> nvqLevelsList1 = new HashSet<string>(new List<string>() { "E", "1", "2" }).ToCaseInsensitiveHashSet();
        private readonly HashSet<string> nvqLevelsList2 = new HashSet<string>(new List<string>() { "3", "4" }).ToCaseInsensitiveHashSet();

        private readonly ILARSDataService _larsDataService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDerivedData_12Rule _dd12;
        private readonly IDerivedData_21Rule _derivedDataRule21;
        private readonly ILearningDeliveryFAMQueryService _famQueryService;
        private readonly IFileDataService _fileDataService;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnDelFAMType_56Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsDataService,
            IDerivedData_07Rule dd07,
            IDerivedData_12Rule dd12,
            IDerivedData_21Rule derivedDataRule21,
            ILearningDeliveryFAMQueryService famQueryService,
            IFileDataService fileDataService,
            IOrganisationDataService organisationDataService,
            IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_56)
        {
            _larsDataService = larsDataService;
            _dd07 = dd07;
            _dd12 = dd12;
            _derivedDataRule21 = derivedDataRule21;
            _famQueryService = famQueryService;
            _fileDataService = fileDataService;
            _organisationDataService = organisationDataService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            if (IsProviderExcluded())
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery, learner.DateOfBirthNullable, learner.PriorAttainNullable) &&
                    !IsLearningDeliveryExcluded(learner, learningDelivery))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(_fileDataService.UKPRN(), learner, learningDelivery));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, DateTime? dateofBirth, int? priorAttain)
        {
            return StartDateConditionMet(learningDelivery.LearnStartDate) &&
                   FundModelConditionMet(learningDelivery.FundModel) &&
                   AgeConditionMet(learningDelivery.LearnStartDate, dateofBirth) &&
                   FamConditionMet(learningDelivery.LearningDeliveryFAMs) &&
                   NvQLevelConditionMet(learningDelivery.LearnAimRef, priorAttain);
        }

        public bool StartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate < minimumStartDate;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == TypeOfFunding.AdultSkills;
        }

        public bool AgeConditionMet(DateTime learnStartDate, DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
            {
                return false;
            }

            var ageAtCourseStart = _dateTimeQueryService.AgeAtGivenDate(dateOfBirth.Value, learnStartDate);
            if (ageAtCourseStart >= MinAge && ageAtCourseStart <= MaxAge)
            {
                return true;
            }

            return false;
        }

        public bool FamConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> fams)
        {
            return _famQueryService.HasLearningDeliveryFAMCodeForType(fams, LearningDeliveryFAMTypeConstants.FFI, LearningDeliveryFAMCodeConstants.FFI_Fully);
        }

        public bool NvQLevelConditionMet(string learnAimRef, int? priorAttain)
        {
            if (!priorAttain.HasValue)
            {
                return false;
            }

            if (!priorAttainList1.Contains(priorAttain.Value) &&
                !priorAttainList2.Contains(priorAttain.Value))
            {
                return false;
            }

            var nvqLevel = _larsDataService.GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef);

            if ((priorAttainList1.Contains(priorAttain.Value) && nvqLevelsList1.Contains(nvqLevel)) ||
                (priorAttainList2.Contains(priorAttain.Value) && nvqLevelsList2.Contains(nvqLevel)))
            {
                return true;
            }

            return false;
        }

        public bool IsProviderExcluded()
        {
            return _organisationDataService.LegalOrgTypeMatchForUkprn(_fileDataService.UKPRN(), LegalOrgTypeConstants.USDC);
        }

        public bool IsLearningDeliveryExcluded(ILearner learner, ILearningDelivery learningDelivery)
        {
            if (learningDelivery.ProgTypeNullable.HasValue &&
                learningDelivery.ProgTypeNullable.Value == TypeOfLearningProgramme.Traineeship)
            {
                return true;
            }

            if (_dd07.IsApprenticeship(learningDelivery.ProgTypeNullable))
            {
                return true;
            }

            if (_famQueryService.HasAnyLearningDeliveryFAMCodesForType(
                learningDelivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                famCodesForExclusion))
            {
                return true;
            }

            if (_dd12.IsAdultSkillsFundedOnBenefits(learner.LearnerEmploymentStatuses, learningDelivery))
            {
                return true;
            }

            if (_derivedDataRule21.IsAdultFundedUnemployedWithOtherStateBenefits(learningDelivery, learner))
            {
                return true;
            }

            if (_famQueryService.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES))
            {
                return true;
            }

            if (_larsDataService.BasicSkillsMatchForLearnAimRefAndStartDate(
                TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills,
                learningDelivery.LearnAimRef,
                learningDelivery.LearnStartDate))
            {
                return true;
            }

            if (_larsDataService.LearnAimRefExistsForLearningDeliveryCategoryRef(
                learningDelivery.LearnAimRef,
                TradeUnionAimsCategoryRef))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long ukprn, ILearner learner, ILearningDelivery learningDelivery)
        {
            return new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn),
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learningDelivery.LearnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, learningDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.FFI),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.FFI_Fully),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable),
                BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, learner.PriorAttainNullable)
            };
        }
    }
}
