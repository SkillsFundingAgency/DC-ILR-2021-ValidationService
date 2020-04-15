using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_21Rule :
        IDerivedData_21Rule
    {
        private IProvideRuleCommonOperations _check;

        public DerivedData_21Rule(IProvideRuleCommonOperations commonOperations)
        {
            _check = commonOperations;
        }

        public bool IsNotEmployed(ILearnerEmploymentStatus candidate) =>
            candidate?.EmpStat == TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable
            || candidate?.EmpStat == TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable;

        public bool InReceiptOfAnotherBenefit(IEmploymentStatusMonitoring employmentMonitoring) =>
            Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit.CaseInsensitiveEquals($"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}");

        public bool InReceiptOfUniversalCredit(IEmploymentStatusMonitoring employmentMonitoring) =>
            Monitoring.EmploymentStatus.InReceiptOfUniversalCredit.CaseInsensitiveEquals($"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}");

        public bool InReceiptOfBenefits(ILearnerEmploymentStatus learnerEmploymentStatus) =>
            learnerEmploymentStatus.EmploymentStatusMonitorings.NullSafeAny(InReceiptOfAnotherBenefit);

        public bool InReceiptOfCredits(ILearnerEmploymentStatus learnerEmploymentStatus) =>
            learnerEmploymentStatus.EmploymentStatusMonitorings.NullSafeAny(InReceiptOfUniversalCredit);

        public bool NotIsMonitored(ILearningDeliveryFAM fam) =>
            !fam.LearnDelFAMType.CaseInsensitiveEquals(Monitoring.Delivery.Types.Learning);

        public bool NotIsMonitored(IReadOnlyCollection<ILearningDeliveryFAM> fams) =>
            fams.NullSafeAny(NotIsMonitored);

        public bool MandatedToSkillsTraining(ILearningDeliveryFAM fam) =>
            Monitoring.Delivery.MandationToSkillsTraining.CaseInsensitiveEquals($"{fam.LearnDelFAMType}{fam.LearnDelFAMCode}");

        public bool MandatedToSkillsTraining(IReadOnlyCollection<ILearningDeliveryFAM> fams) =>
            fams.NullSafeAny(MandatedToSkillsTraining);

        public bool IsAdultFundedUnemployedWithOtherStateBenefits(ILearningDelivery thisDelivery, ILearner forThisCandidate)
        {
            /*
               if
                   // is adult skills
                   LearningDelivery.FundModel = 35

                   //  is umemployed (not employed, seeking and available or otherwise)
                   and     LearnerEmploymentStatus.EmpStat = 11 or 12 for the latest Employment Status on (or before) the LearningDelivery.LearnStartDate

                           // in receipt of another benefit.
                   and     ((Monitoring.EmploymentStatus.ESMType = BSI and Monitoring.EmploymentStatus.ESMCode = 3)
                           or
                           // in receipt of universal credit.
                           (Monitoring.EmploymentStatus.ESMType = BSI and Monitoring.EmploymentStatus.ESMCode = 4
                           // is learning delivery monitored
                           and LearningDeliveryFAM.LearnDelFAMType = LDM
                           // and not mandated to skills training
                           and LearningDeliveryFAM.LearnDelFAMCode <> 318))

                       set to Y,
                       otherwise set to N
            */

            var employment = _check.GetEmploymentStatusOn(thisDelivery.LearnStartDate, forThisCandidate.LearnerEmploymentStatuses);

            return _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.AdultSkills)
                && employment != null
                && IsNotEmployed(employment)
                && (InReceiptOfBenefits(employment)
                    || (InReceiptOfCredits(employment)
                        && (NotIsMonitored(thisDelivery.LearningDeliveryFAMs)
                            || !MandatedToSkillsTraining(thisDelivery.LearningDeliveryFAMs))));
        }
    }
}
