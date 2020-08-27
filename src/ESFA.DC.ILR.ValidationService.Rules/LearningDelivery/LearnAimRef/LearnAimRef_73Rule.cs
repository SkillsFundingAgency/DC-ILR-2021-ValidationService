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

        public void Validate(ILearner thisLearner)
        {
            var learnRefNumber = thisLearner.LearnRefNumber;

            thisLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public bool IsNotValid(ILearningDelivery thisDelivery)
        {
            var esfEligibilities = _fcsData.GetEligibilityRuleSectorSubjectAreaLevelsFor(thisDelivery.ConRefNumber).ToReadOnlyCollection();
            var larsLearningDelivery = _larsData.GetDeliveryFor(thisDelivery.LearnAimRef);

            if (esfEligibilities.Any() && larsLearningDelivery != null)
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

        public bool HasDisqualifyingSubjectSector(ILARSLearningDelivery larsDelivery, IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> subjectAreaLevels)
        {
            if (!subjectAreaLevels.Any(IsUsableSubjectArea))
            {
                return false;
            }

            var filteredAreaLevels = subjectAreaLevels.Where(x => SubjectAreaTierFilter(x, larsDelivery));

            return larsDelivery == null
                   || (!filteredAreaLevels.Any() || filteredAreaLevels.Any(x => HasDisqualifyingSubjectSector(x, larsDelivery)));
        }

        public bool SubjectAreaTierFilter(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, ILARSLearningDelivery larsDelivery)
        {
            return subjectAreaLevel.SectorSubjectAreaCode == larsDelivery.SectorSubjectAreaTier1
                   || subjectAreaLevel.SectorSubjectAreaCode == larsDelivery.SectorSubjectAreaTier2;
        }

        public bool HasDisqualifyingSubjectSector(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, ILARSLearningDelivery larsDelivery)
        {
            return IsUsableSubjectArea(subjectAreaLevel)
                   && IsDisqualifyingSubjectAreaLevel(subjectAreaLevel, GetNotionalNVQLevelV2(larsDelivery));
        }

        public bool IsUsableSubjectArea(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel)
        {
            return subjectAreaLevel?.SectorSubjectAreaCode != null
                   && (!string.IsNullOrWhiteSpace(subjectAreaLevel.MinLevelCode)
                       || !string.IsNullOrWhiteSpace(subjectAreaLevel.MaxLevelCode));
        }

        public bool IsDisqualifyingSubjectAreaLevel(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, double notionalNVQLevel2)
        {
            return notionalNVQLevel2 != LARSConstants.NotionalNVQLevelV2Doubles.OutOfScope
                   && (notionalNVQLevel2 < subjectAreaLevel.MinLevelCode.AsNotionalNVQLevelV2()
                       || notionalNVQLevel2 > subjectAreaLevel.MaxLevelCode.AsNotionalNVQLevelV2());
        }

        public double GetNotionalNVQLevelV2(ILARSLearningDelivery larsDelivery)
        {
            return larsDelivery.NotionalNVQLevelv2.AsNotionalNVQLevelV2();
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