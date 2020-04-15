using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_10Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _check;

        public EmpId_10Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.EmpId_10)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var employments = thisLearner.LearnerEmploymentStatuses.ToReadOnlyCollection();

            thisLearner.LearningDeliveries
                .ForAny(
                    x => IsNotValid(x, GetEmploymentStatusOn(x.LearnStartDate, employments)),
                    x => RaiseValidationMessage(thisLearner.LearnRefNumber, x));
        }

        public bool IsNotValid(ILearningDelivery thisdelivery, ILearnerEmploymentStatus thisEmployment) =>
            IsPrimaryLearningAim(thisdelivery)
            && HasQualifyingEmploymentStatus(thisEmployment)
            && HasDisqualifyingEmployerID(thisEmployment);

        public bool IsPrimaryLearningAim(ILearningDelivery thisDelivery) =>
            _check.InApprenticeship(thisDelivery)
            && _check.InAProgramme(thisDelivery);

        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus thisEmployment) =>
            thisEmployment != null
                && thisEmployment.EmpStat == TypeOfEmploymentStatus.InPaidEmployment;

        public bool HasDisqualifyingEmployerID(ILearnerEmploymentStatus thisEmployment) =>
            !thisEmployment.EmpIdNullable.HasValue;

        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime thisDate, IReadOnlyCollection<ILearnerEmploymentStatus> fromEmployments) =>
            _check.GetEmploymentStatusOn(thisDate, fromEmployments);

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, TypeOfEmploymentStatus.InPaidEmployment),
                BuildErrorMessageParameter(PropertyNameConstants.EmpId, "(missing)")
            };
        }
    }
}
