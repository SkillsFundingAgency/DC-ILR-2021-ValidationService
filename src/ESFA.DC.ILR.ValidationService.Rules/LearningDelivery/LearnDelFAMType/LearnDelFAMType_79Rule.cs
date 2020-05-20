using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_79Rule : AbstractRule, IRule<ILearner>
    {
        private const int MinAge = 19;
        private const int MaxAge = 23;
        private readonly DateTime _latestStartDate = new DateTime(2020, 07, 31, 23, 59, 59);
        private readonly HashSet<int?> _basicSkillTypes = new HashSet<int?>()
        {
            LARSConstants.BasicSkills.Certificate_AdultLiteracy,
            LARSConstants.BasicSkills.Certificate_AdultNumeracy,
            LARSConstants.BasicSkills.GCSE_EnglishLanguage,
            LARSConstants.BasicSkills.GCSE_Mathematics,
            LARSConstants.BasicSkills.KeySkill_Communication,
            LARSConstants.BasicSkills.KeySkill_ApplicationOfNumbers,
            LARSConstants.BasicSkills.FunctionalSkillsMathematics,
            LARSConstants.BasicSkills.FunctionalSkillsEnglish,
            LARSConstants.BasicSkills.UnitsOfTheCertificate_AdultNumeracy,
            LARSConstants.BasicSkills.UnitsOfTheCertificate_AdultLiteracy,
            LARSConstants.BasicSkills.NonNQF_QCFS4LLiteracy,
            LARSConstants.BasicSkills.NonNQF_QCFS4LNumeracy,
            LARSConstants.BasicSkills.QCFBasicSkillsEnglishLanguage,
            LARSConstants.BasicSkills.QCFBasicSkillsMathematics,
            LARSConstants.BasicSkills.UnitQCFBasicSkillsEnglishLanguage,
            LARSConstants.BasicSkills.UnitQCFBasicSkillsMathematics,
            LARSConstants.BasicSkills.InternationalGCSEEnglishLanguage,
            LARSConstants.BasicSkills.InternationalGCSEMathematics,
            LARSConstants.BasicSkills.FreeStandingMathematicsQualification
        };

        private readonly HashSet<string> _ldmExclusions = new HashSet<string>
        {
            LearningDeliveryFAMCodeConstants.LDM_OLASS,
            LearningDeliveryFAMCodeConstants.LDM_RoTL,
            LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy,
            LearningDeliveryFAMCodeConstants.LDM_LowWages,
        };

        private readonly IFileDataService _fileDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDerivedData_29Rule _dd29;
        private readonly IDerivedData_37Rule _dd37;
        private readonly IDerivedData_38Rule _dd38;

        public LearnDelFAMType_79Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IDateTimeQueryService dateTimeQueryService,
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IDerivedData_07Rule dd07,
            IDerivedData_29Rule dd29,
            IDerivedData_37Rule dd37,
            IDerivedData_38Rule dd38)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_79)
        {
            _fileDataService = fileDataService;
            _dateTimeQueryService = dateTimeQueryService;
            _larsDataService = larsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dd07 = dd07;
            _dd29 = dd29;
            _dd37 = dd37;
            _dd38 = dd38;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            var ukprn = _fileDataService.UKPRN();

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (ConditionMet(learner.DateOfBirthNullable, learner.LearnerEmploymentStatuses, learningDelivery))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            ukprn,
                            learner.DateOfBirthNullable,
                            learningDelivery.LearnAimRef,
                            learningDelivery.LearnStartDate,
                            learningDelivery.FundModel,
                            learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? dateOfBirth, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, ILearningDelivery learningDelivery)
        {
            var larsLearningDelivery = _larsDataService.GetDeliveryFor(learningDelivery.LearnAimRef);

            if (larsLearningDelivery == null)
            {
                return false;
            }

            return !Excluded(learningDelivery, learnerEmploymentStatuses, larsLearningDelivery)
                && FundModelCondition(learningDelivery.FundModel)
                && StartDateCondition(learningDelivery.LearnStartDate)
                && AgeConditionMet(learningDelivery.LearnStartDate, dateOfBirth)
                && LarsCondition(larsLearningDelivery)
                && LearningDeliveryFAMsCondition(learningDelivery.LearningDeliveryFAMs);
        }

        public bool FundModelCondition(int fundModel) => fundModel == FundModels.AdultSkills;

        public bool StartDateCondition(DateTime learnStartDate) => learnStartDate > _latestStartDate;

        public bool AgeConditionMet(DateTime learnStartDate, DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
            {
                return false;
            }

            var ageAtCourseStart = _dateTimeQueryService.YearsBetween(dateOfBirth.Value, learnStartDate);
            if (ageAtCourseStart >= MinAge && ageAtCourseStart <= MaxAge)
            {
                return true;
            }

            return false;
        }

        public bool LarsCondition(ILARSLearningDelivery larsLearningDelivery)
        {
            return larsLearningDelivery.NotionalNVQLevelv2 == LARSConstants.NotionalNVQLevelV2Strings.Level2
                && !larsLearningDelivery.Categories.Any(x => x.CategoryRef == LARSConstants.Categories.LegalEntitlementLevel2);
        }

        public bool LearningDeliveryFAMsCondition(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs) =>
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.FFI, LearningDeliveryFAMCodeConstants.FFI_Fully);

        public bool Excluded(ILearningDelivery learningDelivery, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, ILARSLearningDelivery larsLearningDelivery)
        {
            return DD07Condition(learningDelivery.ProgTypeNullable)
                || DD29Condition(learningDelivery)
                || DD37Condition(learningDelivery.FundModel, learningDelivery.LearnStartDate, learnerEmploymentStatuses, learningDelivery.LearningDeliveryFAMs)
                || DD38Condition(learningDelivery.FundModel, learningDelivery.LearnStartDate, learnerEmploymentStatuses)
                || LearningDeliveryFAMExclusion(learningDelivery.LearningDeliveryFAMs)
                || LARSExclusionCondition(larsLearningDelivery, learningDelivery.LearnStartDate);
        }

        public bool DD07Condition(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public bool DD29Condition(ILearningDelivery learningDelivery)
        {
            return _dd29.IsInflexibleElementOfTrainingAimLearningDelivery(learningDelivery);
        }

        public bool DD37Condition(int fundModel, DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _dd37.Derive(fundModel, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs);
        }

        public bool DD38Condition(int fundModel, DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return _dd38.Derive(fundModel, learnStartDate, learnerEmploymentStatuses);
        }

        public bool LearningDeliveryFAMExclusion(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)
                || _learningDeliveryFAMQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, _ldmExclusions)
                || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_DevolvedLevelTwoOrThreeExclusion);
        }

        public bool LARSExclusionCondition(ILARSLearningDelivery larsLearningDelivery, DateTime learnStartDate)
        {
            return _dateTimeQueryService.IsDateBetween(learnStartDate, larsLearningDelivery.EffectiveFrom, larsLearningDelivery.EffectiveTo ?? DateTime.MaxValue)
                && _larsDataService.GetAnnualValuesFor(larsLearningDelivery.LearnAimRef).Any(la => _basicSkillTypes.Contains(la.BasicSkillsType));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int ukprn, DateTime? dateOfBirth, string learnAimRef, DateTime learnStartDate, int fundModel, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.FFI),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.FFI_Fully)
            };
        }
    }
}
