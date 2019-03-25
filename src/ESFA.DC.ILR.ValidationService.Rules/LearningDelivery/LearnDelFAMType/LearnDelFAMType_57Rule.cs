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
    public class LearnDelFAMType_57Rule : AbstractRule, IRule<ILearner>
    {
        private const int MinAge = 24;

        private const int TradeUnionAimsCategoryRef = 19;
        private readonly DateTime minimumStartDate = new DateTime(2016, 08, 01);

        private readonly List<string> famCodesForExclusion = new List<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_OLASS,
            LearningDeliveryFAMCodeConstants.LDM_RoTL,
            LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy,
            LearningDeliveryFAMCodeConstants.LDM_Military
        };

        private readonly HashSet<string> n = new HashSet<string>(new List<string>() { "E", "1", "2" }).ToCaseInsensitiveHashSet();
        private readonly HashSet<int> basicSkillTypes = new HashSet<int>() { 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35 };

        private readonly ILARSDataService _larsDataService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDerivedData_12Rule _dd12;
        private readonly IDerivedData_21Rule _derivedDataRule21;
        private readonly ILearningDeliveryFAMQueryService _famQueryService;
        private readonly IFileDataService _fileDataService;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnDelFAMType_57Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsDataService,
            IDerivedData_07Rule dd07,
            IDerivedData_12Rule dd12,
            IDerivedData_21Rule derivedDataRule21,
            ILearningDeliveryFAMQueryService famQueryService,
            IFileDataService fileDataService,
            IOrganisationDataService organisationDataService,
            IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_57)
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
                if (ConditionMet(learningDelivery, learner.DateOfBirthNullable) &&
                    !IsLearningDeliveryExcluded(learner, learningDelivery))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(_fileDataService.UKPRN(), learner, learningDelivery));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, DateTime? dateofBirth)
        {
            return StartDateConditionMet(learningDelivery.LearnStartDate) &&
                   FundModelConditionMet(learningDelivery.FundModel) &&
                   AgeConditionMet(learningDelivery.LearnStartDate, dateofBirth) &&
                   FamConditionMet(learningDelivery.LearningDeliveryFAMs) &&
                   NvQLevelConditionMet(learningDelivery.LearnAimRef);
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
            if (ageAtCourseStart >= MinAge)
            {
                return true;
            }

            return false;
        }

        public bool FamConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> fams)
        {
            return _famQueryService.HasLearningDeliveryFAMCodeForType(fams, LearningDeliveryFAMTypeConstants.FFI, LearningDeliveryFAMCodeConstants.FFI_Fully);
        }

        public bool NvQLevelConditionMet(string learnAimRef)
        {
            var nvqLevel = _larsDataService.GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef);

            if (n.Contains(nvqLevel))
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
                basicSkillTypes,
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
            };
        }
    }
}
