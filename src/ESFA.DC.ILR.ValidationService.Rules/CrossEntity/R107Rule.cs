using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
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
        public const string Name = RuleNameConstants.R107;

        private readonly IValidationErrorHandler _messageHandler;

        public R107Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public ILearningDelivery GetLastDelivery(ILearner learner) =>
            learner.LearningDeliveries?
                .Where(x => It.Has(x.LearnActEndDateNullable))
                .OrderByDescending(x => x.LearnActEndDateNullable.Value)
                .FirstOrDefault();

        public ILearnerDestinationAndProgression GetDAndP(string learnRefNumber, IMessage message) =>
              message.LearnerDestinationAndProgressions?
                 .Where(x => x.LearnRefNumber == learnRefNumber)
                 .FirstOrDefault();

        public bool HasQualifyingOutcome(IDPOutcome outcome, DateTime actualEndDate) =>
            actualEndDate <= outcome.OutStartDate;

        public bool HasQualifyingOutcome(ILearner learner, IMessage message)
        {
            var dps = GetDAndP(learner.LearnRefNumber, message);
            var delivery = GetLastDelivery(learner);

            return It.Has(dps)
                && It.Has(delivery)
                && dps.DPOutcomes.NullSafeAny(x => HasQualifyingOutcome(x, delivery.LearnActEndDateNullable.Value));
        }

        public bool HasQualifyingFundModel(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.AdultSkills, TypeOfFunding.EuropeanSocialFund, TypeOfFunding.OtherAdult);

        public bool HasQualifyingFundModel(ILearner learner) =>
            learner.LearningDeliveries.NullSafeAny(HasQualifyingFundModel);

        public bool HasTemporarilyWithdrawn(ILearningDelivery delivery) =>
            It.IsInRange(delivery?.CompStatus, CompletionState.HasTemporarilyWithdrawn);

        public bool HasTemporarilyWithdrawn(ILearner learner) =>
            HasTemporarilyWithdrawn(GetLastDelivery(learner));

        public bool HasCompletedCourse(ILearningDelivery delivery) =>
            It.Has(delivery.LearnActEndDateNullable);

        public bool HasCompletedCourse(ILearner learner) =>
            It.HasValues(learner.LearningDeliveries)
                && learner.LearningDeliveries.All(HasCompletedCourse);

        public bool InTraining(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship, TypeOfLearningProgramme.ApprenticeshipStandard);

        public bool InTraining(ILearner learner) =>
            learner.LearningDeliveries.NullSafeAny(InTraining);

        public bool RequiresQualifyingOutcome(ILearner learner) =>
            !InTraining(learner) && HasQualifyingFundModel(learner) && HasCompletedCourse(learner) && !HasTemporarilyWithdrawn(learner);

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

        public void RaiseValidationMessage(string learnRefNumber)
        {
            _messageHandler.Handle(RuleName, learnRefNumber, null, null);
        }
    }
}