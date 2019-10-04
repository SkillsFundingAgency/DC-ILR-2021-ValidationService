using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_73Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IFCSDataService _fcsData;

        private readonly ILARSDataService _larsData;

        private readonly IProvideRuleCommonOperations _check;

        public LearnAimRef_73Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations,
            IFCSDataService fcsDataService,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_73)
        {
            _fcsData = fcsDataService;
            _larsData = larsDataService;
            _check = commonOperations;
        }

        public bool IsDisqualifyingSubjectAreaTier(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, ILARSLearningDelivery larsDelivery) =>
            !(subjectAreaLevel.SectorSubjectAreaCode == larsDelivery.SectorSubjectAreaTier1
            || subjectAreaLevel.SectorSubjectAreaCode == larsDelivery.SectorSubjectAreaTier2);

        public bool HasDisqualifyingSubjectSector(ILARSLearningDelivery larsDelivery, IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> subjectAreaLevels) =>
            It.IsNull(larsDelivery)
            || subjectAreaLevels.Any(x => HasDisqualifyingSubjectSector(x, larsDelivery));

        public bool HasDisqualifyingSubjectSector(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, ILARSLearningDelivery larsDelivery) =>
            IsUsableSubjectArea(subjectAreaLevel)
            && (IsDisqualifyingSubjectAreaLevel(subjectAreaLevel, GetNotionalNVQLevelV2(larsDelivery))
                || IsDisqualifyingSubjectAreaTier(subjectAreaLevel, larsDelivery));

        public bool IsUsableSubjectArea(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel) =>
            It.Has(subjectAreaLevel?.SectorSubjectAreaCode)
            && (It.Has(subjectAreaLevel.MinLevelCode)
                || It.Has(subjectAreaLevel.MaxLevelCode));

        public double GetNotionalNVQLevelV2(ILARSLearningDelivery larsDelivery) =>
            larsDelivery.NotionalNVQLevelv2.AsNotionalNVQLevelV2();

        public bool IsDisqualifyingSubjectAreaLevel(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, double notionalNVQLevel2) =>
            notionalNVQLevel2 != TypeOfNotionalNVQLevelV2.OutOfScope
            && (notionalNVQLevel2 < subjectAreaLevel.MinLevelCode.AsNotionalNVQLevelV2()
                || notionalNVQLevel2 > subjectAreaLevel.MaxLevelCode.AsNotionalNVQLevelV2());

        public bool IsNotValid(ILearningDelivery thisDelivery) =>
            !thisDelivery.LearnAimRef.CaseInsensitiveEquals(TypeOfAim.References.ESFLearnerStartandAssessment)
                && _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.EuropeanSocialFund)
                && HasDisqualifyingSubjectSector(_larsData.GetDeliveryFor(thisDelivery.LearnAimRef), _fcsData.GetEligibilityRuleSectorSubjectAreaLevelsFor(thisDelivery.ConRefNumber).AsSafeReadOnlyList());

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