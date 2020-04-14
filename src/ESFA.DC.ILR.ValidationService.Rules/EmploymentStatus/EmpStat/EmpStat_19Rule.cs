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
    public class EmpStat_19Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The checks (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_19Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations.</param>
        public EmpStat_19Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_19)
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
        public static DateTime NewCodeMonitoringThresholdDate => new DateTime(2018, 08, 01);

        /// <summary>
        /// Gets the employment status on.
        /// </summary>
        /// <param name="thisDate">this date.</param>
        /// <param name="usingEmployments">using employments.</param>
        /// <returns>the latest employment status candidate</returns>
        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime thisDate, IReadOnlyCollection<ILearnerEmploymentStatus> usingEmployments) =>
            _check.GetEmploymentStatusOn(thisDate, usingEmployments);

        /// <summary>
        /// Determines whether [has a qualifying monitor status] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has a qualifying monitor status] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasADisqualifyingMonitorStatus(IEmploymentStatusMonitoring monitor) =>
            monitor.ESMType.CaseInsensitiveEquals(Monitoring.EmploymentStatus.Types.EmploymentIntensityIndicator)
            && It.IsOutOfRange(
                $"{monitor.ESMType}{monitor.ESMCode}",
                Monitoring.EmploymentStatus.EmployedFor0To10HourPW,
                Monitoring.EmploymentStatus.EmployedFor11To20HoursPW);

        /// <summary>
        /// Checks the employment status.
        /// </summary>
        /// <param name="thisEmployment">this employment.</param>
        /// <param name="doThisAction">do this action.</param>
        public void CheckEmploymentStatus(ILearnerEmploymentStatus thisEmployment, Action<IEmploymentStatusMonitoring> doThisAction)
        {
            if (It.Has(thisEmployment) && It.IsInRange(thisEmployment.EmpStat, TypeOfEmploymentStatus.InPaidEmployment))
            {
                thisEmployment.EmploymentStatusMonitorings.ForAny(HasADisqualifyingMonitorStatus, doThisAction);
            }
        }

        /// <summary>
        /// Determines whether [is restriction match] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is restriction match] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRestrictionMatch(ILearningDelivery delivery) =>
            _check.IsTraineeship(delivery)
                && _check.InAProgramme(delivery)
                && _check.HasQualifyingStart(delivery, NewCodeMonitoringThresholdDate);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var employments = objectToValidate.LearnerEmploymentStatuses.ToReadOnlyCollection();

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsRestrictionMatch)
                .ForEach(x => CheckEmploymentStatus(GetEmploymentStatusOn(x.LearnStartDate, employments), y => RaiseValidationMessage(learnRefNumber, x, y)));
        }

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
        /// Builds the error message parameters.
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
