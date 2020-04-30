using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_37Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        private readonly DateTime _firstAugust2016 = new DateTime(2016, 08, 01);
        private readonly DateTime _firstAugust2014 = new DateTime(2014, 08, 01);

        public DateOfBirth_37Rule(
            IDateTimeQueryService dateTimeQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_37)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            if (objectToValidate.DateOfBirthNullable.HasValue)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(
                        learningDelivery.LearnStartDate,
                        objectToValidate.DateOfBirthNullable.Value,
                        learningDelivery.FundModel,
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.AimType,
                        learningDelivery.LearnPlanEndDate,
                        learningDelivery.LearningDeliveryFAMs))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.LearnPlanEndDate));
                    }
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, DateTime dateOfBirth, int fundModel, int? progType, int aimType, DateTime learnPlanEndDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return !Excluded(learningDeliveryFams)
                   && learnStartDate < _firstAugust2016
                   && learnStartDate >= _firstAugust2014
                   && (fundModel == FundModels.AdultSkills || fundModel == FundModels.OtherAdult)
                   && (progType.HasValue && progType.Value == TypeOfLearningProgramme.ApprenticeshipStandard)
                   && aimType == TypeOfAim.ProgrammeAim
                   && _dateTimeQueryService.YearsBetween(dateOfBirth, learnStartDate) >= 19
                   && _dateTimeQueryService.DaysBetween(learnStartDate, learnPlanEndDate) < 365;
        }

        public bool Excluded(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.RES);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, DateTime learnPlanEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate)
            };
        }
    }
}
