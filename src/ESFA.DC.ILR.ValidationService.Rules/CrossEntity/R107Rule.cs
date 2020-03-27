using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R107Rule :
        IRule<IMessage>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.R107;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="R107Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="message">The message.</param>
        public R107Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Gets the last delivery.
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <returns>returns the last learning delivery (or null)</returns>
        public ILearningDelivery GetLastDelivery(ILearner learner) =>
            learner.LearningDeliveries?
                .Where(x => It.Has(x.LearnActEndDateNullable))
                .OrderByDescending(x => x.LearnActEndDateNullable.Value)
                .FirstOrDefault();

        /// <summary>
        /// Gets the destination and progression (record) for the learner.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// the destination and progression record
        /// </returns>
        public ILearnerDestinationAndProgression GetDAndP(string learnRefNumber, IMessage message) =>
              message.LearnerDestinationAndProgressions?
                 .Where(x => x.LearnRefNumber == learnRefNumber)
                 .FirstOrDefault();

        /// <summary>
        /// Determines whether [has qualifying outcome] [the specified outcome].
        /// </summary>
        /// <param name="outcome">The outcome.</param>
        /// <param name="actualEndDate">The actual end date.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying outcome] [the specified outcome]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingOutcome(IDPOutcome outcome, DateTime actualEndDate) =>
            actualEndDate <= outcome.OutStartDate;

        /// <summary>
        /// Determines whether [has qualifying outcome] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying outcome] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingOutcome(ILearner learner, IMessage message)
        {
            var dps = GetDAndP(learner.LearnRefNumber, message);
            var delivery = GetLastDelivery(learner);

            return It.Has(dps)
                && It.Has(delivery)
                && dps.DPOutcomes.SafeAny(x => HasQualifyingOutcome(x, delivery.LearnActEndDateNullable.Value));
        }

        /// <summary>
        /// Determines whether [has qualifying fund model] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying fund model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFundModel(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.AdultSkills, TypeOfFunding.EuropeanSocialFund, TypeOfFunding.OtherAdult);

        /// <summary>
        /// Determines whether [has qualifying fund model] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying fund model] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFundModel(ILearner learner) =>
            learner.LearningDeliveries.SafeAny(HasQualifyingFundModel);

        /// <summary>
        /// Determines whether [has temporarily withdrawn] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has temporarily withdrawn] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasTemporarilyWithdrawn(ILearningDelivery delivery) =>
            It.IsInRange(delivery?.CompStatus, CompletionState.HasTemporarilyWithdrawn);

        /// <summary>
        /// Determines whether [has temporarily withdrawn] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [has temporarily withdrawn] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasTemporarilyWithdrawn(ILearner learner) =>
            HasTemporarilyWithdrawn(GetLastDelivery(learner));

        /// <summary>
        /// Determines whether [has completed course] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has completed course] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasCompletedCourse(ILearningDelivery delivery) =>
            It.Has(delivery.LearnActEndDateNullable);

        /// <summary>
        /// Determines whether [has completed course] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [has completed course] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasCompletedCourse(ILearner learner) =>
            It.HasValues(learner.LearningDeliveries)
                && learner.LearningDeliveries.SafeAll(HasCompletedCourse);

        /// <summary>
        /// Determines whether [in training] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in training] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InTraining(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship, TypeOfLearningProgramme.ApprenticeshipStandard);

        /// <summary>
        /// Determines whether [in training] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [in training] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool InTraining(ILearner learner) =>
            learner.LearningDeliveries.SafeAny(InTraining);

        /// <summary>
        /// Requires a qualifying DP outcome.
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <returns>true if all the conditions are met</returns>
        public bool RequiresQualifyingOutcome(ILearner learner) =>
            !InTraining(learner) && HasQualifyingFundModel(learner) && HasCompletedCourse(learner) && !HasTemporarilyWithdrawn(learner);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(IMessage objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));
            objectToValidate.Learners.ForEach(learner =>
            {
                if (RequiresQualifyingOutcome(learner) && !HasQualifyingOutcome(learner, objectToValidate))
                {
                    RaiseValidationMessage(learner.LearnRefNumber);
                }
            });
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