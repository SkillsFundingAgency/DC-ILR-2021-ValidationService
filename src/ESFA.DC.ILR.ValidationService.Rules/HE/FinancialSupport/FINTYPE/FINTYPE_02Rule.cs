using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.FinancialSupport.FINTYPE
{
    public class FINTYPE_02Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "FINTYPE";

        public const string Name = RuleNameConstants.FinType_02;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly IProvideLookupDetails _lookupDetails;

        public FINTYPE_02Rule(IValidationErrorHandler validationErrorHandler, IProvideLookupDetails lookupDetails)
        {
            _messageHandler = validationErrorHandler;
            _lookupDetails = lookupDetails;
        }

        public string RuleName => Name;

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;
            var learnerHE = objectToValidate.LearnerHEEntity;
            var financialSupport = learnerHE?.LearnerHEFinancialSupports;

            var failedValidation = !ConditionMet(financialSupport);

            if (failedValidation)
            {
                RaiseValidationMessage(learnRefNumber, financialSupport);
            }
        }

        public bool ConditionMet(IReadOnlyCollection<ILearnerHEFinancialSupport> financialSupport)
        {
            return !financialSupport.IsNullOrEmpty()
                ? _lookupDetails.Get(TypeOfIntegerCodedLookup.FinType).All(x => financialSupport.Count(y => y.FINTYPE == x) <= 1)
                : true;
        }

        public void RaiseValidationMessage(string learnRefNumber, IReadOnlyCollection<ILearnerHEFinancialSupport> financialSupport)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, financialSupport)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
