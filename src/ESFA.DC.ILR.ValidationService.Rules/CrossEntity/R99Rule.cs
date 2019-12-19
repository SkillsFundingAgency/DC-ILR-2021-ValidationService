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

        public bool ConditionMet(ILearningDelivery learningDelivery, ILearningDelivery comparisonLearningDelivery, IEnumerable<ILearningDelivery> standardProgAims)
        {
            return
                OverlappingAimEndDatesConditionMet(learningDelivery, comparisonLearningDelivery)
                || MultipleUnknownLearnActEndDateConditionMet(learningDelivery, comparisonLearningDelivery)
                || ProgAimLearnActEndDateConditionMet(learningDelivery, comparisonLearningDelivery, standardProgAims)
                || ApprenticeshipStandardAchDateConditionMet(learningDelivery, comparisonLearningDelivery)
                || AchDateConditionMet(learningDelivery, comparisonLearningDelivery);
        }

        public IEnumerable<ILearningDelivery> GetProgrammeAims(IEnumerable<ILearningDelivery> learningDeliveries) =>
            learningDeliveries.Where(ld => ld.AimType == TypeOfAim.ProgrammeAim && !Excluded(ld));

        public bool HasMoreThanOneProgrammeAim(IEnumerable<ILearningDelivery> candidates) =>
            candidates.Count() > 1;

        public bool OverlappingAimEndDatesConditionMet(ILearningDelivery theDelivery, ILearningDelivery comparisonLearningDelivery) =>
            It.IsBetween(theDelivery.LearnStartDate, comparisonLearningDelivery.LearnStartDate, comparisonLearningDelivery.LearnActEndDateNullable ?? DateTime.MaxValue);

        public bool MultipleUnknownLearnActEndDateConditionMet(ILearningDelivery theDelivery, ILearningDelivery comparisonLearningDelivery) =>
            !theDelivery.LearnActEndDateNullable.HasValue && !comparisonLearningDelivery.LearnActEndDateNullable.HasValue;
        
        public bool ProgAimLearnActEndDateConditionMet(ILearningDelivery theDelivery, ILearningDelivery comparisonLearningDelivery, IEnumerable<ILearningDelivery> standardProgAims)
        {
            if (standardProgAims == null || !standardProgAims.Any())
            {
                return false;
            }

            return theDelivery.LearnActEndDateNullable.HasValue
                && standardProgAims.Any(standardAim => 
                comparisonLearningDelivery.AimSeqNumber != standardAim.AimSeqNumber
                && It.IsBetween(comparisonLearningDelivery.LearnStartDate, standardAim.LearnStartDate, standardAim.AchDateNullable ?? DateTime.MaxValue));
        }

        public bool ApprenticeshipStandardAchDateConditionMet(ILearningDelivery theDelivery, ILearningDelivery comparisonLearningDelivery)
        {
            return theDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
                && theDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
                && theDelivery.AchDateNullable.HasValue
                && It.IsBetween(comparisonLearningDelivery.LearnStartDate, theDelivery.LearnStartDate, theDelivery.LearnActEndDateNullable ?? DateTime.MaxValue);
        }

        public bool AchDateConditionMet(ILearningDelivery theDelivery, ILearningDelivery comparisonLearningDelivery)
        {
            return theDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
                && theDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
                && !theDelivery.AchDateNullable.HasValue
                && comparisonLearningDelivery.LearnStartDate >= theDelivery.LearnStartDate;
        }

        public bool Excluded(ILearningDelivery learningDelivery) =>
                learningDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
            && learningDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
            && !learningDelivery.AchDateNullable.HasValue
            && (learningDelivery.WithdrawReasonNullable.HasValue
            || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, Monitoring.Delivery.Types.Restart));

        public IEnumerable<ILearningDelivery> CompareAgainstOtherDeliveries(IEnumerable<ILearningDelivery> learningDeliveries, Func<ILearningDelivery, ILearningDelivery, IEnumerable<ILearningDelivery>, bool> predicate)
        {
            var learningDeliveriesList = learningDeliveries.ToList();
            var standardProgrammeAims = learningDeliveries.Where(ld =>
               ld.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
               && ld.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard).ToList();

            var collectionSize = learningDeliveriesList.Count;

            for (var i = 0; i < collectionSize; i++)
            {
                for (var j = 0; j < collectionSize; j++)
                {
                    if (i != j)
                    {
                        var learningDeliveryOne = learningDeliveriesList[i];
                        var learningDeliveryTwo = learningDeliveriesList[j];

                        if (predicate(learningDeliveryOne, learningDeliveryTwo, standardProgrammeAims))
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
