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
    public class LearnAimRef_72Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fcsData;
        private readonly ILARSDataService _larsData;

        public LearnAimRef_72Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fcsDataService,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_72)
        {
            _fcsData = fcsDataService;
            _larsData = larsDataService;
        }

        public bool HasDisqualifyingSubjectSector(ILARSLearningDelivery larsDelivery, IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> subjectAreaLevels) =>
            larsDelivery == null
            || subjectAreaLevels.Any(x => HasDisqualifyingSubjectSector(x, larsDelivery));

        public bool HasDisqualifyingSubjectSector(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, ILARSLearningDelivery larsDelivery) =>
            IsUsableSubjectArea(subjectAreaLevel)
            && IsDisqualifyingSubjectAreaLevel(subjectAreaLevel, GetNotionalNVQLevelV2(larsDelivery));

        public bool IsUsableSubjectArea(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel) =>
            subjectAreaLevel != null
            && !subjectAreaLevel.SectorSubjectAreaCode.HasValue
            && (!string.IsNullOrWhiteSpace(subjectAreaLevel.MinLevelCode)
                || !string.IsNullOrWhiteSpace(subjectAreaLevel.MaxLevelCode));

        public double GetNotionalNVQLevelV2(ILARSLearningDelivery larsDelivery) =>
            larsDelivery.NotionalNVQLevelv2.AsNotionalNVQLevelV2();

        public bool IsDisqualifyingSubjectAreaLevel(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, double notionalNVQLevel2) =>
            !IsOutOfScope(notionalNVQLevel2)
            && (HasDisqualifyingMinimumLevel(subjectAreaLevel, notionalNVQLevel2)
                || HasDisqualifyingMaximumLevel(subjectAreaLevel, notionalNVQLevel2));

        public bool IsOutOfScope(double notionalNVQLevel2) =>
            notionalNVQLevel2 == TypeOfNotionalNVQLevelV2.OutOfScope;

        public bool HasDisqualifyingMinimumLevel(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, double notionalNVQLevel2) =>
            notionalNVQLevel2 < subjectAreaLevel.MinLevelCode.AsNotionalNVQLevelV2();

        public bool HasDisqualifyingMaximumLevel(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, double notionalNVQLevel2) =>
            notionalNVQLevel2 > subjectAreaLevel.MaxLevelCode.AsNotionalNVQLevelV2();

        public ILARSLearningDelivery GetLARSLearningDeliveryFor(ILearningDelivery thisDelivery) =>
            _larsData.GetDeliveryFor(thisDelivery.LearnAimRef);

        public IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> GetSubjectAreaLevelsFor(ILearningDelivery thisDelivery) =>
            _fcsData.GetEligibilityRuleSectorSubjectAreaLevelsFor(thisDelivery.ConRefNumber).ToReadOnlyCollection();

        public bool IsExcluded(ILearningDelivery thisDelivery) =>
            thisDelivery.LearnAimRef.CaseInsensitiveEquals(TypeOfAim.References.ESFLearnerStartandAssessment);

        public bool IsNotValid(ILearningDelivery thisDelivery) =>
            !IsExcluded(thisDelivery)
                && thisDelivery.FundModel == FundModels.EuropeanSocialFund
                && HasDisqualifyingSubjectSector(GetLARSLearningDeliveryFor(thisDelivery), GetSubjectAreaLevelsFor(thisDelivery));

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
