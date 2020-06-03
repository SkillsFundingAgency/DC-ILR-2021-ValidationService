using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_53Rule : AbstractRule, IRule<ILearner>
    {
        private readonly List<string> _fundingStreamPeriodCodes = new List<string>
        {
            FundingStreamPeriodCodeConstants.ALLBC2021
        };

        private readonly IFCSDataService _fcsDataService;
        private readonly IFileDataService _fileDataService;

        public LearnDelFAMType_53Rule(
            IFCSDataService fcsDataService,
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_53)
        {
            _fcsDataService = fcsDataService;
            _fileDataService = fileDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var ukprn = _fileDataService.UKPRN();
            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                foreach (var learnDelFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (ConditionMet(learnDelFam))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learnDelFam, ukprn));
                    }
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryFAM learningDeliveryFAM)
        {
            return LearningDeliveryFAMsConditionMet(learningDeliveryFAM) && FCTFundingConditionMet();
        }

        public virtual bool LearningDeliveryFAMsConditionMet(ILearningDeliveryFAM learningDeliveryFAM)
        {
            return (learningDeliveryFAM?.LearnDelFAMType == LearningDeliveryFAMTypeConstants.ALB && learningDeliveryFAM.LearnDelFAMCode.Equals("1"))
                   || (learningDeliveryFAM?.LearnDelFAMType == LearningDeliveryFAMTypeConstants.ALB && learningDeliveryFAM.LearnDelFAMCode.Equals("3"));
        }

        public virtual bool FCTFundingConditionMet()
        {
            return _fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDeliveryFAM learnDelFam, int ukprn)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFam.LearnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFam.LearnDelFAMCode)
            };
        }
    }
}
