using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R20Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int?> _frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };

        private readonly ILARSDataService _lARSDataService;
        private readonly IDerivedData_07Rule _dd07;

        public R20Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService lARSDataService,
            IDerivedData_07Rule dd07)
            : base(validationErrorHandler, RuleNameConstants.R20)
        {
            _lARSDataService = lARSDataService;
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            var learningDeliveries = objectToValidate?.LearningDeliveries?.Where(
                ld => DD07ConditionMet(ld.ProgTypeNullable)
                      && ApprenticeshipStandardsConditionMet(ld.ProgTypeNullable)
                      && ComponentAimTypeConditionMet(ld.AimType)
                      && LARSConditionMet(ld.LearnAimRef, ld.ProgTypeNullable, ld.FworkCodeNullable, ld.PwayCodeNullable, ld.LearnStartDate));

            if (learningDeliveries == null || learningDeliveries.Count() < 2)
            {
                return;
            }

            DateTime? learnActEndDatePrevious = null;
            bool firstRecord = true;

            foreach (var learningDelivery in learningDeliveries.OrderBy(d => d.LearnStartDate))
            {
                if (firstRecord)
                {
                    learnActEndDatePrevious = learningDelivery.LearnActEndDateNullable;
                }

                if (ConditionMet(learningDelivery.LearnStartDate, learnActEndDatePrevious, firstRecord))
                {
                    if (!firstRecord)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                learningDelivery.AimType,
                                learningDelivery.LearnStartDate,
                                learningDelivery.LearnActEndDateNullable));
                    }

                    learnActEndDatePrevious = learningDelivery.LearnActEndDateNullable;
                    firstRecord = false;
                }
            }
        }

        public bool ConditionMet(
            DateTime learnStartDate,
            DateTime? learnActEndDatePrevious,
            bool firstRecord)
        {
            return firstRecord || LearnStartDateConditionMet(learnStartDate, learnActEndDatePrevious, firstRecord);
        }

        public bool LearnStartDateConditionMet(
            DateTime learnStartDate,
            DateTime? learnActEndDatePrevious,
            bool firstRecord)
            => firstRecord ? learnActEndDatePrevious == null
                : (!learnActEndDatePrevious.HasValue || learnStartDate < learnActEndDatePrevious);

        public bool DD07ConditionMet(int? progType) => _dd07.IsApprenticeship(progType);

        public bool ComponentAimTypeConditionMet(int aimType) => aimType == TypeOfAim.ComponentAimInAProgramme;

        public bool LARSConditionMet(string learnAimRef, int? progType, int? fworkCode, int? pwayCode, DateTime startDate) => _lARSDataService.FrameworkCodeExistsForFrameworkAimsAndFrameworkComponentTypes(learnAimRef, progType, fworkCode, pwayCode, _frameWorkComponentTypes, startDate);

        public bool ApprenticeshipStandardsConditionMet(int? progTypeNullable) => progTypeNullable.HasValue
                && progTypeNullable != TypeOfLearningProgramme.ApprenticeshipStandard;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, DateTime learnStartDate, DateTime? learnActEndDateNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDateNullable)
            };
        }
    }
}
