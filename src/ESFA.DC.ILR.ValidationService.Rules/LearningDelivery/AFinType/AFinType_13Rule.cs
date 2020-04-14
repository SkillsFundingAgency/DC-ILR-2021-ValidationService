using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_13Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "AFINDATE";

        public const string Name = RuleNameConstants.AFinType_13;

        private readonly IValidationErrorHandler _messageHandler;

        public AFinType_13Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsApprenticeshipFunded(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.ApprenticeshipsFrom1May2017);

        public bool IsInAProgramme(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ProgrammeAim);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(d => IsApprenticeshipFunded(d) && IsInAProgramme(d))
                .ForEach(x =>
                {
                    var failedValidation = !x.AppFinRecords
                        .NullSafeWhere(afr => afr.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice))
                        .Any(y => ConditionMet(x, y));

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        public bool ConditionMet(ILearningDelivery thisDelivery, IAppFinRecord thisFinancialRecord)
        {
            return It.Has(thisDelivery) && It.Has(thisFinancialRecord)
                ? thisFinancialRecord.AFinDate > DateTime.MinValue
                    && thisDelivery.LearnStartDate == thisFinancialRecord.AFinDate
                : true;
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.AFinType, ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.AFinDate, string.Empty)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
