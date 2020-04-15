using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_15Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IDerivedData_22Rule _derivedData22;

        private readonly IProvideRuleCommonOperations _check;

        public LearnStartDate_15Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_22Rule derivedData22,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_15)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData22)
                .AsGuard<ArgumentNullException>(nameof(derivedData22));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _derivedData22 = derivedData22;
            _check = commonOperations;
        }

        public DateTime? GetStartFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData22.GetLatestLearningStartForESFContract(thisDelivery, usingSources);

        public bool HasQualifyingStart(ILearningDelivery thisDelivery, DateTime? requiredStart) =>
            !requiredStart.HasValue
            || _check.HasQualifyingStart(thisDelivery, requiredStart.Value);

        public bool IsNotValid(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            !HasQualifyingStart(thisDelivery, GetStartFor(thisDelivery, usingSources));

        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

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
