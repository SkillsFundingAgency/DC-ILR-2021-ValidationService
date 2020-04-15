using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_14Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The (academic) year data (service)
        /// </summary>
        private readonly IFCSDataService _fcsData;

        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_14Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData22">The derived data 22 rule.</param>
        /// <param name="fcsData">The year data.</param>
        /// <param name="commonOperations">The common operations.</param>
        public EmpStat_14Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fcsData,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_14)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fcsData)
                .AsGuard<ArgumentNullException>(nameof(fcsData));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _fcsData = fcsData;
            _check = commonOperations;
        }

        /// <summary>
        /// Gets the qualifying aim on this date using sources.
        /// the incoming set is guaranteed not to be null
        /// </summary>
        /// <param name="usingSources">using sources.</param>
        /// <returns>
        /// the qualifying aim on this date
        /// </returns>
        public ILearningDelivery GetQualifyingdAimOn(IReadOnlyCollection<ILearningDelivery> usingSources) =>
            usingSources
                .Where(x => x.FundModel == TypeOfFunding.EuropeanSocialFund
                    && x.LearnAimRef == TypeOfAim.References.ESFLearnerStartandAssessment
                    && x.CompStatus == CompletionState.HasCompleted)
                .OrderByDescending(x => x.LearnStartDate)
                .FirstOrDefault();

        /// <summary>
        /// Gets the eligible employment status.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the eligible employment status for the allocated contract</returns>
        public IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> GetEligibilityRulesFor(ILearningDelivery delivery) =>
            _fcsData.GetEligibilityRuleEmploymentStatusesFor(delivery?.ConRefNumber).ToReadOnlyCollection();

        /// <summary>
        /// Determines whether [has a qualifying employment status] [the specified employment].
        /// at the point of calling, neither the employment or the eligibility can be null
        /// </summary>
        /// <param name="eligibility">The eligibility (status).</param>
        /// <param name="thisEmployment">this employment.</param>
        /// <returns>
        ///   <c>true</c> if [has a qualifying employment status] [the specified this employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAQualifyingEmploymentStatus(IEsfEligibilityRuleEmploymentStatus eligibility, ILearnerEmploymentStatus thisEmployment) =>
            eligibility.Code == thisEmployment.EmpStat;

        public bool IsNotValid(IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> eligibilities, ILearnerEmploymentStatus employment) =>
            !eligibilities.Any(x => HasAQualifyingEmploymentStatus(x, employment));

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            var fromDeliveries = objectToValidate.LearningDeliveries.ToReadOnlyCollection();
            var qualifyingAim = GetQualifyingdAimOn(fromDeliveries);

            if (It.IsNull(qualifyingAim))
            {
                return;
            }

            var eligibilities = GetEligibilityRulesFor(qualifyingAim);

            if (eligibilities.IsNullOrEmpty())
            {
                return;
            }

            var fromEmployments = objectToValidate.LearnerEmploymentStatuses.ToReadOnlyCollection();
            var employment = _check.GetEmploymentStatusOn(qualifyingAim.LearnStartDate, fromEmployments);

            if (It.IsNull(employment))
            {
                return;
            }

            if (IsNotValid(eligibilities, employment))
            {
                RaiseValidationMessage(learnRefNumber, qualifyingAim, employment);
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="thisEmployment">this employment.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, ILearnerEmploymentStatus thisEmployment)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery, thisEmployment));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="thisEmployment">this employment.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
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
