using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate
{
    public class AchDate_11Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly HashSet<long> _fundModels = new HashSet<long>
        {
            FundModels.ApprenticeshipsFrom1May2017
        };

        public AchDate_11Rule(IAcademicYearDataService academicYearDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AchDate_11)
        {
            _academicYearDataService = academicYearDataService;
        }

        public AchDate_11Rule()
            : base(null, RuleNameConstants.AchDate_11)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                            learningDelivery.AimType,
                            learningDelivery.FundModel,
                            learningDelivery.ProgTypeNullable,
                            learningDelivery.LearnActEndDateNullable,
                            learningDelivery.AchDateNullable))
                {
                    HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                             learningDelivery.AimType,
                                             learningDelivery.FundModel,
                                             learningDelivery.ProgTypeNullable,
                                             learningDelivery.LearnActEndDateNullable,
                                             learningDelivery.AchDateNullable));
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
            return aimType == AimTypes.ProgrammeAim;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public virtual bool ProgTypeConditionMet(int? progType)
        {
            return progType == TypeOfLearningProgramme.ApprenticeshipStandard;
        }

        public virtual bool LearnActEndDateConditionMet(DateTime? learnActEndDate)
        {
            return learnActEndDate.HasValue && learnActEndDate >= _academicYearDataService.Start();
        }

        public virtual bool AchDateConditionMet(DateTime? achDate, DateTime? learnActEndDate)
        {
            return achDate.HasValue && achDate < learnActEndDate.Value.AddDays(7);
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
