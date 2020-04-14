using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_74Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModelsAppsAndEsf = new HashSet<int>
        {
            TypeOfFunding.ApprenticeshipsFrom1May2017,
            TypeOfFunding.EuropeanSocialFund,
            TypeOfFunding.OtherAdult
        };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnDelFAMType_74Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_74)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public static DateTime LastInviableDate => new DateTime(2019, 07, 31);

        public bool HasQualifyingStart(DateTime learnStartDate) =>
            learnStartDate > LastInviableDate;

        public bool HasDisqualifyingMonitor(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs) =>
            !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFAMs,
                Monitoring.Delivery.Types.SourceOfFunding,
                LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult);

        public bool HasQualifyingFunding(int fundModel) =>
            _fundModelsAppsAndEsf.Contains(fundModel);

        public bool HasTraineeshipFunding(int fundModel, int? progType) =>
            fundModel == TypeOfFunding.AdultSkills
            && progType.HasValue;

        public bool ConditionMet(ILearningDelivery theDelivery) =>
            HasQualifyingStart(theDelivery.LearnStartDate)
            && (HasQualifyingFunding(theDelivery.FundModel) || HasTraineeshipFunding(theDelivery.FundModel, theDelivery.ProgTypeNullable))
            && HasDisqualifyingMonitor(theDelivery.LearningDeliveryFAMs);

        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(ConditionMet, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery) =>
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildErrorMessageParameters(thisDelivery.FundModel));

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.SourceOfFunding),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult)
        };
    }
}
