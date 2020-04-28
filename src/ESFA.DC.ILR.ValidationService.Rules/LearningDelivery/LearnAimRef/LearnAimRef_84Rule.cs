﻿using System;
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
    public class LearnAimRef_84Rule : IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnAimRef_84;

        private readonly IValidationErrorHandler _messageHandler;
        private readonly ILARSDataService _larsData;
        private readonly IProvideRuleCommonOperations _check;
        private readonly IDerivedData_07Rule _dd07;

        public LearnAimRef_84Rule(
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

        public bool IsQualifyingNotionalNVQ(ILARSLearningDelivery delivery) =>
            delivery != null
            && delivery.NotionalNVQLevelv2.CaseInsensitiveEquals(LARSNotionalNVQLevelV2.Level3);

        public bool HasQualifyingNotionalNVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return IsQualifyingNotionalNVQ(larsDelivery);
        }

        public bool IsQualifyingCategory(ILARSLearningCategory category) =>
            category.CategoryRef == TypeOfLARSCategory.OnlyForLegalEntitlementAtLevel3;

        public bool HasQualifyingCategory(ILearningDelivery delivery)
        {
            var categories = _larsData.GetCategoriesFor(delivery.LearnAimRef);

            return categories.NullSafeAny(IsQualifyingCategory);
        }

        public bool PassesRestrictions(ILearningDelivery delivery) =>
            _check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
                && _check.HasQualifyingStart(delivery, FirstViableDate)
                && HasQualifyingNotionalNVQ(delivery);

        public bool IsExcluded(ILearningDelivery delivery) =>
            _check.IsRestart(delivery)
            || _check.IsLearnerInCustody(delivery)
            || _check.IsSteelWorkerRedundancyTraining(delivery)
            || _dd07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsExcluded(delivery)
            && PassesRestrictions(delivery)
            && !HasQualifyingCategory(delivery);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.FundModel), thisDelivery.FundModel)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
