using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_10Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IDateTimeQueryService _dateTimeQuery;

        private readonly IProvideRuleCommonOperations _check;

        public ULN_10Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService academicDataQueryService,
            IDateTimeQueryService dateTimeQueryService,
            IFileDataService fileDataService,
            IProvideRuleCommonOperations commonOps)
            : base(validationErrorHandler, RuleNameConstants.ULN_10)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(academicDataQueryService)
                .AsGuard<ArgumentNullException>(nameof(academicDataQueryService));
            It.IsNull(dateTimeQueryService)
                .AsGuard<ArgumentNullException>(nameof(dateTimeQueryService));
            It.IsNull(fileDataService)
                .AsGuard<ArgumentNullException>(nameof(fileDataService));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));

            FilePreparationDate = fileDataService.FilePreparationDate();
            FirstJanuary = academicDataQueryService.JanuaryFirst();

            _dateTimeQuery = dateTimeQueryService;
            _check = commonOps;
        }

        public const int MinimumCourseDuration = 5;  

        public const int RuleLeniencyPeriod = 60;  

        public DateTime FilePreparationDate { get; }

        public DateTime FirstJanuary { get; }

        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            if (IsOutsideQualifyingPeriod() || IsValidULN(theLearner))
            {
                return;
            }

            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public bool IsOutsideQualifyingPeriod() =>
            FirstJanuary > FilePreparationDate;

        public bool IsValidULN(ILearner theLearner) =>
            theLearner.ULN != ValidationConstants.TemporaryULN;

        public bool IsNotValid(ILearningDelivery theDelivery) =>
            !IsExcluded(theDelivery)
            && HasQualifyingModel(theDelivery)
            && HasQualifyingMonitor(theDelivery)
            && (HasQualifyingPlannedDuration(theDelivery)
                || (HasActualEndDate(theDelivery) && HasQualifyingActualDuration(theDelivery)))
            && IsOutsideLeniencyPeriod(theDelivery);

        public bool IsExcluded(ILearningDelivery theDelivery) =>
            IsLearnerInCustody(theDelivery);

        public bool IsLearnerInCustody(ILearningDelivery theDelivery) =>
            _check.IsLearnerInCustody(theDelivery);

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.NotFundedByESFA);

        public bool IsHigherEducationFundingCouncilEngland(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}", Monitoring.Delivery.HigherEducationFundingCouncilEngland);

        public bool HasQualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsHigherEducationFundingCouncilEngland);

        public bool HasQualifyingPlannedDuration(ILearningDelivery theDelivery) =>
            _dateTimeQuery.DaysBetween(theDelivery.LearnStartDate, theDelivery.LearnPlanEndDate) >= MinimumCourseDuration;

        public bool HasActualEndDate(ILearningDelivery theDelivery) =>
            It.Has(theDelivery.LearnActEndDateNullable);

        public bool HasQualifyingActualDuration(ILearningDelivery theDelivery) =>
            _dateTimeQuery.DaysBetween(theDelivery.LearnStartDate, (DateTime)theDelivery.LearnActEndDateNullable) >= MinimumCourseDuration;

        public bool IsOutsideLeniencyPeriod(ILearningDelivery theDelivery) =>
            _dateTimeQuery.DaysBetween(theDelivery.LearnStartDate, FilePreparationDate) > RuleLeniencyPeriod;

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) =>
            new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, ValidationConstants.TemporaryULN),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, theDelivery.LearnPlanEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")
            };
    }
}