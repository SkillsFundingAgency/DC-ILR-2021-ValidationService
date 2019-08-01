using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_52Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        private readonly DateTime _mayFirst2017 = new DateTime(2017, 5, 1);

        public DateOfBirth_52Rule(
            IDerivedData_07Rule dd07,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IDateTimeQueryService dateTimeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_52)
        {
            _dd07 = dd07;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.AimType,
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearnPlanEndDate,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, int aimType, DateTime learnStartDate, DateTime learnPlanEndDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return !Excluded(progType, learningDeliveryFams)
                   && fundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
                   && learnStartDate >= _mayFirst2017
                   && _dd07.IsApprenticeship(progType)
                   && aimType == TypeOfAim.ProgrammeAim
                   && _dateTimeQueryService.WholeDaysBetween(learnStartDate, learnPlanEndDate) < 365;
        }

        public bool Excluded(int? progType, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return (progType.HasValue && progType == TypeOfLearningProgramme.ApprenticeshipStandard) ||
                   _learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.RES);
        }
    }
}
