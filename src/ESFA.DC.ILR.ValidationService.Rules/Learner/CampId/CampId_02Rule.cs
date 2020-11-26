using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.CampId
{
    public class CampId_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IFileDataCache _fileDataCache;

        public CampId_02Rule(IOrganisationDataService organisationDataService, IValidationErrorHandler validationErrorHandler, IFileDataCache fileDataCache)
            : base(validationErrorHandler, RuleNameConstants.CampId_02)
        {
            _organisationDataService = organisationDataService;
            _fileDataCache = fileDataCache;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(objectToValidate.CampId))
            {
                if (ConditionMet(objectToValidate.CampId, _fileDataCache.UKPRN))
                {
                    HandleValidationError(learnRefNumber: objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.CampId, _fileDataCache.UKPRN));
                }
            }
        }

        public bool ConditionMet(string campId, long ukprn)
        {
            return !_organisationDataService.CampIdMatchForUkprn(campId, ukprn);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string campId, long ukprn)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.CampId, campId),
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn)
            };
        }
    }
}