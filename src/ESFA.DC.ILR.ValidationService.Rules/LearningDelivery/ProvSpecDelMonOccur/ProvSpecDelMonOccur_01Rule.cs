﻿namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProvSpecDelMonOccur
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.ValidationService.Data.Extensions;
    using ESFA.DC.ILR.ValidationService.Interface;
    using ESFA.DC.ILR.ValidationService.Rules.Abstract;
    using ESFA.DC.ILR.ValidationService.Rules.Constants;
    using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

    public class ProvSpecDelMonOccur_01Rule : AbstractRule, IRule<ILearner>
    {
        public ProvSpecDelMonOccur_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ProvSpecDelMonOccur_01Rule)
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
                if (learningDelivery.ProviderSpecDeliveryMonitorings == null)
                {
                    continue;
                }

                foreach (var providerSpecDeliveryMonitoring in learningDelivery.ProviderSpecDeliveryMonitorings)
                {
                    if (ConditionMet(providerSpecDeliveryMonitoring.ProvSpecDelMonOccur))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(providerSpecDeliveryMonitoring.ProvSpecDelMonOccur));
                    }
                }
            }
        }

        public bool ConditionMet(string provSpecDelMonOccur)
        {
            return !string.IsNullOrWhiteSpace(provSpecDelMonOccur) && !Monitoring.Delivery.ValidProvSpecDelMonOccurValues.Any(x => x.CaseInsensitiveEquals(provSpecDelMonOccur));
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
