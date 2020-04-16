using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_11Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "ULN";

        public const string Name = RuleNameConstants.ULN_11;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly IFileDataService _fileDataService;

        private readonly IAcademicYearDataService _yearService;

        public ULN_11Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IAcademicYearDataService yearService)
        {
            _messageHandler = validationErrorHandler;
            _fileDataService = fileDataService;
            _yearService = yearService;
        }

        public string RuleName => Name;

        public TimeSpan FiveDays => new TimeSpan(5, 0, 0, 0);

        public TimeSpan SixtyDays => new TimeSpan(60, 0, 0, 0);

        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.NullSafeAny(matchCondition);

        public bool CheckLearningDeliveries(ILearner candidate, Func<ILearningDelivery, bool> matchCondition) =>
            candidate.LearningDeliveries.NullSafeAny(matchCondition);

        public bool IsExternallyFunded(ILearningDelivery delivery) =>
           delivery.FundModel == TypeOfFunding.NotFundedByESFA;

        public bool IsHEFCEFunded(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.HigherEducationFundingCouncilEngland.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsHEFCEFunded(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsHEFCEFunded);

        public bool IsShortCourse(ILearningDelivery delivery) =>
            IsPlannedShortCourse(delivery) || IsCompletedShortCourse(delivery);

        public bool IsPlannedShortCourse(ILearningDelivery delivery) =>
            (delivery.LearnPlanEndDate - delivery.LearnStartDate) < FiveDays;

        public bool IsCompletedShortCourse(ILearningDelivery delivery) =>
            delivery.LearnActEndDateNullable.HasValue
                && ((delivery.LearnActEndDateNullable.Value - delivery.LearnStartDate) < FiveDays);

        public bool HasExceedRegistrationPeriod(ILearningDelivery delivery) =>
            (_fileDataService.FilePreparationDate() - delivery.LearnStartDate) > SixtyDays;

        public bool IsInsideGeneralRegistrationThreshold() => _fileDataService.FilePreparationDate() < _yearService.JanuaryFirst();

        public bool IsRegisteredLearner(ILearner candidate) =>
            candidate.ULN != 9999999999;

        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            Monitoring.Delivery.OLASSOffendersInCustody.CaseInsensitiveEquals($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public bool IsExcluded(ILearner candidate)
        {
            return IsInsideGeneralRegistrationThreshold()
                || IsRegisteredLearner(candidate)
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsLearnerInCustody));
        }

        public void Validate(ILearner objectToValidate)
        {
            if (IsExcluded(objectToValidate))
            {
                return;
            }

            ValidateDeliveries(objectToValidate);
        }

        public void ValidateDeliveries(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => IsExternallyFunded(x) && IsHEFCEFunded(x) && !IsShortCourse(x))
                .ForEach(x =>
                {
                    var failedValidation = HasExceedRegistrationPeriod(x);

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}