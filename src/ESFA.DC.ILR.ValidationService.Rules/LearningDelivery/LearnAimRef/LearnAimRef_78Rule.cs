﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_78Rule :
        IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnAimRef_78;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly ILARSDataService _larsData;

        private readonly IProvideRuleCommonOperations _check;

        private readonly IFileDataService _fileData;

        private readonly IOrganisationDataService _organisationData;

        public LearnAimRef_78Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IProvideRuleCommonOperations commonChecks,
            IFileDataService fileData,
            IOrganisationDataService organisationData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));
            It.IsNull(commonChecks)
                .AsGuard<ArgumentNullException>(nameof(commonChecks));
            It.IsNull(fileData)
                .AsGuard<ArgumentNullException>(nameof(fileData));
            It.IsNull(organisationData)
                .AsGuard<ArgumentNullException>(nameof(organisationData));

            _messageHandler = validationErrorHandler;
            _larsData = larsData;
            _check = commonChecks;
            _fileData = fileData;
            _organisationData = organisationData;
        }

        public static DateTime FirstViableDate => new DateTime(2016, 08, 01);

        public static DateTime LastViableDate => new DateTime(2017, 07, 31);

        public string RuleName => Name;

        public bool IsSpecialistDesignatedCollege()
        {
            var ukprn = _fileData.UKPRN();
            return _organisationData.LegalOrgTypeMatchForUkprn(ukprn, "USDC");
        }

        public bool IsQualifyingNotionalNVQ(ILARSLearningDelivery delivery) =>
            It.IsInRange(delivery?.NotionalNVQLevelv2, LARSNotionalNVQLevelV2.Level3);

        public bool HasQualifyingNotionalNVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return IsQualifyingNotionalNVQ(larsDelivery);
        }

        public bool IsQualifyingCategory(ILARSLearningCategory category) =>
            It.IsInRange(category.CategoryRef, TypeOfLARSCategory.OnlyForLegalEntitlementAtLevel3);

        public bool HasQualifyingCategory(ILearningDelivery delivery)
        {
            var categories = _larsData.GetCategoriesFor(delivery.LearnAimRef);

            return categories.NullSafeAny(IsQualifyingCategory);
        }

        public bool PassesRestrictions(ILearningDelivery delivery) =>
            _check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
            && _check.HasQualifyingStart(delivery, FirstViableDate, LastViableDate)
            && HasQualifyingNotionalNVQ(delivery);

        public bool IsExcluded(ILearningDelivery delivery) =>
            _check.IsRestart(delivery)
            || _check.IsLearnerInCustody(delivery)
            || _check.IsSteelWorkerRedundancyTraining(delivery)
            || _check.InApprenticeship(delivery)
            || IsSpecialistDesignatedCollege();

        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsExcluded(delivery)
            && PassesRestrictions(delivery)
            && !HasQualifyingCategory(delivery);

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
