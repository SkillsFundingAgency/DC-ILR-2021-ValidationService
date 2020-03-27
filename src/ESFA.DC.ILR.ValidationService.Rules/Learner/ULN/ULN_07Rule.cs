using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IAcademicYearDataService _academicDataQueryService;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly IFileDataService _fileDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly IEnumerable<int> _fundModels = new HashSet<int> { 25, 35, 36, 70, 81, 82 };

        public ULN_07Rule(
            IAcademicYearDataService academicDataQueryService,
            IDateTimeQueryService dateTimeQueryService,
            IFileDataService fileDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ULN_07)
        {
            _academicDataQueryService = academicDataQueryService;
            _dateTimeQueryService = dateTimeQueryService;
            _fileDataService = fileDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var filePrepDate = _fileDataService.FilePreparationDate();
            var januaryFirst = _academicDataQueryService.JanuaryFirst();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (!LearningDeliveryFAMConditionMet(learningDelivery.LearningDeliveryFAMs))
                {
                    return;
                }

                if (ConditionMet(
                    objectToValidate.ULN,
                    learningDelivery.FundModel,
                    learningDelivery.LearningDeliveryFAMs,
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearnPlanEndDate,
                    learningDelivery.LearnActEndDateNullable,
                    filePrepDate,
                    januaryFirst))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.ULN, filePrepDate, learningDelivery.LearnStartDate));
                    return;
                }
            }
        }

        public bool ConditionMet(
            long uln,
            int fundModel,
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs,
            DateTime learnStartDate,
            DateTime learnPlanEndDate,
            DateTime? learnActEndDate,
            DateTime filePrepDate,
            DateTime januaryFirst)
        {
            return UlnConditionMet(uln)
                && FundModelConditionMet(fundModel, learningDeliveryFAMs)
                && FilePreparationDateConditionMet(learnStartDate, filePrepDate, januaryFirst)
                && LearningDatesConditionMet(learnStartDate, learnPlanEndDate, learnActEndDate);
        }

        public bool UlnConditionMet(long uln)
        {
            return uln == ValidationConstants.TemporaryULN;
        }

        public bool FundModelConditionMet(int fundModel, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _fundModels.Contains(fundModel)
                || (fundModel == 99 && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "ADL", "1"));
        }

        public bool FilePreparationDateConditionMet(DateTime learnStartDate, DateTime filePrepDate, DateTime januaryFirst)
        {
            return filePrepDate >= januaryFirst
                && _dateTimeQueryService.DaysBetween(learnStartDate, filePrepDate) > 60;
        }

        public bool LearningDatesConditionMet(DateTime learnStartDate, DateTime learnPlanEndDate, DateTime? learnActEndDate)
        {
            return _dateTimeQueryService.DaysBetween(learnStartDate, learnPlanEndDate) >= 5
                || (learnActEndDate.HasValue && _dateTimeQueryService.DaysBetween(learnStartDate, (DateTime)learnActEndDate) >= 5);
        }

        public virtual bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return !(_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.LDM, "034")
                || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.ACT, "1"));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln, DateTime filePrepDate, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln),
                BuildErrorMessageParameter(PropertyNameConstants.FilePreparationDate, filePrepDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}