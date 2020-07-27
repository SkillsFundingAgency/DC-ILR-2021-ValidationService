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
    public class UKPRN_21Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _learnDelFundModel = FundModels.ApprenticeshipsFrom1May2017;
        private readonly string[] _fundingStreamPeriodCodes = { FundingStreamPeriodCodeConstants.LEVY1799, FundingStreamPeriodCodeConstants.NONLEVY2019 };

        private readonly IFileDataService _fileDataService;
        private readonly IFCSDataService _fcsDataService;

        public UKPRN_21Rule(
            IFileDataService fileDataService,
            IFCSDataService fcsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_21)
        {
            _fileDataService = fileDataService;
            _fcsDataService = fcsDataService;
        }

        public UKPRN_21Rule()
           : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            { // If there are no Learning deliveries then do not progress. No Error
                return;
            }

            var ukprn = _fileDataService.UKPRN();

            // prepare contract allocations list before iterating the learning deliveries
            var filteredContractAllocations = ContractAllocationsForUkprnAndFundingStreamPeriodCodes(ukprn);

            if (filteredContractAllocations == null)
            { // If there are no Contract Allocations then do not progress. No Error
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
                .OrderBy(a => a.StartDate)
                .LastOrDefault()
                ?.StopNewStartsFromDate;

            return latestStopNewStartsDate != null && latestStopNewStartsDate <= learningDelivery.LearnStartDate;
        }

        public IList<IFcsContractAllocation> ContractAllocationsForUkprnAndFundingStreamPeriodCodes(int ukprn)
        {
            var contractAllocations = _fcsDataService.GetContractAllocationsFor(ukprn);

            return contractAllocations
                ?.Where(ca => ca != null && _fundingStreamPeriodCodes.Contains(ca.FundingStreamPeriodCode, StringComparer.OrdinalIgnoreCase))
                .ToList();
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
