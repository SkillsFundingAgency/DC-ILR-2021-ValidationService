using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade
{
    public class EngGrade_01Rule : AbstractRule, IRule<ILearner>
    {
        private int[] _fundModels = new[] { TypeOfFunding.Age16To19ExcludingApprenticeships };

        public EngGrade_01Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EngGrade_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (EngGradeConditionMet(objectToValidate.EngGrade))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (FundModelConditionMet(learningDelivery.FundModel))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(learningDelivery.FundModel));
                        return;
                    }
                }
            }
        }

        public bool EngGradeConditionMet(string engGrade)
        {
            return string.IsNullOrWhiteSpace(engGrade);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
            };
        }
    }
}
