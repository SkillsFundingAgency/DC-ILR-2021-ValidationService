using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate
{
    public class AchDate_11Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2019 = new DateTime(2019, 08, 01);
        private readonly int _aimType = TypeOfAim.ProgrammeAim;
        private readonly int _progType = TypeOfLearningProgramme.ApprenticeshipStandard;

        private readonly HashSet<long> _fundModels = new HashSet<long>
        {
            TypeOfFunding.ApprenticeshipsFrom1May2017
        };

        public AchDate_11Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AchDate_11)
        {
        }

        public AchDate_11Rule()
            : base(null, RuleNameConstants.AchDate_11)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AimType, learningDelivery.FundModel, learningDelivery.ProgTypeNullable,
                                 learningDelivery.LearnActEndDateNullable, learningDelivery.AchDateNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                                                    learningDelivery.AimType,
                                                    learningDelivery.FundModel,
                                                    learningDelivery.ProgTypeNullable,
                                                    learningDelivery.LearnActEndDateNullable,
                                                    learningDelivery.AchDateNullable
                                                    ));
                }
            }
        }

        public bool ConditionMet(int aimType, int fundModel, int? progType, DateTime? learnActEndDate, DateTime? achDate)
        {
            return AimTypeConditionMet(aimType)
                && FundModelConditionMet(fundModel)
                && ProgTypeConditionMet(progType)
                && LearnActEndDateConditionMet(learnActEndDate)
                && AchDateConditionMet(achDate, learnActEndDate);
        }

        public virtual bool AimTypeConditionMet(int aimType)
        {
            return aimType == _aimType;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public virtual bool ProgTypeConditionMet(int? progType)
        {
            return progType == _progType;
        }

        public virtual bool LearnActEndDateConditionMet(DateTime? learnActEndDate)
        {
            return learnActEndDate.HasValue && learnActEndDate >= _firstAugust2019;
        }

        public virtual bool AchDateConditionMet(DateTime? achDate, DateTime? learnActEndDate)
        {
            return achDate.HasValue && achDate < learnActEndDate.Value.AddDays(-7);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, int fundModel, int? progType, DateTime? learnActEndDate, DateTime? achDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate),
            };
        }
    }
}
