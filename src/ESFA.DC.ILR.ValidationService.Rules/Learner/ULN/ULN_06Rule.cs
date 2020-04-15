using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_06Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IDateTimeQueryService _dateTimeQuery;

        private readonly IProvideRuleCommonOperations _check;

        public ULN_06Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService academicDataQueryService,
            IDateTimeQueryService dateTimeQueryService,
            IFileDataService fileDataService,
            IProvideRuleCommonOperations commonOps)
            : base(validationErrorHandler, RuleNameConstants.ULN_06)
        {
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
            _check.IsLearnerInCustody(theDelivery);

        public bool IsLevyFundedApprenticeship(ILearningDeliveryFAM theMonitor) =>
            Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer.CaseInsensitiveEquals($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}");

        public bool IsLevyFundedApprenticeship(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsLevyFundedApprenticeship);

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(
                theDelivery,
                TypeOfFunding.Age16To19ExcludingApprenticeships,
                TypeOfFunding.AdultSkills,
                TypeOfFunding.ApprenticeshipsFrom1May2017,
                TypeOfFunding.EuropeanSocialFund,
                TypeOfFunding.OtherAdult,
                TypeOfFunding.Other16To19);

        public bool IsNotFundedByESFA(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(
                theDelivery,
                TypeOfFunding.NotFundedByESFA);

        public bool IsFinancedByAdvancedLearnerLoans(ILearningDeliveryFAM theMonitor) =>
            Monitoring.Delivery.FinancedByAdvancedLearnerLoans.CaseInsensitiveEquals($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}");

        public bool HasAdvancedLearnerLoan(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsFinancedByAdvancedLearnerLoans);

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