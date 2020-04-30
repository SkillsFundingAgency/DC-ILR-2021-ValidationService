using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> larsValidityCategories = new HashSet<string>()
        {
            LARSValidities.AdultSkills,
            LARSValidities.Unemployed,
            LARSValidities.OLASSAdult
        };

        private readonly IDerivedData_07Rule _dd07;
        private readonly ILARSDataService _larsDataService;

        public OrigLearnStartDate_06Rule(
            IDerivedData_07Rule dd07,
            ILARSDataService larsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OrigLearnStartDate_06)
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
                    learningDelivery.LearnAimRef))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.OrigLearnStartDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? origLearnStartDate, int fundModel, int? progType, string learnAimRef)
        {
            return OrigLearnStartDateConditionMet(origLearnStartDate)
                && FundModelConditionMet(fundModel)
                && LARSConditionMet(origLearnStartDate.Value, learnAimRef)
                && !Excluded(progType);
        }

        public bool OrigLearnStartDateConditionMet(DateTime? origLearnStartDate)
        {
            return origLearnStartDate.HasValue;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModels.AdultSkills;
        }

        public bool LARSConditionMet(DateTime origLearnStartDate, string learnAimRef)
        {
            return !_larsDataService.OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(origLearnStartDate, learnAimRef, larsValidityCategories);
        }

        public bool Excluded(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? origLearnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OrigLearnStartDate, origLearnStartDate)
            };
        }
    }
}
