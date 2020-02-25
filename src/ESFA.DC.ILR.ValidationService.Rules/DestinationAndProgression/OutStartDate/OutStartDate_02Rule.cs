using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutStartDate
{
    public class OutStartDate_02Rule : AbstractRule, IRule<ILearnerDestinationAndProgression>
    {
        private readonly IAcademicYearDataService _academicYearDataService;

        public OutStartDate_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService academicYearDataService)
            : base(validationErrorHandler, RuleNameConstants.OutStartDate_02)
        {
            _academicYearDataService = academicYearDataService;
        }

        public void Validate(ILearnerDestinationAndProgression objectToValidate)
        {
            if (objectToValidate?.DPOutcomes == null)
            {
                return;
            }

            DateTime oneYearAfterEndOfAcademicYear = _academicYearDataService.End().AddYears(1);

            foreach (var dpOutCome in objectToValidate.DPOutcomes)
            {
                if (ConditionMet(dpOutCome.OutStartDate, oneYearAfterEndOfAcademicYear))
                {
                    HandleValidationError(
                        learnRefNumber: objectToValidate.LearnRefNumber,
                        errorMessageParameters: BuildErrorMessageParameters(dpOutCome.OutStartDate));
                }
            }
        }

        public bool ConditionMet(DateTime outStartDate, DateTime oneYearAfterEndOfAcademicYear) => outStartDate > oneYearAfterEndOfAcademicYear;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime outStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutStartDate, outStartDate)
            };
        }
    }
}
