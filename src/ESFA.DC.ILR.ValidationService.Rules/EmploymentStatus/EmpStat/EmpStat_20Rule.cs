using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_20Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public EmpStat_20Rule(
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_20)
        {
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    objectToValidate.LearnerEmploymentStatuses,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(
                                                            learningDelivery.LearnStartDate,
                                                            LearningDeliveryFAMTypeConstants.LDM,
                                                            LearningDeliveryFAMCodeConstants.LDM_LowWages));
                    return;
                }
            }
        }

        public bool ConditionMet(
            DateTime learnStartDate,
            IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses,
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return EmpStatConditionMet(learnStartDate, learnerEmploymentStatuses)
                    && LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
        }

        public bool EmpStatConditionMet(DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return _learnerEmploymentStatusQueryService.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)?.EmpStat != EmploymentStatusEmpStats.InPaidEmployment;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                                                                                        learningDeliveryFAMs,
                                                                                        LearningDeliveryFAMTypeConstants.LDM,
                                                                                        LearningDeliveryFAMCodeConstants.LDM_LowWages);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, string learningDelFamType, string learningDelFamCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learningDelFamType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learningDelFamCode)
            };
        }
    }
}
