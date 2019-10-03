using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode
{
    public class LSDPostcode_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int>() { TypeOfFunding.CommunityLearning, TypeOfFunding.AdultSkills };
        private readonly DateTime _firstAugust2019 = new DateTime(2019, 08, 01);

        private readonly IPostcodesDataService _postcodesDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LSDPostcode_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IPostcodesDataService postcodesDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LSDPostcode_01)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _postcodesDataService = postcodesDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (
                    ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LSDPostcode,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.FundModel, learningDelivery.LSDPostcode));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, int fundModel, int? ProgType, string lsdPostcode, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return 
                LearnStartDateConditionMet(learnStartDate)
                && FundModelConditionMet(fundModel)
                && PostcodeConditionMet(lsdPostcode)
                && !IsExcluded(ProgType, lsdPostcode, learningDeliveryFAMs);
        }
          
        public bool LearnStartDateConditionMet(DateTime learnStartDate) => learnStartDate >= _firstAugust2019;

        public bool FundModelConditionMet(int fundModel) => _fundModels.Contains(fundModel);

        public bool PostcodeConditionMet(string lsdPostcode) => !_postcodesDataService.PostcodeExists(lsdPostcode);

        public bool IsExcluded(int? progType, string lsdPostcode, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return progType.HasValue
                || CaseInsensitiveEquals(lsdPostcode, ValidationConstants.TemporaryPostCode)
                || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int fundModel, string lsdPostcode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, lsdPostcode)
            };
        }
    }
}
