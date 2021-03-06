﻿using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R85Rule :
        IRule<IMessage>
    {
        public const string Name = RuleNameConstants.R85;

        private readonly IValidationErrorHandler _messageHandler;

        public R85Rule(IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsNotMatchingLearnerNumber(ILearnerDestinationAndProgression dAndP, ILearner learner) =>
             dAndP.ULN != learner.ULN;

        public bool HasMatchingReferenceNumber(ILearnerDestinationAndProgression dAndP, ILearner learner) =>
             dAndP.LearnRefNumber == learner.LearnRefNumber;

        public bool IsNotValid(ILearnerDestinationAndProgression dAndP, ILearner learner) =>
             HasMatchingReferenceNumber(dAndP, learner) && IsNotMatchingLearnerNumber(dAndP, learner);

        public void Validate(IMessage message)
        {
            var learners = message.Learners.ToReadOnlyCollection();
            var dAndPs = message.LearnerDestinationAndProgressions.ToReadOnlyCollection();

            if (learners.IsNullOrEmpty() || dAndPs.IsNullOrEmpty())
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

        public void RaiseValidationMessage(ILearner learner, ILearnerDestinationAndProgression dAndP)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(learner.ULN), learner.ULN),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearningDestinationAndProgressionULN, dAndP.ULN),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearningDestinationAndProgressionLearnRefNumber, dAndP.LearnRefNumber)
            };

            _messageHandler.Handle(RuleName, learner.LearnRefNumber, null, parameters);
        }
    }
}