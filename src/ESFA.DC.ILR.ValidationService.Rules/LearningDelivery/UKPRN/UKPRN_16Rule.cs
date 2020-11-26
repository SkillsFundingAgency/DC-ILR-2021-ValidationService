using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_16Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _fundingStreamPeriodCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            FundingStreamPeriodCodeConstants.C1618_NLAP2018,
            FundingStreamPeriodCodeConstants.ANLAP2018
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

            var filteredContractAllocations = ContractAllocationsForUkprnAndFundingStreamPeriodCodes(ukprn);

            if (filteredContractAllocations == null || objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            var filterLearningDeliveries = MatchingLearningDeliveries(objectToValidate.LearningDeliveries);

            foreach (var learningDelivery in filterLearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnStartDate, filteredContractAllocations))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, ukprn, learningDelivery.LearnStartDate));
                }
            }
        }

        public IEnumerable<ILearningDelivery> MatchingLearningDeliveries(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries
                .Where(d => d.FundModel == FundModels.ApprenticeshipsFrom1May2017
                            && d.AimType == AimTypes.ProgrammeAim
                            && d.LearningDeliveryFAMs != null
                            && d.LearningDeliveryFAMs.Any(ldf =>
                                                            ldf.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMCodeConstants.ACT_ContractESFA)
                                                            && ldf.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ACT))
                            && !d.LearningDeliveryFAMs.Any(ldf => ldf.LearnDelFAMType == LearningDeliveryFAMTypeConstants.RES)); // Exclusion
        }

        public IEnumerable<IFcsContractAllocation> ContractAllocationsForUkprnAndFundingStreamPeriodCodes(int ukprn)
        {
            var contractAllocations = _fcsDataService.GetContractAllocationsFor(ukprn);

            return contractAllocations?.Where(ca => ca != null && _fundingStreamPeriodCodes.Contains(ca.FundingStreamPeriodCode)).ToList();
        }

        public bool ConditionMet(DateTime learnStartDate, IEnumerable<IFcsContractAllocation> contractAllocations)
        {
            var latestStopNewStartsFromDate = contractAllocations?
                .OrderByDescending(ca => ca.StartDate)
                .FirstOrDefault()
                ?.StopNewStartsFromDate;

            return latestStopNewStartsFromDate != null && learnStartDate >= latestStopNewStartsFromDate;
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
