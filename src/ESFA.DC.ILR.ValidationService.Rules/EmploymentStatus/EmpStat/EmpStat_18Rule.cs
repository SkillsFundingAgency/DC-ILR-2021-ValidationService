using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_18Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The checks (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_18Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations.</param>
        public EmpStat_18Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_18)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// Gets the old code monitoring threshold date.
        /// </summary>
        public static DateTime OldCodeMonitoringThresholdDate => new DateTime(2018, 07, 31);

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;
            var employments = thisLearner.LearnerEmploymentStatuses.ToReadOnlyCollection();

            thisLearner.LearningDeliveries
                .ForEach(x => RunChecks(x, GetEmploymentStatusOn(x.LearnStartDate, employments), y => RaiseValidationMessage(learnRefNumber, x, y)));
        }

        /// <summary>
        /// Runs the checks.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="thisEmployment">this employment.</param>
        /// <param name="raiseMessage">The raise message (action).</param>
        public void RunChecks(ILearningDelivery thisDelivery, ILearnerEmploymentStatus thisEmployment, Action<IEmploymentStatusMonitoring> raiseMessage)
        {
            if (IsQualifyingPrimaryLearningAim(thisDelivery)
                && HasQualifyingEmploymentStatus(thisEmployment))
            {
                CheckEmploymentMonitors(thisEmployment, raiseMessage);
            }
        }

        /// <summary>
        /// Gets the employment status on (this date) (from employments).
        /// </summary>
        /// <param name="thisDate">this date.</param>
        /// <param name="fromEmployments">from employments.</param>
        /// <returns>the closest learner employmentstatus for the learn start date</returns>
        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime thisDate, IReadOnlyCollection<ILearnerEmploymentStatus> fromEmployments) =>
            _check.GetEmploymentStatusOn(thisDate, fromEmployments);

        /// <summary>
        /// Determines whether [is qualifying primary learning aim] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying primary learning aim] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingPrimaryLearningAim(ILearningDelivery thisDelivery) =>
            It.Has(thisDelivery)
            && _check.HasQualifyingStart(thisDelivery, DateTime.MinValue, OldCodeMonitoringThresholdDate)
            && _check.IsTraineeship(thisDelivery)
            && _check.InAProgramme(thisDelivery);

        /// <summary>
        /// Determines whether [has qualifying employment status] [this employment].
        /// </summary>
        /// <param name="thisEmployment">this employment.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employment status] [this employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus thisEmployment) =>
            It.Has(thisEmployment)
            && It.IsInRange(thisEmployment.EmpStat, TypeOfEmploymentStatus.InPaidEmployment);

        /// <summary>
        /// Checks the employment monitors.
        /// </summary>
        /// <param name="employment">The employment.</param>
        /// <param name="raiseMessage">The raise message (action).</param>
        public void CheckEmploymentMonitors(ILearnerEmploymentStatus employment, Action<IEmploymentStatusMonitoring> raiseMessage) =>
            employment.EmploymentStatusMonitorings.ForAny(HasDisqualifyingMonitor, raiseMessage);

        /// <summary>
        /// Determines whether [has disqualifying monitor] [this monitor].
        /// </summary>
        /// <param name="thisMonitor">The this monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying monitor] [this monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingMonitor(IEmploymentStatusMonitoring thisMonitor) =>
            It.IsInRange(thisMonitor.ESMType, Monitoring.EmploymentStatus.Types.EmploymentIntensityIndicator)
            && It.IsOutOfRange($"{thisMonitor.ESMType}{thisMonitor.ESMCode}", Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW);

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <param name="thisMonitor">The this monitor.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, IEmploymentStatusMonitoring thisMonitor)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisMonitor));
        }

        /// <summary>
        /// Builds the message parameters for (this monitor).
        /// </summary>
        /// <param name="thisMonitor">The this monitor.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(IEmploymentStatusMonitoring thisMonitor)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ESMType, thisMonitor.ESMType),
                BuildErrorMessageParameter(PropertyNameConstants.ESMCode, thisMonitor.ESMCode)
            };
        }
    }
}
