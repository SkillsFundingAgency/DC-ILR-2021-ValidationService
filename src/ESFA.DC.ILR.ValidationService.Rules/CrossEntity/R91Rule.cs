using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R91Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundModel = FundModels.EuropeanSocialFund;
        private readonly string _learnAimReference = AimTypes.References.ESFLearnerStartandAssessment;
        private readonly int _completionState = CompletionState.HasCompleted;

        public R91Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R91)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            bool isCompletedLearnAimRefFound = false;
            bool isFundModelCondition = false;
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (FundModelConditionMet(learningDelivery.FundModel))
                {
                    isFundModelCondition = true;
                    if (LearnAimRefConditionMet(learningDelivery.LearnAimRef)
                        && CompStatusConditionMet(learningDelivery.CompStatus))
                    {
                        isCompletedLearnAimRefFound = true;
                        break;
                    }
                }
            }

            if (isFundModelCondition
                && !isCompletedLearnAimRefFound)
            {
                HandleValidationError(
                                learnRefNumber: objectToValidate.LearnRefNumber,
                                errorMessageParameters: BuildErrorMessageParameters(
                                _fundModel,
                                _learnAimReference,
                                _completionState));
            }
        }

        public bool FundModelConditionMet(int fundModel) => _fundModel == fundModel;

        public bool LearnAimRefConditionMet(string learnAimRef) => learnAimRef.CaseInsensitiveEquals(_learnAimReference);

        public bool CompStatusConditionMet(int compStatus) => compStatus == _completionState;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learnAimRef, int compStatus)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus)
            };
        }
    }
}
