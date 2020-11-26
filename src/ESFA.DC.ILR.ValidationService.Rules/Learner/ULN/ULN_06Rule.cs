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
    public class ULN_06Rule : AbstractRule, IRule<ILearner>
    {
        public const int MinimumCourseDuration = 5;
        public const int RuleLeniencyPeriod = 60;
        private readonly HashSet<int> _fundModels = new HashSet<int>
        {
            FundModels.Age16To19ExcludingApprenticeships,
            FundModels.AdultSkills,
            FundModels.ApprenticeshipsFrom1May2017,
            FundModels.EuropeanSocialFund,
            FundModels.OtherAdult,
            FundModels.Other16To19
        };

        private readonly IDateTimeQueryService _dateTimeQuery;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public ULN_06Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService academicDataQueryService,
            IDateTimeQueryService dateTimeQueryService,
            IFileDataService fileDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.ULN_06)
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
            && (HasQualifyingModel(theDelivery)
                || (IsNotFundedByESFA(theDelivery) && HasAdvancedLearnerLoan(theDelivery)))
            && (HasQualifyingPlannedDuration(theDelivery)
                || (HasActualEndDate(theDelivery) && HasQualifyingActualDuration(theDelivery)))
            && IsInsideLeniencyPeriod(theDelivery);

        public bool IsExcluded(ILearningDelivery theDelivery) =>
            IsLearnerInCustody(theDelivery)
            || IsLevyFundedApprenticeship(theDelivery);

        public bool IsLearnerInCustody(ILearningDelivery theDelivery) =>
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                theDelivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_OLASS);

        public bool IsLevyFundedApprenticeship(ILearningDelivery theDelivery) =>
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                theDelivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.ACT,
                LearningDeliveryFAMCodeConstants.ACT_ContractEmployer);

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _fundModels.Contains(theDelivery.FundModel);

        public bool IsNotFundedByESFA(ILearningDelivery theDelivery) =>
            theDelivery.FundModel == FundModels.NotFundedByESFA;

        public bool HasAdvancedLearnerLoan(ILearningDelivery theDelivery) =>
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                theDelivery.LearningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.ADL,
                LearningDeliveryFAMCodeConstants.ADL_Code);

        public bool HasQualifyingPlannedDuration(ILearningDelivery theDelivery) =>
            _dateTimeQuery.DaysBetween(theDelivery.LearnStartDate, theDelivery.LearnPlanEndDate) >= MinimumCourseDuration;

        public bool HasActualEndDate(ILearningDelivery theDelivery) =>
            theDelivery.LearnActEndDateNullable.HasValue;

        public bool HasQualifyingActualDuration(ILearningDelivery theDelivery) =>
            _dateTimeQuery.DaysBetween(theDelivery.LearnStartDate, (DateTime)theDelivery.LearnActEndDateNullable) >= MinimumCourseDuration;

        public bool IsInsideLeniencyPeriod(ILearningDelivery theDelivery) =>
            _dateTimeQuery.DaysBetween(theDelivery.LearnStartDate, FilePreparationDate) <= RuleLeniencyPeriod;

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) =>
            new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, ValidationConstants.TemporaryULN),
                BuildErrorMessageParameter(PropertyNameConstants.FilePreparationDate, FilePreparationDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
            };
    }
}