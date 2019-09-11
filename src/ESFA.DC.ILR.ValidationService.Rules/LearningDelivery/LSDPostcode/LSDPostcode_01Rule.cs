using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode
{
    /// <summary>
    /// the learning start date postcode rule 01
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class LSDPostcode_01Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check(er, rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The postcode data (service)
        /// </summary>
        private readonly IPostcodesDataService _postcodeData;

        /// <summary>
        /// Initializes a new instance of the <see cref="LSDPostcode_01Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOps">The common ops.</param>
        /// <param name="academicYearDataService">The academic year data service.</param>
        /// <param name="postcodesDataService">The postcodes data service.</param>
        public LSDPostcode_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOps,
            IPostcodesDataService postcodesDataService)
            : base(validationErrorHandler, RuleNameConstants.LSDPostcode_01)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));
            It.IsNull(postcodesDataService)
                .AsGuard<ArgumentNullException>(nameof(postcodesDataService));

            _check = commonOps;
            _postcodeData = postcodesDataService;
        }

        /// <summary>
        /// Gets the first august 2019.
        /// </summary>
        public static DateTime FirstAugust2019 => new DateTime(2019, 08, 01);

        /// <summary>
        /// Validates the specified learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery theDelivery) =>
            !IsExcluded(theDelivery)
            && HasQualifyingModel(theDelivery)
            && HasQualifyingStart(theDelivery)
            && (IsEmptyPostcode(theDelivery)
                || (!IsTemporaryPostcode(theDelivery) && !IsRegisteredPostcode(theDelivery)));

        /// <summary>
        /// Determines whether the specified the delivery is excluded.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery theDelivery) =>
            HasProgrammeDefined(theDelivery);

        /// <summary>
        /// Determines whether [has programme defined] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has programme defined] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasProgrammeDefined(ILearningDelivery theDelivery) =>
            It.Has(theDelivery.ProgTypeNullable);

        /// <summary>
        /// Determines whether [has qualifying model] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.AdultSkills, TypeOfFunding.CommunityLearning);

        /// <summary>
        /// Determines whether [has qualifying start] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery theDelivery) =>
            _check.HasQualifyingStart(theDelivery, FirstAugust2019);

        /// <summary>
        /// Determines whether [is empty postcode] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is empty postcode] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmptyPostcode(ILearningDelivery theDelivery) =>
            It.IsEmpty(theDelivery.LSDPostcode);

        /// <summary>
        /// Determines whether [is temporary postcode] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not temporary postcode] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTemporaryPostcode(ILearningDelivery theDelivery) =>
            ValidationConstants.TemporaryPostCode.ComparesWith(theDelivery.LSDPostcode);

        /// <summary>
        /// Determines whether [is registered postcode] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is registered postcode] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRegisteredPostcode(ILearningDelivery theDelivery) =>
            _postcodeData.PostcodeExists(theDelivery.LSDPostcode);

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
        /// <returns>a collection of message parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) =>
            new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, theDelivery.LSDPostcode)
            };
    }
}
