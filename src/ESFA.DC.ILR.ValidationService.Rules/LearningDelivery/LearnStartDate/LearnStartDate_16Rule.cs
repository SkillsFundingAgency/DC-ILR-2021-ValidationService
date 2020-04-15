using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
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
    public class LearnStartDate_16Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IFCSDataService _contracts;

        private readonly IProvideRuleCommonOperations _check;

        public LearnStartDate_16Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fcsData,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_16)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fcsData)
                .AsGuard<ArgumentNullException>(nameof(fcsData));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _contracts = fcsData;
            _check = commonOperations;
        }

        public IFcsContractAllocation GetAllocationFor(ILearningDelivery thisDelivery) =>
            _contracts.GetContractAllocationFor(thisDelivery.ConRefNumber);

        public bool HasQualifyingStart(ILearningDelivery thisDelivery, IFcsContractAllocation allocation) =>
            allocation != null
            && allocation.StartDate.HasValue
            && _check.HasQualifyingStart(thisDelivery, allocation.StartDate.Value);

        public bool HasQualifyingAim(ILearningDelivery thisDelivery) =>
            thisDelivery.LearnAimRef.CaseInsensitiveEquals(TypeOfAim.References.ESFLearnerStartandAssessment);

        public bool HasQualifyingModel(ILearningDelivery thisDelivery) =>
            _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.EuropeanSocialFund);

        public bool IsNotValid(ILearningDelivery thisDelivery) =>
            HasQualifyingModel(thisDelivery)
            && HasQualifyingAim(thisDelivery)
            && !HasQualifyingStart(thisDelivery, GetAllocationFor(thisDelivery));

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery) =>
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
        };
    }
}
