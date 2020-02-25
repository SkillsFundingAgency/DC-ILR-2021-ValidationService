using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_13Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;
        private readonly IFileDataService _fileDataService;

        public EmpId_13Rule(
            IDerivedData_07Rule dd07,
            IFileDataService fileDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpId_13)
        {
            _dd07 = dd07;
            _fileDataService = fileDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (EmpIdConditionMet(objectToValidate.LearnerEmploymentStatuses))
            {
                DateTime filePrepDate = _fileDataService.FilePreparationDate();

                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.AimType,
                        learningDelivery.LearnStartDate,
                        filePrepDate))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearnStartDate));
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(
            int? progType,
            int aimType,
            DateTime learnStartDate,
            DateTime filePrepDate)
        {
            return DD07ConditionMet(progType)
                   && AimTypeConditionMet(aimType)
                   && LearnStartDateConditionMet(learnStartDate, filePrepDate);
        }

        public bool EmpIdConditionMet(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            if (learnerEmploymentStatuses == null)
            {
                return false;
            }

            return learnerEmploymentStatuses.Any(x => x.EmpIdNullable == 999999999);
        }

        public bool DD07ConditionMet(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate, DateTime filePrepDate)
        {
            var days = 60;

            return filePrepDate.Subtract(learnStartDate).Days > days;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
