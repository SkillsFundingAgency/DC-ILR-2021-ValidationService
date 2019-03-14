using System;
using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_48Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augustFirst2016 = new DateTime(2016, 08, 01);
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IDerivedData_04Rule _dd04;

        public DateOfBirth_48Rule(
            IDerivedData_07Rule dd07,
            IDerivedData_04Rule dd04,
            IAcademicYearQueryService academicYearQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_48)
        {
            _dd07 = dd07;
            _dd04 = dd04;
            _academicYearQueryService = academicYearQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null
                || LearnerConditionMet(objectToValidate.DateOfBirthNullable))
            {
                return;
            }

            DateTime sixteenthBirthDate = objectToValidate.DateOfBirthNullable.Value.AddYears(16);

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                DateTime? dd04Date = _dd04.GetEarliesStartDateFor(learningDelivery, objectToValidate.LearningDeliveries);
                DateTime lastFridayInJuneForAcademicYear = _academicYearQueryService.LastFridayInJuneForDateInAcademicYear(sixteenthBirthDate);

                if (ConditionMet(learningDelivery.ProgTypeNullable, dd04Date, lastFridayInJuneForAcademicYear))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable, learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(int? progType, DateTime? dd04Date, DateTime lastFridayInJune)
        {
            return DD07ConditionMet(progType)
                && DD04ConditionMet(dd04Date, lastFridayInJune);
        }

        public bool LearnerConditionMet(DateTime? dateOfBirth)
        {
            return !dateOfBirth.HasValue;
        }

        public bool DD07ConditionMet(int? progType)
        {
            return progType.HasValue
                && progType != TypeOfLearningProgramme.ApprenticeshipStandard
                && _dd07.IsApprenticeship(progType);
        }

        public bool DD04ConditionMet(DateTime? dd04, DateTime lastFridayJuneAcademicYearLearnerSixteen)
        {
            return dd04.HasValue
                && dd04 >= _augustFirst2016
                && dd04 <= lastFridayJuneAcademicYearLearnerSixteen;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
