using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.StdCode
{
    public class StdCode_02Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = PropertyNameConstants.StdCode;

        public const string Name = RuleNameConstants.StdCode_02;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly ILARSDataService _larsData;

        public StdCode_02Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));

            _messageHandler = validationErrorHandler;
            _larsData = larsData;
        }

        public string RuleName => Name;

        public bool IsValidStandardCode(ILearningDelivery delivery) =>
            _larsData.ContainsStandardFor(delivery.StdCodeNullable.Value);

        public bool HasStandardCode(ILearningDelivery delivery) =>
            It.Has(delivery.StdCodeNullable);

        public bool IsNotValid(ILearningDelivery delivery) =>
            HasStandardCode(delivery) && !IsValidStandardCode(delivery);

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
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery.StdCodeNullable)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}