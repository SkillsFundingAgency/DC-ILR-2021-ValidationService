using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    /// <summary>
    /// learn start date rule 16
    /// </summary>
    public class LearnStartDate_16Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The fcs contract data provider
        /// </summary>
        private readonly IFCSDataService _contracts;

        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnStartDate_16Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="fcsData">The FCS data provider.</param>
        /// <param name="commonOperations">The common operations.</param>
        public LearnStartDate_16Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fcsData,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_16)
        {
            // this check should be in the base class
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fcsData)
                .AsGuard<ArgumentNullException>(nameof(fcsData));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _contracts = fcsData;
            _check = commonOperations;
        }

        /// <summary>
        /// Gets the allocation for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>a conttract allocation</returns>
        public IFcsContractAllocation GetAllocationFor(ILearningDelivery thisDelivery) =>
            _contracts.GetContractAllocationFor(thisDelivery.ConRefNumber);

        /// <summary>
        /// Determines whether [has qualifying start] [the specified delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="allocation">The allocation.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery thisDelivery, IFcsContractAllocation allocation) =>
            It.Has(allocation)
            && It.Has(allocation.StartDate)
            && _check.HasQualifyingStart(thisDelivery, allocation.StartDate.Value);

        /// <summary>
        /// Determines whether [has qualifying aim] [the specified delivery].
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingAim(ILearningDelivery thisDelivery) =>
            It.IsInRange(thisDelivery.LearnAimRef, TypeOfAim.References.ESFLearnerStartandAssessment);

        /// <summary>
        /// Determines whether [has qualifying model] [the specified delivery].
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery thisDelivery) =>
            _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.EuropeanSocialFund);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisDelivery) =>
            HasQualifyingModel(thisDelivery)
            && HasQualifyingAim(thisDelivery)
            && !HasQualifyingStart(thisDelivery, GetAllocationFor(thisDelivery));

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery) =>
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));

        /// <summary>
        /// Builds the error message parameters.
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
        };
    }
}
