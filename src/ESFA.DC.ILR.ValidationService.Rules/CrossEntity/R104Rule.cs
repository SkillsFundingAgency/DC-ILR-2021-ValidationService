﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R104Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _famTypeACT = Monitoring.Delivery.Types.ApprenticeshipContract;

        public R104Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R104)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                var learningDeliveryFAMs = learningDelivery?.LearningDeliveryFAMs?.Where(ldf => ldf.LearnDelFAMType == _famTypeACT).ToList();

                if (DoesNotHaveMultipleACTFams(learningDeliveryFAMs))
                {
                    return;
                }

                var invalidLearningDeliveryFAMs = LearningDeliveryFamForOverlappingACTTypes(learningDeliveryFAMs);

                if (invalidLearningDeliveryFAMs.Count() > 0)
                {
                    foreach (var learningDeliveryFAM in invalidLearningDeliveryFAMs)
                    {
                        HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(
                            learningDelivery.LearnPlanEndDate,
                            learningDelivery.LearnActEndDateNullable,
                            _famTypeACT,
                            learningDeliveryFAM.LearnDelFAMDateFromNullable,
                            learningDeliveryFAM.LearnDelFAMDateToNullable));
                    }
                }
            }
        }

        public bool DoesNotHaveMultipleACTFams(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return
                learningDeliveryFAMs != null
                ? learningDeliveryFAMs.Count() < 2
                : true;
        }

        public IEnumerable<ILearningDeliveryFAM> LearningDeliveryFamForOverlappingACTTypes(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            var invalidLearningDeliveryFAMs = new List<ILearningDeliveryFAM>();

            if (learningDeliveryFAMs != null)
            {
                var ldFAMs = learningDeliveryFAMs.OrderBy(ld => ld.LearnDelFAMDateFromNullable).ToArray();

                var ldFAMsCount = ldFAMs.Length;

                var i = 1;

                while (i < ldFAMsCount)
                {
                    if (ldFAMs[i - 1].LearnDelFAMDateToNullable == null)
                    {
                        invalidLearningDeliveryFAMs.Add(ldFAMs[i]);
                        i++;

                        continue;
                    }

                    var errorConditionMet =
                        ldFAMs[i].LearnDelFAMDateFromNullable == null
                        ? false
                        : ldFAMs[i - 1].LearnDelFAMDateToNullable >= ldFAMs[i].LearnDelFAMDateFromNullable;

                    if (errorConditionMet)
                    {
                        invalidLearningDeliveryFAMs.Add(ldFAMs[i]);
                        i++;

                        continue;
                    }

                    i++;
                }
            }

            return invalidLearningDeliveryFAMs;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnPlanEndDate, DateTime? learnActEndDate, string famType, DateTime? learnDelFamDateFrom, DateTime? learnDelFamDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, learnDelFamDateFrom),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFamDateTo)
            };
        }
    }
}
