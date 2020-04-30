using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    public class ProgType_14Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "ProgType";

        public const string Name = RuleNameConstants.ProgType_14;

        private readonly IValidationErrorHandler _messageHandler;

        public ProgType_14Rule(IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => x.ProgTypeNullable == ProgTypes.Traineeship)
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
                ? !thisDelivery.LearnAimRef.CaseInsensitiveEquals(AimTypes.References.IndustryPlacement)
                : true;
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
