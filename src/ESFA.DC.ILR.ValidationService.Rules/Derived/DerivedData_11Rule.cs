using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_11Rule :
        IDerivedData_11Rule
    {
        private readonly IProvideRuleCommonOperations _check;

        public DerivedData_11Rule(IProvideRuleCommonOperations commonOps)
        {
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));

            _check = commonOps;
        }

        public bool InReceiptOfBenefits(IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatus, DateTime startDate)
        {
            var candidate = _check.GetEmploymentStatusOn(startDate, learnerEmploymentStatus);

            return InReceiptOfBenefits(candidate?.EmploymentStatusMonitorings);
        }

        public bool InReceiptOfBenefits(IReadOnlyCollection<IEmploymentStatusMonitoring> monitors)
        {
            return monitors.SafeAny(InReceiptOfBenefits);
        }

        public bool InReceiptOfBenefits(IEmploymentStatusMonitoring monitor) =>
            It.IsInRange(
                $"{monitor.ESMType}{monitor.ESMCode}",
                Monitoring.EmploymentStatus.InReceiptOfUniversalCredit,
                Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit,
                Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance,
                Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance);

        public bool IsAdultFundedOnBenefitsAtStartOfAim(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmployments)
        {
            It.IsNull(delivery)
                .AsGuard<ArgumentNullException>(nameof(delivery));
            var employments = learnerEmployments.AsSafeReadOnlyList();

            /*
                if
                    LearningDelivery.FundModel = 35
                    and the learner's Employment status on the LearningDelivery.LearnStartDate of the learning aim
                    is (EmploymentStatusMonitoring.ESMType = BSI and EmploymentStatusMonitoring.ESMCode = 1, 2, 3 or 4)
                        set to Y,
                        otherwise set to N
             */

            return _check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
                && InReceiptOfBenefits(employments, delivery.LearnStartDate);
        }
    }
}
