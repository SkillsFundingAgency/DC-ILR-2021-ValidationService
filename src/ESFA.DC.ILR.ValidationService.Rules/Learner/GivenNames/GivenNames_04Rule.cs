using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.GivenNames
{
    public class GivenNames_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public GivenNames_04Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.GivenNames_04)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.GivenNames, objectToValidate.PlanLearnHoursNullable, objectToValidate.ULN, objectToValidate.LearningDeliveries))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.GivenNames, objectToValidate.PlanLearnHoursNullable));
            }
        }

        public bool ConditionMet(string givenNames, int? planLearnHours, long uln, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return GivenNamesConditionMet(givenNames)
                && PlanLearnHoursConditionMet(planLearnHours)
                && ULNConditionMet(uln)
                && CrossLearningDeliveryConditionMet(learningDeliveries);
        }

        public bool GivenNamesConditionMet(string givenNames)
        {
            return string.IsNullOrWhiteSpace(givenNames);
        }

        public bool PlanLearnHoursConditionMet(int? planLearnHours)
        {
            return planLearnHours.HasValue && planLearnHours <= 10;
        }

        public bool ULNConditionMet(long uln)
        {
            return uln != ValidationConstants.TemporaryULN;
        }

        public bool CrossLearningDeliveryConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries.All(ld =>
                ld.FundModel == FundModels.CommunityLearning
                || (ld.FundModel == FundModels.NotFundedByESFA && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(ld.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, LearningDeliveryFAMCodeConstants.SOF_LA)));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string givenNames, int? planLearnHours)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.GivenNames, givenNames),
                BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, planLearnHours)
            };
        }
    }
}