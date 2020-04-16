using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_17Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        public const string Name = RuleNameConstants.EmpStat_17;

        private readonly IValidationErrorHandler _messageHandler;

        public EmpStat_17Rule(IValidationErrorHandler validationErrorHandler)
        {
            
                

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public DateTime LastInviableDate => new DateTime(2016, 07, 31);

        public bool IsViableStart(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        public bool InTraining(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == TypeOfLearningProgramme.Traineeship;

        public bool IsInAProgramme(ILearningDelivery delivery) =>
            delivery.AimType == TypeOfAim.ProgrammeAim;

        public bool HasAQualifyingEmploymentStatus(ILearnerEmploymentStatus thisEmployment) =>
               thisEmployment?.EmpStat != TypeOfEmploymentStatus.NotKnownProvided;

        public ILearnerEmploymentStatus GetMatchingEmployment(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> usingEmployments) =>
            usingEmployments?
                .Where(x => x.DateEmpStatApp <= delivery.LearnStartDate)
                .OrderByDescending(x => x.DateEmpStatApp)
                .FirstOrDefault();

        public bool IsNotValid(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> usingEmployments) =>
            IsViableStart(delivery)
                && InTraining(delivery)
                && IsInAProgramme(delivery)
                && !HasAQualifyingEmploymentStatus(GetMatchingEmployment(delivery, usingEmployments));

        public void Validate(ILearner objectToValidate)
        {
            
                

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var employments = objectToValidate.LearnerEmploymentStatuses.ToReadOnlyCollection();

            objectToValidate.LearningDeliveries
                .Where(x => IsNotValid(x, employments))
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, TypeOfEmploymentStatus.NotKnownProvided),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
