using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_32Rule : IDerivedData_32Rule
    {
        public bool IsOpenApprenticeshipFundedProgramme(ILearningDelivery learningDelivery)
        {
            return AimTypeConditionMet(learningDelivery.AimType)
                   && FundModelConditionMet(learningDelivery.FundModel)
                   && LearnActEndDateConditionMet(learningDelivery.LearnActEndDateNullable);
        }

        public bool AimTypeConditionMet(int aimType) => aimType == TypeOfAim.ProgrammeAim;

        public bool FundModelConditionMet(int fundModel) => fundModel == FundModels.ApprenticeshipsFrom1May2017;

        public bool LearnActEndDateConditionMet(DateTime? learnActEndDate) => !learnActEndDate.HasValue;
    }
}
