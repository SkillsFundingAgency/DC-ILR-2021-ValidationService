using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.SWSupAimId
{
    public class SWSupAimId_01Rule :
        AbstractRule, IRule<ILearner>
    {
        public SWSupAimId_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.SWSupAimId_01)
        {
        }

        public bool IsValidGuid(string candidate) =>
            Guid.TryParse(candidate, out Guid result) && !result.Equals(Guid.Empty);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => !string.IsNullOrWhiteSpace(x.SWSupAimId))
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
                ? IsValidGuid(thisDelivery.SWSupAimId)
                : true;
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildErrorMessageParameters(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.SWSupAimId, thisDelivery.SWSupAimId.ToString())
            };
        }
    }
}
