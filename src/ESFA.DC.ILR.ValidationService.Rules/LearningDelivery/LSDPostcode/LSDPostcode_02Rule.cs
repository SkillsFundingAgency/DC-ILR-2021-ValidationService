using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
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
        private readonly IAcademicYearDataService _academicYearDataService;

        private readonly IEnumerable<int> _fundModels = new HashSet<int>()
        {
            TypeOfFunding.AdultSkills
        };

        public LSDPostcode_02Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IOrganisationDataService organisationDataService,
            IPostcodesDataService postcodeService,
            IAcademicYearDataService academicYearDataService,
            IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler, RuleNameConstants.LSDPostcode_02)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _organisationDataService = organisationDataService;
            _postcodeService = postcodeService;
            _academicYearDataService = academicYearDataService;
        }

        public LSDPostcode_02Rule()
       : base(null, null)
        {

        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    var mcaglaPostocdeList = _postcodeService.GetMcaglaSOFPostcodes(learningDelivery.LSDPostcode);

                    if (ConditionMet(learningDelivery.PartnerUKPRNNullable.Value, learningDelivery.ProgTypeNullable,
                                     learningDelivery.FundModel, learningDelivery.LearnStartDate,
                                     mcaglaPostocdeList, learningDelivery.LearningDeliveryFAMs))
                    {
                        HandleValidationError(
                                          objectToValidate.LearnRefNumber,
                                          learningDelivery.AimSeqNumber,
                                          BuildErrorMessageParameters(learningDelivery.LearnPlanEndDate,
                                          learningDelivery.FundModel,
                                          learningDelivery.LSDPostcode,
                                          LearningDeliveryFAMTypeConstants.SOF));
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(int ukprn, int? progType, int fundModel, DateTime learnStartDate, IEnumerable<IMcaglaSOFPostcode> mcaglaSOFPostcodes, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return ProgTypeConditionMet(progType)
                 && FundModelConditionMet(fundModel)
                 && LearnStartDateConditionMet(learnStartDate)
                 && CheckQualifyingPeriod(learnStartDate, learningDeliveryFAMs, mcaglaSOFPostcodes)
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
            return learnStartDate >= _academicYearDataService.Start();
        }

        public virtual bool CheckQualifyingPeriod(DateTime learnStartDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, IEnumerable<IMcaglaSOFPostcode> mcaglaSOFPostcodes)
        {
            return mcaglaSOFPostcodes.Count() > 0
                   && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF)
                   && mcaglaSOFPostcodes
                         .Where(sof => sof.SofCode != LearningDeliveryFAMTypeConstants.SOF)
                         .Any(sof => learnStartDate < sof.EffectiveFrom);
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
                       .HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_PostcodeExclusion_001);            
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
