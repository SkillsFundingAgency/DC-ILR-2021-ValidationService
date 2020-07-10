using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_CategoryRef : IDerivedData_CategoryRef
    {
        private readonly IDerivedData_35Rule _dd35;

        public DerivedData_CategoryRef(IDerivedData_35Rule dd35)
        {
            _dd35 = dd35;
        }

        public int? Derive(ILearningDelivery learningDelivery)
        {
            if (McaAdultSkillsMatch(learningDelivery))
            {
                return LARSConstants.Categories.McaGlaAim;
            }

            return null;
        }

        public bool McaAdultSkillsMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == FundModels.AdultSkills
                && HasDD35(learningDelivery);
        }

        private bool HasDD35(ILearningDelivery learningDelivery)
        {
            return _dd35.IsCombinedAuthorities(learningDelivery);
        }
    }
}
