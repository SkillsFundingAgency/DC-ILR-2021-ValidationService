using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEmployersDataService _edrsData;

        public EmpId_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IEmployersDataService edrsData)
            : base(validationErrorHandler, RuleNameConstants.EmpId_01)
        {
            _edrsData = edrsData;
        }

        public bool IsNotValid(int? empId) =>
            empId.HasValue
            && empId != ValidationConstants.TemporaryEmployerId
            && !_edrsData.IsValid(empId);

        public void Validate(ILearner learner)
        {
            if (learner.LearnerEmploymentStatuses != null)
            {
                foreach (var learnerEmploymentStatus in learner.LearnerEmploymentStatuses)
                {
                    if (IsNotValid(learnerEmploymentStatus.EmpIdNullable))
                    {
                        RaiseValidationMessage(learner.LearnRefNumber, learnerEmploymentStatus);
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