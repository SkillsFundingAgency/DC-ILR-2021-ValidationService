using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade
{
    public class EngGrade_03Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "EngGrade";

        public const string Name = RuleNameConstants.EngGrade_03;

        private readonly IValidationErrorHandler _messageHandler;

        public EngGrade_03Rule(IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsEligibleForFunding(ILearner candidate) =>
            Monitoring.Learner.Level1AndLowerGrades.Contains(candidate.EngGrade);

        public bool HasEligibleFunding(ILearnerFAM monitor) =>
            Monitoring.Learner.NotAchievedLevel2EnglishGCSEByYear11.CaseInsensitiveEquals($"{monitor.LearnFAMType}{monitor.LearnFAMCode}");

        public bool CheckFAMs(ILearner learner, Func<ILearnerFAM, bool> matchCondition) =>
            learner.LearnerFAMs.NullSafeAny(matchCondition);

        public bool HasEligibleFunding(ILearner learner) =>
            CheckFAMs(learner, HasEligibleFunding);

        public void Validate(ILearner objectToValidate)
        {         
            var learnRefNumber = objectToValidate.LearnRefNumber;

            if (IsEligibleForFunding(objectToValidate))
            {
                var failedValidation = !objectToValidate.LearnerFAMs.NullSafeAny(HasEligibleFunding);

                if (failedValidation)
                {
                    RaiseValidationMessage(learnRefNumber);
                }
            }
        }

        public void RaiseValidationMessage(string learnRefNumber)
        {
            _messageHandler.Handle(RuleName, learnRefNumber, null, null);
        }
    }
}