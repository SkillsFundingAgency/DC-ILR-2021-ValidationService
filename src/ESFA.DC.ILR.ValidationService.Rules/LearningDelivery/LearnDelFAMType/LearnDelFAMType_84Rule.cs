using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_84Rule : AbstractRule, IRule<ILearner>
    {
        private const int AgeOffset = -19;

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IAcademicYearDataService _academicYearService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        private readonly HashSet<string> _ldmCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_376
        };

        public LearnDelFAMType_84Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IAcademicYearDataService academicYearService,
            IDateTimeQueryService dateTimeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_84)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _academicYearService = academicYearService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (!_learningDeliveryFAMQueryService.GetLearningDeliveryFAMsForTypeAndCodes(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, _ldmCodes).Any())
                {
                    continue;
                }

                if (ConditionMet(learner.DateOfBirthNullable, learningDelivery.FundModel))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learner.DateOfBirthNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? dateOfBirth, int fundModel) =>
            IsOutsideDateOfBirthRange(dateOfBirth)
            && IsAdultSkillsFundingModel(fundModel);

        public bool IsOutsideDateOfBirthRange(DateTime? dateOfBirth) =>
            dateOfBirth < _dateTimeQueryService.AddYearsToDate(_academicYearService.Start(), AgeOffset);

        public bool IsAdultSkillsFundingModel(int fundModel) =>
            fundModel == FundModels.AdultSkills;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_376),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
            };
        }
    }
}
