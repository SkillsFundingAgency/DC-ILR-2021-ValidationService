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
    public class LearnStartDate_14Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly IDerivedData_18Rule _derivedData18;

        public LearnStartDate_14Rule(
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IDerivedData_18Rule derivedData18,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_14)
        {
            _larsDataService = larsDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
            _derivedData18 = derivedData18;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                var dd18Date = _derivedData18.GetApprenticeshipStandardProgrammeStartDateFor(learningDelivery, objectToValidate.LearningDeliveries);

                if (dd18Date == null)
                {
                    continue;
                }

                if (ConditionMet(
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.AimType,
                    learningDelivery.StdCodeNullable,
                    dd18Date.Value,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.StdCodeNullable));
                }
            }
        }

        public bool ConditionMet(int? progType, int aimType, int? stdCode, DateTime dd18Date, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return !Excluded(learningDeliveryFams)
                && ProgTypeConditionMet(progType)
                && AimTypeConditionMet(aimType)
                && StandardCodeExists(stdCode)
                && LARSConditionMet(stdCode.Value, dd18Date);
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == TypeOfLearningProgramme.ApprenticeshipStandard;
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == AimTypes.ComponentAimInAProgramme;
        }

        public bool StandardCodeExists(int? stdCode)
        {
            return stdCode.HasValue;
        }

        public bool LARSConditionMet(int stdCode, DateTime dd18Date)
        {
            var larsStandard = _larsDataService.GetStandardFor(stdCode);

            if (larsStandard != null)
            {
                return larsStandard.EffectiveTo.HasValue && dd18Date > larsStandard.EffectiveTo;
            }

            return false;
        }

        public bool Excluded(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFams, Monitoring.Delivery.Types.Restart);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? stdCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, stdCode)
            };
        }
    }
}
