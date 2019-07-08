using System;
using System.Linq;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate
{
    public class AchDate_09Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _learnStartDate = new DateTime(2015, 7, 31);
        private readonly int _aimType = TypeOfAim.ProgrammeAim;
        private readonly int _progTypeTraineeship = TypeOfLearningProgramme.Traineeship;
        private readonly int _progTypeApprenticeship = TypeOfLearningProgramme.ApprenticeshipStandard;
        private readonly int _fundModelOtherAdult = TypeOfFunding.OtherAdult;
        private readonly int _fundModelApprencticeMay2017 = TypeOfFunding.ApprenticeshipsFrom1May2017;
    
        public AchDate_09Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AchDate_09)
        {
        }

        public AchDate_09Rule()
            : base(null, RuleNameConstants.AchDate_09)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AchDateNullable, learningDelivery.LearnStartDate, learningDelivery.AimType, learningDelivery.ProgTypeNullable, learningDelivery.FundModel))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                                                 learningDelivery.AimType,
                                                 learningDelivery.LearnStartDate,
                                                 learningDelivery.ProgTypeNullable,
                                                 learningDelivery.AchDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? achDate, DateTime learnStartDate, int aimType, int? progType, int fundModel)
        {
            return AchDateConditionMet(achDate)
                   && LearnStartDateConditionMet(learnStartDate)
                   && ApprenticeshipConditionMet(aimType, progType, fundModel);
        }

        public virtual bool AchDateConditionMet(DateTime? achDate)
        {
            return achDate.HasValue;
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate > _learnStartDate;
        }

        public virtual bool ApprenticeshipConditionMet(int aimType, int? progType, int fundModel)
        {
            return !(aimType == _aimType &&
                     (progType == _progTypeTraineeship ||
                      (progType == _progTypeApprenticeship && fundModel == _fundModelOtherAdult) || 
                      (progType == _progTypeApprenticeship && fundModel == _fundModelApprencticeMay2017)
                      ));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, DateTime learnStartDate, int? progType, DateTime? achDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate),
            };
        }
    }
}
