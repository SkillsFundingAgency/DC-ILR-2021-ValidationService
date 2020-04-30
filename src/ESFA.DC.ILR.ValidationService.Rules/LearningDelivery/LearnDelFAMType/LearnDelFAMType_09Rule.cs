using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
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

        public LearnDelFAMType_09Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_09)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public static DateTime FirstInviableDate => new DateTime(2019, 08, 01);

        public bool HasQualifyingStart(ILearningDelivery theDelivery) =>
            theDelivery.LearnStartDate < FirstInviableDate;

        public bool HasDisqualifyingMonitor(ILearningDelivery theDelivery) =>
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(theDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF)
            && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                theDelivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.SOF,
                LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult);

        public bool HasQualifyingFunding(ILearningDelivery theDelivery) =>
            _fundModels.Contains(theDelivery.FundModel);

        public bool IsNotValid(ILearningDelivery theDelivery) =>
            HasQualifyingStart(theDelivery)
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
