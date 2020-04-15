﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_79Rule :
        IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnAimRef_79;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly ILARSDataService _larsData;

        private readonly IProvideRuleCommonOperations _check;

        public LearnAimRef_79Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IProvideRuleCommonOperations commonChecks)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));
            It.IsNull(commonChecks)
                .AsGuard<ArgumentNullException>(nameof(commonChecks));

            _messageHandler = validationErrorHandler;
            _larsData = larsData;
            _check = commonChecks;
        }

        public static DateTime FirstViableDate => new DateTime(2016, 08, 01);

        public string RuleName => Name;

        public bool IsQualifyingNotionalNVQ(ILARSLearningDelivery delivery) =>
            delivery != null
            && !delivery.NotionalNVQLevelv2.CaseInsensitiveEquals(LARSNotionalNVQLevelV2.Level4);

        public bool HasQualifyingNotionalNVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return !IsQualifyingNotionalNVQ(larsDelivery);
        }

        public bool IsExcluded(ILearningDelivery delivery) =>
            _check.IsRestart(delivery)
            || _check.IsLearnerInCustody(delivery)
            || _check.IsSteelWorkerRedundancyTraining(delivery)
            || _check.InApprenticeship(delivery);

        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsExcluded(delivery)
            && _check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
            && _check.HasQualifyingStart(delivery, FirstViableDate)
            && HasQualifyingNotionalNVQ(delivery);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

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
