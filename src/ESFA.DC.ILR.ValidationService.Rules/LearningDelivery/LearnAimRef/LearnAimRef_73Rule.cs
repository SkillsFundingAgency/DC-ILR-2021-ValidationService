using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_73Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fcsData;
        private readonly ILARSDataService _larsData;

        public LearnAimRef_73Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fcsDataService,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_73)
        {
            _fcsData = fcsDataService;
            _larsData = larsDataService;
        }

        public bool SubjectAreaTierFilter(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, ILARSLearningDelivery larsDelivery) =>
            (subjectAreaLevel.SectorSubjectAreaCode == larsDelivery.SectorSubjectAreaTier1
            || subjectAreaLevel.SectorSubjectAreaCode == larsDelivery.SectorSubjectAreaTier2);

        public bool HasDisqualifyingSubjectSector(ILARSLearningDelivery larsDelivery, IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> subjectAreaLevels) =>
            larsDelivery == null
            || (subjectAreaLevels.Where(x => SubjectAreaTierFilter(x, larsDelivery)).Count() > 0
            ? subjectAreaLevels.Where(x => SubjectAreaTierFilter(x, larsDelivery)).Any(x => HasDisqualifyingSubjectSector(x, larsDelivery))
            : true);

        public bool HasDisqualifyingSubjectSector(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, ILARSLearningDelivery larsDelivery) =>
            IsUsableSubjectArea(subjectAreaLevel)
            && IsDisqualifyingSubjectAreaLevel(subjectAreaLevel, GetNotionalNVQLevelV2(larsDelivery));

        public bool IsUsableSubjectArea(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel) =>
            subjectAreaLevel?.SectorSubjectAreaCode != null
            && (!string.IsNullOrWhiteSpace(subjectAreaLevel.MinLevelCode)
                || !string.IsNullOrWhiteSpace(subjectAreaLevel.MaxLevelCode));

        public double GetNotionalNVQLevelV2(ILARSLearningDelivery larsDelivery) =>
            larsDelivery.NotionalNVQLevelv2.AsNotionalNVQLevelV2();

        public bool IsDisqualifyingSubjectAreaLevel(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, double notionalNVQLevel2) =>
            notionalNVQLevel2 != TypeOfNotionalNVQLevelV2.OutOfScope
            && (notionalNVQLevel2 < subjectAreaLevel.MinLevelCode.AsNotionalNVQLevelV2()
                || notionalNVQLevel2 > subjectAreaLevel.MaxLevelCode.AsNotionalNVQLevelV2());

        public bool IsNotValid(ILearningDelivery thisDelivery)
        {
            var esfEligibilities = _fcsData.GetEligibilityRuleSectorSubjectAreaLevelsFor(thisDelivery.ConRefNumber).ToReadOnlyCollection();
            var larsLearningDelivery = _larsData.GetDeliveryFor(thisDelivery.LearnAimRef);

            if (esfEligibilities.Count > 0 && larsLearningDelivery != null)
            {
                return
                    !thisDelivery.LearnAimRef.CaseInsensitiveEquals(AimTypes.References.ESFLearnerStartandAssessment)
                && FundModelConditionMet(thisDelivery.FundModel)
                && HasDisqualifyingSubjectSector(larsLearningDelivery, esfEligibilities);
            }

            return false;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModels.EuropeanSocialFund;
        }

        public void Validate(ILearner thisLearner)
        {
            var learnRefNumber = thisLearner.LearnRefNumber;

            thisLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, thisDelivery.ConRefNumber)
            };
        }
    }
}