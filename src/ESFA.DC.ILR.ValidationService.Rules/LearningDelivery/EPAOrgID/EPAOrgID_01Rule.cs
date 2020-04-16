using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EPAOrgID
{
    public class EPAOrgID_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEPAOrganisationDataService _epaOrganisationDataService;

        public EPAOrgID_01Rule(
            IEPAOrganisationDataService epaOrganisationDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EPAOrgID_01)
        {
            _epaOrganisationDataService = epaOrganisationDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.EPAOrgID,
                    learningDelivery.StdCodeNullable,
                    learningDelivery.LearnPlanEndDate))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.EPAOrgID, learningDelivery.StdCodeNullable, learningDelivery.LearnPlanEndDate));
                }
            }
        }

        public bool ConditionMet(string epaOrgID, int? stdCode, DateTime learnPlanEndDate)
        {
            return epaOrgID != null && stdCode.HasValue
                ? !_epaOrganisationDataService.IsValidEpaOrg(epaOrgID, stdCode, learnPlanEndDate)
                : false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string epaOrgID, int? stdCode, DateTime learnPlanEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EPAOrgID, epaOrgID),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, stdCode),
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate)
            };
        }
    }
}
