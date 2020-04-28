﻿using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_10Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _check;
        private readonly IDerivedData_07Rule _dd07;

        public EmpId_10Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations,
            IDerivedData_07Rule dd07)
            : base(validationErrorHandler, RuleNameConstants.EmpId_10)
        {
            _check = commonOperations;
            _dd07 = dd07;
        }

        public void Validate(ILearner thisLearner)
        {
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
            _dd07.IsApprenticeship(thisDelivery.ProgTypeNullable)
            && thisDelivery.AimType == TypeOfAim.ProgrammeAim;

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
