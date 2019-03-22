using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public abstract class LearnAimRefRuleBase :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The action provider
        /// </summary>
        private readonly IProvideLearnAimRefRuleActions _actionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRefRuleBase" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="ruleName">Name of the rule.</param>
        protected LearnAimRefRuleBase(
            IValidationErrorHandler validationErrorHandler,
            IProvideLearnAimRefRuleActions provider,
            ILARSDataService larsData,
            string ruleName)
            : base(validationErrorHandler, ruleName)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(provider)
                .AsGuard<ArgumentNullException>(nameof(provider));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));

            _actionProvider = provider;
            LarsData = larsData;
        }

        /// <summary>
        /// Gets the lars data (service)
        /// </summary>
        protected ILARSDataService LarsData { get; }

        /// <summary>
        /// Passes the (rule) conditions.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="branchCategory">The branch category.</param>
        /// <returns>
        /// true if it does...
        /// </returns>
        public abstract bool PassesConditions(ILearningDelivery delivery, IBranchResult branchCategory);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="branchResult">The branch result.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery, IBranchResult branchResult) =>
            !PassesConditions(delivery, branchResult);

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;
            IBranchResult expected = null;

            thisLearner.LearningDeliveries
                .ForAny(x => IsNotValid(x, expected = _actionProvider.GetBranchingResultFor(x, thisLearner)), x => RaiseValidationMessage(learnRefNumber, x, expected));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="expected">The expected category.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, IBranchResult expected)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery, expected));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="andExpected">and expected category.</param>
        /// <returns>
        /// a collection of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery, IBranchResult andExpected)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, thisDelivery.LearnAimRef),
                BuildErrorMessageParameter("Expected Category", andExpected.Category)
            };
        }
    }
}
