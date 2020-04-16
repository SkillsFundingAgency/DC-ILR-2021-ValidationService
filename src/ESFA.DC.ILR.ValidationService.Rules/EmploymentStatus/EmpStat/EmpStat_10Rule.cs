using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_10Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "DateEmpStatApp";

        public const string Name = RuleNameConstants.EmpStat_10;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly IDerivedData_22Rule _derivedData22;

        public EmpStat_10Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_22Rule derivedData22)
        {
            _messageHandler = validationErrorHandler;
            _derivedData22 = derivedData22;
        }

        public string RuleName => Name;

        public DateTime? GetContractCompletionDate(ILearningDelivery delivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData22.GetLatestLearningStartForESFContract(delivery, usingSources);

        public DateTime? GetLatestContractCompletionDate(IReadOnlyCollection<ILearningDelivery> usingSources)
        {
            var candidates = new List<DateTime?>();
            usingSources.ForEach(source => candidates.Add(GetContractCompletionDate(source, usingSources)));

            return candidates.Max();
        }

        public bool HasAQualifyingEmploymentStatus(ILearnerEmploymentStatus thisEmployment, DateTime thresholdDate) =>
            thisEmployment.DateEmpStatApp < thresholdDate;

        public bool IsNotValid(ILearner learner, DateTime referenceDate) =>
            !learner.LearnerEmploymentStatuses.NullSafeAny(x => HasAQualifyingEmploymentStatus(x, referenceDate));

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;
            var forDeliveries = objectToValidate.LearningDeliveries.ToReadOnlyCollection();
            var completionDate = GetLatestContractCompletionDate(forDeliveries);

            if (completionDate.HasValue && IsNotValid(objectToValidate, completionDate.Value))
            {
                RaiseValidationMessage(learnRefNumber, completionDate.Value);
            }
        }

        public void RaiseValidationMessage(string learnRefNumber, DateTime learnStartDate)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, learnStartDate),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, TypeOfAim.References.ESFLearnerStartandAssessment)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
