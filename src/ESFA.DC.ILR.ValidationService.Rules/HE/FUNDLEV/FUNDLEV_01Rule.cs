using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.FUNDLEV
{
    public class FUNDLEV_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public FUNDLEV_01Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FUNDLEV_01)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryHEEntity != null && ConditionMet(learningDelivery.LearningDeliveryHEEntity.FUNDLEV))
                {
                    HandleValidationError(
                        learnRefNumber: objectToValidate.LearnRefNumber,
                        aimSequenceNumber: learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.FUNDLEV));
                }
            }
        }

        public bool ConditionMet(int fundlevValue)
        {
            return !_provideLookupDetails.Contains(TypeOfIntegerCodedLookup.FundLev, fundlevValue);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundlevValue)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FUNDLEV, fundlevValue)
            };
        }
    }
}