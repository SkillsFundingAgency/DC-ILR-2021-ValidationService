using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_86Rule : IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnAimRef_86;

        private readonly IValidationErrorHandler _messageHandler;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnAimRef_86Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
        {
            _messageHandler = validationErrorHandler;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public string RuleName => Name;

        public bool IsWorkExperience(ILearningDelivery delivery) =>
            delivery.LearnAimRef.CaseInsensitiveEquals(TypeOfAim.References.WorkExperience);

        public bool IsNotValid(ILearningDelivery delivery) =>
            delivery.FundModel == TypeOfFunding.AdultSkills
            && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                delivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy)
            && delivery.AimType != TypeOfAim.ProgrammeAim
            && IsWorkExperience(delivery);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.FundModel), thisDelivery.FundModel),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.ProgType, thisDelivery.ProgTypeNullable)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
