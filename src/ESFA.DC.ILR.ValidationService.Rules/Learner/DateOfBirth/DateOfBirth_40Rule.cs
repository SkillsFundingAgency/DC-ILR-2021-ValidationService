using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
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

        private readonly int[] _fundModels =
         {
            TypeOfFunding.AdultSkills,
            TypeOfFunding.OtherAdult
        };

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

        public DateOfBirth_40Rule()
         : base(null, null)
        {
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
                if (ConditionMet(
                                 learningDelivery.FundModel,
                                 learningDelivery.AimType,
                                 learningDelivery.ProgTypeNullable,
                                 learningDelivery.OutcomeNullable,
                                 learningDelivery.LearnStartDate,
                                 learner.DateOfBirthNullable,
                                 learningDelivery.LearnActEndDateNullable,
                                 learningDelivery.LearningDeliveryFAMs))
                {
                    RaiseValidationMessage(learner, learningDelivery);
                }
            }
        }

        public virtual bool ConditionMet(int fundModel, int aimType, int? progType, int? outcome, DateTime startDate, DateTime? dateOfBirth, DateTime? actEndDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return ProgTypeConditionMet(progType)
                && FundModelConditionMet(fundModel)
                && OutcomeConditionMet(outcome)
                && AimsStartDateConditionMet(startDate)
                && AimTypeConditionMet(aimType)
                && AgeConditionMet(dateOfBirth, startDate)
                && DurationConditionMet(startDate, actEndDate)
                && RestartConditionMet(learningDeliveryFAMs);
        }

        public virtual bool AimsStartDateConditionMet(DateTime startDate)
        {
            return startDate <= _ruleEndDate;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public virtual bool AimTypeConditionMet(int aimType)
        {
            return aimType == TypeOfAim.ProgrammeAim;
        }

        public virtual bool ProgTypeConditionMet(int? progType)
        {
            return progType == TypeOfLearningProgramme.ApprenticeshipStandard;
        }

        public virtual bool OutcomeConditionMet(int? outcome)
        {
            return outcome == OutcomeConstants.Achieved;
        }

        public virtual bool AgeConditionMet(DateTime? dateOfBirth, DateTime startDate)
        {
            return dateOfBirth.HasValue && (_dateTimeQueryService.YearsBetween(dateOfBirth.Value, startDate)) >= MinAge;
        }

        public virtual bool DurationConditionMet(DateTime startDate, DateTime? actEndDate)
        {
            return actEndDate.HasValue && _dateTimeQueryService.WholeDaysBetween(startDate, actEndDate.Value) < _days;
        }

        public virtual bool RestartConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);
        }

        private void RaiseValidationMessage(ILearner learner, ILearningDelivery learningDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, learningDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, learningDelivery.OutcomeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.RES)
            };

            HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, parameters);
        }
    }
}