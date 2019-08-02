using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp
{
    public class DateEmpStatApp_01Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "DateEmpStatApp_01";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The (academic) year data (service)
        /// </summary>
        private readonly IAcademicYearDataService _yearData;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateEmpStatApp_01Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="yearData">The year data.</param>
        public DateEmpStatApp_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService yearData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(yearData)
                .AsGuard<ArgumentNullException>(nameof(yearData));

            _messageHandler = validationErrorHandler;
            _yearData = yearData;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets an academic year date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a date time representing the start of the comming year</returns>
        public DateTime GetNextAcademicYearDate(DateTime candidate) =>
            _yearData.GetAcademicYearOfLearningDate(candidate, AcademicYearDates.NextYearCommencement);

        /// <summary>
        /// Determines whether [has qualifying employment status] [the specified employment status].
        /// </summary>
        /// <param name="eStatus">The employment status.</param>
        /// <param name="thresholdDate">The threshold date.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employment status] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus eStatus, DateTime thresholdDate) =>
            eStatus.DateEmpStatApp.Year <= thresholdDate.Year;

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            // the educational year covers two actual years, in order to determine if
            // an employment status is inside the 'current educational year' we need next
            // years start date
            var thresholdDate = GetNextAcademicYearDate(_yearData.End());

            objectToValidate.LearnerEmploymentStatuses
                 .SafeWhere(x => !HasQualifyingEmploymentStatus(x, thresholdDate))
                 .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisEmployment">this employment.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.DateEmpStatApp), thisEmployment.DateEmpStatApp));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
