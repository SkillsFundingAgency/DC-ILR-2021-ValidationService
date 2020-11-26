using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_55Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<string> _workExperienceLearnAimRefs = new HashSet<string>
        {
            "Z0007834",
            "Z0007835",
            "Z0007836",
            "Z0007837",
            "Z0007838",
            "ZWRKX001",
        }.ToCaseInsensitiveHashSet();

        public LearnAimRef_55Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_55)
        {
        }

        public LearnAimRef_55Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AimType, learningDelivery.FundModel, learningDelivery.ProgTypeNullable, learningDelivery.LearnAimRef))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnAimRef, learningDelivery.AimType));
                }
            }
        }

        public bool ConditionMet(int aimType, int fundModel, int? progType, string learnAimRef)
        {
            return AimTypeConditionMet(aimType)
                   && TraineeshipConditionMet(fundModel, progType)
                   && WorkExperienceConditionMet(learnAimRef);
        }

        public virtual bool WorkExperienceConditionMet(string learnAimRef)
        {
            return _workExperienceLearnAimRefs.Contains(learnAimRef);
        }

        public virtual bool TraineeshipConditionMet(int fundModel, int? progType)
        {
            return fundModel == FundModels.Age16To19ExcludingApprenticeships && progType == ProgTypes.Traineeship;
        }

        public virtual bool AimTypeConditionMet(int aimType)
        {
            return aimType != 5;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, int aimType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType)
            };
        }
    }
}
