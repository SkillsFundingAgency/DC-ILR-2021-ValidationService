using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_71Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundModel = TypeOfFunding.EuropeanSocialFund;

        private readonly IFCSDataService _fCSDataService;
        private readonly ILARSDataService _lARSDataService;

        public LearnAimRef_71Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fCSDataService,
            ILARSDataService lARSDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_71)
        {
            _fCSDataService = fCSDataService;
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
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.ConRefNumber, learningDelivery.LearnAimRef))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.LearnAimRef));
                }
            }
        }

        public bool ConditionMet(int fundModel, string conRefNumber, string learnAimRef)
        {
            return FundModelConditionMet(fundModel)
                && EsfSectorSubjectAreaLevelConditionMet(conRefNumber)
                && LARSConditionMet(conRefNumber, learnAimRef)
                && LearnAimRefConditionMet(learnAimRef);
        }

        public bool LearnAimRefConditionMet(string learnAimRef) => !learnAimRef.CaseInsensitiveEquals(ValidationConstants.ZESF0001);

        public bool LARSConditionMet(string conRefNumber, string learnAimRef)
        {
            List<decimal?> sectorSubjectAreaCodes =
                _fCSDataService.GetEligibilityRuleSectorSubjectAreaLevelsFor(conRefNumber)?
                .Where(s => s.SectorSubjectAreaCode.HasValue)
                .Select(s => s.SectorSubjectAreaCode).ToList();

            var learningDelivery = _lARSDataService.GetDeliveryFor(learnAimRef);
            bool isMatchNotFoundForSectorSubjectAreaTier1 = !sectorSubjectAreaCodes.Contains(learningDelivery.SectorSubjectAreaTier1);
            bool isMatchNotFoundForSectorSubjectAreaTier2 = !sectorSubjectAreaCodes.Contains(learningDelivery.SectorSubjectAreaTier2);

            return isMatchNotFoundForSectorSubjectAreaTier1 && isMatchNotFoundForSectorSubjectAreaTier2;
        }

        public bool EsfSectorSubjectAreaLevelConditionMet(string conRefNumber)
        {
            var subjectAreaLevels = _fCSDataService.GetEligibilityRuleSectorSubjectAreaLevelsFor(conRefNumber);

            return subjectAreaLevels.Any(
                s => s.SectorSubjectAreaCode.HasValue
                && string.IsNullOrEmpty(s.MinLevelCode)
                && string.IsNullOrEmpty(s.MaxLevelCode));
        }

        public bool FundModelConditionMet(int fundModel) => fundModel == _fundModel;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learnAimRef)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef)
            };
        }
    }
}
