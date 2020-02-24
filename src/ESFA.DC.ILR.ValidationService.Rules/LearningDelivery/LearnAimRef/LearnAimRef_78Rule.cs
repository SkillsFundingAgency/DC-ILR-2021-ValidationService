using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_78Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// The (rule) name
        /// </summary>
        public const string Name = RuleNameConstants.LearnAimRef_78;

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
        /// the file data (service).
        /// </summary>
        private readonly IFileDataService _fileData;

        /// <summary>
        /// The organisation data (service)
        /// </summary>
        private readonly IOrganisationDataService _organisationData;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRef_78Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="commonChecks">The common checks.</param>
        /// <param name="fileData">The file data (service).</param>
        /// <param name="organisationData">The organisation data (service).</param>
        public LearnAimRef_78Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IProvideRuleCommonOperations commonChecks,
            IFileDataService fileData,
            IOrganisationDataService organisationData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));
            It.IsNull(commonChecks)
                .AsGuard<ArgumentNullException>(nameof(commonChecks));
            It.IsNull(fileData)
                .AsGuard<ArgumentNullException>(nameof(fileData));
            It.IsNull(organisationData)
                .AsGuard<ArgumentNullException>(nameof(organisationData));

            _messageHandler = validationErrorHandler;
            _larsData = larsData;
            _check = commonChecks;
            _fileData = fileData;
            _organisationData = organisationData;
        }

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public static DateTime FirstViableDate => new DateTime(2016, 08, 01);

        /// <summary>
        /// Gets the last viable date.
        /// </summary>
        public static DateTime LastViableDate => new DateTime(2017, 07, 31);

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Determines whether [is specialist designated college].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is specialist designated college]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSpecialistDesignatedCollege()
        {
            var ukprn = _fileData.UKPRN();
            return _organisationData.LegalOrgTypeMatchForUkprn(ukprn, "USDC");
        }

        /// <summary>
        /// Determines whether [is qualifying notional NVQ] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying notional NVQ] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingNotionalNVQ(ILARSLearningDelivery delivery) =>
            It.IsInRange(delivery?.NotionalNVQLevelv2, LARSNotionalNVQLevelV2.Level3);

        /// <summary>
        /// Determines whether [has qualifying notional NVQ] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying notional NVQ] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingNotionalNVQ(ILearningDelivery delivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(delivery.LearnAimRef);

            return IsQualifyingNotionalNVQ(larsDelivery);
        }

        /// <summary>
        /// Determines whether [is qualifying category] [the specified category].
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying category] [the specified category]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingCategory(ILARSLearningCategory category) =>
            It.IsInRange(category.CategoryRef, TypeOfLARSCategory.OnlyForLegalEntitlementAtLevel3);

        /// <summary>
        /// Determines whether [has qualifying category] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying category] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingCategory(ILearningDelivery delivery)
        {
            var categories = _larsData.GetCategoriesFor(delivery.LearnAimRef);

            return categories.SafeAny(IsQualifyingCategory);
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
            && _check.HasQualifyingStart(delivery, FirstViableDate, LastViableDate)
            && HasQualifyingNotionalNVQ(delivery);

        /// <summary>
        /// Determines whether the specified delivery is excluded.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery delivery) =>
            _check.IsRestart(delivery)
            || _check.IsLearnerInCustody(delivery)
            || _check.IsSteelWorkerRedundancyTraining(delivery)
            || _check.InApprenticeship(delivery)
            || IsSpecialistDesignatedCollege();

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
            && !HasQualifyingCategory(delivery);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.FundModel), thisDelivery.FundModel));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
