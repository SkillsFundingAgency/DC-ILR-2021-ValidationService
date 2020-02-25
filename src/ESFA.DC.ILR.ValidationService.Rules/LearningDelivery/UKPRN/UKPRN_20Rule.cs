using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_20Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _learnDelFundModel = TypeOfFunding.EuropeanSocialFund;
        private readonly string _fundingStreamPeriodCode = FundingStreamPeriodCodeConstants.ESF1420;

        private readonly IFileDataService _fileDataService;
        private readonly IFCSDataService _fcsDataService;

        public UKPRN_20Rule(
            IFileDataService fileDataService,
            IFCSDataService fcsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_20)
        {
            _fileDataService = fileDataService;
            _fcsDataService = fcsDataService;
        }

        public UKPRN_20Rule()
           : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var ukprn = _fileDataService.UKPRN();

            // prepare contract allocations list before iterating the learning deliveries 
            var filteredContractAllocations = ContractAllocationsForUkprnAndFundingStreamPeriodCodes(ukprn);

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.FundModel == _learnDelFundModel))
            {
                if (ConditionMet(learningDelivery.ConRefNumber, learningDelivery.LearnStartDate, filteredContractAllocations))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.ConRefNumber, ukprn, learningDelivery.LearnStartDate));
                }
            }
        }

        public IEnumerable<IFcsContractAllocation> ContractAllocationsForUkprnAndFundingStreamPeriodCodes(int ukprn)
        {
            var contractAllocations = _fcsDataService.GetContractAllocationsFor(ukprn);

            return contractAllocations.Where(ca => _fundingStreamPeriodCode.Equals(ca.FundingStreamPeriodCode, StringComparison.OrdinalIgnoreCase)).ToList();
        }


        public bool ConditionMet(string learnConRef, DateTime learnStartDate, IEnumerable<IFcsContractAllocation> contractAllocations)
        {
            return contractAllocations.Any(ca =>
                learnConRef.Equals(ca.ContractAllocationNumber, StringComparison.OrdinalIgnoreCase) &&
                (ca.StopNewStartsFromDate ?? DateTime.MaxValue) <= learnStartDate);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int learningDeliveryFundModel, string learningDeliveryConRefNumber, int learningProviderUKPRN, DateTime learningDeliveryStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDeliveryFundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, learningDeliveryConRefNumber),
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, learningProviderUKPRN),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDeliveryStartDate)
            };
        }
    }
}
