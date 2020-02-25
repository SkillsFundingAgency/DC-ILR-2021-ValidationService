using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fcsDataService;
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly int _fundModel = TypeOfFunding.EuropeanSocialFund;
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.ESF1420
        };

        public UKPRN_05Rule(
            IFCSDataService fcsDataService,
            IAcademicYearDataService academicYearDataService,
            IAcademicYearQueryService academicYearQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_05)
        {
            _fcsDataService = fcsDataService;
            _academicYearDataService = academicYearDataService;
            _academicYearQueryService = academicYearQueryService;
        }

        public UKPRN_05Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var academicYearStart = _academicYearDataService.Start();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    academicYearStart,
                    learningDelivery.LearnActEndDateNullable,
                    learningDelivery.ConRefNumber))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.ConRefNumber));
                }
            }
        }

        public bool ConditionMet(int fundModel, DateTime academicYearStart, DateTime? learnActEndDate, string conRefNumber)
        {
            return FundModelConditionMet(fundModel)
                && (!learnActEndDate.HasValue || LearnActEndDateConditionMet(learnActEndDate.Value, academicYearStart))
                && FCTFundingConditionMet(conRefNumber);
        }

        public virtual bool FundModelConditionMet(int fundModel) => fundModel == _fundModel;

        public virtual bool LearnActEndDateConditionMet(DateTime learnActEndDate, DateTime academicYearStart)
        {
            return !_academicYearQueryService.DateIsInPrevAcademicYear(learnActEndDate, academicYearStart);
        }

        public virtual bool FCTFundingConditionMet(string conRefNumber)
        {
            return !_fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)
                || !_fcsDataService.ConRefNumberExists(conRefNumber);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string conRefNumber)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, conRefNumber)
            };
        }
    }
}
