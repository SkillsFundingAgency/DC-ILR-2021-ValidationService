using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade
{
    public class OutGrade_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;
        private readonly IProvideLookupDetails _lookupDetails;

        private readonly DateTime _ruleStartDate = new DateTime(2014, 8, 1);

        private readonly string[] _learnAinRefTypes =
        {
            LARSConstants.LearnAimRefTypes.GCEASLevel,
            LARSConstants.LearnAimRefTypes.GCEALevel,
            LARSConstants.LearnAimRefTypes.GCSE,
            LARSConstants.LearnAimRefTypes.GCEA2Level,
            LARSConstants.LearnAimRefTypes.GSCEVocational,
            LARSConstants.LearnAimRefTypes.GCEAppliedALevel,
            LARSConstants.LearnAimRefTypes.GCEAppliedASLevelDoubleAward,
            LARSConstants.LearnAimRefTypes.GCEAppliedA2,
            LARSConstants.LearnAimRefTypes.GCEAppliedA2DoubleAward,
            LARSConstants.LearnAimRefTypes.GCEALevelWithGCEAdvancedSubsidiary,
            LARSConstants.LearnAimRefTypes.ShortCourseGCSE
        };

        public OutGrade_07Rule(
            ILARSDataService larsDataService,
            IProvideLookupDetails lookupDetails,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutGrade_07)
        {
            _larsDataService = larsDataService;
            _lookupDetails = lookupDetails;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if ((learningDelivery.LearnActEndDateNullable ?? DateTime.MaxValue) < _ruleStartDate)
                {
                    continue;
                }

                if ((learningDelivery.OutcomeNullable ?? -1) != OutcomeConstants.Achieved
                    || string.IsNullOrEmpty(learningDelivery.OutGrade))
                {
                    continue;
                }

                var larsLearningDelivery = _larsDataService.GetDeliveryFor(learningDelivery.LearnAimRef);

                if (!_learnAinRefTypes.Any(l => l.CaseInsensitiveEquals(larsLearningDelivery?.LearnAimRefType)))
                {
                    continue;
                }

                if (!_lookupDetails.Contains(
                    TypeOfListItemLookup.LearningAimType,
                    larsLearningDelivery.LearnAimRefType,
                    learningDelivery.OutGrade))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery));
                }
            }
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learningDelivery.LearnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, learningDelivery.OutcomeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.OutGrade, learningDelivery.OutGrade)
            };
        }
    }
}
