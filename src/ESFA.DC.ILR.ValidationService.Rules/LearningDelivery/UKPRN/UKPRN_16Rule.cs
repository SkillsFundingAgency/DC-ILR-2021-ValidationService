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
    public class UKPRN_16Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _learnDelFundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
        private readonly HashSet<string> _fundingStreamPeriodCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            FundingStreamPeriodCodeConstants.C1618_NLAP2018,
            FundingStreamPeriodCodeConstants.ANLAP2018,
            FundingStreamPeriodCodeConstants.LEVY1799,
            FundingStreamPeriodCodeConstants.NONLEVY2019
        };

        private readonly IFileDataService _fileDataService;
        private readonly IFCSDataService _fcsDataService;

        public UKPRN_16Rule(
            IFileDataService fileDataService,
            IFCSDataService fcsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_16)
        {
            _fileDataService = fileDataService;
            _fcsDataService = fcsDataService;
        }

        public UKPRN_16Rule()
           : base(null, null)
        {
        }

        /* Assumptions made for rule
         * 1. No matching Contract Allocations is OK, this will be caught by another rule
         * 2. There will not be multiple contract assignments with different Stop dates, so any failing contract should flag the violation
         */ 
        public void Validate(ILearner objectToValidate)
        {
            var ukprn = _fileDataService.UKPRN();

            // prepare contract allocations list before iterating the learning deliveries 
            var filteredContractAllocations = ContractAllocationsForUkprnAndFundingStreamPeriodCodes(ukprn);

            if (filteredContractAllocations == null || objectToValidate.LearningDeliveries == null)
            { // If there are no Contract Allocations or Learning deliveries then do not progress. No Error
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.FundModel == _learnDelFundModel))
            {
                if (ConditionMet(learningDelivery.LearnStartDate, filteredContractAllocations))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, ukprn, learningDelivery.LearnStartDate));
                }
            }
        }

        public IEnumerable<IFcsContractAllocation> ContractAllocationsForUkprnAndFundingStreamPeriodCodes(int ukprn)
        {
            var contractAllocations = _fcsDataService.GetContractAllocationsFor(ukprn);

            return contractAllocations?.Where(ca => ca != null && _fundingStreamPeriodCodes.Contains(ca.FundingStreamPeriodCode)).ToList();
        }

        public bool ConditionMet(DateTime learnStartDate, IEnumerable<IFcsContractAllocation> contractAllocations)
        {
            return contractAllocations.Any(ca => (ca.StopNewStartsFromDate ?? DateTime.MaxValue) <= learnStartDate);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int learningDeliveryFundModel, int learningProviderUKPRN, DateTime learningDeliveryStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDeliveryFundModel),
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, learningProviderUKPRN),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDeliveryStartDate)
            };
        }
    }
}
