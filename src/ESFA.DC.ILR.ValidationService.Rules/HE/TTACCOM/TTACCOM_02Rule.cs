using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM
{
    public class TTACCOM_02Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "TTACCOM";

        public const string Name = RuleNameConstants.TTACCOM_02;

        private readonly IValidationErrorHandler _messageHandler;
        private readonly IProvideLookupDetails _lookupDetails;
        private readonly IDerivedData_06Rule _derivedData06;

        public TTACCOM_02Rule(IValidationErrorHandler validationErrorHandler, IProvideLookupDetails lookupDetails, IDerivedData_06Rule derivedData06)
        {
            _messageHandler = validationErrorHandler;
            _lookupDetails = lookupDetails;
            _derivedData06 = derivedData06;
        }

        public string RuleName => Name;

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;
            var learnerHE = objectToValidate.LearnerHEEntity;
            var tTAccom = learnerHE?.TTACCOMNullable;
            var referenceDate = _derivedData06.Derive(objectToValidate.LearningDeliveries);
            var failedValidation = !ConditionMet(tTAccom, referenceDate);

            if (failedValidation)
            {
                RaiseValidationMessage(learnRefNumber, tTAccom.Value);
            }
        }

        public bool ConditionMet(int? tTAccom, DateTime referenceDate)
        {
            if (!tTAccom.HasValue)
            {                 
                return true;
            }

            if (!_lookupDetails.Contains(TypeOfLimitedLifeLookup.TTAccom, tTAccom.Value))
            {                 
                return true;
            }

            return _lookupDetails.IsCurrent(TypeOfLimitedLifeLookup.TTAccom, tTAccom.Value, referenceDate);
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
