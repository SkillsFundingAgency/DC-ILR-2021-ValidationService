using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_10Rule : AbstractRule, IRule<ILearner>
    {
        public const int MinimumCourseDuration = 5;
        public const int RuleLeniencyPeriod = 60;

        private readonly IDateTimeQueryService _dateTimeQuery;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public ULN_10Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService academicDataQueryService,
            IDateTimeQueryService dateTimeQueryService,
            IFileDataService fileDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.ULN_10)
        {
            FilePreparationDate = fileDataService.FilePreparationDate();
            FirstJanuary = academicDataQueryService.JanuaryFirst();

            _dateTimeQuery = dateTimeQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public DateTime FilePreparationDate { get; }

        public DateTime FirstJanuary { get; }

        public void Validate(ILearner theLearner)
        {
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
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                theDelivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_OLASS);

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            theDelivery.FundModel == FundModels.NotFundedByESFA;

        public bool HasQualifyingMonitor(ILearningDelivery theDelivery) =>
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                theDelivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.SOF,
                LearningDeliveryFAMCodeConstants.SOF_HEFCE);

        public bool HasQualifyingPlannedDuration(ILearningDelivery theDelivery) =>
            _dateTimeQuery.DaysBetween(theDelivery.LearnStartDate, theDelivery.LearnPlanEndDate) >= MinimumCourseDuration;

        public bool HasActualEndDate(ILearningDelivery theDelivery) =>
            theDelivery.LearnActEndDateNullable.HasValue;

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