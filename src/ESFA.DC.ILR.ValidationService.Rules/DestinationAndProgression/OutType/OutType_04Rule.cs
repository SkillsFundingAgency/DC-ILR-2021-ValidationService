using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutType
{
    public class OutType_04Rule : AbstractRule, IRule<ILearnerDestinationAndProgression>
    {
        private readonly HashSet<string> _outTypes = new HashSet<string>
        {
            OutTypeConstants.PaidEmployment,
            OutTypeConstants.NotInPaidEmployment
        };

        private DateTime? duplicateOutStartDate;

        public OutType_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutType_04)
        {
        }

        public void Validate(ILearnerDestinationAndProgression objectToValidate)
        {
            if (objectToValidate != null && ConditionMet(objectToValidate.DPOutcomes))
            {
                var outTypesForError = string.Join(", ", _outTypes);

                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(duplicateOutStartDate, outTypesForError));
            }
        }

        public bool ConditionMet(IEnumerable<IDPOutcome> dpOutcomes)
        {
            return OutTypesConditionMet(dpOutcomes)
                   && OutStartDateConditionMet(dpOutcomes);
        }

        public bool OutTypesConditionMet(IEnumerable<IDPOutcome> dpOutcomes)
        {
            return dpOutcomes != null
                   && dpOutcomes.Any(dp => dp.OutType == OutTypeConstants.PaidEmployment)
                   && dpOutcomes.Any(dp => dp.OutType == OutTypeConstants.NotInPaidEmployment);
        }

        public bool OutStartDateConditionMet(IEnumerable<IDPOutcome> dpOutcomes)
        {
            var dpList = dpOutcomes
                .Where(dp => _outTypes.Contains(dp.OutType))
                .Select(dp => new { dp.OutType, dp.OutStartDate }).Distinct().ToList();

            if (dpList.Count != dpList.Select(dp => dp.OutStartDate).Distinct().Count())
            {
                duplicateOutStartDate = dpList.GroupBy(dp => dp.OutStartDate)
                    .Where(dp => dp.Count() > 1)
                    .Select(x => x.Key).FirstOrDefault();

                return true;
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? outStartDate, string outTypes)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutStartDate, outStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.OutType, outTypes),
            };
        }
    }
}
