using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp
{
    public class DateEmpStatApp_02Rule :
        IRule<ILearner>
    {
        public const string Name = RuleNameConstants.DateEmpStatApp_02;

        private readonly IValidationErrorHandler _messageHandler;

        public DateEmpStatApp_02Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public DateTime LastInviableDate => new DateTime(1990, 07, 31);

        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus eStatus) =>
            eStatus.DateEmpStatApp > LastInviableDate;

        public bool IsNotValid(ILearnerEmploymentStatus eStatus) =>
            !HasQualifyingEmploymentStatus(eStatus);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearnerEmploymentStatuses
                 .NullSafeWhere(IsNotValid)
                 .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.DateEmpStatApp), thisEmployment.DateEmpStatApp)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
