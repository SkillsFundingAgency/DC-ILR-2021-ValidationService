using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
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

        private readonly DateTime _firstAugust2017 = new DateTime(2017, 08, 01);

        private readonly string[] _learnAimRefTypes =
        {
            TypeOfLARSLearnAimRef.GCEALevel,
            TypeOfLARSLearnAimRef.GCEA2Level,
            TypeOfLARSLearnAimRef.GCEAppliedALevel,
            TypeOfLARSLearnAimRef.GCEAppliedALevelDoubleAward,
            TypeOfLARSLearnAimRef.GCEALevelWithGCEAdvancedSubsidiary
        };

        private readonly string[] _notionalNvqLevels =
        {
            LARSNotionalNVQLevelV2.Level3,
            LARSNotionalNVQLevelV2.Level4,
            LARSNotionalNVQLevelV2.Level5,
            LARSNotionalNVQLevelV2.Level6,
            LARSNotionalNVQLevelV2.Level7,
            LARSNotionalNVQLevelV2.Level8,
            LARSNotionalNVQLevelV2.HigherLevel
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
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_55)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _larsDataService = larsDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
            _derivedData07 = derivedData07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null || !objectToValidate.DateOfBirthNullable.HasValue)
            {
                return;
            }

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
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, DateTime learnStartDate, DateTime dateOfBirth, string learnAimRef, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return !Excluded(progType, learningDeliveryFams, learnAimRef)
                   && fundModel == TypeOfFunding.AdultSkills
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
    }
}
