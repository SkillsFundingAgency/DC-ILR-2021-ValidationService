using System;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R06Rule : AbstractRule,
        IRule<IMessage>
    {
        private readonly IValidationErrorHandler _messageHandler;

        public R06Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R06)
        {
            _messageHandler = validationErrorHandler;
        }

        public void Validate(IMessage objectToValidate)
        {
            if (objectToValidate?.Learners == null)
            {
                return;
            }

            var duplicateLearnRefs = objectToValidate.Learners
                .GroupBy(x => x.LearnRefNumber, StringComparer.InvariantCultureIgnoreCase)
                .Select(x => new { LearnrefNumber = x.Key, Count = x.Count() });

            foreach (var learnerRef in duplicateLearnRefs)
            {
                if (learnerRef.Count > 1)
                {
                    for (int i = 0; i < learnerRef.Count; i++)
                    {
                        _messageHandler.Handle(RuleName, learnerRef.LearnrefNumber);
                    }
                }
            }
        }
    }
}