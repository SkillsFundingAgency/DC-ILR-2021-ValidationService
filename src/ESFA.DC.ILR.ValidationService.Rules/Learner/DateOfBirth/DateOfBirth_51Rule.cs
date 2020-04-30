using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_51Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly DateTime _julyThirtyFirst2016 = new DateTime(2016, 7, 31);

        public DateOfBirth_51Rule(
            IDateTimeQueryService dateTimeQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_51)
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
                    if (ConditionMet(learningDelivery, objectToValidate.DateOfBirthNullable.Value))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable.Value, learningDelivery.LearnStartDate));
                    }
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, DateTime dateOfBirth)
        {
            return !Excluded(learningDelivery.LearningDeliveryFAMs)
                   && learningDelivery.LearnStartDate > _julyThirtyFirst2016
                   && (learningDelivery.ProgTypeNullable.HasValue && learningDelivery.ProgTypeNullable == TypeOfLearningProgramme.Traineeship)
                   && learningDelivery.AimType == AimTypes.ProgrammeAim
                   && _dateTimeQueryService.YearsBetween(dateOfBirth, learningDelivery.LearnStartDate) >= 25;
        }

        public bool Excluded(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.RES);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime dateOfBirth, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}