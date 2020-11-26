using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_38Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { FundModels.AdultSkills, FundModels.OtherAdult };
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDerivedData_07Rule _dd07;

        public DateOfBirth_38Rule(
            IValidationErrorHandler validationErrorHandler,
            IDateTimeQueryService dateTimeQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IDerivedData_07Rule dd07)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_38)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    objectToValidate.DateOfBirthNullable,
                    learningDelivery.LearnStartDate,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.AimType,
                    learningDelivery.LearnActEndDateNullable,
                    learningDelivery.OutcomeNullable,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            objectToValidate.DateOfBirthNullable,
                            learningDelivery.AimType,
                            learningDelivery.LearnStartDate,
                            learningDelivery.FundModel,
                            LearningDeliveryFAMTypeConstants.RES));
                }
            }
        }

        public bool ConditionMet(
            int fundModel,
            DateTime? dateOfBirth,
            DateTime learnStartDate,
            int? progType,
            int aimType,
            DateTime? learnActEndDate,
            int? outCome,
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
                && DateOfBirthConditionMet(dateOfBirth, learnStartDate)
                && DD07ConditionMet(progType)
                && AimTypeConditionMet(aimType)
                && ApprenticeshipDurationConditionMet(learnStartDate, learnActEndDate)
                && OutComeConditionMet(outCome)
                && LearningDeliveryFAMTypeConditionMet(learningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool DateOfBirthConditionMet(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween((DateTime)dateOfBirth, learnStartDate) < 19;
        }

        public bool DD07ConditionMet(int? progType)
        {
            return progType.HasValue
                && progType != 25
                && _dd07.IsApprenticeship(progType);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool ApprenticeshipDurationConditionMet(DateTime learnStartDate, DateTime? learnActEndDate)
        {
            return learnActEndDate.HasValue
                && _dateTimeQueryService.DaysBetween(learnStartDate, (DateTime)learnActEndDate) < 365;
        }

        public bool OutComeConditionMet(int? outCome)
        {
            return outCome.HasValue
                && outCome == 1;
        }

        public bool LearningDeliveryFAMTypeConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, int aimType, DateTime learnStartDate, int fundModel, string learnDelFAMType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType)
            };
        }
    }
}
