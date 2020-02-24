using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp
{
    public class DateEmpStatApp_02Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.DateEmpStatApp_02;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateEmpStatApp_02Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="yearData">The year data.</param>
        public DateEmpStatApp_02Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets the last inviable date.
        /// </summary>
        public DateTime LastInviableDate => new DateTime(1990, 07, 31);

        /// <summary>
        /// Determines whether [has qualifying employment status] [the specified employment status].
        /// </summary>
        /// <param name="eStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employment status] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus eStatus) =>
            eStatus.DateEmpStatApp > LastInviableDate;

        /// <summary>
        /// Determines whether [is not valid] [the specified e status].
        /// </summary>
        /// <param name="eStatus">The e status.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified e status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus eStatus) =>
            !HasQualifyingEmploymentStatus(eStatus);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearnerEmploymentStatuses
                 .SafeWhere(IsNotValid)
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
