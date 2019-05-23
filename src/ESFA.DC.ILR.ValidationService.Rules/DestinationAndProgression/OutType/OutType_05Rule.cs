using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutType
{
    public class OutType_05Rule : AbstractRule, IRule<ILearnerDestinationAndProgression>
    {
        private readonly HashSet<string> _outTypes = new HashSet<string>
        {
            OutTypeConstants.Education,
            OutTypeConstants.PaidEmployment,
            OutTypeConstants.GapYear,
            OutTypeConstants.NotInPaidEmployment,
            OutTypeConstants.Other,
            OutTypeConstants.SocialDestination,
            OutTypeConstants.VoluntaryWork
        }.ToCaseInsensitiveHashSet();

        private readonly IProvideLookupDetails _lookups;

        public OutType_05Rule(IProvideLookupDetails lookups, IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.OutType_05)
        {
            _lookups = lookups;
        }

        public void Validate(ILearnerDestinationAndProgression objectToValidate)
        {
            if (objectToValidate.DPOutcomes != null)
            {
                foreach (var dpOutcome in objectToValidate.DPOutcomes)
                {
                    if (ConditionMet(dpOutcome))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(dpOutcome.OutStartDate));
                    }
                }
            }
        }

        public bool ConditionMet(IDPOutcome dpOutcome)
        {
            return OutTypeConditionMet(dpOutcome)
                && OutCodeConditionMet(dpOutcome);
        }

        public bool OutTypeConditionMet(IDPOutcome dpOutcome)
        {
            return dpOutcome.OutType != null && _outTypes.Contains(dpOutcome.OutType);
        }

        public bool OutCodeConditionMet(IDPOutcome dpOutcome)
        {
            return !_lookups.IsCurrent(
                TypeOfLimitedLifeLookup.OutType,
                $"{dpOutcome.OutType}{dpOutcome.OutCode}",
                dpOutcome.OutStartDate);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(System.DateTime outStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutStartDate, outStartDate),
            };
        }
    }
}
