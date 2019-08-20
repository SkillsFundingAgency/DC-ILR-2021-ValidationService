using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode
{
    public class LSDPostcode_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IPostcodesDataService _postcodeService;

        private readonly DateTime _firstAugust2019 = new DateTime(2019, 08, 01);
        private readonly IEnumerable<int> _fundModels = new HashSet<int>()
        {
            TypeOfFunding.AdultSkills
        };

        public LSDPostcode_02Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IOrganisationDataService organisationDataService,
            IPostcodesDataService postcodeService,
            IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler, RuleNameConstants.LSDPostcode_02)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _organisationDataService = organisationDataService;
            _postcodeService = postcodeService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    var devolvedPostcodes = _postcodeService.GetDevolvedPostcodes(learningDelivery.LSDPostcode);

                    if (ConditionMet(learningDelivery.PartnerUKPRNNullable.Value, learningDelivery.ProgTypeNullable,
                                     learningDelivery.FundModel, learningDelivery.LearnStartDate,
                                     devolvedPostcodes, learningDelivery.LearningDeliveryFAMs))
                    {
                        HandleValidationError(
                                          objectToValidate.LearnRefNumber,
                                          learningDelivery.AimSeqNumber,
                                          BuildErrorMessageParameters(learningDelivery.LearnPlanEndDate,
                                          learningDelivery.FundModel,
                                          learningDelivery.LSDPostcode,
                                          LearningDeliveryFAMTypeConstants.SOF));
                    }
                }
            }
        }

        public bool ConditionMet(int ukprn, int? progType, int fundModel, DateTime learnStartDate, IEnumerable<IDevolvedPostcode> devolvedPostcodes, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return ProgTypeConditionMet(progType)
                 && FundModelConditionMet(fundModel)
                 && LearnStartDateConditionMet(learnStartDate)
                 && IsInvalidSofCodeOnLearnStartDate(learnStartDate, learningDeliveryFAMs, devolvedPostcodes)
                 && OrganisationConditionMet(ukprn)
                 && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)
                 && ExclusionConditionMet(learningDeliveryFAMs);
        }

        public virtual bool ProgTypeConditionMet(int? progType)
        {
            return progType.HasValue
                      && progType != TypeOfLearningProgramme.Traineeship;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _firstAugust2019;
        }

        public virtual bool IsInvalidSofCodeOnLearnStartDate(DateTime learnStartDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, IEnumerable<IDevolvedPostcode> devolvedPostcodes)
        {
            var ldFamSofs = _learningDeliveryFAMQueryService
                .GetLearningDeliveryFAMsForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF)?
                .Select(l => l.LearnDelFAMCode)
                .ToList() ?? Enumerable.Empty<string>();

            var devolvedPostcodesForSof = devolvedPostcodes.Where(dp => ldFamSofs.Contains(dp.SourceOfFunding)).ToList() ?? Enumerable.Empty<IDevolvedPostcode>();

            var isNotValid = devolvedPostcodesForSof.Count() > 0 ? devolvedPostcodesForSof.All(dp => dp.EffectiveFrom > learnStartDate) : false;

            return isNotValid;
        }

        public virtual bool OrganisationConditionMet(int ukprn)
        {
            return !_organisationDataService.LegalOrgTypeMatchForUkprn(ukprn, LegalOrgTypeConstants.USDC);
        }

        public virtual bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);
        }

        public virtual bool ExclusionConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService
                       .HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_Code_001);            
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int fundModel, string lsdPostcode, string famType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, lsdPostcode),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType)
            };
        }
    }
}
