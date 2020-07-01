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
    public class UKPRN_12Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _learnDelFamType = LearningDeliveryFAMTypeConstants.LDM;
        private readonly string _learnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_ProcuredAdultEducationBudget;
        private readonly HashSet<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.AEB_19TRN2021,
            FundingStreamPeriodCodeConstants.AEB_AS2021,
            FundingStreamPeriodCodeConstants.STFI2021
        };

        private readonly DateTime _firstNov2017 = new DateTime(2017, 11, 01);

        private readonly IFileDataService _fileDataService;
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly IFCSDataService _fcsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public UKPRN_12Rule(
            IFileDataService fileDataService,
            IAcademicYearDataService academicYearDataService,
            IAcademicYearQueryService academicYearQueryService,
            IFCSDataService fcsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_12)
        {
            _fileDataService = fileDataService;
            _academicYearDataService = academicYearDataService;
            _academicYearQueryService = academicYearQueryService;
            _fcsDataService = fcsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public UKPRN_12Rule()
           : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var ukprn = _fileDataService.UKPRN();
            var academicYearStart = _academicYearDataService.Start();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    academicYearStart,
                    learningDelivery.LearnActEndDateNullable,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, ukprn, _learnDelFamType, _learnDelFAMCode));
                }
            }
        }

        public bool ConditionMet(
            DateTime learnStartDate,
            DateTime academicYearStart,
            DateTime? learnActEndDate,
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return (learnActEndDate == null || LearnActEndDateConditionMet(learnActEndDate.Value, academicYearStart))
                && (learningDeliveryFAMs != null && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs))
                && LearnStartDateConditionMet(learnStartDate)
                && FCTFundingConditionMet();
        }

        public virtual bool LearnActEndDateConditionMet(DateTime learnActEndDate, DateTime academicYearStart)
        {
            return !_academicYearQueryService.DateIsInPrevAcademicYear(learnActEndDate, academicYearStart);
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _firstNov2017;
        }

        public virtual bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, _learnDelFamType, _learnDelFAMCode);
        }

        public virtual bool FCTFundingConditionMet()
        {
            return !_fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            DateTime learnStartDate,
            int ukprn,
            string learningDelFAMType,
            string learningDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learningDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learningDelFAMCode)
            };
        }
    }
}
