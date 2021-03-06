﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R72Rule : AbstractRule, IRule<ILearner>
    {
        public const int ThresholdProportion = 3;

        private readonly IDerivedData_17Rule _derivedData17;

        public R72Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_17Rule dd17)
            : base(validationErrorHandler, RuleNameConstants.R72)
        {
            _derivedData17 = dd17;
        }

        public void Validate(ILearner theLearner)
        {
            var learnRefNumber = theLearner.LearnRefNumber;
            var deliveries = GetQualifyingItemsFrom(theLearner.LearningDeliveries);
            var pmrTotals = GetPaymentRecordTotalsFor(deliveries);

            pmrTotals.Keys.ForAny(
                x => IsNotValid(GetDeliveriesMatching(deliveries, y => HasMatchingStdCode(y, x)), x, pmrTotals[x]),
                x => RaiseValidationMessage(learnRefNumber, x));
        }

        public IReadOnlyCollection<ILearningDelivery> GetQualifyingItemsFrom(IReadOnlyCollection<ILearningDelivery> theDeliveries) =>
            theDeliveries
                .Where(IsQualifyingItem)
                .ToReadOnlyCollection();

        public bool IsQualifyingItem(ILearningDelivery theDelivery) =>
            HasQualifyingModel(theDelivery)
                && IsProgrammeAim(theDelivery)
                && IsStandardApprenticeship(theDelivery)
                && HasStandardCode(theDelivery);

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
           theDelivery.FundModel == FundModels.OtherAdult;

        public bool IsProgrammeAim(ILearningDelivery theDelivery) =>
            theDelivery.AimType == AimTypes.ProgrammeAim;

        public bool IsStandardApprenticeship(ILearningDelivery theDelivery) =>
            theDelivery.ProgTypeNullable == ProgTypes.ApprenticeshipStandard;

        public bool HasStandardCode(ILearningDelivery theDelivery) =>
            theDelivery.StdCodeNullable.HasValue;

        public IReadOnlyDictionary<int, int> GetPaymentRecordTotalsFor(IReadOnlyCollection<ILearningDelivery> theDeliveries)
        {
            var dict = new Dictionary<int, int>();

            theDeliveries
                .ForEach(delivery =>
                {
                    var finRecords = delivery.AppFinRecords.ToReadOnlyCollection();
                    var stdCode = delivery.StdCodeNullable.Value;
                    var totalPaymentRequests = GetRecordTotals(finRecords, IsPaymentRequest);
                    var totalReimbursement = GetRecordTotals(finRecords, IsProviderReimbursement);

                    if (!dict.ContainsKey(stdCode))
                    {
                        dict.Add(stdCode, 0);
                    }

                    dict[stdCode] += totalPaymentRequests - totalReimbursement;
                });

            return dict;
        }

        public int GetRecordTotals(IReadOnlyCollection<IAppFinRecord> records, Func<IAppFinRecord, bool> matchesRecordType) =>
            records
                .Where(matchesRecordType)
                .Sum(s => s.AFinAmount);

        public bool IsPaymentRequest(IAppFinRecord theRecord) =>
            theRecord.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.PaymentRecord)
            && (theRecord.AFinCode == ApprenticeshipFinancialRecord.PaymentRecordCodes.TrainingPayment || theRecord.AFinCode == ApprenticeshipFinancialRecord.PaymentRecordCodes.AssessmentPayment);

        public bool IsProviderReimbursement(IAppFinRecord theRecord) =>
            theRecord.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.PaymentRecord)
            && theRecord.AFinCode == ApprenticeshipFinancialRecord.PaymentRecordCodes.EmployerPaymentReimbursedByProvider;

        public IReadOnlyCollection<ILearningDelivery> GetDeliveriesMatching(IReadOnlyCollection<ILearningDelivery> theDeliveries, Func<ILearningDelivery, bool> hasMatchingStdCode) =>
            theDeliveries
                .Where(hasMatchingStdCode)
                .ToReadOnlyCollection();

        public bool HasMatchingStdCode(ILearningDelivery theDelivery, int theStdCode) =>
            theDelivery.StdCodeNullable == theStdCode;

        public bool IsNotValid(IReadOnlyCollection<ILearningDelivery> theDeliveries, int theStandard, int thePMRTotal) =>
            !IsTNPMoreThanContributionCapFor(theStandard, theDeliveries)
            && IsThresholdProportionExceededFor(thePMRTotal, GetTotalTNPPriceFor(theDeliveries));

        public bool IsTNPMoreThanContributionCapFor(int theStandard, IReadOnlyCollection<ILearningDelivery> theDeliveries) =>
            _derivedData17.IsTNPMoreThanContributionCapFor(theStandard, theDeliveries);

        public int GetTotalTNPPriceFor(IReadOnlyCollection<ILearningDelivery> theDeliveries) =>
            _derivedData17.GetTotalTNPPriceFor(theDeliveries);

        public bool IsThresholdProportionExceededFor(int pmrTotal, int tnpTotal) =>
            pmrTotal > (tnpTotal / ThresholdProportion);

        public void RaiseValidationMessage(string learnRefNumber, int stdCode) =>
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(learnRefNumber, stdCode));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(string learnRefNumber, int stdCode) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.LearnRefNumber, learnRefNumber),
            BuildErrorMessageParameter(PropertyNameConstants.AimType, AimTypes.ProgrammeAim),
            BuildErrorMessageParameter(PropertyNameConstants.ProgType, ProgTypes.ApprenticeshipStandard),
            BuildErrorMessageParameter(PropertyNameConstants.StdCode, stdCode),
        };
    }
}
