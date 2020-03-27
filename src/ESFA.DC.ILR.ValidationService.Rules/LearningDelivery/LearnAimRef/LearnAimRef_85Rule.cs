using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_85Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// The (rule) name
        /// </summary>
        public const string Name = RuleNameConstants.LearnAimRef_85;

        /// <summary>
        /// the message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// the lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// The common rule (operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRef_85Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="commonChecks">The common checks.</param>
        public LearnAimRef_85Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IProvideRuleCommonOperations commonChecks)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));
            It.IsNull(commonChecks)
                .AsGuard<ArgumentNullException>(nameof(commonChecks));

            _messageHandler = validationErrorHandler;
            _larsData = larsData;
            _check = commonChecks;
        }

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public static DateTime FirstViableDate => new DateTime(2017, 08, 01);

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Determines whether [is qualifying notional NVQ] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying notional NVQ] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDisqualifyingNotionalNVQ(ILARSLearningDelivery delivery) =>
            It.IsInRange(delivery?.NotionalNVQLevelv2, LARSNotionalNVQLevelV2.Level3);

        /// <summary>
        /// Determines whether [has disqualifying notional NVQ] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying notional NVQ] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingNotionalNVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return IsDisqualifyingNotionalNVQ(larsDelivery);
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool PassesRestrictions(ILearningDelivery delivery) =>
            _check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
            && _check.HasQualifyingStart(delivery, FirstViableDate);

        /// <summary>
        /// Determines whether the specified delivery is excluded.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery delivery) =>
            _check.IsRestart(delivery)
            || _check.InApprenticeship(delivery);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsExcluded(delivery)
            && PassesRestrictions(delivery)
            && HasDisqualifyingNotionalNVQ(delivery);

        /// <summary>
        /// Determines whether [has qualifying attainment] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying attainment] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingAttainment(ILearner learner) =>
            It.IsInRange(
                learner.PriorAttainNullable,
                TypeOfPriorAttainment.FullLevel3,
                TypeOfPriorAttainment.Level4Expired20130731,
                TypeOfPriorAttainment.Level5AndAboveExpired20130731,
                TypeOfPriorAttainment.Level4,
                TypeOfPriorAttainment.Level5,
                TypeOfPriorAttainment.Level6,
                TypeOfPriorAttainment.Level7AndAbove,
                TypeOfPriorAttainment.NotKnown,
                TypeOfPriorAttainment.OtherLevelNotKnown);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            if (HasQualifyingAttainment(objectToValidate))
            {
                objectToValidate.LearningDeliveries
                    .SafeWhere(IsNotValid)
                    .ForEach(x => RaiseValidationMessage(objectToValidate, x));
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(ILearner learner, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, learner.PriorAttainNullable));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.FundModel), thisDelivery.FundModel));

            _messageHandler.Handle(RuleName, learner.LearnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
