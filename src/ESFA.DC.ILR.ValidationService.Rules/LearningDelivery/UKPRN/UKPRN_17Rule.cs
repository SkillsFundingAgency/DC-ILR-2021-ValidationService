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
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_17Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IFCSDataService _fcsData;

        public UKPRN_17Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IFCSDataService fcsDataService)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_17)
        {
            ProviderUKPRN = fileDataService.UKPRN();
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
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
            HasNoRestartFamType(theDelivery.LearningDeliveryFAMs)
            && HasQualifyingModel(theDelivery)
            && IsTraineeship(theDelivery)
            && IsESFAAdultFunding(theDelivery)
            && HasDisQualifyingFundingRelationship(x => HasStartedAfterStopDate(x, theDelivery));

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            theDelivery.FundModel == FundModels.Age16To19ExcludingApprenticeships;

        public bool IsTraineeship(ILearningDelivery theDelivery) =>
            theDelivery.ProgTypeNullable == ProgTypes.Traineeship;

        public bool IsESFAAdultFunding(ILearningDelivery theDelivery) =>
             _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                 theDelivery.LearningDeliveryFAMs,
                 LearningDeliveryFAMTypeConstants.SOF,
                 LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult);

        public bool HasDisQualifyingFundingRelationship(Func<IFcsContractAllocation, bool> hasStartedAfterStopDate)
        {
            var fcsRecord = _fcsData
                .GetContractAllocationsFor(ProviderUKPRN)
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefault();

            if (fcsRecord == null)
            {
                return false;
            }

            return HasFundingRelationship(fcsRecord) && hasStartedAfterStopDate(fcsRecord);
        }

        public bool HasFundingRelationship(IFcsContractAllocation theAllocation) =>
            theAllocation.FundingStreamPeriodCode.CaseInsensitiveEquals(FundingStreamPeriodCodeConstants.C16_18TRN2021);

        public bool HasStartedAfterStopDate(IFcsContractAllocation theAllocation, ILearningDelivery theDelivery) =>
            theDelivery.LearnStartDate >= theAllocation.StopNewStartsFromDate;

        public bool HasNoRestartFamType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams) =>
            !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.RES);

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