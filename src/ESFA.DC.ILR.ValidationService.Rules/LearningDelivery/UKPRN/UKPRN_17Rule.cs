using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_17Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _check;

        private readonly IFCSDataService _fcsData;

        private readonly HashSet<string> _fundingStreams = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { FundingStreamPeriodCodeConstants.C16_18TRN1920 };

        public UKPRN_17Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IProvideRuleCommonOperations commonOps,
            IFCSDataService fcsDataService)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_17)
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
                && IsTraineeship(theDelivery)
                && HasQualifyingMonitor(theDelivery)
                && HasDisQualifyingFundingRelationship(x => HasStartedAfterStopDate(x, theDelivery));

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.Age16To19ExcludingApprenticeships);

        public bool IsTraineeship(ILearningDelivery theDelivery) =>
            theDelivery.ProgTypeNullable == TypeOfLearningProgramme.Traineeship;

        public bool HasQualifyingMonitor(ILearningDeliveryFAM theMonitor) =>
            Monitoring.Delivery.ESFAAdultFunding.CaseInsensitiveEquals($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}");

        public bool HasQualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, HasQualifyingMonitor);

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
            BuildErrorMessageParameter(PropertyNameConstants.ProgType, theDelivery.ProgTypeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult)
        };
    }
}