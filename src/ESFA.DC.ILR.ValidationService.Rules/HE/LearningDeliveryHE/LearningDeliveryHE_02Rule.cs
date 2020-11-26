using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.LearningDeliveryHE
{
    public class LearningDeliveryHE_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly IFileDataService _fileDataService;
        private readonly IOrganisationDataService _organisationDataService;

        public LearningDeliveryHE_02Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IFileDataService fileDataService,
            IOrganisationDataService organisationDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearningDeliveryHE_02)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
            _fileDataService = fileDataService;
            _organisationDataService = organisationDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var ukprn = _fileDataService.UKPRN();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.LearningDeliveryFAMs,
                    learningDelivery.LearningDeliveryHEEntity,
                    ukprn))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int fundModel, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams, ILearningDeliveryHE learningDeliveryHe, int ukprn)
        {
            return FundModelConditionMet(fundModel)
                   && DeliveryFAMConditionMet(learningDeliveryFams)
                   && DeliveryHEConditionMet(learningDeliveryHe)
                   && !Excluded(ukprn);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 99;
        }

        public bool DeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.SOF, "1")
                && !_learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.LDM, "352");
        }

        public bool DeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHe)
        {
            return learningDeliveryHe == null;
        }

        public bool Excluded(int ukprn)
        {
            return _organisationDataService.LegalOrgTypeMatchForUkprn(ukprn, LegalOrgTypeConstants.UHEO);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
