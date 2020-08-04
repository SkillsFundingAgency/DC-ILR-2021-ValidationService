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

            learner.LearningDeliveries.Where(ld => ld.FundModel == FundModels.AdultSkills).ForEach(ld =>
            {
                var learningDeliveryFams = _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsForTypeAndCodes(
                    ld.LearningDeliveryFAMs,
                    LearningDeliveryFAMTypeConstants.LDM,
                    _ldmCodes);

                if (learningDeliveryFams.Any())
                {
                    learningDeliveryFams.ForEach(ldf =>
                    {
                        if (IsOutsideDateOfBirthRange(learner.DateOfBirthNullable))
                        {
                            HandleValidationError(
                                learner.LearnRefNumber,
                                ld.AimSeqNumber,
                                BuildErrorMessageParameters(ldf.LearnDelFAMCode, learner.DateOfBirthNullable, ld.FundModel));
                        }
                    });
                }
            });
        }

        public bool IsOutsideDateOfBirthRange(DateTime? dateOfBirth) =>
            dateOfBirth < _dateTimeQueryService.AddYearsToDate(_academicYearService.Start(), AgeOffset);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string delFamCode, DateTime? dateOfBirth, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, delFamCode),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
            };
        }
    }
}
