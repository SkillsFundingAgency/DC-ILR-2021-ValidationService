using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.LearningDeliveryHE
{
    public class LearningDeliveryHE_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int[] _fundModels =
            {
                FundModels.Age16To19ExcludingApprenticeships,
                FundModels.AdultSkills,
                FundModels.NotFundedByESFA
            };

        private readonly string[] _notionalNVQLevels =
            {
                LARSConstants.NotionalNVQLevelV2Strings.Level4,
                LARSConstants.NotionalNVQLevelV2Strings.Level5,
                LARSConstants.NotionalNVQLevelV2Strings.Level6,
                LARSConstants.NotionalNVQLevelV2Strings.Level7,
                LARSConstants.NotionalNVQLevelV2Strings.Level8,
                LARSConstants.NotionalNVQLevelV2Strings.HigherLevel
            };

        private readonly IDerivedData_07Rule _dd07;
        private readonly IFileDataService _fileDataService;
        private readonly ILARSDataService _lARSDataService;
        private readonly IDerivedData_27Rule _derivedData_27Rule;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearningDeliveryHE_03Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IOrganisationDataService organisationDataService,
            IDerivedData_27Rule derivedData_27Rule,
            ILARSDataService lARSDataService,
            IFileDataService fileDataService,
            IDerivedData_07Rule dd07)
            : base(validationErrorHandler, RuleNameConstants.LearningDeliveryHE_03)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _organisationDataService = organisationDataService;
            _derivedData_27Rule = derivedData_27Rule;
            _lARSDataService = lARSDataService;
            _fileDataService = fileDataService;
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null
                    || _organisationDataService.LegalOrgTypeMatchForUkprn(_fileDataService.UKPRN(), LegalOrgTypeConstants.UHEO)
                    || !DerivedData27ConditionMet(_fileDataService.UKPRN()))
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.LearnAimRef,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearningDeliveryHEEntity,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(
            int fundModel,
            string learnAimRef,
            int? progTypeNullable,
            ILearningDeliveryHE learningDeliveryHEEntity,
            IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
                && LARSNotionalNVQLevelV2ConditionMet(learnAimRef)
                && LearningDeliveryHEConditionMet(learningDeliveryHEEntity)
                && DD07ConditionMet(progTypeNullable)
                && LearningDeliveryFAMsCondtionMet(learningDeliveryFAMs);
        }

        public bool LearningDeliveryFAMsCondtionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
            => !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352");

        public bool DD07ConditionMet(int? progType) => !_dd07.IsApprenticeship(progType);

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHEEntity) => learningDeliveryHEEntity == null;

        public bool LARSNotionalNVQLevelV2ConditionMet(string learnAimRef) => _lARSDataService.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels);

        public bool DerivedData27ConditionMet(int uKPRN) => _derivedData_27Rule.IsUKPRNCollegeOrGrantFundedProvider(uKPRN);

        public bool FundModelConditionMet(int fundModel) => _fundModels.Contains(fundModel);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
