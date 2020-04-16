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
    public class LearnDelFAMType_09Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _check;

        public LearnDelFAMType_09Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_09)
        {
            _check = commonOperations;
        }

        public static DateTime FirstInviableDate => new DateTime(2019, 08, 01);

        public bool HasQualifyingStart(ILearningDelivery theDelivery) =>
            theDelivery.LearnStartDate < FirstInviableDate;

        public bool HasDisqualifyingMonitor(ILearningDeliveryFAM theMonitor) =>
            theMonitor.LearnDelFAMType.CaseInsensitiveEquals(Monitoring.Delivery.Types.SourceOfFunding)
            && !Monitoring.Delivery.ESFAAdultFunding.CaseInsensitiveEquals($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}");

        public bool HasDisqualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, HasDisqualifyingMonitor);

        public bool HasQualifyingFunding(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(
                theDelivery,
                TypeOfFunding.CommunityLearning,
                TypeOfFunding.AdultSkills,
                TypeOfFunding.ApprenticeshipsFrom1May2017,
                TypeOfFunding.EuropeanSocialFund,
                TypeOfFunding.OtherAdult);

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
