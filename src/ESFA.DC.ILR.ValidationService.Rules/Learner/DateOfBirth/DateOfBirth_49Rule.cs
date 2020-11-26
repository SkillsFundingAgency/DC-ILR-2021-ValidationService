using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_49Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augustFirst2016 = new DateTime(2016, 08, 01);
        private readonly DateTime _julyThirtyFirst2017 = new DateTime(2017, 07, 31);
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IFileDataService _fileDataService;

        private readonly HashSet<string> _notionalNvqLevels = new HashSet<string>
        {
            LARSConstants.NotionalNVQLevelV2Strings.Level3,
            LARSConstants.NotionalNVQLevelV2Strings.Level4,
            LARSConstants.NotionalNVQLevelV2Strings.Level5,
            LARSConstants.NotionalNVQLevelV2Strings.Level6,
            LARSConstants.NotionalNVQLevelV2Strings.Level7,
            LARSConstants.NotionalNVQLevelV2Strings.Level8,
            LARSConstants.NotionalNVQLevelV2Strings.HigherLevel
        };

        private readonly HashSet<string> _learningDeliveryFamCodes = new HashSet<string>
        {
            LearningDeliveryFAMCodeConstants.LDM_OLASS,
            LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy,
            LearningDeliveryFAMCodeConstants.LDM_SolentCity
        };

        public DateOfBirth_49Rule(
            IDateTimeQueryService dateTimeQueryService,
            IDerivedData_07Rule dd07,
            IFileDataService fileDataService,
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IOrganisationDataService organisationDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_49)
        {
            _dd07 = dd07;
            _dateTimeQueryService = dateTimeQueryService;
            _larsDataService = larsDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
            _organisationDataService = organisationDataService;
            _fileDataService = fileDataService;
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null || !learner.DateOfBirthNullable.HasValue)
            {
                return;
            }

            var ukprn = _fileDataService.UKPRN();

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (!Excluded(ukprn, learningDelivery.ProgTypeNullable, learningDelivery.LearningDeliveryFAMs)
                    && ConditionMet(learner.DateOfBirthNullable, learningDelivery.LearnStartDate, learningDelivery.FundModel, learningDelivery.LearnAimRef))
                {
                    HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learner.DateOfBirthNullable, learningDelivery.LearnStartDate, learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(DateTime? learnerDateOfBirth, DateTime learnStartDate, int fundModel, string learnAimRef)
        {
            return LearnStartDateConditionMet(learnStartDate)
                   && DateOfBirthConditionMet(learnerDateOfBirth, learnStartDate)
                   && FundModelConditionMet(fundModel)
                   && LARSNotionalNVQL2ConditionMet(learnAimRef);
        }

        public bool Excluded(int ukprn, int? progType, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return _dd07.IsApprenticeship(progType)
                   || _learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.RES)
                   || _learningDeliveryFamQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.LDM, _learningDeliveryFamCodes)
                   || _organisationDataService.LegalOrgTypeMatchForUkprn(ukprn, LegalOrgTypeConstants.USDC);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _augustFirst2016
                   && learnStartDate <= _julyThirtyFirst2017;
        }

        public bool DateOfBirthConditionMet(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return dateOfBirth.HasValue
                   && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, learnStartDate) >= 24;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModels.AdultSkills;
        }

        public bool LARSNotionalNVQL2ConditionMet(string learnAimRef)
        {
            return _larsDataService.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNvqLevels);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, DateTime learnStartDate, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
