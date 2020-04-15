using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.StdCode
{
    public class StdCode_01Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = PropertyNameConstants.ProgType;

        public const string Name = RuleNameConstants.StdCode_01;

        private readonly IValidationErrorHandler _messageHandler;

        public StdCode_01Rule(
            IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool HasStandardCode(ILearningDelivery delivery) =>
            delivery.StdCodeNullable.HasValue;

        public bool IsQualifyingLearningProgramme(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard;

        public bool IsNotValid(ILearningDelivery delivery) =>
            IsQualifyingLearningProgramme(delivery) && !HasStandardCode(delivery);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery.ProgTypeNullable)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}