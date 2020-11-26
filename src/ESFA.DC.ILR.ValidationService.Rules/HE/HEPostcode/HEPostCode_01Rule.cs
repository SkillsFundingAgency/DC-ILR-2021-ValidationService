using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.HEPostcode
{
    public class HEPostCode_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPostcodesDataService _postcodesDataService;

        public HEPostCode_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IPostcodesDataService postcodesDataService)
            : base(validationErrorHandler, RuleNameConstants.HEPostCode_01)
        {
            _postcodesDataService = postcodesDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.HEPostCode));
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryHE learningDeliveryHEEntity)
        {
            return NullConditionMet(learningDeliveryHEEntity)
                   && TemporaryPostcodeConditionMet(learningDeliveryHEEntity.HEPostCode)
                   && PostcodeConditionMet(learningDeliveryHEEntity.HEPostCode);
        }

        public bool NullConditionMet(ILearningDeliveryHE learningDeliveryHEEntity)
        {
            return learningDeliveryHEEntity != null
                && !string.IsNullOrWhiteSpace(learningDeliveryHEEntity.HEPostCode);
        }

        public bool TemporaryPostcodeConditionMet(string postcode)
        {
            return postcode != ValidationConstants.TemporaryPostCode;
        }

        public bool PostcodeConditionMet(string postcode)
        {
            return !_postcodesDataService.PostcodeExists(postcode);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string postcode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.HEPostcode, postcode),
            };
        }
    }
}
