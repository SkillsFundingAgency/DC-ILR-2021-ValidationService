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
    public class LearnAimRef_79Rule : IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnAimRef_79;

        private readonly IValidationErrorHandler _messageHandler;
        private readonly ILARSDataService _larsData;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnAimRef_79Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IDerivedData_07Rule dd07,
            IDateTimeQueryService dateTimeQueryService)
        {
            _messageHandler = validationErrorHandler;
            _larsData = larsData;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dd07 = dd07;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public static DateTime FirstViableDate => new DateTime(2016, 08, 01);

        public string RuleName => Name;

        public bool IsQualifyingNotionalNVQ(ILARSLearningDelivery delivery) =>
            delivery != null
            && !delivery.NotionalNVQLevelv2.CaseInsensitiveEquals(LARSConstants.NotionalNVQLevelV2Strings.Level4);

        public bool HasQualifyingNotionalNVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return !IsQualifyingNotionalNVQ(larsDelivery);
        }

        public bool IsExcluded(ILearningDelivery delivery) =>
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(
                delivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.RES)
            || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                delivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_OLASS)
            || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                delivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy)
            || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                delivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.DAM,
                LearningDeliveryFAMCodeConstants.DAM_DevolvedLevelTwoOrThreeExclusion)
            || _dd07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsExcluded(delivery)
            && delivery.FundModel == FundModels.AdultSkills
            && _dateTimeQueryService.IsDateBetween(delivery.LearnStartDate, FirstViableDate, DateTime.MaxValue)
            && HasQualifyingNotionalNVQ(delivery);

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
