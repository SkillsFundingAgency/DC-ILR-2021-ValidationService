using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp
{
    /// <summary>
    /// date employment status applied rule 01
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class DateEmpStatApp_01Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateEmpStatApp_01Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="yearData">The year data.</param>
        public DateEmpStatApp_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService yearData)
            : base(validationErrorHandler, RuleNameConstants.DateEmpStatApp_01)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(yearData)
                .AsGuard<ArgumentNullException>(nameof(yearData));

            ThresholdDate = yearData.End();
        }

        /// <summary>
        /// Gets the threshold date.
        /// </summary>
        public DateTime ThresholdDate { get; }

        /// <summary>
        /// Validates the specified the learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearnerEmploymentStatuses
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Determines whether [is not valid] [the specified e status].
        /// </summary>
        /// <param name="eStatus">The e status.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified e status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus eStatus) =>
            HasDisqualifyingEmploymentStatusDate(eStatus);

        /// <summary>
        /// Determines whether [has disqualifying employment status date] [the specified e status].
        /// </summary>
        /// <param name="eStatus">The e status.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying employment status date] [the specified e status]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingEmploymentStatusDate(ILearnerEmploymentStatus eStatus) =>
            eStatus.DateEmpStatApp > ThresholdDate;

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="theDelivery">The delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus theEmployment) =>
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(theEmployment));

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>a collection of message paramters</returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearnerEmploymentStatus theEmployment) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.DateEmpStatApp, theEmployment.DateEmpStatApp)
        };
    }
}
