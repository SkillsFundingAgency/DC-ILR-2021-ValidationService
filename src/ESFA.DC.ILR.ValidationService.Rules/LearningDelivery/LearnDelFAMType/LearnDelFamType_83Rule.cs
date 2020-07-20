using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFamType_83Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _ldmCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_370,
            LearningDeliveryFAMCodeConstants.LDM_371,
            LearningDeliveryFAMCodeConstants.LDM_372,
            LearningDeliveryFAMCodeConstants.LDM_373,
        };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IFileDataService _fileDataService;

        public LearnDelFamType_83Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IOrganisationDataService organisationDataService,
            IFileDataService fileDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_83)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _organisationDataService = organisationDataService;
            _fileDataService = fileDataService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                var learningDeliveryFams = _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsForTypeAndCodes(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, _ldmCodes);

                if (learningDeliveryFams == null || !learningDeliveryFams.Any())
                {
                    continue;
                }

                var ukprn = _fileDataService.UKPRN();

                foreach (var deliveryFam in learningDeliveryFams)
                {
                    if (ConditionMet(ukprn, learningDelivery.LearnStartDate, deliveryFam))
                    {
                        HandleValidationError(
                            learner.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(deliveryFam.LearnDelFAMCode));
                    }
                }
            }
        }

        public bool ConditionMet(int ukprn, DateTime learnStartDate, ILearningDeliveryFAM deliveryFam)
        {
            var organisation = _organisationDataService.GetOrganisationFor(ukprn);

            return !organisation?.ShortTermFundingInitiatives?
                .Any(stfi => ukprn == stfi.UKPRN
                               && deliveryFam.LearnDelFAMCode.CaseInsensitiveEquals(stfi.LdmCode)
                               && learnStartDate >= stfi.EffectiveFrom
                               && learnStartDate <= (stfi.EffectiveTo ?? DateTime.MaxValue)) ?? true;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string delFamCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, delFamCode)
            };
        }
    }
}
