using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.HE
{
    public class LearnerHE_02Rule :
            IRule<ILearner>
    {
        public const string MessagePropertyName = "LearnerHE";

        public const string Name = RuleNameConstants.LearnerHE_02;

        private readonly IValidationErrorHandler _messageHandler;

        public LearnerHE_02Rule(IValidationErrorHandler validationErrorHandler)
        {
            
                

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public void Validate(ILearner objectToValidate)
        {
            
                

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var learnerHE = objectToValidate.LearnerHEEntity;
            var learningDeliveries = objectToValidate.LearningDeliveries;

            var failedValidation = !ConditionMet(learnerHE, learningDeliveries);

            if (failedValidation)
            {
                RaiseValidationMessage(learnRefNumber, learnerHE);
            }
        }

        public bool ConditionMet(ILearnerHE learnerHE, IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return learnerHE != null
                ? learningDeliveries != null && learningDeliveries.Any(d => d.LearningDeliveryHEEntity != null)
                : true;
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearnerHE learnerHE)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, learnerHE)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
