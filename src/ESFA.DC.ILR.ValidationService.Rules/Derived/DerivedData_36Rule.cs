using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_36Rule : IDerivedData_36Rule
    {
        private readonly DateTime DefaultDate = new DateTime(2099, 12, 31, 23, 59, 59);

        public DateTime? DeriveEffectiveEndDate(ILearningDelivery learningDelivery)
        {
            if (ProgAimCondition(learningDelivery.AimType))
            {
                if (!ProgTypeCondition(learningDelivery.ProgTypeNullable) && LearnActEndDateKnown(learningDelivery.LearnActEndDateNullable))
                {
                    return learningDelivery.LearnActEndDateNullable;
                }

                if (ProgTypeCondition(learningDelivery.ProgTypeNullable) && !AchDateKNown(learningDelivery.AchDateNullable) && LearnActEndDateKnown(learningDelivery.LearnActEndDateNullable))
                {
                    return learningDelivery.LearnActEndDateNullable;
                }

                if (ProgTypeCondition(learningDelivery.ProgTypeNullable) && AchDateKNown(learningDelivery.AchDateNullable))
                {
                    return learningDelivery.AchDateNullable;
                }

                return DefaultDate;
            }

            return null;
        }

        private bool ProgAimCondition(int aimType) => aimType == TypeOfAim.ProgrammeAim;

        private bool ProgTypeCondition(int? progType) => progType == TypeOfLearningProgramme.ApprenticeshipStandard;

        private bool AchDateKNown(DateTime? achDate) => achDate.HasValue;

        private bool LearnActEndDateKnown(DateTime? learnActEndDate) => learnActEndDate.HasValue;
    }
}
