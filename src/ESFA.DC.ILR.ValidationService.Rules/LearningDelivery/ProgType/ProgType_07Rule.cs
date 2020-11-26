using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    public class ProgType_07Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "ProgType";

        public const string Name = RuleNameConstants.ProgType_07;

        private readonly IValidationErrorHandler _messageHandler;

        public ProgType_07Rule(IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsTrainee(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == ProgTypes.Traineeship;

        public bool IsInAProgramme(ILearningDelivery delivery) =>
            delivery.AimType == AimTypes.ProgrammeAim;

        public bool IsViable(ILearningDelivery delivery) =>
            ProgTypes.IsViableApprenticeship(delivery.LearnStartDate);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => IsViable(x) && IsTrainee(x) && IsInAProgramme(x))
                .ForEach(x =>
                {
                    var failedValidation = !ConditionMet(x);

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        public bool ConditionMet(ILearningDelivery thisDelivery)
        {
            return thisDelivery != null
                ? thisDelivery.LearnStartDate > DateTime.MinValue
                    && ProgTypes.WithinMaxmimumTrainingDuration(thisDelivery.LearnStartDate, thisDelivery.LearnPlanEndDate)
                : true;
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, thisDelivery.LearnPlanEndDate),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
