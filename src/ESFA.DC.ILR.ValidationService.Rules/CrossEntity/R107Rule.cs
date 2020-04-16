using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R107Rule :
        IRule<IMessage>
    {
        public const string Name = RuleNameConstants.R107;
        private readonly HashSet<int> _fundModels = new HashSet<int>
        {
            TypeOfFunding.Age16To19ExcludingApprenticeships,
            TypeOfFunding.AdultSkills,
            TypeOfFunding.EuropeanSocialFund,
            TypeOfFunding.OtherAdult
        };

        private readonly IValidationErrorHandler _messageHandler;

        public R107Rule(IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public ILearningDelivery GetLastDelivery(ILearner learner) =>
            learner.LearningDeliveries?
                .Where(x => x.LearnActEndDateNullable.HasValue)
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

            return dps != null
                && delivery != null
                && dps.DPOutcomes.NullSafeAny(x => HasQualifyingOutcome(x, delivery.LearnActEndDateNullable.Value));
        }

        public bool HasQualifyingFundModel(ILearningDelivery delivery) =>
            _fundModels.Contains(delivery.FundModel);

        public bool HasQualifyingFundModel(ILearner learner) =>
            learner.LearningDeliveries.NullSafeAny(HasQualifyingFundModel);

        public bool HasTemporarilyWithdrawn(ILearningDelivery delivery) =>
            delivery?.CompStatus == CompletionState.HasTemporarilyWithdrawn;

        public bool HasTemporarilyWithdrawn(ILearner learner) =>
            HasTemporarilyWithdrawn(GetLastDelivery(learner));

        public bool HasCompletedCourse(ILearningDelivery delivery) =>
            delivery.LearnActEndDateNullable.HasValue;

        public bool HasCompletedCourse(ILearner learner) =>
            !learner.LearningDeliveries.IsNullOrEmpty()
                && learner.LearningDeliveries.All(HasCompletedCourse);

        public bool InTraining(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == TypeOfLearningProgramme.Traineeship
            || delivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard;

        public bool InTraining(ILearner learner) =>
            learner.LearningDeliveries.NullSafeAny(InTraining);

        public bool RequiresQualifyingOutcome(ILearner learner) =>
            !InTraining(learner) && HasQualifyingFundModel(learner) && HasCompletedCourse(learner) && !HasTemporarilyWithdrawn(learner);

        public void Validate(IMessage objectToValidate)
        {
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