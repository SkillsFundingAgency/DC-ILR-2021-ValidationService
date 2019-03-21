using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_65Rule : AbstractRule, IRule<ILearner>
    {
        private const int FundingModel = TypeOfFunding.AdultSkills;

        private const int MinAge = 19;
        private const int MaxAge = 23;

        private const string InvalidFamCode = "1";

        private readonly ILARSDataService _larsDataService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDerivedData_28Rule _derivedDataRule28;
        private readonly IDerivedData_29Rule _derivedDataRule29;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        private readonly int[] _priorAttain = { 2, 3, 4, 5, 10, 11, 12, 13, 97, 98 };
        private readonly DateTime _startDate = new DateTime(2017, 7, 31);
        private readonly string[] _ldmTypeExcludedCodes = { "034", "328", "347", "363" };
        private readonly string[] _nvqLevels = { "E", "1", "2" };

        private readonly int[] _basicSkillTypes =
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

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
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
                    || learningDelivery.LearnStartDate <= _startDate
                    || learningDelivery.LearningDeliveryFAMs == null)
                {
                    continue;
                }

                var ageAtCourseStart = _dateTimeQueryService.AgeAtGivenDate(learner.DateOfBirthNullable ?? DateTime.MinValue, learningDelivery.LearnStartDate);
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
                        && deliveryFam.LearnDelFAMCode.CaseInsensitiveEquals(InvalidFamCode))
                    {
                        RaiseValidationMessage(learner, learningDelivery, deliveryFam);
                    }
                }
            }
        }

        private bool ExclusionsApply(ILearner learner, ILearningDelivery learningDelivery)
        {
            if (_dd07.IsApprenticeship(learningDelivery.ProgTypeNullable))
            {
                return true;
            }

            if (_derivedDataRule28.IsAdultFundedUnemployedWithBenefits(learningDelivery, learner))
            {
                return true;
            }

            if (_derivedDataRule29.IsInflexibleElementOfTrainingAim(learner))
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

            return _larsDataService.BasicSkillsMatchForLearnAimRefAndStartDate(
                   _basicSkillTypes,
                   learningDelivery.LearnAimRef,
                   learningDelivery.LearnStartDate);
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
