﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_10Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _check;

        private readonly IFCSDataService _fcsData;

        private readonly HashSet<string> _fundingStreams = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            FundingStreamPeriodCodeConstants.LEVY1799,
            FundingStreamPeriodCodeConstants.NONLEVY2019
        };

        public UKPRN_10Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IAcademicYearDataService academicYearDataService,
            IProvideRuleCommonOperations commonOps,
            IFCSDataService fcsDataService)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_10)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fileDataService)
                .AsGuard<ArgumentNullException>(nameof(fileDataService));
            It.IsNull(academicYearDataService)
                .AsGuard<ArgumentNullException>(nameof(academicYearDataService));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));
            It.IsNull(fcsDataService)
                .AsGuard<ArgumentNullException>(nameof(fcsDataService));

            FirstViableStart = new DateTime(2017, 05, 01);
            AcademicYearStartDate = academicYearDataService.Start();
            ProviderUKPRN = fileDataService.UKPRN();

            _check = commonOps;
            _fcsData = fcsDataService;
        }

        public DateTime FirstViableStart { get; }

        public DateTime AcademicYearStartDate { get; }

        public int ProviderUKPRN { get; }

        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public bool IsNotValid(ILearningDelivery theDelivery) =>
            !IsExcluded(theDelivery)
                && HasQualifyingModel(theDelivery)
                && HasQualifyingStart(theDelivery)
                && HasQualifyingMonitor(theDelivery)
                && !HasFundingRelationship();

        public bool IsExcluded(ILearningDelivery theDelivery) =>
            HasDisqualifyingEndDate(theDelivery);

        public bool HasDisqualifyingEndDate(ILearningDelivery theDelivery) =>
            AcademicYearStartDate > theDelivery.LearnActEndDateNullable;

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.ApprenticeshipsFrom1May2017);

        public bool HasQualifyingStart(ILearningDelivery theDelivery) =>
            _check.HasQualifyingStart(theDelivery, FirstViableStart);

        public bool HasQualifyingMonitor(ILearningDeliveryFAM theMonitor) =>
            Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer.CaseInsensitiveEquals($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}");

        public bool HasQualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, HasQualifyingMonitor);

        public bool HasFundingRelationship() =>
            _fcsData
                .GetContractAllocationsFor(ProviderUKPRN)
                .NullSafeAny(x => _fundingStreams.Contains(x.FundingStreamPeriodCode));

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) =>
            new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ProviderUKPRN),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.ACT_ContractEmployer)
            };
    }
}
