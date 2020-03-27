using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R50Rule : AbstractRule, IRule<ILearner>
    {
        public R50Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R50)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.ProviderSpecDeliveryMonitorings != null)
                {
                    var duplicates = learningDelivery.ProviderSpecDeliveryMonitorings
                        .GroupBy(pdm => pdm.ProvSpecDelMonOccur?.ToUpper())
                        .Where(grp => grp.Count() > 1);

                    foreach (var duplicate in duplicates)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(duplicate.Key));
                    }
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string provSpecDelMonOccur)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProvSpecDelMonOccur, provSpecDelMonOccur)
            };
        }
    }
}