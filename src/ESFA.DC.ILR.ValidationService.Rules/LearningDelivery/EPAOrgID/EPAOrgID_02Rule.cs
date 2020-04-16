using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using static ESFA.DC.ILR.ValidationService.Rules.Constants.ApprenticeshipFinancialRecord;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EPAOrgID
{
    public class EPAOrgID_02Rule : AbstractRule, IRule<ILearner>
    {
        public EPAOrgID_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EPAOrgID_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.AppFinRecords != null)
                {
                    foreach (var appFinRecord in learningDelivery.AppFinRecords)
                    {
                        if (ConditionMet(learningDelivery.EPAOrgID, appFinRecord.AFinType, appFinRecord.AFinCode))
                        {
                            HandleValidationError(
                                objectToValidate.LearnRefNumber,
                                learningDelivery.AimSeqNumber,
                                BuildErrorMessageParameters(appFinRecord.AFinType, appFinRecord.AFinCode));
                        }
                    }
                }
            }
        }

        public bool ConditionMet(string epaOrgID, string aFinType, int aFinCode)
        {
            return string.IsNullOrEmpty(epaOrgID) && aFinType == Types.TotalNegotiatedPrice && (aFinCode == TotalNegotiatedPriceCodes.TotalAssessmentPrice || aFinCode == TotalNegotiatedPriceCodes.ResidualAssessmentPrice);
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
