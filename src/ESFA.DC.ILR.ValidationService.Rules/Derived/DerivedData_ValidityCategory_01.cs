using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_ValidityCategory_01 : IDerivedData_ValidityCategory_01
    {
        private readonly DateTime _firstAugust2011 = new DateTime(2011, 08, 01);
        private readonly DateTime _firstAugust2016 = new DateTime(2016, 08, 01);

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDerivedData_11Rule _dd11;
        private readonly IDerivedData_35Rule _dd35;

        public DerivedData_ValidityCategory_01(
             ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
             IDerivedData_07Rule dd07,
             IDerivedData_11Rule dd11,
             IDerivedData_35Rule dd35)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dd07 = dd07;
            _dd11 = dd11;
            _dd35 = dd35;
        }

        public string Derive(ILearningDelivery learningDelivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            //Check MCA Adult skills first as this is the only condition that doesn't exclude RES

            if (McaAdultSkillsMatch(learningDelivery))
            {
                return LARSConstants.Validities.AdultSkills;
            }

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
                return LARSConstants.Validities.CommunityLearning;
            }

            if (ESFMatch(learningDelivery))
            {
                return LARSConstants.Validities.EuropeanSocialFund;
            }

            if (EFA16To19Match(learningDelivery))
            {
                return LARSConstants.Validities.EFA16To19;
            }

            if (AdvancedLearnerLoanMatch(learningDelivery))
            {
                return LARSConstants.Validities.AdvancedLearnerLoan;
            }

            if (AnyMatch(learningDelivery))
            {
                return LARSConstants.Validities.Any;
            }

            if (OlassAdultMatch(learningDelivery))
            {
                return LARSConstants.Validities.OLASSAdult;
            }

            if (ApprenticeshipsMatch(learningDelivery))
            {
                return LARSConstants.Validities.Apprenticeships;
            }

            if (UnemployedMatch(learningDelivery, learnerEmploymentStatuses))
            {
                return LARSConstants.Validities.Unemployed;
            }

            if (AdultSkillsMatch(learningDelivery))
            {
                return LARSConstants.Validities.AdultSkills;
            }

            return null;
        }

        public bool CommunityLearningMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == FundModels.CommunityLearning;
        }

        public bool ESFMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == FundModels.EuropeanSocialFund;
        }

        public bool EFA16To19Match(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == FundModels.Age16To19ExcludingApprenticeships || learningDelivery.FundModel == FundModels.Other16To19;
        }

        public bool AdvancedLearnerLoanMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == FundModels.NotFundedByESFA
                && HasAdvancedLearnerLoansFAMType(learningDelivery.LearningDeliveryFAMs);
        }

        public bool AnyMatch(ILearningDelivery learningDelivery)
        {
            return !HasAdvancedLearnerLoansFAMType(learningDelivery.LearningDeliveryFAMs)
                && (learningDelivery.FundModel == FundModels.NotFundedByESFA || learningDelivery.FundModel == FundModels.OtherAdult)
                || (learningDelivery.FundModel == FundModels.ApprenticeshipsFrom1May2017 && learningDelivery.ProgTypeNullable == ProgTypes.ApprenticeshipStandard);
        }

        public bool OlassAdultMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == FundModels.AdultSkills
                && HasOlassFAMTypeAndCode(learningDelivery.LearningDeliveryFAMs);
        }

        public bool AdultSkillsMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == FundModels.AdultSkills
                && !HasOlassFAMTypeAndCode(learningDelivery.LearningDeliveryFAMs)
                && !HasDD07(learningDelivery.ProgTypeNullable)
                && !HasDD35(learningDelivery);
        }

        public bool McaAdultSkillsMatch(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == FundModels.AdultSkills
                && HasDD35(learningDelivery);
        }

        public bool ApprenticeshipsMatch(ILearningDelivery learningDelivery)
        {
            return (learningDelivery.FundModel == FundModels.AdultSkills || learningDelivery.FundModel == FundModels.ApprenticeshipsFrom1May2017)
                && learningDelivery.ProgTypeNullable != ProgTypes.ApprenticeshipStandard
                && learningDelivery.LearnStartDate >= _firstAugust2011
                && learningDelivery.AimType == AimTypes.ComponentAimInAProgramme
                && HasDD07(learningDelivery.ProgTypeNullable);
        }

        public bool UnemployedMatch(ILearningDelivery learningDelivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return learningDelivery.FundModel == FundModels.AdultSkills
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

        private bool HasDD35(ILearningDelivery learningDelivery)
        {
            return _dd35.IsCombinedAuthorities(learningDelivery);
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
