using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode
{
    public class LSDPostcode_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int>() { TypeOfFunding.CommunityLearning, TypeOfFunding.AdultSkills };
        private readonly DateTime _firstAugust2019 = new DateTime(2019, 08, 01);

        private readonly IFileDataService _fileDataService;
        private readonly IPostcodesDataService _postcodesDataService;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LSDPostcode_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IPostcodesDataService postcodesDataService,
            IOrganisationDataService organisationDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LSDPostcode_02)
        {
            _fileDataService = fileDataService;
            _postcodesDataService = postcodesDataService;
            _organisationDataService = organisationDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            var ukprn = _fileDataService.UKPRN();
            var longTermResUkprn = _organisationDataService.IsLongTermResForUkprn(ukprn);

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                var devolvedPostcodes = _postcodesDataService.GetDevolvedPostcodes(learningDelivery.LSDPostcode);

                foreach (var sofLdFams in _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsForType(learningDelivery?.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF))
                {
                    if (
                       ConditionMet(
                       learningDelivery.LearnStartDate,
                       learningDelivery.FundModel,
                       learningDelivery.ProgTypeNullable,
                       learningDelivery.LSDPostcode,
                       devolvedPostcodes,
                       sofLdFams.LearnDelFAMCode,
                       learningDelivery.LearningDeliveryFAMs,
                       longTermResUkprn))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            errorMessageParameters: BuildErrorMessageParameters(
                                learningDelivery.LearnStartDate,
                                learningDelivery.FundModel,
                                learningDelivery.LSDPostcode,
                                LearningDeliveryFAMTypeConstants.SOF,
                                sofLdFams.LearnDelFAMCode));
                    }
                }
            }
        }

        public bool ConditionMet(
            DateTime learnStartDate,
            int fundModel,
            int? ProgType,
            string lsdPostcode,
            IEnumerable<IDevolvedPostcode> devolvedPostcodes,
            string sofCode,
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs,
            bool longTermResUkprn)
        {
            return
                LearnStartDateConditionMet(learnStartDate)
                && FundModelConditionMet(fundModel)
                && PostcodeConditionMet(devolvedPostcodes, learnStartDate, sofCode)
                && !IsExcluded(ProgType, lsdPostcode, learningDeliveryFAMs, longTermResUkprn);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate) => learnStartDate >= _firstAugust2019;

        public bool FundModelConditionMet(int fundModel) => _fundModels.Contains(fundModel);

        public bool PostcodeConditionMet(IEnumerable<IDevolvedPostcode> devolvedPostcodes, DateTime learnStartDate, string sofCode) =>
            PostcodeConditionOne(devolvedPostcodes, learnStartDate, sofCode)
            || PostcodeConditionTwo(devolvedPostcodes, learnStartDate, sofCode);

        public bool PostcodeConditionOne(IEnumerable<IDevolvedPostcode> devolvedPostcodes, DateTime learnStartDate, string sofCode) =>
            devolvedPostcodes.Any(dp => sofCode != dp.SourceOfFunding);

        public bool PostcodeConditionTwo(IEnumerable<IDevolvedPostcode> devolvedPostcodes, DateTime learnStartDate, string sofCode) =>
            devolvedPostcodes.Any(dp => sofCode == dp.SourceOfFunding && !(learnStartDate >= dp.EffectiveFrom && learnStartDate <= (dp.EffectiveTo ?? DateTime.MaxValue)));

        public bool IsExcluded(int? progType, string lsdPostcode, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, bool longTermResUkprn)
        {
            return progType.HasValue
                || lsdPostcode.CaseInsensitiveEquals(ValidationConstants.TemporaryPostCode)
                || longTermResUkprn
                || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)
                || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.DAM, LearningDeliveryFAMCodeConstants.DAM_Code_001)
                || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);

        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int fundModel, string lsdPostcode, string learnDelFamType, string learnDelFamCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, lsdPostcode),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFamType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFamCode)
            };
        }
    }
}
