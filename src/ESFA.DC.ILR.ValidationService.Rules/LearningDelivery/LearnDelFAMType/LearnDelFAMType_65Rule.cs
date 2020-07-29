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
    public class LearnDelFAMType_65Rule : AbstractRule, IRule<ILearner>
    {
        private const int FundingModel = FundModels.AdultSkills;

        private const int MinAge = 19;
        private const int MaxAge = 23;

        private readonly ILARSDataService _larsDataService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDerivedData_28Rule _derivedDataRule28;
        private readonly IDerivedData_29Rule _derivedDataRule29;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        private readonly HashSet<int> _priorAttain = new HashSet<int> { 2, 3, 4, 5, 10, 11, 12, 13, 97, 98 };
        private readonly DateTime _startDate = new DateTime(2017, 7, 31);
        private readonly DateTime _endDate = new DateTime(2020, 08, 01);
        private readonly HashSet<string> _ldmTypeExcludedCodes = new HashSet<string> { "034", "328", "347", "363" };
        private readonly HashSet<string> _nvqLevels = new HashSet<string> { "E", "1", "2" };

        private readonly HashSet<int> _basicSkillTypes = new HashSet<int>
            { 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35 };

        public LearnDelFAMType_65Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsDataService,
            IDerivedData_07Rule dd07,
            IDerivedData_28Rule derivedDataRule28,
            IDerivedData_29Rule derivedDataRule29,
            IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_65)
        {
            _larsDataService = larsDataService;
            _dd07 = dd07;
            _derivedDataRule28 = derivedDataRule28;
            _derivedDataRule29 = derivedDataRule29;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            if (!_priorAttain.Contains(learner.PriorAttainNullable ?? -1))
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.FundModel != FundingModel
                    || LearnStartDateIsOutsideValidDateRange(learningDelivery.LearnStartDate)
                    || learningDelivery.LearningDeliveryFAMs == null)
                {
                    continue;
                }

                var ageAtCourseStart = _dateTimeQueryService.YearsBetween(learner.DateOfBirthNullable ?? DateTime.MinValue, learningDelivery.LearnStartDate);
                if (ageAtCourseStart < MinAge || ageAtCourseStart > MaxAge)
                {
                    continue;
                }

                var nvqLevel = _larsDataService.GetNotionalNVQLevelv2ForLearnAimRef(learningDelivery.LearnAimRef);
                if (!_nvqLevels.Any(x => x.CaseInsensitiveEquals(nvqLevel)))
                {
                    continue;
                }

                if (ExclusionsApply(learner, learningDelivery))
                {
                    continue;
                }

                foreach (var deliveryFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (deliveryFam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.FFI)
                        && deliveryFam.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMCodeConstants.FFI_Fully))
                    {
                        RaiseValidationMessage(learner, learningDelivery, deliveryFam);
                    }
                }
            }
        }

        public bool LearnStartDateIsOutsideValidDateRange(DateTime learnStartDate)
        {
            return learnStartDate <= _startDate || learnStartDate >= _endDate;
        }

        public bool IsBasicSkillsLearner(ILearningDelivery delivery)
        {
            var larsLearningDelivery = _larsDataService.GetDeliveryFor(delivery.LearnAimRef);

            if (larsLearningDelivery == null)
            {
                return true;
            }

            return _dateTimeQueryService.IsDateBetween(delivery.LearnStartDate, larsLearningDelivery.EffectiveFrom, larsLearningDelivery.EffectiveTo ?? DateTime.MaxValue)
                && _larsDataService.BasicSkillsTypeMatchForLearnAimRef(_basicSkillTypes, delivery.LearnAimRef);
        }

        public bool IsDevolvedLevel2or3ExcludedLearning(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.DevolvedLevelTwoOrThree.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool ExclusionsApply(ILearner learner, ILearningDelivery learningDelivery)
        {
            if (_dd07.IsApprenticeship(learningDelivery.ProgTypeNullable))
            {
                return true;
            }

            if (_derivedDataRule28.IsAdultFundedUnemployedWithBenefits(learningDelivery, learner))
            {
                return true;
            }

            if (_derivedDataRule29.IsInflexibleElementOfTrainingAimLearningDelivery(learningDelivery))
            {
                return true;
            }

            if (learningDelivery.LearningDeliveryFAMs.Any(ldf => ldf.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.LDM)
                && _ldmTypeExcludedCodes.Any(x => x.CaseInsensitiveEquals(ldf.LearnDelFAMCode))))
            {
                return true;
            }

            if (learningDelivery.LearningDeliveryFAMs.Any(ldf => ldf.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.RES)))
            {
                return true;
            }

            if (learningDelivery.LearningDeliveryFAMs.Any(ldf => ldf.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.DAM) && ldf.LearnDelFAMCode == LearningDeliveryFAMCodeConstants.DAM_DevolvedLevelTwoOrThreeExclusion))
            {
                return true;
            }

            return IsBasicSkillsLearner(learningDelivery);
        }

        private void RaiseValidationMessage(ILearner learner, ILearningDelivery learningDelivery, ILearningDeliveryFAM learningDeliveryFam)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learningDeliveryFam.LearnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learningDeliveryFam.LearnDelFAMCode),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable)
            };

            HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, parameters);
        }
    }
}
