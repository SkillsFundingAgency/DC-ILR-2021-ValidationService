using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.Sex
{
    /// <summary>
    /// The learner's Sex must be a valid lookup
    /// </summary>
    public class Sex_01Rule : AbstractRule, IRule<ILearner>
    {
        /// <summary>
        /// The lookup details (provider)
        /// </summary>
        private readonly IProvideLookupDetails _provideLookupDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sex_01Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="provideLookupDetails">The provide lookup details.</param>
        public Sex_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails provideLookupDetails)
                : base(validationErrorHandler, RuleNameConstants.Sex_01)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        /// <summary>
        /// Validates the specified learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            if (!IsValidSex(theLearner.Sex))
            {
                HandleValidationError(theLearner.LearnRefNumber, null, BuildErrorMessageParameters(theLearner.Sex));
            }
        }

        /// <summary>
        /// Determines whether [is valid sex] [the specified sex].
        /// </summary>
        /// <param name="sex">The sex.</param>
        /// <returns>
        ///   <c>true</c> if [is valid sex] [the specified sex]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidSex(string sex) =>
            _provideLookupDetails.Contains(TypeOfStringCodedLookup.Sex, sex);

        /// <summary>
        /// Builds the error message parameters.
        /// </summary>
        /// <param name="sex">The sex.</param>
        /// <returns>a collection of message parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string sex)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Sex, sex)
            };
        }
    }
}