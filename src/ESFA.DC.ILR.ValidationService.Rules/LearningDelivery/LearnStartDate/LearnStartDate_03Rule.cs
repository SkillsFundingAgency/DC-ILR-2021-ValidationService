using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;
        private readonly IAcademicYearDataService _academicYearDataService;

        public LearnStartDate_03Rule(
            IDerivedData_07Rule dd07,
            IAcademicYearDataService academicYearDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_03)
        {
            _dd07 = dd07;
            _academicYearDataService = academicYearDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, int? progType)
        {
            return DD07ConditionMet(progType)
                   && LearnStartDateConditionMet(learnStartDate);
        }

        public bool DD07ConditionMet(int? progType)
        {
            return progType != 24
                   && !_dd07.IsApprenticeship(progType);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            var academicYearEnd = _academicYearDataService.End();

            return learnStartDate > academicYearEnd;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
