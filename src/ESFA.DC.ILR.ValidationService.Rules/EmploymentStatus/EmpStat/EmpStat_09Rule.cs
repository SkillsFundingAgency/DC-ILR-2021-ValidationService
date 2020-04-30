using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_09Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        public const string Name = RuleNameConstants.EmpStat_09;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly IDerivedData_07Rule _derivedData07;

        public EmpStat_09Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_07Rule derivedData07)
        {
            _messageHandler = validationErrorHandler;
            _derivedData07 = derivedData07;
        }

        public string RuleName => Name;

        public DateTime LastInviableDate => new DateTime(2014, 07, 31);

        public bool IsApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool InTraining(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == ProgTypes.Traineeship;

        public bool IsInAProgramme(ILearningDelivery delivery) =>
            delivery.AimType == AimTypes.ProgrammeAim;

        public bool HasAViableStart(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        public bool HasAQualifyingEmploymentStatus(ILearnerEmploymentStatus eStatus, DateTime learningStartDate) =>
            eStatus.DateEmpStatApp < learningStartDate;

        public bool HasAQualifyingEmploymentStatus(ILearner learner, ILearningDelivery delivery) =>
            learner.LearnerEmploymentStatuses.NullSafeAny(x => HasAQualifyingEmploymentStatus(x, delivery.LearnStartDate));

        public bool IsNotValid(ILearner learner, ILearningDelivery delivery) =>
            (IsApprenticeship(delivery) || InTraining(delivery))
                && IsInAProgramme(delivery)
                && HasAViableStart(delivery)
                && !HasAQualifyingEmploymentStatus(learner, delivery);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => IsNotValid(objectToValidate, x))
                .ForEach(x => RaiseValidationMessage(objectToValidate, x));
        }

        public void RaiseValidationMessage(ILearner learner, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, "(missing)"),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };

            _messageHandler.Handle(RuleName, learner.LearnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
