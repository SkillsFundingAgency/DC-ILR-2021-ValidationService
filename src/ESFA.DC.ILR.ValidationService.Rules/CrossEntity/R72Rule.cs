using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    /// <summary>
    /// cross record rule 72
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class R72Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The threshold proportion
        /// </summary>
        public const int ThresholdProportion = 3;

        /// <summary>
        /// The check(er, rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The derived data 17 (rule)
        /// </summary>
        private readonly IDerivedData_17Rule _derivedData17;

        /// <summary>
        /// Initializes a new instance of the <see cref="R72Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOps">The common ops.</param>
        /// <param name="dd17">The DD17.</param>
        public R72Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOps,
            IDerivedData_17Rule dd17)
            : base(validationErrorHandler, RuleNameConstants.R72)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));
            It.IsNull(dd17)
                .AsGuard<ArgumentNullException>(nameof(dd17));

            _check = commonOps;
            _derivedData17 = dd17;
        }

        /// <summary>
        /// Validates the specified learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnRefNumber = theLearner.LearnRefNumber;
            var deliveries = GetQualifyingItemsFrom(theLearner.LearningDeliveries);
            var pmrTotals = GetPaymentRecordTotalsFor(deliveries);

            pmrTotals.Keys.ForAny(
                x => IsNotValid(GetDeliveriesMatching(deliveries, y => HasMatchingStdCode(y, x)), x, pmrTotals[x]),
                x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Gets the qualifying items from.
        /// </summary>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>a shortlist of deliveries</returns>
        public IReadOnlyCollection<ILearningDelivery> GetQualifyingItemsFrom(IReadOnlyCollection<ILearningDelivery> theDeliveries) =>
            theDeliveries
                .Where(IsQualifyingItem)
                .AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [is qualifying item] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying item] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingItem(ILearningDelivery theDelivery) =>
            HasQualifyingModel(theDelivery)
                && IsProgrammeAim(theDelivery)
                && IsStandardApprenticeship(theDelivery)
                && HasStandardCode(theDelivery);

        /// <summary>
        /// Determines whether [has qualifying model] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.OtherAdult);

        /// <summary>
        /// Determines whether [is programme aim] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is programme aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProgrammeAim(ILearningDelivery theDelivery) =>
            _check.InAProgramme(theDelivery);

        /// <summary>
        /// Determines whether [is standard Apprenticeship] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is standard Apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStandardApprenticeship(ILearningDelivery theDelivery) =>
            _check.IsStandardApprenticeship(theDelivery);

        /// <summary>
        /// Determines whether [has standard code] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has standard code] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasStandardCode(ILearningDelivery theDelivery) =>
            theDelivery.StdCodeNullable.HasValue;

        /// <summary>
        /// Gets the payment record totals for.
        /// </summary>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>a dictionary of PMR totals for each standard code</returns>
        public IReadOnlyDictionary<int, int> GetPaymentRecordTotalsFor(IReadOnlyCollection<ILearningDelivery> theDeliveries)
        {
            var dict = new Dictionary<int, int>();

            theDeliveries
                .ForEach(delivery =>
                {
                    var finRecords = delivery.AppFinRecords.AsSafeReadOnlyList();
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

        /// <summary>
        /// Gets the record totals.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <param name="matchesRecordType">Type of record matche.</param>
        /// <returns>a total for the required record type</returns>
        public int GetRecordTotals(IReadOnlyCollection<IAppFinRecord> records, Func<IAppFinRecord, bool> matchesRecordType) =>
            records
                .Where(matchesRecordType)
                .Sum(s => s.AFinAmount);

        /// <summary>
        /// Determines whether [is payment request] [the specified record].
        /// </summary>
        /// <param name="theRecord">The record.</param>
        /// <returns>
        ///   <c>true</c> if [is payment request] [the specified record]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPaymentRequest(IAppFinRecord theRecord) =>
            theRecord.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.PaymentRecord)
            && It.IsInRange(theRecord.AFinCode, TypeOfPMRAFin.TrainingPayment, TypeOfPMRAFin.AssessmentPayment);

        /// <summary>
        /// Determines whether [is provider reimbursement] [the specified record].
        /// </summary>
        /// <param name="theRecord">The record.</param>
        /// <returns>
        ///   <c>true</c> if [is provider reimbursement] [the specified record]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProviderReimbursement(IAppFinRecord theRecord) =>
            theRecord.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.PaymentRecord)
            && It.IsInRange(theRecord.AFinCode, TypeOfPMRAFin.EmployerPaymentReimbursedByProvider);

        /// <summary>
        /// Gets the deliveries matching.
        /// </summary>
        /// <param name="theStdCode">The standard code.</param>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>a shortlist of deliveries carrying the same standard code</returns>
        public IReadOnlyCollection<ILearningDelivery> GetDeliveriesMatching(IReadOnlyCollection<ILearningDelivery> theDeliveries, Func<ILearningDelivery, bool> hasMatchingStdCode) =>
            theDeliveries
                .Where(hasMatchingStdCode)
                .AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [has matching standard code] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="theStdCode">The standard code.</param>
        /// <returns>
        ///   <c>true</c> if [has matching standard code] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingStdCode(ILearningDelivery theDelivery, int theStdCode) =>
            theDelivery.StdCodeNullable == theStdCode;

        /// <summary>
        /// Determines whether [is not valid] [the specified deliveries].
        /// </summary>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <param name="theStandard">For the standard code.</param>
        /// <param name="thePMRTotal">And the PMR total.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified the deliveries]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(IReadOnlyCollection<ILearningDelivery> theDeliveries, int theStandard, int thePMRTotal) =>
            !IsTNPMoreThanContributionCapFor(theStandard, theDeliveries)
            && IsThresholdProportionExceededFor(thePMRTotal, GetTotalTNPPriceFor(theDeliveries));

        /// <summary>
        /// Determines whether [is TNP more than contribution cap for] [the specified standard].
        /// </summary>
        /// <param name="theStandard">The standard.</param>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>
        ///   <c>true</c> if [is TNP more than contribution cap for] [the specified standard]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTNPMoreThanContributionCapFor(int theStandard, IReadOnlyCollection<ILearningDelivery> theDeliveries) =>
            _derivedData17.IsTNPMoreThanContributionCapFor(theStandard, theDeliveries);

        /// <summary>
        /// Gets the total TNP price for.
        /// </summary>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>the total TNP price</returns>
        public int GetTotalTNPPriceFor(IReadOnlyCollection<ILearningDelivery> theDeliveries) =>
            _derivedData17.GetTotalTNPPriceFor(theDeliveries);

        /// <summary>
        /// Determines whether [is threshold proportion exceeded for] [the specified PMR total].
        /// </summary>
        /// <param name="pmrTotal">The PMR total.</param>
        /// <param name="tnpTotal">The TNP total.</param>
        /// <returns>
        ///   <c>true</c> if [is threshold proportion exceeded for] [the specified PMR total]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsThresholdProportionExceededFor(int pmrTotal, int tnpTotal) =>
            pmrTotal > (tnpTotal / ThresholdProportion);

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="stdCode">The standard code.</param>
        public void RaiseValidationMessage(string learnRefNumber, int stdCode) =>
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(learnRefNumber, stdCode));

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="stdCode">The standard code.</param>
        /// <returns></returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(string learnRefNumber, int stdCode) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.LearnRefNumber, learnRefNumber),
            BuildErrorMessageParameter(PropertyNameConstants.AimType, TypeOfAim.ProgrammeAim),
            BuildErrorMessageParameter(PropertyNameConstants.ProgType, TypeOfLearningProgramme.ApprenticeshipStandard),
            BuildErrorMessageParameter(PropertyNameConstants.StdCode, stdCode),
        };
    }
}
