﻿using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_18Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2020 = new DateTime(2020, 8, 1);

        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnStartDate_18Rule(
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_18)
        {
            _larsDataService = larsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery))
                {
                    HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, BuildMessageParametersFor(learningDelivery));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return LearnStartDateConditionMet(learningDelivery.LearnStartDate)
                   && ProgTypeConditionMet(learningDelivery.ProgTypeNullable)
                   && AimTypeConditionMet(learningDelivery.AimType)
                   && StdCodeExists(learningDelivery.StdCodeNullable)
                   && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)
                   && LARSConditionMet(learningDelivery.StdCodeNullable.Value, learningDelivery.LearnStartDate);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate) => learnStartDate >= _firstAugust2020;

        public bool ProgTypeConditionMet(int? progType) =>
            progType == ProgTypes.ApprenticeshipStandard;

        public bool AimTypeConditionMet(int aimType) =>
            aimType == AimTypes.ProgrammeAim;

        public bool StdCodeExists(int? stdCode) =>
            stdCode.HasValue;

        public bool LARSConditionMet(int stdCode, DateTime learnStartDate)
        {
            var larsStandard = _larsDataService.GetStandardFor(stdCode);

            if (larsStandard != null)
            {
                return learnStartDate > larsStandard.LastDateStarts;
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, thisDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, thisDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, thisDelivery.StdCodeNullable)
            };
        }
    }
}
