using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _lookups;

        public AFinType_02Rule(IProvideLookupDetails lookups, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinType_02)
        {
            _lookups = lookups;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(ld => ld.AppFinRecords != null))
            {
                foreach (var appFinRecord in learningDelivery.AppFinRecords)
                {
                    if (ConditionMet(appFinRecord))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            errorMessageParameters: BuildErrorMessageParameters(appFinRecord.AFinType, appFinRecord.AFinCode));
                    }
                }
            }
        }

        public bool ConditionMet(IAppFinRecord appFinRecord)
        {
            return !_lookups.Contains(TypeOfStringCodedLookup.ApprenticeshipFinancialRecord, $"{appFinRecord.AFinType.ToUpper()}{appFinRecord.AFinCode}");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string aFinType, int aFinCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinCode, aFinCode)
            };
        }
    }
}
