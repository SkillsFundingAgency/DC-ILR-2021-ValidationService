using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.ResolveAnything;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_20Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _learnDelFundModel = FundModels.EuropeanSocialFund;
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

            if (filteredContractAllocations == null || objectToValidate.LearningDeliveries == null)
            { // If there are no Contract Allocations or Learning deliveries then do not progress. No Error
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.FundModel == _learnDelFundModel))
            {
                if (ConditionMet(learningDelivery, filteredContractAllocations))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.ConRefNumber, ukprn, learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, IEnumerable<IFcsContractAllocation> filteredContractAllocations)
        {
            if (learningDelivery.LearningDeliveryFAMs?.Any(ldf => LearningDeliveryFAMTypeConstants.RES.Equals(ldf.LearnDelFAMType, StringComparison.OrdinalIgnoreCase)) == true)
            { // This rule is not triggered by restarts
                return false;
            }

            var latestStopNewStartsDate = filteredContractAllocations
                .Where(a => learningDelivery.ConRefNumber.CaseInsensitiveEquals(a.ContractAllocationNumber))
                .OrderBy(a => a.StartDate)
                .LastOrDefault()
                ?.StopNewStartsFromDate;

            return latestStopNewStartsDate != null && latestStopNewStartsDate <= learningDelivery.LearnStartDate;
        }

        public IList<IFcsContractAllocation> ContractAllocationsForUkprnAndFundingStreamPeriodCodes(int ukprn)
        {
            var contractAllocations = _fcsDataService.GetContractAllocationsFor(ukprn);

            return contractAllocations?.Where(ca => ca != null
            && _fundingStreamPeriodCode.Equals(ca.FundingStreamPeriodCode, StringComparison.OrdinalIgnoreCase)).ToList();
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
