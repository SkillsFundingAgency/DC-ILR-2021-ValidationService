using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_13Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public LearnStartDate_13Rule(
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_13)
        {
            _larsDataService = larsDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.AimType,
                    learningDelivery.LearnStartDate,
                    learningDelivery.StdCodeNullable,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(int? progType, int aimType, DateTime learnStartDate, int? stdCode, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return !Excluded(learningDeliveryFams)
                && ProgTypeConditionMet(progType)
                && AimTypeConditionMet(aimType)
                && StandardCodeExists(stdCode)
                && LARSConditionMet(stdCode.Value, learnStartDate);
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == TypeOfLearningProgramme.ApprenticeshipStandard;
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == TypeOfAim.ProgrammeAim;
        }

        public bool StandardCodeExists(int? stdCode)
        {
            return stdCode.HasValue;
        }

        public bool LARSConditionMet(int stdCode, DateTime learnStartDate)
        {
            var larsStandard = _larsDataService.GetStandardFor(stdCode);

            if (larsStandard != null)
            {
                return larsStandard.EffectiveTo.HasValue && learnStartDate > larsStandard.EffectiveTo;
            }

            return false;
        }

        public bool Excluded(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFams, Monitoring.Delivery.Types.Restart);
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
