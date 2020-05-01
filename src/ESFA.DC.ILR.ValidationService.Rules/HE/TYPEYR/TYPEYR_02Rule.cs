using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.TYPEYR
{
    public class TYPEYR_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _dateCondition = new DateTime(2009, 08, 01);
        private readonly IAcademicYearQueryService _academicYearQueryService;

        public TYPEYR_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearQueryService academicYearQueryService)
            : base(validationErrorHandler, RuleNameConstants.TYPEYR_02)
        {
            _academicYearQueryService = academicYearQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryHEEntity != null)
                {
                    if (ConditionMetLearningDelivery(learningDelivery.LearnStartDate, learningDelivery.LearnActEndDateNullable))
                    {
                        if (ConditionMetLearningDeliveryHE(learningDelivery.LearningDeliveryHEEntity.FUNDCOMP, learningDelivery.LearningDeliveryHEEntity.TYPEYR))
                        {
                            HandleValidationError(
                                objectToValidate.LearnRefNumber,
                                learningDelivery.AimSeqNumber,
                                BuildErrorMessageParameters(
                                    learningDelivery.LearningDeliveryHEEntity.TYPEYR,
                                    learningDelivery.LearningDeliveryHEEntity.FUNDCOMP,
                                    learningDelivery.LearnStartDate,
                                    learningDelivery.LearnActEndDateNullable));
                        }
                    }
                }
            }
        }

        public bool ConditionMetLearningDeliveryHE(int fundComp, int typeYr)
        {
            return fundComp == 1 && typeYr != 1;
        }

        public bool ConditionMetLearningDelivery(DateTime learnStartDate, DateTime? learnActDateNullable)
        {
            var result = false;
            if (learnActDateNullable.HasValue)
            {
                var learnStartDateAcademicYear = _academicYearQueryService.GetAcademicYearOfLearningDate(learnStartDate, AcademicYearDates.Commencement);
                var learnEndDateAcademicYear = _academicYearQueryService.GetAcademicYearOfLearningDate(learnActDateNullable.Value, AcademicYearDates.Commencement);

                if (learnStartDate >= _dateCondition && learnStartDateAcademicYear.Year == learnEndDateAcademicYear.Year)
                {
                    result = true;
                }
            }

            return result;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int typeYr, int fundComp, DateTime learnStartDate, DateTime? learnActEndDateNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.TYPEYR, typeYr),
                BuildErrorMessageParameter(PropertyNameConstants.FUNDCOMP, fundComp),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDateNullable)
            };
        }
    }
}
