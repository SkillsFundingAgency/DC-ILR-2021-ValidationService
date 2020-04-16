using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_ValidityCategory : IDerivedData_ValidityCategory
    {
        private readonly DateTime _firstAugust2011 = new DateTime(2011, 08, 01);
        private readonly DateTime _firstAugust2016 = new DateTime(2016, 08, 01);

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDerivedData_11Rule _dd11;

        public DerivedData_ValidityCategory(
             ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
             IDerivedData_07Rule dd07,
             IDerivedData_11Rule dd11)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dd07 = dd07;
            _dd11 = dd11;
        }

        public string Derive(ILearningDelivery learningDelivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            if (HasRestartFAMType(learningDelivery.LearningDeliveryFAMs))
            {
                return null;
            }

            return GetValidityCategory(learningDelivery, learnerEmploymentStatuses);
        }

        public string GetValidityCategory(ILearningDelivery learningDelivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            if (CommunityLearningMatch(learningDelivery))
            {
                return TypeOfLARSValidity.CommunityLearning;
            }

            if (ESFMatch(learningDelivery))
            {
                return TypeOfLARSValidity.EuropeanSocialFund;
            }

            if (EFA16To19Match(learningDelivery))
            {
                return TypeOfLARSValidity.EFA16To19;
            }

            if (AdvancedLearnerLoanMatch(learningDelivery))
            {
                return TypeOfLARSValidity.AdvancedLearnerLoan;
            }

            if (AnyMatch(learningDelivery))
            {
                return TypeOfLARSValidity.Any;
            }

            if (OlassAdultMatch(learningDelivery))
            {
                return TypeOfLARSValidity.OLASSAdult;
            }

            if (ApprenticeshipsMatch(learningDelivery))
            {
                return TypeOfLARSValidity.Apprenticeships;
            }

            if (UnemployedMatch(learningDelivery, learnerEmploymentStatuses))
            {
                return TypeOfLARSValidity.Unemployed;
            }

            if (AdultSkillsMatch(learningDelivery))
            {
                return TypeOfLARSValidity.AdultSkills;
            }

            return null;
        }

        public bool CommunityLearningMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == TypeOfFunding.CommunityLearning;
        }

        public bool ESFMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == TypeOfFunding.EuropeanSocialFund;
        }

        public bool EFA16To19Match(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == TypeOfFunding.Age16To19ExcludingApprenticeships || learningDelivery.FundModel == TypeOfFunding.Other16To19;
        }

        public bool AdvancedLearnerLoanMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == TypeOfFunding.NotFundedByESFA
                && HasAdvancedLearnerLoansFAMType(learningDelivery.LearningDeliveryFAMs);
        }

        public bool AnyMatch(ILearningDelivery learningDelivery)
        {
            return !HasAdvancedLearnerLoansFAMType(learningDelivery.LearningDeliveryFAMs)
                && (learningDelivery.FundModel == TypeOfFunding.NotFundedByESFA || learningDelivery.FundModel == TypeOfFunding.OtherAdult)
                || (learningDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017 && learningDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard);
        }

        public bool OlassAdultMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == TypeOfFunding.AdultSkills
                && HasOlassFAMTypeAndCode(learningDelivery.LearningDeliveryFAMs);
        }

        public bool AdultSkillsMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == TypeOfFunding.AdultSkills
                && !HasOlassFAMTypeAndCode(learningDelivery.LearningDeliveryFAMs)
                && !HasDD07(learningDelivery.ProgTypeNullable);
        }

        public bool ApprenticeshipsMatch(ILearningDelivery learningDelivery)
        {
            return (learningDelivery.FundModel == TypeOfFunding.AdultSkills || learningDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017)
                && learningDelivery.ProgTypeNullable != TypeOfLearningProgramme.ApprenticeshipStandard
                && learningDelivery.LearnStartDate >= _firstAugust2011
                && learningDelivery.AimType == TypeOfAim.ComponentAimInAProgramme
                && HasDD07(learningDelivery.ProgTypeNullable);
        }

        public bool UnemployedMatch(ILearningDelivery learningDelivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return learningDelivery.FundModel == TypeOfFunding.AdultSkills
                && learningDelivery.LearnStartDate < _firstAugust2016
                && HasDD11(learningDelivery, learnerEmploymentStatuses)
                && !HasDD07(learningDelivery.ProgTypeNullable)
                && !HasOlassFAMTypeAndCode(learningDelivery.LearningDeliveryFAMs);
        }

        private bool HasDD11(ILearningDelivery learningDelivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return _dd11.IsAdultFundedOnBenefitsAtStartOfAim(learningDelivery, learnerEmploymentStatuses);
        }

        private bool HasDD07(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        private bool HasRestartFAMType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);
        }

        private bool HasAdvancedLearnerLoansFAMType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ADL);
        }

        private bool HasOlassFAMTypeAndCode(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS);
        }
    }
}
