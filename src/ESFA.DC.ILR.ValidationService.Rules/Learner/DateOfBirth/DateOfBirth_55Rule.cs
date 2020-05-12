using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_55Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly IDerivedData_07Rule _derivedData07;
        private readonly IFileDataService _fileDataService;

        private readonly DateTime _firstAugust2017 = new DateTime(2017, 08, 01);

        private readonly string[] _learnAimRefTypes =
        {
            LARSConstants.LearnAimRefTypes.GCEALevel,
            LARSConstants.LearnAimRefTypes.GCEA2Level,
            LARSConstants.LearnAimRefTypes.GCEAppliedALevel,
            LARSConstants.LearnAimRefTypes.GCEAppliedALevelDoubleAward,
            LARSConstants.LearnAimRefTypes.GCEALevelWithGCEAdvancedSubsidiary
        };

        private readonly string[] _notionalNvqLevels =
        {
            LARSConstants.NotionalNVQLevelV2Strings.Level3,
            LARSConstants.NotionalNVQLevelV2Strings.Level4,
            LARSConstants.NotionalNVQLevelV2Strings.Level5,
            LARSConstants.NotionalNVQLevelV2Strings.Level6,
            LARSConstants.NotionalNVQLevelV2Strings.Level7,
            LARSConstants.NotionalNVQLevelV2Strings.Level8,
            LARSConstants.NotionalNVQLevelV2Strings.HigherLevel
        };

        private readonly string[] _learningDeliveryFamCodes =
        {
            LearningDeliveryFAMCodeConstants.LDM_OLASS,
            LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy,
            LearningDeliveryFAMCodeConstants.LDM_SolentCity
        };

        public DateOfBirth_55Rule(
            IDateTimeQueryService dateTimeQueryService,
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IDerivedData_07Rule derivedData07,
            IFileDataService fileDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_55)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _larsDataService = larsDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
            _derivedData07 = derivedData07;
            _fileDataService = fileDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null || !objectToValidate.DateOfBirthNullable.HasValue)
            {
                return;
            }

            var ukprn = _fileDataService.UKPRN();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnStartDate,
                    objectToValidate.DateOfBirthNullable.Value,
                    learningDelivery.LearnAimRef,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            ukprn,
                            objectToValidate.DateOfBirthNullable,
                            learningDelivery.LearnStartDate,
                            learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, DateTime learnStartDate, DateTime dateOfBirth, string learnAimRef, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return !Excluded(progType, learningDeliveryFams, learnAimRef)
                   && fundModel == FundModels.AdultSkills
                   && learnStartDate >= _firstAugust2017
                   && _dateTimeQueryService.YearsBetween(dateOfBirth, learnStartDate) >= 24
                   && _larsDataService.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNvqLevels);
        }

        public bool Excluded(int? progType, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams, string learnAimRef)
        {
            return _derivedData07.IsApprenticeship(progType)
                || _learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.RES)
                || _learningDeliveryFamQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.LDM, _learningDeliveryFamCodes)
                || _larsDataService.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, _learnAimRefTypes);
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int ukprn, DateTime? dateOfBirth, DateTime learnStartDate, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
