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
    public class ULN_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataService _fileDataService;
        private readonly IAcademicYearDataService _academicDataQueryService;
        private readonly ILearnerQueryService _learnerQueryService;

        private readonly IEnumerable<long> _fundModels =
            new HashSet<long>()
            {
                TypeOfFunding.Age16To19ExcludingApprenticeships,
                TypeOfFunding.Other16To19,
                TypeOfFunding.AdultSkills,
                TypeOfFunding.ApprenticeshipsFrom1May2017,
                TypeOfFunding.OtherAdult,
                TypeOfFunding.EuropeanSocialFund,
            };

        public ULN_03Rule(IFileDataService fileDataService, IAcademicYearDataService academicDataQueryService, ILearnerQueryService learnerQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ULN_03)
        {
            _fileDataService = fileDataService;
            _academicDataQueryService = academicDataQueryService;
            _learnerQueryService = learnerQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (LearnerConditionMet(objectToValidate.ULN))
            {
                var filePrepDate = _fileDataService.FilePreparationDate();
                var januaryFirst = _academicDataQueryService.JanuaryFirst();

                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(learningDelivery.FundModel, filePrepDate, januaryFirst) && LearningDeliveryFAMConditionMet(objectToValidate))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.ULN, filePrepDate));
                        return;
                    }
                }
            }
        }

        public bool LearnerConditionMet(long uln)
        {
            return uln == ValidationConstants.TemporaryULN;
        }

        public bool ConditionMet(long fundModel, DateTime filePreparationDate, DateTime academicYearJanuaryFirst)
        {
            return filePreparationDate < academicYearJanuaryFirst
                   && _fundModels.Contains(fundModel);
        }

        public bool LearningDeliveryFAMConditionMet(ILearner learner)
        {
            return !_learnerQueryService.HasLearningDeliveryFAMCodeForType(learner, LearningDeliveryFAMTypeConstants.ACT, "1");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln, DateTime filePreparationDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln),
                BuildErrorMessageParameter(PropertyNameConstants.FilePreparationDate, filePreparationDate)
            };
        }
    }
}