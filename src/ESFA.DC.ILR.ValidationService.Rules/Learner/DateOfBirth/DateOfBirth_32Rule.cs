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
    public class DateOfBirth_32Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2016 = new DateTime(2016, 8, 1);
        private readonly DateTime _firstAugust2015 = new DateTime(2015, 8, 1);

        private readonly HashSet<string> _nvqLevel2s = new HashSet<string>()
        {
            LARSNotionalNVQLevelV2.Level3,
            LARSNotionalNVQLevelV2.Level4,
            LARSNotionalNVQLevelV2.Level5,
            LARSNotionalNVQLevelV2.Level6,
            LARSNotionalNVQLevelV2.Level7,
            LARSNotionalNVQLevelV2.Level8,
            LARSNotionalNVQLevelV2.HigherLevel
        };

        private readonly HashSet<string> _ldmCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_OLASS,
            LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy,
            LearningDeliveryFAMCodeConstants.LDM_SolentCity            
        };

        private readonly IDerivedData_07Rule _dd07;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly ILARSDataService _larsDataService;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IFileDataService _fileDataService;

        public DateOfBirth_32Rule(
            IDerivedData_07Rule dd07,
            IDateTimeQueryService dateTimeQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            ILARSDataService larsDataService,
            IOrganisationDataService organisationDataService,
            IFileDataService fileDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_32)
        {
            _dd07 = dd07;
            _dateTimeQueryService = dateTimeQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _larsDataService = larsDataService;
            _organisationDataService = organisationDataService;
            _fileDataService = fileDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var ukprn = _fileDataService.UKPRN();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnStartDate,
                    objectToValidate.DateOfBirthNullable,
                    learningDelivery.LearnAimRef,
                    learningDelivery.LearningDeliveryFAMs,
                    ukprn))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber, 
                        learningDelivery.AimSeqNumber, 
                        BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable, learningDelivery.FundModel));
                    return;
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, DateTime learnStartDate, DateTime? dateOfBirth, string learnAimRef, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, int ukprn)
        {
            return FundModelConditionMet(fundModel)
                && LearnStartDateConditionMet(learnStartDate)
                && DateOfBirthConditionMet(dateOfBirth, learnStartDate)
                && LARSNotionalNVQL2ConditionMet(learnAimRef)
                && DD07ConditionMet(progType)
                && LARSCategoryConditionMet(learnAimRef)
                && LearningDeliveryFAMConditionMet(learningDeliveryFAMs)
                && OrganisationTypeConditionMet(ukprn);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 35;
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate < _firstAugust2016 && learnStartDate >= _firstAugust2015;
        }

        public bool DateOfBirthConditionMet(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            var ageAtTime = _dateTimeQueryService.YearsBetween(dateOfBirth.Value, learnStartDate);
            var isTrue = _dateTimeQueryService.YearsBetween(dateOfBirth.Value, learnStartDate) >= 24;
            return dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, learnStartDate) >= 24;
        }

        public bool LARSNotionalNVQL2ConditionMet(string learnAimRef)
        {
            return _larsDataService.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _nvqLevel2s);
        }

        public bool DD07ConditionMet(int? progType)
        {
            return !_dd07.IsApprenticeship(progType);
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return  !(_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")
                || _learningDeliveryFAMQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", _ldmCodes));      
        }

        public bool LARSCategoryConditionMet(string learnAimRef)
        {
            return !_larsDataService.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, 19);
        }

        public bool OrganisationTypeConditionMet(int ukprn)
        {
            return !_organisationDataService.LegalOrgTypeMatchForUkprn(ukprn, "USDC");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}