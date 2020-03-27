using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_72Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The FCS data (service)
        /// </summary>
        private readonly IFCSDataService _fcsData;

        /// <summary>
        /// The lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// The check (common opeartions provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRef_72Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations.</param>
        /// <param name="fcsDataService">The fcs data service.</param>
        /// <param name="larsDataService">The lars data service.</param>
        public LearnAimRef_72Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations,
            IFCSDataService fcsDataService,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_72)
        {
            // this check should be in the base class
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));
            It.IsNull(fcsDataService)
                .AsGuard<ArgumentNullException>(nameof(fcsDataService));
            It.IsNull(larsDataService)
                .AsGuard<ArgumentNullException>(nameof(larsDataService));

            _fcsData = fcsDataService;
            _larsData = larsDataService;
            _check = commonOperations;
        }

        /// <summary>
        /// Determines whether [has disqualifying subject sector] [the specified lars delivery and subject area levels].
        /// </summary>
        /// <param name="larsDelivery">The lars delivery.</param>
        /// <param name="subjectAreaLevels">The subject area levels.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying subject sector] [the specified lars delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingSubjectSector(ILARSLearningDelivery larsDelivery, IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> subjectAreaLevels) =>
            It.IsNull(larsDelivery)
            || subjectAreaLevels.Any(x => HasDisqualifyingSubjectSector(x, larsDelivery));

        /// <summary>
        /// Determines whether [has disqualifying subject sector] [the specified subject area level].
        /// the lars delivery cannot be null here
        /// </summary>
        /// <param name="subjectAreaLevel">The subject area level.</param>
        /// <param name="larsDelivery">The lars delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying subject sector] [the specified subject area level]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingSubjectSector(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, ILARSLearningDelivery larsDelivery) =>
            IsUsableSubjectArea(subjectAreaLevel)
            && IsDisqualifyingSubjectAreaLevel(subjectAreaLevel, GetNotionalNVQLevelV2(larsDelivery));

        /// <summary>
        /// Determines whether [is usable subject area] [the specified subject area level].
        /// </summary>
        /// <param name="subjectAreaLevel">The subject area level.</param>
        /// <returns>
        ///   <c>true</c> if [is usable subject area] [the specified subject area level]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsUsableSubjectArea(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel) =>
            It.Has(subjectAreaLevel)
            && It.IsEmpty(subjectAreaLevel.SectorSubjectAreaCode)
            && (It.Has(subjectAreaLevel.MinLevelCode)
                || It.Has(subjectAreaLevel.MaxLevelCode));

        /// <summary>
        /// Gets the notional NVQ level v2.
        /// lars delivery cannot be null here
        /// </summary>
        /// <param name="larsDelivery">The lars delivery.</param>
        /// <returns>a value representing the notional level</returns>
        public double GetNotionalNVQLevelV2(ILARSLearningDelivery larsDelivery) =>
            larsDelivery.NotionalNVQLevelv2.AsNotionalNVQLevelV2();

        /// <summary>
        /// Determines whether [is disqualifying subject area level] [the specified subject area level].
        /// the subject areas level is prechecked for usability
        /// </summary>
        /// <param name="subjectAreaLevel">The subject area level.</param>
        /// <param name="notionalNVQLevel2">The notional NVQ level2.</param>
        /// <returns>
        ///   <c>true</c> if [is disqualifying subject area level] [the specified subject area level]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDisqualifyingSubjectAreaLevel(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, double notionalNVQLevel2) =>
            !IsOutOfScope(notionalNVQLevel2)
            && (HasDisqualifyingMinimumLevel(subjectAreaLevel, notionalNVQLevel2)
                || HasDisqualifyingMaximumLevel(subjectAreaLevel, notionalNVQLevel2));

        /// <summary>
        /// Determines whether [has disqualifying notional level] [the specified notional NVQ level2].
        /// </summary>
        /// <param name="notionalNVQLevel2">The notional NVQ level2.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying notional level] [the specified notional NVQ level2]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOutOfScope(double notionalNVQLevel2) =>
            notionalNVQLevel2 == TypeOfNotionalNVQLevelV2.OutOfScope;

        /// <summary>
        /// Determines whether [has disqualifying minimum level] [the specified subject area level].
        /// the subject areas level is prechecked for usability
        /// </summary>
        /// <param name="subjectAreaLevel">The subject area level.</param>
        /// <param name="notionalNVQLevel2">The notional NVQ level2.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying minimum level] [the specified subject area level]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingMinimumLevel(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, double notionalNVQLevel2) =>
            notionalNVQLevel2 < subjectAreaLevel.MinLevelCode.AsNotionalNVQLevelV2();

        /// <summary>
        /// Determines whether [has disqualifying maximum level] [the specified subject area level].
        /// the subject areas level is prechecked for usability
        /// </summary>
        /// <param name="subjectAreaLevel">The subject area level.</param>
        /// <param name="notionalNVQLevel2">The notional NVQ level2.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying maximum level] [the specified subject area level]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingMaximumLevel(IEsfEligibilityRuleSectorSubjectAreaLevel subjectAreaLevel, double notionalNVQLevel2) =>
            notionalNVQLevel2 > subjectAreaLevel.MaxLevelCode.AsNotionalNVQLevelV2();

        /// <summary>
        /// Gets the lars learning delivery for.
        /// this delivery will never be null
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>a lars learning delivery</returns>
        public ILARSLearningDelivery GetLARSLearningDeliveryFor(ILearningDelivery thisDelivery) =>
            _larsData.GetDeliveryFor(thisDelivery.LearnAimRef);

        /// <summary>
        /// Gets the subject area levels for.
        /// this delivery will never be null
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>a collection of subject area levels</returns>
        public IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> GetSubjectAreaLevelsFor(ILearningDelivery thisDelivery) =>
            _fcsData.GetEligibilityRuleSectorSubjectAreaLevelsFor(thisDelivery.ConRefNumber).AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether the specified this delivery is excluded.
        /// this delivery will never be null
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified this delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery thisDelivery) =>
            It.IsInRange(thisDelivery.LearnAimRef, TypeOfAim.References.ESFLearnerStartandAssessment);

        /// <summary>
        /// Determines whether [is not valid] [the specified this delivery].
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisDelivery) =>
            !IsExcluded(thisDelivery)
                && _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.EuropeanSocialFund)
                && HasDisqualifyingSubjectSector(GetLARSLearningDeliveryFor(thisDelivery), GetSubjectAreaLevelsFor(thisDelivery));

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;

            thisLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        /// returns a collection of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, thisDelivery.ConRefNumber)
            };
        }
    }
}
