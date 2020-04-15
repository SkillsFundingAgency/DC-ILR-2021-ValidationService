using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_12Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "AFINTYPE";

        public const string Name = RuleNameConstants.AFinType_12;

        private readonly IValidationErrorHandler _messageHandler;

        public AFinType_12Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsApprenticeship(ILearningDelivery delivery) =>
            delivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;

        public bool IsInAProgramme(ILearningDelivery delivery) =>
            delivery.AimType == TypeOfAim.ProgrammeAim;

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(d => IsApprenticeship(d) && IsInAProgramme(d))
                .ForEach(x =>
                {
                    var failedValidation = !ConditionMet(x);

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        public bool ConditionMet(ILearningDelivery thisDelivery)
        {
            return thisDelivery == null || thisDelivery.AppFinRecords.NullSafeAny(afr => afr.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.AFinType, ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
