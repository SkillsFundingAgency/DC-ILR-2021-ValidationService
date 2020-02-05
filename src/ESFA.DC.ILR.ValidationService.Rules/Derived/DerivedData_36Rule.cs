using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_36Rule : IDerivedData_36Rule
    {
        public DateTime? DeriveEffectiveEndDate(ILearningDelivery learningDelivery)
        {
            if (ProgAimCondition(learningDelivery.AimType))
            {
                if (!ProgTypeCondition(learningDelivery.ProgTypeNullable))
                {
                    return learningDelivery.LearnActEndDateNullable;
                }

                if (ProgTypeCondition(learningDelivery.ProgTypeNullable) && !AchDateKNown(learningDelivery.AchDateNullable))
                {
                    return learningDelivery.LearnActEndDateNullable;
                }

                if (ProgTypeCondition(learningDelivery.ProgTypeNullable) && AchDateKNown(learningDelivery.AchDateNullable))
                {
                    return learningDelivery.AchDateNullable;
                }
            }

            return null;
        }

        private bool ProgAimCondition(int aimType) => aimType == TypeOfAim.ProgrammeAim;
        private bool ProgTypeCondition(int? progType) => progType == TypeOfLearningProgramme.ApprenticeshipStandard;
        private bool AchDateKNown(DateTime? achDate) => achDate.HasValue;
    }
}
