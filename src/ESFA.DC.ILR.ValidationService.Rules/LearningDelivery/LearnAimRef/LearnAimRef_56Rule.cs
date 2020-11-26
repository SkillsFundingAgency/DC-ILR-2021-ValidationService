using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_56Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { FundModels.AdultSkills, FundModels.EuropeanSocialFund, FundModels.OtherAdult };
        private readonly IEnumerable<string> _famCodes = new HashSet<string>() { "034", "328" };
        private readonly DateTime _firstAugust2015 = new DateTime(2015, 08, 01);
        private readonly int _larsCategoryRef = 22;

        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public LearnAimRef_56Rule(ILARSDataService larsDataService, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_56)
        {
            _larsDataService = larsDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (LARSConditionMet(learningDelivery) && ConditionMet(learningDelivery))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearnAimRef, learningDelivery.LearnStartDate, learningDelivery.FundModel));
                }
            }
        }

        public bool LARSConditionMet(ILearningDelivery learningDelivery)
        {
            return _larsDataService.LearnAimRefExistsForLearningDeliveryCategoryRef(learningDelivery.LearnAimRef, _larsCategoryRef);
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return _fundModels.Contains(learningDelivery.FundModel)
                && learningDelivery.LearnStartDate >= _firstAugust2015
                && _learningDeliveryFamQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDelivery.LearningDeliveryFAMs, "LDM", _famCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, DateTime learnStartDate, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
            };
        }
    }
}
