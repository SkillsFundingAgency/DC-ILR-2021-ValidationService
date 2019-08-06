using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R99Rule : AbstractRule, IRule<ILearner>
    {
        public R99Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R99)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var mainLearningDeliveries = objectToValidate.LearningDeliveries.Where(ld => ld.AimType == TypeOfAim.ProgrammeAim).ToList();

            foreach (var mainLearningDelivery in mainLearningDeliveries)
            {
                if (mainLearningDeliveries.Any(ld => ld.AimSeqNumber != mainLearningDelivery.AimSeqNumber &&
                    (
                        (!ld.LearnActEndDateNullable.HasValue && !mainLearningDelivery.LearnActEndDateNullable.HasValue) ||
                        (mainLearningDelivery.LearnStartDate >= ld.LearnStartDate
                                    && ld.LearnActEndDateNullable.HasValue
                                    && mainLearningDelivery.LearnStartDate <= ld.LearnActEndDateNullable) ||
                         ExceptionConditionMet(mainLearningDeliveries, mainLearningDelivery)
                     )))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, mainLearningDelivery.AimSeqNumber, BuildErrorMessageParameters(mainLearningDelivery));
                }
            }
        }

        private bool ExceptionConditionMet(List<ILearningDelivery> mainLearningDeliveries, ILearningDelivery mainLearningDelivery)
        {
            var deliveryStartDate = mainLearningDelivery.LearnStartDate;
            var exceptionMet = mainLearningDeliveries.Any(x => x.AimSeqNumber != mainLearningDelivery.AimSeqNumber
                                              && deliveryStartDate >= x.LearnStartDate
                                              && x.AchDateNullable.HasValue && deliveryStartDate <= x.AchDateNullable.Value)
                                              && FundModelConditionMet(mainLearningDelivery.FundModel)
                                              && ProgTypeConditionMet(mainLearningDelivery.ProgTypeNullable);
            return exceptionMet;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == TypeOfLearningProgramme.ApprenticeshipStandard;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, learningDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, learningDelivery.AchDateNullable)
            };
        }
    }
}
