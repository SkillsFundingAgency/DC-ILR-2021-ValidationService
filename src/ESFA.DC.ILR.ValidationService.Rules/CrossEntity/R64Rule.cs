using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    /// <summary>
    /// cross record rule 64
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class R64Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check(er, rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// Initializes a new instance of the <see cref="R64Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOps">The common ops.</param>
        /// <param name="larsDataService">The lars data service.</param>
        public R64Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOps,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.R64)
        {
            _check = commonOps;
            _larsData = larsDataService;
        }

        /// <summary>
        /// Validates the specified learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnrefNumber = theLearner.LearnRefNumber;
            var qualifyingDeliveries = theLearner.LearningDeliveries
                .SafeWhere(IsQualifyingItem)
                .AsSafeReadOnlyList(); ;
            var completedDeliveries = qualifyingDeliveries
                .Where(HasCompletedWithAchievement);

            completedDeliveries
                .ForAny(x => IsNotValid(qualifyingDeliveries, y => IsAMatch(x, y)), x => RaiseValidationMessage(learnrefNumber, x));
        }

        /// <summary>
        /// Determines whether [is qualifying item] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying item] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingItem(ILearningDelivery theDelivery) =>
            !IsExcluded(theDelivery)
            && HasQualifyingModel(theDelivery)
            && IsComponentAim(theDelivery)
            && HasQualifyingFrameworkAim(theDelivery);

        /// <summary>
        /// Determines whether the specified the delivery is excluded.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery theDelivery) =>
            IsTraineeship(theDelivery);

        /// <summary>
        /// Determines whether the specified the delivery is traineeship.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery is traineeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTraineeship(ILearningDelivery theDelivery) =>
            _check.IsTraineeship(theDelivery);

        /// <summary>
        /// Determines whether [has qualifying model] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.AdultSkills, TypeOfFunding.ApprenticeshipsFrom1May2017);

        /// <summary>
        /// Determines whether [is component aim] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is component aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComponentAim(ILearningDelivery theDelivery) =>
            _check.IsComponentOfAProgram(theDelivery);

        /// <summary>
        /// Determines whether [has qualifying framework aim] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying framework aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFrameworkAim(ILearningDelivery theDelivery) =>
            HasQualifyingFrameworkAim(GetFrameworkAimsFor(theDelivery), x => HasMatchingRequirements(x, theDelivery));

        /// <summary>
        /// Gets the framework aims for.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>a readonly collection of framework aims</returns>
        public IReadOnlyCollection<ILARSFrameworkAim> GetFrameworkAimsFor(ILearningDelivery theDelivery) =>
            _larsData.GetFrameworkAimsFor(theDelivery.LearnAimRef);

        /// <summary>
        /// Determines whether [has qualifying framework aim] [the specified framework aims].
        /// </summary>
        /// <param name="frameworkAims">The framework aims.</param>
        /// <param name="hasMatchingRequirements">has matching requirements.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying framework aim] [the specified framework aims]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFrameworkAim(IReadOnlyCollection<ILARSFrameworkAim> frameworkAims, Func<ILARSFrameworkAim, bool> hasMatchingRequirements) =>
            frameworkAims.SafeAny(hasMatchingRequirements);

        /// <summary>
        /// Determines whether [has matching requirements] [the specified aim].
        /// </summary>
        /// <param name="theAim">The aim.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has matching requirements] [the specified aim]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingRequirements(ILARSFrameworkAim theAim, ILearningDelivery theDelivery) =>
            HasMatchingProgramme(theAim, theDelivery)
                && HasMatchingFrameworkCode(theAim, theDelivery)
                && HasMatchingPathwayCode(theAim, theDelivery)
                && IsCurrentAim(theAim, theDelivery)
                && HasQualifyingComponentTypes(theAim);

        /// <summary>
        /// Determines whether [has matching programme] [the specified aim].
        /// </summary>
        /// <param name="theAim">The aim.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has matching programme] [the specified aim]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingProgramme(ILARSFrameworkAim theAim, ILearningDelivery theDelivery) =>
            theAim.ProgType == theDelivery.ProgTypeNullable;

        /// <summary>
        /// Determines whether [has matching framework code] [the specified aim].
        /// </summary>
        /// <param name="theAim">The aim.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has matching framework code] [the specified aim]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingFrameworkCode(ILARSFrameworkAim theAim, ILearningDelivery theDelivery) =>
            theAim.FworkCode == theDelivery.FworkCodeNullable;

        /// <summary>
        /// Determines whether [has matching pathway code] [the specified aim].
        /// </summary>
        /// <param name="theAim">The aim.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has matching pathway code] [the specified aim]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingPathwayCode(ILARSFrameworkAim theAim, ILearningDelivery theDelivery) =>
            theAim.PwayCode == theDelivery.PwayCodeNullable;

        /// <summary>
        /// Determines whether [has qualifying component types] [the specified aim].
        /// </summary>
        /// <param name="theAim">The aim.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying component types] [the specified aim]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingComponentTypes(ILARSFrameworkAim theAim) =>
            It.IsInRange(theAim.FrameworkComponentType, TypeOfLARSCommonComponent.Apprenticeship.CompetencyElement, TypeOfLARSCommonComponent.Apprenticeship.MainAimOrTechnicalCertificate);

        /// <summary>
        /// Determines whether [is current aim] [the specified aim].
        /// </summary>
        /// <param name="theAim">The aim.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is current aim] [the specified aim]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrentAim(ILARSFrameworkAim theAim, ILearningDelivery theDelivery) =>
            theAim.IsCurrent(theDelivery.LearnStartDate);

        /// <summary>
        /// Determines whether [has completed with achievement] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has completed with achievement] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasCompletedWithAchievement(ILearningDelivery theDelivery) =>
            HasCompleted(theDelivery)
            && HasAchievement(theDelivery);

        /// <summary>
        /// Determines whether the specified the delivery has completed.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery has completed; otherwise, <c>false</c>.
        /// </returns>
        public bool HasCompleted(ILearningDelivery theDelivery) =>
            theDelivery.CompStatus == CompletionState.HasCompleted;

        /// <summary>
        /// Determines whether the specified the delivery has achievement.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery has achievement; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAchievement(ILearningDelivery theDelivery) =>
            theDelivery.OutcomeNullable == OutcomeConstants.Achieved;

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(IReadOnlyCollection<ILearningDelivery> theDeliveries, Func<ILearningDelivery, bool> hasFoundAnother) =>
            theDeliveries.Any(hasFoundAnother);

        /// <summary>
        /// Determines whether [is a match] [the specified candidate].
        /// </summary>
        /// <param name="theCandidate">The candidate.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is a match] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAMatch(ILearningDelivery theCandidate, ILearningDelivery theDelivery) =>
            HasMatchingProgramme(theCandidate, theDelivery)
            && HasMatchingFrameworkCode(theCandidate, theDelivery)
            && HasMatchingPathwayCode(theCandidate, theDelivery)
            && HasQualifyingDates(theCandidate, theDelivery);

        /// <summary>
        /// Determines whether [has matching programme] [the specified candidate].
        /// </summary>
        /// <param name="theCandidate">The candidate.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has matching programme] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingProgramme(ILearningDelivery theCandidate, ILearningDelivery theDelivery) =>
            theCandidate.ProgTypeNullable == theDelivery.ProgTypeNullable;

        /// <summary>
        /// Determines whether [has matching framework code] [the specified candidate].
        /// </summary>
        /// <param name="theCandidate">The candidate.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has matching framework code] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingFrameworkCode(ILearningDelivery theCandidate, ILearningDelivery theDelivery) =>
            theCandidate.FworkCodeNullable == theDelivery.FworkCodeNullable;

        /// <summary>
        /// Determines whether [has matching pathway code] [the specified candidate].
        /// </summary>
        /// <param name="theCandidate">The candidate.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has matching pathway code] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingPathwayCode(ILearningDelivery theCandidate, ILearningDelivery theDelivery) =>
            theCandidate.PwayCodeNullable == theDelivery.PwayCodeNullable;

        /// <summary>
        /// Determines whether [has qualifying dates] [the specified candidate].
        /// </summary>
        /// <param name="theCandidate">The candidate.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying dates] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingDates(ILearningDelivery theCandidate, ILearningDelivery theDelivery) =>
            theCandidate.LearnStartDate > theDelivery.LearnStartDate;

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="theDelivery">The delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns></returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.AimType, theDelivery.AimType),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.ProgType, theDelivery.ProgTypeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.FworkCode, theDelivery.FworkCodeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.PwayCode, theDelivery.PwayCodeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.StdCode, theDelivery.StdCodeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
            BuildErrorMessageParameter(PropertyNameConstants.Outcome, theDelivery.OutcomeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.CompStatus, theDelivery.CompStatus)
        };
    }
}
