using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_81Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// The (rule) name
        /// </summary>
        public const string Name = RuleNameConstants.LearnAimRef_81;

        /// <summary>
        /// the message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// the lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// The common rule (operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRef_81Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="commonChecks">The common rule (operations provider).</param>
        public LearnAimRef_81Rule(
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

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public static DateTime FirstViableDate => new DateTime(2016, 08, 01);

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Determines whether [has disqualifying learning category] [the specified category].
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying learning category] [the specified category]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingLearningCategory(ILARSLearningCategory category) =>
            It.IsInRange(category.CategoryRef, TypeOfLARSCategory.LicenseToPractice);

        /// <summary>
        /// Determines whether [has disqualifying learning category] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying learning category] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingLearningCategory(ILearningDelivery delivery)
        {
            var categories = _larsData.GetCategoriesFor(delivery.LearnAimRef).AsSafeReadOnlyList();

            return categories.Any(HasDisqualifyingLearningCategory);
        }

        /// <summary>
        /// In receipt of another state benefit.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        /// true if the learner is in receipt at the start of the learning aim
        /// </returns>
        public bool InReceiptOfAnotherStateBenefit(ILearningDelivery delivery, ILearner learner)
        {
            var candidate = _check.GetEmploymentStatusOn(delivery.LearnStartDate, learner.LearnerEmploymentStatuses);

            var esms = candidate?.EmploymentStatusMonitorings.AsSafeReadOnlyList();
            return esms.SafeAny(InReceiptOfAnotherStateBenefit);
        }

        /// <summary>
        /// In receipt of another state benefit.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        /// true if the learner is in receipt for this monitor
        /// </returns>
        public bool InReceiptOfAnotherStateBenefit(IEmploymentStatusMonitoring monitor) =>
            It.IsInRange($"{monitor.ESMType}{monitor.ESMCode}", Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit);

        /// <summary>
        /// Determines whether the specified delivery is excluded.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery delivery) =>
            _check.IsSteelWorkerRedundancyTraining(delivery);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery, ILearner learner) =>
            !IsExcluded(delivery)
            && _check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills, TypeOfFunding.OtherAdult, TypeOfFunding.EuropeanSocialFund)
            && _check.HasQualifyingStart(delivery, FirstViableDate)
            && InReceiptOfAnotherStateBenefit(delivery, learner)
            && HasDisqualifyingLearningCategory(delivery);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .SafeWhere(x => IsNotValid(x, objectToValidate))
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(IEmploymentStatusMonitoring.ESMType), Monitoring.EmploymentStatus.Types.BenefitStatusIndicator));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(IEmploymentStatusMonitoring.ESMCode), 3));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.FundModel), thisDelivery.FundModel));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
