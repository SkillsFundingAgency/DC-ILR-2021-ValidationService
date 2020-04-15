using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_19Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _check;

        private readonly IFCSDataService _fcsData;

        private readonly HashSet<string> _fundingStreams = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            FundingStreamPeriodCodeConstants.AEB_19TRN1920,
            FundingStreamPeriodCodeConstants.AEB_AS1920
        };

        public UKPRN_19Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IProvideRuleCommonOperations commonOps,
            IFCSDataService fcsDataService)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_19)
        {
            ProviderUKPRN = fileDataService.UKPRN();
            _check = commonOps;
            _fcsData = fcsDataService;
        }

        public int ProviderUKPRN { get; }

        public void Validate(ILearner theLearner)
        {
            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public bool IsNotValid(ILearningDelivery theDelivery) =>
            HasQualifyingModel(theDelivery)
                && HasQualifyingMonitor(theDelivery, IsESFAAdultFunding)
                && HasQualifyingMonitor(theDelivery, IsAdultEducationBudgets)
                && HasDisQualifyingFundingRelationship(x => HasStartedAfterStopDate(x, theDelivery));

        public bool IsAdultEducationBudgets(ILearningDeliveryFAM theMonitor) =>
            Monitoring.Delivery.AdultEducationBudgets.CaseInsensitiveEquals($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}");

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.AdultSkills);

        public bool IsESFAAdultFunding(ILearningDeliveryFAM theMonitor) =>
            Monitoring.Delivery.ESFAAdultFunding.CaseInsensitiveEquals($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}");

        public bool HasQualifyingMonitor(ILearningDelivery theDelivery, Func<ILearningDeliveryFAM, bool> docheck) =>
            _check.CheckDeliveryFAMs(theDelivery, docheck);

        public bool HasDisQualifyingFundingRelationship(Func<IFcsContractAllocation, bool> hasStartedAfterStopDate) =>
            _fcsData
                .GetContractAllocationsFor(ProviderUKPRN)
                .NullSafeAny(x => HasFundingRelationship(x) && hasStartedAfterStopDate(x));

        public bool HasFundingRelationship(IFcsContractAllocation theAllocation) =>
            _fundingStreams.Contains(theAllocation.FundingStreamPeriodCode);

        public bool HasStartedAfterStopDate(IFcsContractAllocation theAllocation, ILearningDelivery theDelivery) =>
            theDelivery.LearnStartDate >= theAllocation.StopNewStartsFromDate;

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ProviderUKPRN),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.Learning),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_ProcuredAdultEducationBudget),
        };
    }
}
