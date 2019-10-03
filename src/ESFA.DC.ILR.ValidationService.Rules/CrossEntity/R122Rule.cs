using ESFA.DC.ILR.Model.Interface;
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
    public class R122Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public R122Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService)
            : base(validationErrorHandler, RuleNameConstants.R122)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner theLearner)
        {
            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries?
                .ForAny(ConditionMet, x =>
                {
                    var latestFAMDateTo = GetLatestFAMDateTo(x.LearningDeliveryFAMs);

                    RaiseValidationMessage(learnRefNumber, x, latestFAMDateTo);
                });
        }
        
        public DateTime? GetLatestFAMDateTo(IEnumerable<ILearningDeliveryFAM> fromMonitors) =>
            _learningDeliveryFamQueryService.GetLearningDeliveryFAMsForType(fromMonitors, LearningDeliveryFAMTypeConstants.ACT)
                .OrderByDescending(x => x.LearnDelFAMDateFromNullable)
                .FirstOrDefault()?
                .LearnDelFAMDateToNullable;
        
        public bool ConditionMet(ILearningDelivery theDelivery) =>
            FundModelConditionMet(theDelivery.FundModel)
            && ProgTypeConditionMet(theDelivery.ProgTypeNullable)
            && CompletionStatusConditionMet(theDelivery.CompStatus)
            && HasApprenticeshipContract(theDelivery.LearningDeliveryFAMs)
            && AchievementDateConditionMet(theDelivery.AchDateNullable)
            && FAMDateConditionMet(theDelivery.LearningDeliveryFAMs, theDelivery.LearnActEndDateNullable);

        public bool FundModelConditionMet(int fundModel) => fundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;

        public bool ProgTypeConditionMet(int? progType) => progType == TypeOfLearningProgramme.ApprenticeshipStandard;

        public bool CompletionStatusConditionMet(int compStatus) => compStatus != CompletionState.IsOngoing;

        public bool HasApprenticeshipContract(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams) =>
            _learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.ACT);

        public bool AchievementDateConditionMet(DateTime? achDate) => !achDate.HasValue;

        public bool FAMDateConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, DateTime? actEndDate)
        {
            var learnDelFAMDateTo = GetLatestFAMDateTo(learningDeliveryFAMs);

            return learnDelFAMDateTo.HasValue && actEndDate.HasValue && actEndDate != learnDelFAMDateTo;
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery, DateTime? latestFamDateTo)
        {
            var errorMessageParameters = BuildErrorMessageParametersFor(theDelivery.CompStatus, latestFamDateTo, theDelivery.AchDateNullable, theDelivery.LearnActEndDateNullable);

            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, errorMessageParameters);
        }
            

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParametersFor(int compStatus, DateTime? learnDelFamDateTo, DateTime? achDate, DateTime? learnActEndDate) 
            => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFamDateTo),
            BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate),
            BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate)
        };
    }
}
