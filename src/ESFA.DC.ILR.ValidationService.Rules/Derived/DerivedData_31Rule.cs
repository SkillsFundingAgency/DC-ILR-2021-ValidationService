using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_31Rule : IDerivedData_31Rule
    {
        private readonly IEnumerable<int> _englishOrMathsBasicSkillsTypes = new HashSet<int>(TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills);

        private readonly ILARSDataService _larsDataService;

        public DerivedData_31Rule(ILARSDataService larsDataService)
        {
            _larsDataService = larsDataService;
        }

        public bool IsAdultSkillsFundedEnglishOrMathsAim(ILearningDelivery learningDelivery)
        {
            return FundModelConditionMet(learningDelivery.FundModel)
                && BasicSkillsConditionMet(learningDelivery.LearnAimRef, learningDelivery.LearnStartDate);
        }

        public bool FundModelConditionMet(int fundModel) => fundModel == FundModels.AdultSkills;

        public bool BasicSkillsConditionMet(string learnAimRef, DateTime learnStartDate) => _larsDataService.BasicSkillsMatchForLearnAimRefAndStartDate(_englishOrMathsBasicSkillsTypes, learnAimRef, learnStartDate);
    }
}
