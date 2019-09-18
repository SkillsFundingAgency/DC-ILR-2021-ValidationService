using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_04Rule : AbstractRule, IRule<ILearner>
    {
        private const int FundModel = TypeOfFunding.EuropeanSocialFund;

        private readonly IDerivedData_22Rule _derivedData22;

        public EmpStat_04Rule(
            IDerivedData_22Rule derivedData22,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_04)
        {
            _derivedData22 = derivedData22;
        }

        public void Validate(ILearner learner)
        {

            if (learner?.LearnerEmploymentStatuses == null)
            {
                return;
            }

            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {

                if (learningDelivery.FundModel != FundModel)
                {
                    continue;
                }

                DateTime? latestLearningStart = 
                    _derivedData22.GetLatestLearningStartForESFContract(learningDelivery, learner.LearningDeliveries);
                if (!latestLearningStart.HasValue)
                {
                    continue;
                }

                if (GetQualifyingEmploymentStatus(learner, latestLearningStart) == TypeOfEmploymentStatus.NotKnownProvided
                    )
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(latestLearningStart));
                }
            }
        }

        public int? GetQualifyingEmploymentStatus(ILearner learner, DateTime? DD22) =>
            learner.LearnerEmploymentStatuses
            .Where(x => x.DateEmpStatApp <= DD22)
            .OrderByDescending(x => x.DateEmpStatApp)
            .FirstOrDefault()?.EmpStat;


        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? latestLearningStart)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, latestLearningStart),
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, TypeOfEmploymentStatus.NotKnownProvided)
            };
        }
    }
}






