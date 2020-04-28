using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_85Rule : IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnAimRef_85;
        private readonly HashSet<int> _attainmentLevels = new HashSet<int>
        {
            TypeOfPriorAttainment.FullLevel3,
            TypeOfPriorAttainment.Level4Expired20130731,
            TypeOfPriorAttainment.Level5AndAboveExpired20130731,
            TypeOfPriorAttainment.Level4,
            TypeOfPriorAttainment.Level5,
            TypeOfPriorAttainment.Level6,
            TypeOfPriorAttainment.Level7AndAbove,
            TypeOfPriorAttainment.NotKnown,
            TypeOfPriorAttainment.OtherLevelNotKnown
        };

        private readonly IValidationErrorHandler _messageHandler;
        private readonly ILARSDataService _larsData;
        private readonly IProvideRuleCommonOperations _check;
        private readonly IDerivedData_07Rule _dd07;

        public LearnAimRef_85Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IProvideRuleCommonOperations commonChecks,
            IDerivedData_07Rule dd07)
        {
            _messageHandler = validationErrorHandler;
            _larsData = larsData;
            _check = commonChecks;
            _dd07 = dd07;
        }

        public static DateTime FirstViableDate => new DateTime(2017, 08, 01);

        public string RuleName => Name;

        public bool IsDisqualifyingNotionalNVQ(ILARSLearningDelivery delivery) =>
            delivery != null
            && delivery.NotionalNVQLevelv2.CaseInsensitiveEquals(LARSNotionalNVQLevelV2.Level3);

        public bool HasDisqualifyingNotionalNVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return IsDisqualifyingNotionalNVQ(larsDelivery);
        }

        public bool PassesRestrictions(ILearningDelivery delivery) =>
            _check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
            && _check.HasQualifyingStart(delivery, FirstViableDate);

        public bool IsExcluded(ILearningDelivery delivery) =>
            _check.IsRestart(delivery)
            || _dd07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsExcluded(delivery)
            && PassesRestrictions(delivery)
            && HasDisqualifyingNotionalNVQ(delivery);

        public bool HasQualifyingAttainment(ILearner learner) =>
            learner.PriorAttainNullable.HasValue
            && _attainmentLevels.Contains(learner.PriorAttainNullable.Value);

        public void Validate(ILearner objectToValidate)
        {
            if (HasQualifyingAttainment(objectToValidate))
            {
                objectToValidate.LearningDeliveries
                    .NullSafeWhere(IsNotValid)
                    .ForEach(x => RaiseValidationMessage(objectToValidate, x));
            }
        }

        public void RaiseValidationMessage(ILearner learner, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, learner.PriorAttainNullable),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.FundModel), thisDelivery.FundModel)
            };

            _messageHandler.Handle(RuleName, learner.LearnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
