using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R58Rule : AbstractRule, IRule<ILearner>
    {
        private const int _aimType = 5;

        public R58Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R58)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var coreAims = objectToValidate.LearningDeliveries?.Where(ld => ld.AimType == _aimType).ToList();

            if (DoesNotHaveMultipleCoreAims(coreAims))
            {
                return;
            }

            var learnActEndDateTuple = LearnActEndDateForOverlappingCoreAims(coreAims);

            if (learnActEndDateTuple.Invalid)
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(_aimType, learnActEndDateTuple.LearnActEndDate));
            }
        }

        public bool DoesNotHaveMultipleCoreAims(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return
                learningDeliveries != null
                ? learningDeliveries.Count() < 2
                : true;
        }

        public(bool Invalid, DateTime? LearnActEndDate) LearnActEndDateForOverlappingCoreAims(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            if (learningDeliveries != null)
            {
                var coreAims = learningDeliveries.OrderBy(ld => ld.LearnStartDate).ToArray();

                var coreAimCount = coreAims.Length;

                var i = 1;

                while (i < coreAimCount)
                {
                    var errorConditionMet =
                        coreAims[i - 1].LearnActEndDateNullable == null
                        ? true
                        : coreAims[i - 1].LearnActEndDateNullable >= coreAims[i].LearnStartDate;

                    if (errorConditionMet)
                    {
                        return (true, coreAims[i - 1].LearnActEndDateNullable);
                    }

                    i++;
                }
            }

            return (false, null);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate)
            };
        }
    }
}
