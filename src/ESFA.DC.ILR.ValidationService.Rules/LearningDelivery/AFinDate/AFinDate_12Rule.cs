using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate
{
    public class AFinDate_12Rule : AbstractRule, IRule<ILearner>
    {
        private const int _aimType = TypeOfAim.ProgrammeAim;
        private const int _numberOfYears = 2;

        private readonly IDerivedData_07Rule _dd07;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public AFinDate_12Rule(IDerivedData_07Rule dd07, IDateTimeQueryService dateTimeQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_12)
        {
            _dd07 = dd07;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.AppFinRecords != null
                    && learningDelivery.LearnActEndDateNullable != null
                    && IsAppsStandardOrFramework(learningDelivery.AimType, learningDelivery.ProgTypeNullable))
                {
                    foreach (var appFinRecord in learningDelivery.AppFinRecords)
                    {
                        var aFinRecord = AFinRecordWithDateGreaterThanLearnActEndDate(learningDelivery.LearnActEndDateNullable.Value, appFinRecord);

                        if (aFinRecord != null)
                        {
                            HandleValidationError(
                                objectToValidate.LearnRefNumber,
                                learningDelivery.AimSeqNumber,
                                BuildErrorMessageParameters(
                                    aFinRecord.AFinDate,
                                    learningDelivery.LearnActEndDateNullable));
                        }
                    }
                }
            }
        }

        public IAppFinRecord AFinRecordWithDateGreaterThanLearnActEndDate(DateTime learnActEndDate, IAppFinRecord appFinRecord)
            => appFinRecord.AFinDate > _dateTimeQueryService.AddYearsToDate(learnActEndDate, _numberOfYears) ? appFinRecord : null;

        public bool IsAppsStandardOrFramework(int aimType, int? progType) => aimType == _aimType
                && _dd07.IsApprenticeship(progType);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime aFinDate, DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate)
            };
        }
    }
}
