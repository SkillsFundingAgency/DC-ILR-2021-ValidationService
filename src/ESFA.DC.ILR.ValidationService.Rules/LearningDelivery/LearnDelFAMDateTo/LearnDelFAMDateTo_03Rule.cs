using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateTo
{
    public class LearnDelFAMDateTo_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int>
        {
            FundModels.AdultSkills,
            FundModels.ApprenticeshipsFrom1May2017,
            FundModels.OtherAdult,
            FundModels.NotFundedByESFA
        };

        public LearnDelFAMDateTo_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMDateTo_03)
        {
        }

        public void Validate(ILearner theLearner)
        {
            theLearner.LearningDeliveries
                .ForEach(x => RunChecksFor(x, y => RaiseValidationMessage(theLearner.LearnRefNumber, x, y)));
        }

        public void RunChecksFor(ILearningDelivery theDelivery, Action<ILearningDeliveryFAM> doRaiseMessage)
        {
            if (HasQualifyingFunding(theDelivery))
            {
                theDelivery.LearningDeliveryFAMs.ForAny(x => IsNotValid(theDelivery, x), doRaiseMessage);
            }
        }

        public bool HasQualifyingFunding(ILearningDelivery theDelivery) =>
            _fundModels.Contains(theDelivery.FundModel);

        public bool IsNotValid(ILearningDelivery theDelivery, ILearningDeliveryFAM theMonitor) =>
            IsQualifyingMonitor(theMonitor)
            && HasDisqualifyingDates(theDelivery, theMonitor);

        public bool IsQualifyingMonitor(ILearningDeliveryFAM theMonitor) =>
            !theMonitor.LearnDelFAMType.CaseInsensitiveEquals(Monitoring.Delivery.Types.ApprenticeshipContract);

        public bool HasDisqualifyingDates(ILearningDelivery theDelivery, ILearningDeliveryFAM theMonitor) =>
            theMonitor.LearnDelFAMDateToNullable.HasValue && theMonitor.LearnDelFAMDateToNullable > theDelivery.LearnActEndDateNullable;

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery, ILearningDeliveryFAM theInvalidMonitor)
        {
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery, theInvalidMonitor));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery, ILearningDeliveryFAM theInvalidMonitor)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, theDelivery.LearnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, theInvalidMonitor.LearnDelFAMDateToNullable)
            };
        }
    }
}
