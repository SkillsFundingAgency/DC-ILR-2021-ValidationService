using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus
{
    public class CompStatus_02Rule : AbstractRule, IRule<ILearner>
    {
        public CompStatus_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.CompStatus_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                             learningDelivery.LearnActEndDateNullable, 
                             learningDelivery.CompStatus,
                             learningDelivery.FundModel, 
                             learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(
                                        objectToValidate.LearnRefNumber, 
                                        learningDelivery.AimSeqNumber, 
                                        BuildErrorMessageParameters(
                                                 learningDelivery.CompStatus, 
                                                 learningDelivery.LearnActEndDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? learnActEndDate, int compStatus, int fundModel, int? progType)
        {
            return learnActEndDate.HasValue
                && compStatus == CompletionState.IsOngoing
                && FundModelConditionMet(fundModel)
                && ProgTypeConditionMet(progType);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel != TypeOfFunding.ApprenticeshipsFrom1May2017;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType != TypeOfLearningProgramme.ApprenticeshipStandard;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int compStatus, DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
            };
        }
    }
}
