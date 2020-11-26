using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_14Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fcsData;
        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;

        public EmpStat_14Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fcsData,
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_14)
        {
            _fcsData = fcsData;
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
        }

        public ILearningDelivery GetQualifyingdAimOn(IReadOnlyCollection<ILearningDelivery> usingSources) =>
            usingSources
                .Where(x => x.FundModel == FundModels.EuropeanSocialFund
                    && x.LearnAimRef == AimTypes.References.ESFLearnerStartandAssessment
                    && x.CompStatus == CompletionState.HasCompleted)
                .OrderByDescending(x => x.LearnStartDate)
                .FirstOrDefault();

        public IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> GetEligibilityRulesFor(ILearningDelivery delivery) =>
            _fcsData.GetEligibilityRuleEmploymentStatusesFor(delivery?.ConRefNumber).ToReadOnlyCollection();

        public bool HasAQualifyingEmploymentStatus(IEsfEligibilityRuleEmploymentStatus eligibility, ILearnerEmploymentStatus thisEmployment) =>
            eligibility.Code == thisEmployment.EmpStat;

        public bool IsNotValid(IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> eligibilities, ILearnerEmploymentStatus employment) =>
            !eligibilities.Any(x => HasAQualifyingEmploymentStatus(x, employment));

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            var fromDeliveries = objectToValidate.LearningDeliveries.ToReadOnlyCollection();
            var qualifyingAim = GetQualifyingdAimOn(fromDeliveries);

            if (qualifyingAim == null)
            {
                return;
            }

            var eligibilities = GetEligibilityRulesFor(qualifyingAim);

            if (eligibilities.IsNullOrEmpty())
            {
                return;
            }

            var fromEmployments = objectToValidate.LearnerEmploymentStatuses.ToReadOnlyCollection();
            var employment = _learnerEmploymentStatusQueryService.LearnerEmploymentStatusForDate(fromEmployments, qualifyingAim.LearnStartDate);

            if (employment == null)
            {
                return;
            }

            if (IsNotValid(eligibilities, employment))
            {
                RaiseValidationMessage(learnRefNumber, qualifyingAim, employment);
            }
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, ILearnerEmploymentStatus thisEmployment)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery, thisEmployment));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery, ILearnerEmploymentStatus thisEmployment)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, thisEmployment.EmpStat),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, thisDelivery.ConRefNumber),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };
        }
    }
}
