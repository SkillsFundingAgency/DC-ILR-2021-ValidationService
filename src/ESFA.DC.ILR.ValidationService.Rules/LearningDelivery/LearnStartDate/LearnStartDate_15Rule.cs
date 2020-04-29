using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_15Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_22Rule _derivedData22;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnStartDate_15Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_22Rule derivedData22,
            IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_15)
        {
            _derivedData22 = derivedData22;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public DateTime? GetStartFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData22.GetLatestLearningStartForESFContract(thisDelivery, usingSources);

        public bool HasQualifyingStart(ILearningDelivery thisDelivery, DateTime? requiredStart) =>
            !requiredStart.HasValue
            || _dateTimeQueryService.IsDateBetween(thisDelivery.LearnStartDate, requiredStart.Value, DateTime.MaxValue);

        public bool IsNotValid(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            !HasQualifyingStart(thisDelivery, GetStartFor(thisDelivery, usingSources));

        public void Validate(ILearner thisLearner)
        {
            var learnRefNumber = thisLearner.LearnRefNumber;
            var deliveries = thisLearner.LearningDeliveries.ToReadOnlyCollection();

            deliveries
                .ForAny(x => IsNotValid(x, deliveries), x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };
        }
    }
}
