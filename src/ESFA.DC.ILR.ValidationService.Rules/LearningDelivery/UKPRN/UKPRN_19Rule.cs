using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_19Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundModel = TypeOfFunding.AdultSkills;
        private readonly string _learnDelFamType = LearningDeliveryFAMTypeConstants.LDM;
        private readonly string _learnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_ProcuredAdultEducationBudget;

        private readonly HashSet<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.AEB_19TRN1920,
            FundingStreamPeriodCodeConstants.AEB_AS1920
        };

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var ukprn = _fileDataService.UKPRN();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.LearningDeliveryFAMs, learningDelivery.ConRefNumber, learningDelivery.LearnStartDate))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber,
                                          learningDelivery.AimSeqNumber,
                                          BuildErrorMessageParameters(ukprn,
                                                                      learningDelivery.FundModel,
                                                                      _learnDelFamType,
                                                                      _learnDelFAMCode,
                                                                      learningDelivery.LearnStartDate));
                }
            }
        }

        private readonly IFileDataService _fileDataService;
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly IFCSDataService _fcsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public UKPRN_19Rule(
            IFileDataService fileDataService,
            IAcademicYearDataService academicYearDataService,
            IAcademicYearQueryService academicYearQueryService,
            IFCSDataService fcsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_19)
        {
            _fileDataService = fileDataService;
            _academicYearDataService = academicYearDataService;
            _academicYearQueryService = academicYearQueryService;
            _fcsDataService = fcsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public UKPRN_19Rule()
         : base(null, null)
        {

        }

        public bool ConditionMet(int fundModel, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string conRefNumber, DateTime learnStartDate)
        {
            return FundModelConditionMet(fundModel)
                && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)
                && FCTFundingConditionMet()
                && StopNewStartsFromDateConditionMet(conRefNumber, learnStartDate);
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return fundModel == _fundModel;
        }

        public virtual bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, _learnDelFamType, _learnDelFAMCode);
        }

        public virtual bool FCTFundingConditionMet()
        {
            return _fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes);
        }

        public virtual bool StopNewStartsFromDateConditionMet(string conRefNumber, DateTime learnStartDate)
        {
            var contractAllocation = _fcsDataService.GetContractAllocationFor(conRefNumber);
            if (contractAllocation == null)
                return false;

            return contractAllocation.StopNewStartsFromDate != null && learnStartDate >= contractAllocation.StopNewStartsFromDate;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int uKPRN, int fundModel, string learnDelFAMType, string learnDelFAMCode, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, uKPRN),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, _learnDelFamType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, _learnDelFAMCode),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
