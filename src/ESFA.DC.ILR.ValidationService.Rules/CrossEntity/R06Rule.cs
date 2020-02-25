using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R06Rule : AbstractRule,
        IRule<IMessage>
    {
        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="R06Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public R06Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R06)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
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