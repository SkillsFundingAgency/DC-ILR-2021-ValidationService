using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_14Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The checker (common rule operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The fcs data (service)
        /// </summary>
        private readonly IFCSDataService _fcsData;

        /// <summary>
        /// derived data rule 22
        /// </summary>
        private readonly IDerivedData_26Rule _ddrule26;

        /// <summary>
        /// Initializes a new instance of the <see cref="ESMType_14Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="ddrule26">derived data rule 26.</param>
        /// <param name="fcsData">The lars data.</param>
        /// <param name="commonOperations">The common operations provider.</param>
        public ESMType_14Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_26Rule ddrule26,
            IFCSDataService fcsData,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.ESMType_14)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(ddrule26)
                .AsGuard<ArgumentNullException>(nameof(ddrule26));
            It.IsNull(fcsData)
                .AsGuard<ArgumentNullException>(nameof(fcsData));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _ddrule26 = ddrule26;
            _fcsData = fcsData;
            _check = commonOperations;
        }

        /// <summary>
        /// Gets the eligibility rule for (this delivery).
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>an eligibility rule</returns>
        public IEsfEligibilityRule GetEligibilityRuleFor(ILearningDelivery thisDelivery) =>
            _fcsData.GetEligibilityRuleFor(thisDelivery.ConRefNumber);

        /// <summary>
        /// Gets the derived rule benefits indicator for (this learner and this delivery).
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>the value of the benefits indicator</returns>
        public bool GetDerivedRuleBenefitsIndicatorFor(ILearner thisLearner, ILearningDelivery thisDelivery) =>
            _ddrule26.LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(thisLearner, thisDelivery.ConRefNumber);

        /// <summary>
        /// Determines whether [has matching benefits indicator] [the specified eligibility].
        /// </summary>
        /// <param name="eligibility">The eligibility.</param>
        /// <param name="derivedRuleResult">if set to <c>true</c> [derived rule result].</param>
        /// <returns>
        ///   <c>true</c> if [has matching benefits indicator] [the specified eligibility]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingBenefitsIndicator(IEsfEligibilityRule eligibility, bool derivedRuleResult) =>
            eligibility == null || eligibility.Benefits == null || eligibility.Benefits == derivedRuleResult;

        /// <summary>
        /// Determines whether [has matching benefits indicator] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <param name="derivedRuleAction">The derived rule action.</param>
        /// <returns>
        ///   <c>true</c> if [has matching benefits indicator] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingBenefitsIndicator(ILearningDelivery thisDelivery, Func<bool> derivedRuleAction) =>
            HasMatchingBenefitsIndicator(GetEligibilityRuleFor(thisDelivery), derivedRuleAction());

        /// <summary>
        /// Determines whether [is not valid] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="derivedRuleAction">The derived rule action.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisDelivery, Func<bool> derivedRuleAction) =>
            _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.EuropeanSocialFund)
                && !HasMatchingBenefitsIndicator(thisDelivery, derivedRuleAction);

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;

            thisLearner.LearningDeliveries
                .ForAny(x => IsNotValid(x, () => GetDerivedRuleBenefitsIndicatorFor(thisLearner, x)), x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(thisDelivery));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, thisDelivery.ConRefNumber)
            };
        }
    }
}