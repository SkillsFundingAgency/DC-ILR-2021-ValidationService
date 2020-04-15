using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_15Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        public const string Name = RuleNameConstants.EmpStat_15;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly IDerivedData_07Rule _derivedData07;

        public EmpStat_15Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_07Rule derivedData07)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData07)
                .AsGuard<ArgumentNullException>(nameof(derivedData07));

            _messageHandler = validationErrorHandler;
            _derivedData07 = derivedData07;
        }

        public string RuleName => Name;

        public DateTime LastInviableDate => new DateTime(2016, 07, 31);

        public bool IsApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        public bool InAProgramme(ILearningDelivery delivery) =>
            delivery.AimType == TypeOfAim.ProgrammeAim;

        public bool IsQualifyingAim(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus eStatus) =>
            eStatus?.EmpStat != TypeOfEmploymentStatus.NotKnownProvided;

        public ILearnerEmploymentStatus GetQualifyingEmploymentStatus(ILearner learner, ILearningDelivery delivery) =>
            learner.LearnerEmploymentStatuses
                .NullSafeWhere(x => x.DateEmpStatApp <= delivery.LearnStartDate)
                .OrderByDescending(x => x.DateEmpStatApp)
                .FirstOrDefault();

        public bool HasQualifyingEmploymentStatus(ILearner learner, ILearningDelivery delivery) =>
            HasQualifyingEmploymentStatus(GetQualifyingEmploymentStatus(learner, delivery));

        public bool IsNotValid(ILearner learner, ILearningDelivery delivery) =>
            IsApprenticeship(delivery)
                && InAProgramme(delivery)
                && IsQualifyingAim(delivery)
                && !HasQualifyingEmploymentStatus(learner, delivery);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => IsNotValid(objectToValidate, x))
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
