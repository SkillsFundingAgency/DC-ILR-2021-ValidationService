using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_57Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { FundModels.AdultSkills, FundModels.EuropeanSocialFund, FundModels.OtherAdult };
        private readonly DateTime _firstAugust2015 = new DateTime(2015, 08, 01);
        private readonly DateTime _endJuly2016 = new DateTime(2016, 07, 31);
        private readonly int _larsCategoryRef = 20;

        private readonly ILARSDataService _larsDataService;
        private readonly ILearnerEmploymentStatusMonitoringQueryService _learnerEmploymentStatusMonitoringQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public LearnAimRef_57Rule(ILARSDataService larsDataService, ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_57)
        {
            _larsDataService = larsDataService;
            _learnerEmploymentStatusMonitoringQueryService = learnerEmploymentStatusMonitoringQueryService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public LearnAimRef_57Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.LearnAimRef,
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearningDeliveryFAMs,
                    objectToValidate.LearnerEmploymentStatuses))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearnAimRef,
                            learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(int fundModel, string learnAimRef, DateTime learnStartDate,  IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return FundModelConditionMet(fundModel)
                && RuleNotTriggeredConditionMet(learningDeliveryFAMs)
                && LARSCategoryConditionMet(learnAimRef)
                && LearnStartDateConditionMet(learnStartDate)
                && EsmConditionMet(learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs);
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public virtual bool RuleNotTriggeredConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "347");
        }

        public virtual bool LARSCategoryConditionMet(string learnAimRef)
        {
            return _larsDataService.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, _larsCategoryRef);
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _firstAugust2015 && learnStartDate <= _endJuly2016;
        }

        public virtual bool EsmConditionMet(DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            bool bsiThree = false;
            bool bsiFour = false;

            if (_learnerEmploymentStatusMonitoringQueryService.HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "BSI", 3))
            {
                bsiThree = learnerEmploymentStatuses
                    .Where(esm => esm.EmploymentStatusMonitorings.Any(esmt => esmt.ESMType == "BSI" && esmt.ESMCode == 3))
                    .Select(les => les.DateEmpStatApp).First() <= learnStartDate;
            }

            if (_learnerEmploymentStatusMonitoringQueryService.HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "BSI", 4))
            {
                bsiFour =
                !_learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")
                && (learnerEmploymentStatuses
                  .Where(esm => esm.EmploymentStatusMonitorings.Any(esmt => esmt.ESMType == "BSI" && esmt.ESMCode == 4))
                  .Select(les => les.DateEmpStatApp).First() <= learnStartDate);
            }

            return bsiThree || bsiFour;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
            };
        }
    }
}
