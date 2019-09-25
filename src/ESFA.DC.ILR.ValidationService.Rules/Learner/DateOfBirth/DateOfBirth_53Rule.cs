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
    public class DateOfBirth_53Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { TypeOfFunding.ApprenticeshipsFrom1May2017 };
        private readonly DateTime _mayFirst2017 = new DateTime(2017, 5, 1);

        public DateOfBirth_53Rule(
            IDerivedData_07Rule dd07,
            IDateTimeQueryService dateTimeQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_53)
        {
            _dd07 = dd07;
            _dateTimeQueryService = dateTimeQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
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
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnStartDate,
                    learningDelivery.AimType,
                    learningDelivery.LearnActEndDateNullable,
                    learningDelivery.CompStatus,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(
                        learningDelivery.LearnStartDate,
                        learningDelivery.LearnActEndDateNullable));
                }
            }
        }

        public bool ConditionMet(
            int fundModel,
            int? progType,
            DateTime learnStartDate,
            int aimType,
            DateTime? learnActEndDate,
            int compStatus,
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
                && LearnStartDateConditionMet(learnStartDate)
                && DD07ConditionMet(progType)
                && AimTypeConditionMet(aimType)
                && LearnActEndDateConditionMet(learnStartDate, learnActEndDate)
                && CompStatusConditionMet(compStatus)
                && LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _mayFirst2017;
        }

        public bool DD07ConditionMet(int? progType)
        {
            return progType.HasValue
                && progType != TypeOfLearningProgramme.ApprenticeshipStandard
                && _dd07.IsApprenticeship(progType);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == TypeOfAim.ProgrammeAim;
        }

        public bool LearnActEndDateConditionMet(DateTime learnStartDate, DateTime? learnActEndDate)
        {
            return learnActEndDate.HasValue
                && _dateTimeQueryService.WholeDaysBetween(learnStartDate, learnActEndDate.Value) < 365;
        }

        public bool CompStatusConditionMet(int compStatus)
        {
            return compStatus == CompletionState.HasCompleted;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate)
            };
        }
    }
}
