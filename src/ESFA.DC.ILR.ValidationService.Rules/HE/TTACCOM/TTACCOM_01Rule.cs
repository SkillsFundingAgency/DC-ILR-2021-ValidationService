using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM
{
    public class TTACCOM_01Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "TTACCOM";

        public const string Name = RuleNameConstants.TTACCOM_01;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly IProvideLookupDetails _lookupDetails;

        public TTACCOM_01Rule(IValidationErrorHandler validationErrorHandler, IProvideLookupDetails lookupDetails)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(lookupDetails)
                .AsGuard<ArgumentNullException>(nameof(lookupDetails));

            _messageHandler = validationErrorHandler;
            _lookupDetails = lookupDetails;
        }

        public string RuleName => Name;

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var learnerHE = objectToValidate.LearnerHEEntity;
            var tTAccom = learnerHE?.TTACCOMNullable;

            var failedValidation = !ConditionMet(tTAccom);

            if (failedValidation)
            {
                RaiseValidationMessage(learnRefNumber, tTAccom.Value);
            }
        }

        public bool ConditionMet(int? tTAccom)
        {
            return It.Has(tTAccom)
                ? _lookupDetails.Contains(TypeOfLimitedLifeLookup.TTAccom, tTAccom.Value)
                : true;
        }

        public void RaiseValidationMessage(string learnRefNumber, int tTAccom)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, tTAccom)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
