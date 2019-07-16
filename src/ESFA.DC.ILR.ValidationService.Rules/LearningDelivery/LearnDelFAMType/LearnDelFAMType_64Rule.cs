using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_64Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>()
        {
            TypeOfFunding.ApprenticeshipsFrom1May2017
        };

        private readonly IEnumerable<int> _basicSkillsType = new HashSet<int>()
        {
            TypeOfLARSBasicSkill.Certificate_AdultLiteracy,
            TypeOfLARSBasicSkill.Certificate_AdultNumeracy,
            TypeOfLARSBasicSkill.GCSE_EnglishLanguage,
            TypeOfLARSBasicSkill.GCSE_Mathematics,
            TypeOfLARSBasicSkill.KeySkill_Communication,
            TypeOfLARSBasicSkill.KeySkill_ApplicationOfNumbers,
            TypeOfLARSBasicSkill.FunctionalSkillsMathematics,
            TypeOfLARSBasicSkill.FunctionalSkillsEnglish,
            TypeOfLARSBasicSkill.UnitsOfTheCertificate_AdultNumeracy,
            TypeOfLARSBasicSkill.UnitsOfTheCertificate_AdultLiteracy,
            TypeOfLARSBasicSkill.NonNQF_QCFS4LLiteracy,
            TypeOfLARSBasicSkill.NonNQF_QCFS4LNumeracy,            
            TypeOfLARSBasicSkill.QCFBasicSkillsEnglishLanguage,
            TypeOfLARSBasicSkill.QCFBasicSkillsMathematics,
            TypeOfLARSBasicSkill.UnitQCFBasicSkillsEnglishLanguage,
            TypeOfLARSBasicSkill.UnitQCFBasicSkillsMathematics,
            TypeOfLARSBasicSkill.InternationalGCSEEnglishLanguage,
            TypeOfLARSBasicSkill.InternationalGCSEMathematics,
            TypeOfLARSBasicSkill.FreeStandingMathematicsQualification,
        };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly ILARSDataService _lARSDataService;

        public LearnDelFAMType_64Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            ILARSDataService lARSDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_64)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _lARSDataService = lARSDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.AimType,
                    learningDelivery.LearningDeliveryFAMs,
                    learningDelivery.LearnAimRef,
                    learningDelivery.LearnStartDate))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.AimType, learningDelivery.FundModel, LearningDeliveryFAMTypeConstants.ACT));
                }
            }
        }

        public bool ConditionMet(
                              int fundModel,
                              int aimType,
                              IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs,
                              string learnAimRef,
                              DateTime learnStartDate)
        {
            return FundModelConditionMet(fundModel)
                && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)
                && AimTypeConditionMet(aimType, learnAimRef, learnStartDate);
        }      

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT);
        }

        public bool AimTypeConditionMet(int aimType, string learnAimRef, DateTime learnStartDate)
        {
            return aimType == 1
                || (aimType == 3 && LarsConditionMet(learnAimRef, learnStartDate));
        }

        public bool LarsConditionMet(string learnAimRef, DateTime learnStartDate)
        {
            var larsFramework = _lARSDataService.GetDeliveryFor(learnAimRef).Frameworks;

            return _lARSDataService.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsType, learnAimRef, learnStartDate) ||
                   larsFramework.Any(IsCommonComponent);
        }

        public bool IsCommonComponent(ILARSFramework larsFramework)
        {
            return larsFramework
                        .FrameworkCommonComponents
                        .Any(x => x.CommonComponent.Equals(TypeOfLARSCommonComponent.BritishSignLanguage));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, int fundModel, string learnDelFAMType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType)
            };
        }
    }
}
