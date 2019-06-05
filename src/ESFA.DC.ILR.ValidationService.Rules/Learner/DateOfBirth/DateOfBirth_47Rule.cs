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
    public class DateOfBirth_47Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<long?> _fundModels = new HashSet<long?>() { 36, 81 };
        private readonly DateTime _firstAug2016 = new DateTime(2016, 08, 01);

        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public DateOfBirth_47Rule(IDateTimeQueryService dateTimeQueryService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler, RuleNameConstants.DateOfBirth_47)
        {
            _dateTimeQueryService = dateTimeQueryService;

            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.FundModel,
                        learningDelivery.CompStatus,
                        learningDelivery.LearnStartDate,
                        learningDelivery.LearnActEndDateNullable,
                        learningDelivery.AimType,
                        objectToValidate.DateOfBirthNullable,
                        learningDelivery.LearningDeliveryFAMs))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable, learningDelivery.ProgTypeNullable));
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(int? progType, int fundModel, int compStatus, DateTime learnStartDate, DateTime? learnActEndDate, int aimType, DateTime? dateOfBirth, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
            && LearnStartDateConditionMet(learnStartDate)
            && AimTypeConditionMet(aimType)
            && ProgTypeConditionMet(progType)
            && CompStatusConditionMet(compStatus)
            && DateOfBirthConditionMet(dateOfBirth, learnStartDate)
            && LearnActEndDateConditionMet(learnStartDate, learnActEndDate)
            && LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _firstAug2016;
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType.HasValue
                && progType == 25;
        }

        public bool CompStatusConditionMet(int compStatus)
        {
            return compStatus == 2;
        }

        public bool DateOfBirthConditionMet(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween((DateTime)dateOfBirth, learnStartDate) >= 16;
        }

        public bool LearnActEndDateConditionMet(DateTime learnStartDate, DateTime? learnActEndDate)
        {
            return learnActEndDate.HasValue
                && _dateTimeQueryService.WholeDaysBetween(learnStartDate, learnActEndDate.Value) < 372;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)
            };
        }
    }
}
