using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp
{
    public class DateEmpStatApp_01Rule :
        AbstractRule,
        IRule<ILearner>
    {
        public DateEmpStatApp_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService yearData)
            : base(validationErrorHandler, RuleNameConstants.DateEmpStatApp_01)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(yearData)
                .AsGuard<ArgumentNullException>(nameof(yearData));

            ThresholdDate = yearData.End();
        }

        public DateTime ThresholdDate { get; }

        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearnerEmploymentStatuses
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public bool IsNotValid(ILearnerEmploymentStatus eStatus) =>
            HasDisqualifyingEmploymentStatusDate(eStatus);

        public bool HasDisqualifyingEmploymentStatusDate(ILearnerEmploymentStatus eStatus) =>
            eStatus.DateEmpStatApp > ThresholdDate;

        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus theEmployment) =>
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(theEmployment));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearnerEmploymentStatus theEmployment) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.DateEmpStatApp, theEmployment.DateEmpStatApp)
        };
    }
}
