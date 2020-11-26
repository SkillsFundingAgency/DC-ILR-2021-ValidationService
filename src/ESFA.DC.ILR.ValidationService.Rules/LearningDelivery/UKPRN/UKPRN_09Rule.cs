using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_09Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int> { FundModels.ApprenticeshipsFrom1May2017, FundModels.NotFundedByESFA };
        private readonly HashSet<string> _fundingStreamPeriodCodes = new HashSet<string> { FundingStreamPeriodCodeConstants.APPS2021 };
        private readonly string _learnDelFamType = LearningDeliveryFAMTypeConstants.LDM;
        private readonly HashSet<string> _learnDelFamCodes = new HashSet<string> { "034", "353", "354", "355" };

        private readonly IFileDataService _fileDataService;
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IFCSDataService _fcsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public UKPRN_09Rule(
            IFileDataService fileDataService,
            IAcademicYearDataService academicYearDataService,
            IAcademicYearQueryService academicYearQueryService,
            IFCSDataService fcsDataService,
            IDerivedData_07Rule dd07,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_09)
        {
            _fileDataService = fileDataService;
            _academicYearDataService = academicYearDataService;
            _academicYearQueryService = academicYearQueryService;
            _dd07 = dd07;
            _fcsDataService = fcsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public UKPRN_09Rule()
           : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var ukprn = _fileDataService.UKPRN();
            var academicYearStart = _academicYearDataService.Start();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => !_fundModels.Contains(d.FundModel)))
            {
                if (ConditionMet(learningDelivery.ProgTypeNullable,  academicYearStart, learningDelivery.LearnActEndDateNullable, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(ukprn, learningDelivery.FundModel, learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(int? progType, DateTime academicYearStart, DateTime? learnActEndDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return DD07ConditionMet(progType)
                && FCTFundingConditionMet()
                && LearnActEndDateConditionMet(learnActEndDate, academicYearStart)
                && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);
        }

        public virtual bool DD07ConditionMet(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public virtual bool FCTFundingConditionMet()
        {
            return !_fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes);
        }

        public virtual bool LearnActEndDateConditionMet(DateTime? learnActEndDate, DateTime academicYearStart)
        {
            return learnActEndDate == null ? true : !_academicYearQueryService.DateIsInPrevAcademicYear(learnActEndDate.Value, academicYearStart);
        }

        public virtual bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, _learnDelFamType, _learnDelFamCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int ukprn, int fundModel, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)
            };
        }
    }
}
