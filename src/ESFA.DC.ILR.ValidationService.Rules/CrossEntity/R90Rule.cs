using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R90Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;
        private readonly IEnumerable<int> englishOrMathsBasicSkillsTypes = new HashSet<int>(TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills);

        public R90Rule(IValidationErrorHandler validationErrorHandler, ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.R90)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var latestMainAim = objectToValidate.LearningDeliveries.Where(ld =>
                ld.AimType == AimTypes.ProgrammeAim)
                .OrderByDescending(x => x.LearnStartDate)
                .FirstOrDefault();

            if (latestMainAim?.LearnActEndDateNullable != null)
            {
                if (ConditionMet(latestMainAim, objectToValidate.LearningDeliveries))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        latestMainAim.AimSeqNumber,
                        BuildErrorMessageParameters(latestMainAim));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery mainAim, IReadOnlyCollection<ILearningDelivery> componentAims)
        {
            return componentAims.Any(x => x.AimType == AimTypes.ComponentAimInAProgramme &&
                                          !x.LearnActEndDateNullable.HasValue &&
                                          x.ProgTypeNullable == mainAim.ProgTypeNullable &&
                                          x.FworkCodeNullable == mainAim.FworkCodeNullable &&
                                          x.PwayCodeNullable == mainAim.PwayCodeNullable &&
                                          x.StdCodeNullable == mainAim.StdCodeNullable &&
                                          !Excluded(x.LearnAimRef));
        }

        public bool Excluded(string learnAimRef)
        {
            return _larsDataService.BasicSkillsTypeMatchForLearnAimRef(englishOrMathsBasicSkillsTypes, learnAimRef);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learningDelivery.LearnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, learningDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, learningDelivery.FworkCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, learningDelivery.PwayCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, learningDelivery.StdCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable)
            };
        }
    }
}
