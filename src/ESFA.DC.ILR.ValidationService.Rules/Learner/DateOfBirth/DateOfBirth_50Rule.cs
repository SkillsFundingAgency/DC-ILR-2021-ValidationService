using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_50Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly DateTime _julyThirtyFirst2016 = new DateTime(2016, 7, 31);

        public DateOfBirth_50Rule(
            IAcademicYearDataService academicYearDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_50)
        {
            _academicYearDataService = academicYearDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            if (objectToValidate.DateOfBirthNullable.HasValue)
            {
                var learnersSixteenthBirthdate = objectToValidate.DateOfBirthNullable.Value.AddYears(16);
                var firstAugustForAcademicYearOfLearnersSixteenthBirthDate = _academicYearDataService.GetAcademicYearOfLearningDate(learnersSixteenthBirthdate, AcademicYearDates.Commencement);

                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(
                        learningDelivery,
                        firstAugustForAcademicYearOfLearnersSixteenthBirthDate))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable, learningDelivery.LearnStartDate));
                    }
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, DateTime firstAugustForAcademicYearOfLearnersSixteenthBirthDate)
        {
            return (learningDelivery.ProgTypeNullable.HasValue && learningDelivery.ProgTypeNullable == TypeOfLearningProgramme.Traineeship)
                   && learningDelivery.AimType == TypeOfAim.ProgrammeAim
                   && learningDelivery.LearnStartDate > _julyThirtyFirst2016
                   && learningDelivery.LearnStartDate <= firstAugustForAcademicYearOfLearnersSixteenthBirthDate;
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
