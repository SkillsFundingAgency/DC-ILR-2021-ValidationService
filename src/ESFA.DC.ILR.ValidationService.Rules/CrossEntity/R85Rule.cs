using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    /// <summary>
    /// the cross record rule 85 implementation
    /// </summary>
    /// <seealso cref="Interface.IRule{IMessage}" />
    public class R85Rule :
        IRule<IMessage>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.R85;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="R85Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public R85Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Determines whether [is not matching learner number] [the specified d and p].
        /// </summary>
        /// <param name="dAndP">The d and p.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is not matching learner number] [the specified d and p]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotMatchingLearnerNumber(ILearnerDestinationAndProgression dAndP, ILearner learner) =>
             dAndP.ULN != learner.ULN;

        /// <summary>
        /// Determines whether [has matching reference number] [the specified d and p].
        /// </summary>
        /// <param name="dAndP">The d and p.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [has matching reference number] [the specified d and p]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingReferenceNumber(ILearnerDestinationAndProgression dAndP, ILearner learner) =>
             dAndP.LearnRefNumber == learner.LearnRefNumber;

        /// <summary>
        /// Determines whether [is not valid] [the specified (learner) destination and progression].
        /// </summary>
        /// <param name="dAndP">The d and p.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified d and p]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerDestinationAndProgression dAndP, ILearner learner) =>
             HasMatchingReferenceNumber(dAndP, learner) && IsNotMatchingLearnerNumber(dAndP, learner);

        /// <summary>
        /// Validates the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Validate(IMessage message)
        {
            It.IsNull(message)
                .AsGuard<ArgumentNullException>(nameof(message));

            var learners = message.Learners.AsSafeReadOnlyList();
            var dAndPs = message.LearnerDestinationAndProgressions.AsSafeReadOnlyList();

            if (It.IsEmpty(learners) || It.IsEmpty(dAndPs))
            {
                return;
            }

            learners.ForEach(learner =>
            {
                dAndPs
                    .Where(ldap => IsNotValid(ldap, learner))
                    .ForEach(x => RaiseValidationMessage(learner, x));
            });
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="dAndP">The destination and progression.</param>
        public void RaiseValidationMessage(ILearner learner, ILearnerDestinationAndProgression dAndP)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(learner.ULN), learner.ULN));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearningDestinationAndProgressionULN, dAndP.ULN));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearningDestinationAndProgressionLearnRefNumber, dAndP.LearnRefNumber));

            _messageHandler.Handle(RuleName, learner.LearnRefNumber, null, parameters);
        }
    }
}