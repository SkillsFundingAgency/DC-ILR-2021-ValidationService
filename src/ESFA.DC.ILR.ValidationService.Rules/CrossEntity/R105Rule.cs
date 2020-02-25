using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R105Rule : AbstractRule, IRule<ILearner>
    {
        public R105Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R105)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            string learnDelFAMType = string.Empty;
            string learnDelFAMCode = string.Empty;

            var learningDeliverFAMs = objectToValidate.LearningDeliveries
                .Where(ld => ld.LearningDeliveryFAMs != null)
                .SelectMany(ld => ld.LearningDeliveryFAMs)
                .Where(f => f.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ACT))
                .OrderBy(f => f.LearnDelFAMDateFromNullable).ToList();

            if (learningDeliverFAMs.Count > 1
                && LearnDelFAMCodeConditionMet(
                    learningDeliverFAMs,
                    out learnDelFAMType,
                    out learnDelFAMCode))
            {
                HandleValidationError(
                    learnRefNumber: objectToValidate.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(
                        learnDelFAMType,
                        learnDelFAMCode));
            }
        }

        public bool LearnDelFAMCodeConditionMet(
            IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs,
            out string learnDelFAMType,
            out string learnDelFAMCode)
        {
            int record = 1;
            bool isConditionMet = false;
            string previousLearnDelFAMCode = string.Empty;
            DateTime? previousDateTo = null;

            learnDelFAMType = string.Empty;
            learnDelFAMCode = string.Empty;

            foreach (var fam in learningDeliveryFAMs)
            {
                if (record > 1)
                {
                    if ((!previousDateTo.HasValue
                        || fam.LearnDelFAMDateFromNullable <= previousDateTo)
                        && fam.LearnDelFAMCode != previousLearnDelFAMCode)
                    {
                        isConditionMet = true;
                        learnDelFAMType = fam.LearnDelFAMType;
                        learnDelFAMCode = fam.LearnDelFAMCode;
                        break;
                    }
                }

                previousLearnDelFAMCode = fam.LearnDelFAMCode;
                previousDateTo = fam.LearnDelFAMDateToNullable;
                record++;
            }

            return isConditionMet;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnDelFAMType, string learnDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFAMCode)
            };
        }
    }
}
