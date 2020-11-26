using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_09Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int>
        {
            FundModels.CommunityLearning,
            FundModels.AdultSkills,
            FundModels.ApprenticeshipsFrom1May2017,
            FundModels.EuropeanSocialFund,
            FundModels.OtherAdult
        };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDerivedData_35Rule _derivedData35;

        public LearnDelFAMType_09Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IDerivedData_35Rule derivedData35)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_09)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _derivedData35 = derivedData35;
        }

        public bool HasDisqualifyingMonitor(ILearningDelivery theDelivery) =>
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(theDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF)
            && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                theDelivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.SOF,
                LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult);

        public bool HasQualifyingFunding(ILearningDelivery theDelivery) =>
            _fundModels.Contains(theDelivery.FundModel);

        public bool IsNotValid(ILearningDelivery theDelivery) =>
            !_derivedData35.IsCombinedAuthorities(theDelivery)
            && HasQualifyingFunding(theDelivery)
            && HasDisqualifyingMonitor(theDelivery);

        public void Validate(ILearner theLearner)
        {
            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.SourceOfFunding),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult)
        };
    }
}
