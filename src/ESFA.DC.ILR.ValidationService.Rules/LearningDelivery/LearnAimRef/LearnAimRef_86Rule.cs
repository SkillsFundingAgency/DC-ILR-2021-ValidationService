using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_86Rule :
        IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnAimRef_86;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly IProvideRuleCommonOperations _check;

        public LearnAimRef_86Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonChecks)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonChecks)
                .AsGuard<ArgumentNullException>(nameof(commonChecks));

            _messageHandler = validationErrorHandler;
            _check = commonChecks;
        }

        public string RuleName => Name;

        public bool IsWorkExperience(ILearningDelivery delivery) =>
            It.IsInRange(delivery.LearnAimRef, TypeOfAim.References.WorkExperience);

        public bool IsNotValid(ILearningDelivery delivery) =>
            _check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
            && !_check.IsSteelWorkerRedundancyTraining(delivery)
            && !_check.IsTraineeship(delivery)
            && IsWorkExperience(delivery);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.FundModel), thisDelivery.FundModel),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.ProgType, thisDelivery.ProgTypeNullable)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
