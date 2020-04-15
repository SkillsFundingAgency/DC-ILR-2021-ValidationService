using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_02Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "OrigLearnStartDate";

        public const string Name = RuleNameConstants.OrigLearnStartDate_02;

        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly IValidationErrorHandler _messageHandler;

        private readonly HashSet<int> fundModels = new HashSet<int>
        {
            TypeOfFunding.AdultSkills,
            TypeOfFunding.ApprenticeshipsFrom1May2017,
            TypeOfFunding.OtherAdult,
            TypeOfFunding.NotFundedByESFA
        };

        public OrigLearnStartDate_02Rule(IDateTimeQueryService dateTimeQueryService, IValidationErrorHandler validationErrorHandler)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool HasOriginalLearningStartDate(ILearningDelivery delivery) =>
            delivery.OrigLearnStartDateNullable.HasValue;

        public bool HasQualifyingDates(ILearningDelivery delivery) =>
            _dateTimeQueryService.IsDateBetween(delivery.OrigLearnStartDateNullable.Value, DateTime.MinValue, delivery.LearnStartDate, false);

        public bool HasValidFundModel(ILearningDelivery delivery) =>
            fundModels.Contains(delivery.FundModel);

        public bool IsNotValid(ILearningDelivery delivery) =>
            HasOriginalLearningStartDate(delivery) &&
            HasValidFundModel(delivery) &&
            !HasQualifyingDates(delivery);

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
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery.OrigLearnStartDateNullable.Value)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
