using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_02Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "LearnStartDate_02";

        /// <summary>
        /// The oldest learning submission offset
        /// </summary>
        public const int OldestLearningSubmissionOffset = -10; // minus 10 years

        /// <summary>
        /// The file data service
        /// </summary>
        private readonly IAcademicYearDataService _yearData;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnStartDate_02Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="yearData">The file data service.</param>
        public LearnStartDate_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService yearData)
            : base(validationErrorHandler, Name)
        {
            It.IsNull(validationErrorHandler)
               .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(yearData)
                .AsGuard<ArgumentNullException>(nameof(yearData));

            _yearData = yearData;
        }

        /// <summary>
        /// Determines whether [is outside valid submission period] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is outside valid submission period] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOutsideValidSubmissionPeriod(ILearningDelivery delivery) =>
            delivery.LearnStartDate < _yearData.Start().AddYears(OldestLearningSubmissionOffset);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery) =>
            IsOutsideValidSubmissionPeriod(delivery);

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
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate)
            };
        }
    }
}
