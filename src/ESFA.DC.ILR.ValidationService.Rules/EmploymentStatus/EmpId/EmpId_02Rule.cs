using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_05Rule _derivedData05;

        public EmpId_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_05Rule derivedData05)
            : base(validationErrorHandler, RuleNameConstants.EmpId_02)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData05)
                .AsGuard<ArgumentNullException>(nameof(derivedData05));

            _derivedData05 = derivedData05;
        }

        public bool HasValidChecksum(int employerID)
        {
            var checkSum = _derivedData05.GetEmployerIDChecksum(employerID);

            if (checkSum == _derivedData05.InvalidLengthChecksum)
            {
                return false;
            }

            var candidate = employerID.SplitIntDigitsToList().ElementAt(8).ToString();
            return candidate.CaseInsensitiveEquals(checkSum.ToString());
        }

        public bool IsNotValid(ILearnerEmploymentStatus employment) =>
            employment.EmpIdNullable.HasValue
                && employment.EmpIdNullable != ValidationConstants.TemporaryEmployerId
                && !HasValidChecksum(employment.EmpIdNullable.Value);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            if (objectToValidate.LearnerEmploymentStatuses != null)
            {
                foreach (var learnerEmploymentStatus in objectToValidate.LearnerEmploymentStatuses)
                {
                    if (IsNotValid(learnerEmploymentStatus))
                    {
                        RaiseValidationMessage(learnRefNumber, learnerEmploymentStatus);
                    }
                }
            }
        }

        private void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpId, thisEmployment.EmpIdNullable)
            };

            HandleValidationError(learnRefNumber, null, parameters);
        }
    }
}