using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;
        private readonly ILARSDataService _larsDataService;

        public OrigLearnStartDate_05Rule(
            IDerivedData_07Rule dd07,
            ILARSDataService larsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OrigLearnStartDate_05)
        {
            _dd07 = dd07;
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.OrigLearnStartDateNullable,
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.AimType,
                    learningDelivery.LearnAimRef))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.OrigLearnStartDateNullable, learningDelivery.FundModel, learningDelivery.AimType));
                }
            }
        }

        public bool ConditionMet(DateTime? origLearnStartDate, int fundModel, int? progType, int aimType, string learnAimRef)
        {
            return !Excluded(progType)
                && OrigLearnStartDateConditionMet(origLearnStartDate)
                && FundModelConditionMet(fundModel)
                && DD07ConditionMet(progType)
                && AimTypeConditionMet(aimType)
                && LARSConditionMet(origLearnStartDate.Value, learnAimRef);
        }

        public bool OrigLearnStartDateConditionMet(DateTime? origLearnStartDate)
        {
            return origLearnStartDate.HasValue;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            var fundModels = new[] { 35, 36 };

            return fundModels.Contains(fundModel);
        }

        public bool DD07ConditionMet(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 3;
        }

        public bool LARSConditionMet(DateTime origLearnStartDate, string learnAimRef)
        {
            return !_larsDataService.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSConstants.Validities.Apprenticeships);
        }

        public bool Excluded(int? progType)
        {
            return progType == 25;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? origLearnStartDate, int fundModel, int aimType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OrigLearnStartDate, origLearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType)
            };
        }
    }
}
