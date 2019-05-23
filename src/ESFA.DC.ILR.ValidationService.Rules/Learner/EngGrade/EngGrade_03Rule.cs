using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class EngGrade_03Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "EngGrade";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "EngGrade_03";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngGrade_03Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public EngGrade_03Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Determines whether [is eligible for funding] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is eligible for funding] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEligibleForFunding(ILearner candidate) =>
            Monitoring.Learner.Level1AndLowerGrades.Contains(candidate.EngGrade);

        /// <summary>
        /// Determines whether [has eligible funding] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has eligible funding] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasEligibleFunding(ILearnerFAM monitor) =>
            It.IsInRange($"{monitor.LearnFAMType}{monitor.LearnFAMCode}", Monitoring.Learner.NotAchievedLevel2EnglishGCSEByYear11);

        /// <summary>
        /// Checks the fams.
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>
        /// true if any of the delivery fams match the condition
        /// </returns>
        public bool CheckFAMs(ILearner learner, Func<ILearnerFAM, bool> matchCondition) =>
            learner.LearnerFAMs.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether [has eligible funding] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [has eligible funding] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasEligibleFunding(ILearner learner) =>
            CheckFAMs(learner, HasEligibleFunding);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            if (IsEligibleForFunding(objectToValidate))
            {
                var failedValidation = !objectToValidate.LearnerFAMs.SafeAny(HasEligibleFunding);

                if (failedValidation)
                {
                    RaiseValidationMessage(learnRefNumber);
                }
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        public void RaiseValidationMessage(string learnRefNumber)
        {
            _messageHandler.Handle(RuleName, learnRefNumber, null, null);
        }
    }
}