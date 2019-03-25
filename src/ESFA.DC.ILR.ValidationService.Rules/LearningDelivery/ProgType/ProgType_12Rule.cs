using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    public class ProgType_12Rule : AbstractRule, IRule<ILearner>
    {
        private const int FworkCode = 445;
        private const int PWayCode = 1;
        private readonly IDerivedData_04Rule _dd04;
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly IEnumerable<int> _fundModels = new HashSet<int>
        {
            TypeOfFunding.AdultSkills,
            TypeOfFunding.ApprenticeshipsFrom1May2017
        };

        private readonly HashSet<int?> _componentTypes = new HashSet<int?> { 1, 2, 3 };
        private readonly IEnumerable<int> _basicSkillTypes = new HashSet<int>
        {
            TypeOfLARSBasicSkill.NotApplicable,
            TypeOfLARSBasicSkill.Unknown,
            TypeOfLARSBasicSkill.GCSE_EnglishLanguage,
            TypeOfLARSBasicSkill.GCSE_Mathematics,
            TypeOfLARSBasicSkill.FunctionalSkillsMathematics,
            TypeOfLARSBasicSkill.FunctionalSkillsEnglish,
            TypeOfLARSBasicSkill.InternationalGCSEEnglishLanguage,
            TypeOfLARSBasicSkill.InternationalGCSEMathematics
        };

        public ProgType_12Rule(
            IDerivedData_04Rule dd04,
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ProgType_12)
        {
            _dd04 = dd04;
            _larsDataService = larsDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public DateTime LastViableStartDate => new DateTime(2014, 09, 01);

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                DateTime? dd04Date = _dd04.GetEarliesStartDateFor(learningDelivery, objectToValidate.LearningDeliveries);

                if (ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.FundModel,
                    dd04Date,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                                        learningDelivery.ProgTypeNullable,
                                        learningDelivery.FworkCodeNullable,
                                        learningDelivery.PwayCodeNullable));
                }
            }
        }

        public bool ConditionMet(
                                string learnAimRef,
                                int fundModel,
                                DateTime? dd04Date,
                                int aimType,
                                int? progType,
                                int? fWorkCode,
                                int? pWayCode,
                                IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !Excluded(learnAimRef)
                       && FundModelConditionMet(fundModel)
                       && DD04ConditionMet(dd04Date)
                       && LearningDeliveryFamConditionMet(learningDeliveryFAMs)
                       && AimTypeConditionMet(aimType)
                       && ProgTypeConditionMet(progType)
                       && FworkCodeConditionMet(fWorkCode)
                       && PwayCodeConditionMet(pWayCode)
                       && BasicSkillsTypeConditionMet(learnAimRef);
        }

        public bool Excluded(string learnAimRef)
        {
            return _larsDataService.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, _componentTypes);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool DD04ConditionMet(DateTime? dd04Date)
        {
            return dd04Date.HasValue && dd04Date >= LastViableStartDate;
        }

        public bool LearningDeliveryFamConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return learningDeliveryFAMs != null && !_learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == TypeOfAim.ComponentAimInAProgramme;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == TypeOfLearningProgramme.AdvancedLevelApprenticeship;
        }

        public bool FworkCodeConditionMet(int? fworkCode)
        {
            return fworkCode == FworkCode;
        }

        public bool PwayCodeConditionMet(int? pwayCode)
        {
            return pwayCode == PWayCode;
        }

        public bool BasicSkillsTypeConditionMet(string learnAimRef)
        {
            return !_larsDataService.BasicSkillsTypeMatchForLearnAimRef(_basicSkillTypes, learnAimRef);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? progType, int? fworkCode, int? pwayCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, fworkCode),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, pwayCode)
            };
        }
    }
}
