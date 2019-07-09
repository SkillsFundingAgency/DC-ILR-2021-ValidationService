using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Linq;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus
{
    public class CompStatus_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2019 = new DateTime(2019, 08, 01);

        public CompStatus_07Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.CompStatus_07)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AimType, learningDelivery.FundModel,
                                    learningDelivery.ProgTypeNullable, learningDelivery.CompStatus,
                                    learningDelivery.LearnActEndDateNullable, learningDelivery.AchDateNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber,
                                            BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.ProgTypeNullable,
                                                                        learningDelivery.CompStatus, learningDelivery.LearnStartDate, learningDelivery.AchDateNullable));
                }
            }
        }

        public bool ConditionMet(int aimType, int fundModel, int? progType, int compStatus, DateTime? learnActEndDate, DateTime? achDate)
        {
            return AimTypeConditionMet(aimType)
                  && FundModelConditionMet(fundModel)
                  && ProgTypeConditionMet(progType)
                  && CompStatusConditionMet(compStatus)
                  && LearnActEndDateConditionMet(learnActEndDate)
                  && AchDateConditionMet(achDate); ;
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == TypeOfAim.ProgrammeAim;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType.HasValue
                    && progType == TypeOfLearningProgramme.ApprenticeshipStandard;
        }

        public bool LearnActEndDateConditionMet(DateTime? learnActEndDate)
        {
            return learnActEndDate.HasValue && learnActEndDate >= _firstAugust2019;
        }

        public bool AchDateConditionMet(DateTime? achDate)
        {
            return achDate.HasValue;
        }

        public bool CompStatusConditionMet(int compStatus)
        {
            return compStatus != CompletionState.HasCompleted;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int? progType, int compStatus, DateTime learnStartDate, DateTime? achDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate),
            };
        }
    }
}
