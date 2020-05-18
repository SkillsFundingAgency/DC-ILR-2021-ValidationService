using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R59Rule : IRule<IMessage>
    {
        public const string Name = RuleNameConstants.R59;

        private readonly IValidationErrorHandler _messageHandler;

        public R59Rule(IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public void Validate(IMessage objectToValidate)
        {
            if (objectToValidate?.Learners == null)
            {
                return;
            }

            var duplicateUlns = objectToValidate.Learners
                .Where(x => x.ULN != ValidationConstants.TemporaryULN)
                .GroupBy(x => x.ULN)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key);

            foreach (var uln in duplicateUlns)
            {
                objectToValidate.Learners.Where(x => x.ULN == uln)
                    .ForEach(learner =>
                        RaiseValidationMessage(
                            objectToValidate.LearningProviderEntity?.UKPRN,
                            learner));
            }
        }

        public void RaiseValidationMessage(int? ukprn, ILearner thisLearner)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.ULN, thisLearner.ULN)
            };

            _messageHandler.Handle(RuleName, thisLearner.LearnRefNumber, null, parameters);
        }
    }
}
