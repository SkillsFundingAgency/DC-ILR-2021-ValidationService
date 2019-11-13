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
    public class R99Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R99Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.R99)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner theLearner)
        {
            if (theLearner.LearningDeliveries == null)
            {
                return;
            }

            var learningDeliveries = GetProgrammeAims(theLearner.LearningDeliveries).ToList();

            if (HasMoreThanOneProgrammeAim(learningDeliveries))
            {
                var learnRefNumber = theLearner.LearnRefNumber;

                var errorLearningDeliveries = CompareAgainstOtherDeliveries(learningDeliveries, ConditionMet);
                
                foreach (var learningDelivery in errorLearningDeliveries)
                {
                    RaiseValidationMessage(learnRefNumber, learningDelivery);
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, ILearningDelivery comparisonLearningDelivery)
        {
            return OverlappingAimEndDatesConditionMet(learningDelivery, comparisonLearningDelivery)
                   || ApprenticeshipStandardConditionMet(learningDelivery, comparisonLearningDelivery);
        }

        public IEnumerable<ILearningDelivery> GetProgrammeAims(IEnumerable<ILearningDelivery> learningDeliveries) =>
            learningDeliveries.Where(ld => ld.AimType == TypeOfAim.ProgrammeAim);

        public bool HasMoreThanOneProgrammeAim(IEnumerable<ILearningDelivery> candidates) =>
            candidates.Count() > 1;

        public bool OverlappingAimEndDatesConditionMet(ILearningDelivery theDelivery, ILearningDelivery comparisonLearningDelivery) =>
            It.IsBetween(theDelivery.LearnStartDate, comparisonLearningDelivery.LearnStartDate, comparisonLearningDelivery.LearnActEndDateNullable ?? DateTime.MaxValue);

        public bool ApprenticeshipStandardConditionMet(ILearningDelivery learningDelivery, ILearningDelivery comparisonLearningDelivery)
        {
            return (!Excluded(learningDelivery, comparisonLearningDelivery))
                && comparisonLearningDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
                && comparisonLearningDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
                && It.IsBetween(learningDelivery.LearnStartDate, comparisonLearningDelivery.LearnStartDate, comparisonLearningDelivery.AchDateNullable ?? DateTime.MaxValue);
        }

        public bool Excluded(ILearningDelivery learningDelivery, ILearningDelivery comparisonLearningDelivery) =>
                comparisonLearningDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
            &&  comparisonLearningDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
            &&  (comparisonLearningDelivery.CompStatus == 3 
            || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, Monitoring.Delivery.Types.Restart));

        public IEnumerable<ILearningDelivery> CompareAgainstOtherDeliveries(IEnumerable<ILearningDelivery> learningDeliveries, Func<ILearningDelivery, ILearningDelivery, bool> predicate)
        {
            var learningDeliveriesList = learningDeliveries.ToList();

            var collectionSize = learningDeliveriesList.Count;

            for (var i = 0; i < collectionSize; i++)
            {
                for (var j = 0; j < collectionSize; j++)
                {
                    if (i != j)
                    {
                        var learningDeliveryOne = learningDeliveriesList[i];
                        var learningDeliveryTwo = learningDeliveriesList[j];

                        if (predicate(learningDeliveryOne, learningDeliveryTwo))
                        {
                            yield return learningDeliveryOne;
                            break;
                        }
                    }
                }
            }
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.AimType, theDelivery.AimType),
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
            BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, theDelivery.LearnActEndDateNullable),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.ProgType, theDelivery.ProgTypeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.AchDate, theDelivery.AchDateNullable)
        };
    }
}
