using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Entity
{
    public class Entity_1Rule : AbstractRule, IRule<IMessage>
    {
        public Entity_1Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Entity_1)
        {
        }

        public void Validate(IMessage objectToValidate)
        {
            if (ConditionMet(objectToValidate.Learners, objectToValidate.LearnerDestinationAndProgressions))
            {
                HandleValidationError();
            }
        }

        public bool ConditionMet(IReadOnlyCollection<ILearner> learners, IReadOnlyCollection<ILearnerDestinationAndProgression> learnerDestinationAndProgressions)
        {
            return (learners?.Count() ?? 0) == 0
                && (learnerDestinationAndProgressions?.Count() ?? 0) == 0;
        }
    }
}
