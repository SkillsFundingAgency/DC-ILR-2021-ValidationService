using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_40Rule : AbstractRule, IRule<ILearner>
    {
        private const int _days = 365;
        private const int MinAge = 19;
        private const int MinimumContractMonths = 12;        

        private const int ProgrammeType = TypeOfLearningProgramme.ApprenticeshipStandard;
        private const int AimType = TypeOfAim.ProgrammeAim;

        private readonly DateTime _ruleEndDate = new DateTime(2016, 7, 31);
        private readonly int[] _fundModels = { TypeOfFunding.AdultSkills, TypeOfFunding.OtherAdult };

        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public DateOfBirth_40Rule(
            IDateTimeQueryService dateTimeQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_40)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
        public void Validate(ILearner learner)
        {
           if (learner?.LearningDeliveries == null
                || !learner.DateOfBirthNullable.HasValue)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.LearnStartDate > _ruleEndDate)
                {
                    continue;
                }

                if (learningDelivery.LearnActEndDateNullable.HasValue)
                {
                    var duration = _dateTimeQueryService.DaysBetween(learningDelivery.LearnStartDate, learningDelivery.LearnActEndDateNullable.Value);
                    if (duration > _days)
                    {
                        continue;
                    }                
                }            

                var age = _dateTimeQueryService.AgeAtGivenDate(
                    learner.DateOfBirthNullable.Value,
                    learningDelivery.LearnStartDate);

                if (age < MinAge)
                {
                    continue;
                }

                if (_fundModels.All(fm => fm != learningDelivery.FundModel) ||
                    (learningDelivery.ProgTypeNullable ?? -1) != ProgrammeType ||
                    learningDelivery.AimType != AimType)
                {
                    continue;
                }

                if (_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(
                        learningDelivery.LearningDeliveryFAMs,
                        LearningDeliveryFAMTypeConstants.RES))
                {
                    continue;
                }

                if (!learningDelivery.OutcomeNullable.HasValue
                    || learningDelivery.OutcomeNullable.Value != OutcomeConstants.Achieved)
                {
                    continue;
                }

                if (!learningDelivery.LearnActEndDateNullable.HasValue
                    || _dateTimeQueryService.YearsBetween(
                        learningDelivery.LearnStartDate,
                        learningDelivery.LearnActEndDateNullable.Value) >= 1)
                {
                    continue;
                }

                 RaiseValidationMessage(learner, learningDelivery);
            }
        }

        private void RaiseValidationMessage(ILearner learner, ILearningDelivery learningDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, learningDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, learningDelivery.OutcomeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.RES)
            };

            HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, parameters);
        }
    }
}