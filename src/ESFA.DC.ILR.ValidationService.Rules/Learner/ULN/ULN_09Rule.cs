using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_09Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataService _fileDataService;
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public ULN_09Rule(IFileDataService fileDataService, IAcademicYearDataService academicYearDataService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ULN_09)
        {
            _fileDataService = fileDataService;
            _academicYearDataService = academicYearDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var filePrepDate = _fileDataService.FilePreparationDate();
            var januraryFirst = _academicYearDataService.JanuaryFirst();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(objectToValidate.ULN, filePrepDate, januraryFirst, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.ULN, filePrepDate));
                }
            }
        }

        public bool ConditionMet(long uln, DateTime filePrepDate, DateTime januraryFirst, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return ULNConditionMet(uln)
                   && FilePrepDateConditionMet(filePrepDate, januraryFirst)
                   && LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
        }

        public bool ULNConditionMet(long uln)
        {
            return uln == ValidationConstants.TemporaryULN;
        }

        public bool FilePrepDateConditionMet(DateTime filePrepDate, DateTime januraryFirst)
        {
            return filePrepDate >= januraryFirst;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln, DateTime filePrepDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln),
                BuildErrorMessageParameter(PropertyNameConstants.FilePreparationDate, filePrepDate)
            };
        }
    }
}