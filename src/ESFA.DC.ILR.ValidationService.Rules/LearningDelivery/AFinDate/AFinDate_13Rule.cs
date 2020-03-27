using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate
{
    public class AFinDate_13Rule : AbstractRule, IRule<ILearner>
    {
        private const string _aFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice;
        private readonly IDerivedData_07Rule _dd07;
        private readonly ILearningDeliveryAppFinRecordQueryService _appFinRecordQueryService;

        public AFinDate_13Rule(
            IDerivedData_07Rule dd07,
            ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_13)
        {
            _dd07 = dd07;
            _appFinRecordQueryService = appFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                var tnpRecords = _appFinRecordQueryService.GetAppFinRecordsForType(learningDelivery.AppFinRecords, _aFinType);

                if (tnpRecords.Any())
                {
                    foreach (var tnpRecord in tnpRecords)
                    {
                        if (LearnerAchievedCondition(learningDelivery.ProgTypeNullable, learningDelivery.AchDateNullable, tnpRecord.AFinDate))
                        {
                            HandleError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, learningDelivery.AchDateNullable, tnpRecord.AFinDate);
                            continue;
                        }

                        if (LearnerWithdrawnCondition(
                            learningDelivery.ProgTypeNullable,
                            learningDelivery.AchDateNullable,
                            learningDelivery.WithdrawReasonNullable,
                            learningDelivery.LearnActEndDateNullable,
                            tnpRecord.AFinDate))
                        {
                            HandleError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, learningDelivery.AchDateNullable, tnpRecord.AFinDate);
                        }
                    }
                }
            }
        }

        public bool LearnerAchievedCondition(int? progType, DateTime? achDate, DateTime aFinDate) =>
               ApprenticeshipStandardProgrammeCondition(progType)
            && AchDateReturnedCondition(achDate)
            && TNPRecordAfterDate(achDate, aFinDate);

        public bool LearnerWithdrawnCondition(int? progType, DateTime? achDate, int? withdrawReason, DateTime? learnActEndDate, DateTime aFinDate) =>
               ApprenticeshipStandardProgrammeCondition(progType)
            && !AchDateReturnedCondition(achDate)
            && WithdrawReasonReturnedCondition(withdrawReason)
            && TNPRecordAfterDate(learnActEndDate, aFinDate);

        public bool ApprenticeshipStandardProgrammeCondition(int? progType) => _dd07.IsApprenticeship(progType) && progType == TypeOfLearningProgramme.ApprenticeshipStandard;

        public bool AchDateReturnedCondition(DateTime? achDate) => achDate.HasValue;

        public bool WithdrawReasonReturnedCondition(int? withdrawReason) => withdrawReason.HasValue;

        public bool TNPRecordAfterDate(DateTime? date, DateTime aFinDate)
        {
             return date.HasValue ? aFinDate > date.Value : false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? achDate, string aFinType, DateTime aFinDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate),
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate)
            };
        }

        private void HandleError(string learnRefNumber, int aimSeqNumber, DateTime? dateValue, DateTime aFinDate)
        {
            HandleValidationError(learnRefNumber, aimSeqNumber, BuildErrorMessageParameters(dateValue, _aFinType, aFinDate));
        }
    }
}
