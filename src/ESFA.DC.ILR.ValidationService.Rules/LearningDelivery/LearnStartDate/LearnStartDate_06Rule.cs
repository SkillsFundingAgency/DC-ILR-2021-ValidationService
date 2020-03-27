using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnStartDate_06Rule(
            IDerivedData_07Rule dd07,
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_06)
        {
            _dd07 = dd07;
            _larsDataService = larsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.AimType,
                    learningDelivery.LearnStartDate,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, int? progType, int aimType, DateTime learnStartDate, int? fWorkCode, int? pwayCode, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return ApprenticeshipConditionMet(progType)
                && AimTypeConditionMet(aimType)
                && FrameworkConditionMet(learnAimRef, learnStartDate, progType, fWorkCode, pwayCode)
                && !Excluded(progType, learningDeliveryFAMs);
        }

        public bool ApprenticeshipConditionMet(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool FrameworkConditionMet(string learnAimRef, DateTime learnStartDate, int? progType, int? fWorkCode, int? pwayCode)
        {
            return _larsDataService.LearnStartDateGreaterThanFrameworkEffectiveTo(learnAimRef, learnStartDate, progType, fWorkCode, pwayCode);
        }

        public bool Excluded(int? progType, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return progType == 25
                || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
