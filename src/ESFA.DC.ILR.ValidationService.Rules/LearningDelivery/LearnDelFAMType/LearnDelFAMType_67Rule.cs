using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_67Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _basicSkills = new HashSet<int>()
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
            TypeOfLARSBasicSkill.FreeStandingMathematicsQualification
        };

        private readonly ILARSDataService _larsDataService;

        public LearnDelFAMType_67Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_67)
        {
            _larsDataService = larsDataService;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.FundModel != TypeOfFunding.ApprenticeshipsFrom1May2017
                    || learningDelivery.AimType != TypeOfAim.ComponentAimInAProgramme
                    || learningDelivery.LearningDeliveryFAMs == null)
                {
                    continue;
                }

                var basicSkill = _larsDataService
                                    .BasicSkillsMatchForLearnAimRefAndStartDate(
                                                   _basicSkills, 
                                                   learningDelivery.LearnAimRef, 
                                                   learningDelivery.LearnStartDate);

                var larsDelivery = _larsDataService.GetDeliveryFor(learningDelivery.LearnAimRef);

                foreach (var deliveryFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if ((!basicSkill || IsCommonComponent(larsDelivery)) && deliveryFam.LearnDelFAMType == LearningDeliveryFAMTypeConstants.LSF)
                    {
                        RaiseValidationMessage(learner.LearnRefNumber, learningDelivery, deliveryFam);
                    }
                }
            }
        }

        public bool IsCommonComponent(ILARSLearningDelivery lARSLearningDelivery)
        {
            return lARSLearningDelivery.FrameworkCommonComponent == TypeOfLARSCommonComponent.BritishSignLanguage;
        }

        private void RaiseValidationMessage(string learnRefNum, ILearningDelivery learningDelivery, ILearningDeliveryFAM thisMonitor)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, thisMonitor.LearnDelFAMType)
            };

            HandleValidationError(learnRefNum, learningDelivery.AimSeqNumber, parameters);
        }
    }
}
