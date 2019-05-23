using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R73Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;
        private readonly ILARSDataService _larsDataService;
        private readonly IDerivedData_17Rule _dd17;
        private readonly HashSet<int> _trainingAndAssementAFinCodes = new HashSet<int>() { TypeOfPMRAFin.TrainingPayment, TypeOfPMRAFin.AssessmentPayment };

        public R73Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService,
            ILARSDataService larsDataService,
            IDerivedData_17Rule dd17)
            : base(validationErrorHandler, RuleNameConstants.R73)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
            _larsDataService = larsDataService;
            _dd17 = dd17;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var filteredLearningDeliveries = objectToValidate.LearningDeliveries.Where(
                    x => x.StdCodeNullable.HasValue &&
                    x.AimType == TypeOfAim.ProgrammeAim &&
                    x.ProgTypeNullable == TypeOfFunding.Age16To19ExcludingApprenticeships &&
                    x.FundModel == TypeOfFunding.OtherAdult).ToList();

            if (!filteredLearningDeliveries.Any())
            {
                return;
            }

            var standardPMRTotalValues = GetPMRTotalsDictionary(filteredLearningDeliveries);

            foreach (var standardCode in standardPMRTotalValues.Keys)
            {
                if (ConditionMet(standardCode, standardPMRTotalValues[standardCode], filteredLearningDeliveries))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        null,
                        BuildErrorMessageParameters(objectToValidate.LearnRefNumber, standardCode));
                }
            }
        }

        public Dictionary<int, int> GetPMRTotalsDictionary(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            var dict = new Dictionary<int, int>();

            foreach (var learningDelivery in learningDeliveries)
            {
                var totalAFin1And2 = learningDelivery.AppFinRecords?.Where(
                                fin => fin.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.PaymentRecord) &&
                               _trainingAndAssementAFinCodes.Contains(fin.AFinCode)).Sum(s => s.AFinAmount);

                var totalAFin3 = learningDelivery.AppFinRecords?.Where(
                    fin => fin.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.PaymentRecord) &&
                           fin.AFinCode == TypeOfPMRAFin.EmployerPaymentReimbursedByProvider).Sum(s => s.AFinAmount);

                if (dict.ContainsKey(learningDelivery.StdCodeNullable.Value))
                {
                    dict[learningDelivery.StdCodeNullable.Value] += totalAFin1And2.GetValueOrDefault() - totalAFin3.GetValueOrDefault();
                }
                else
                {
                    dict[learningDelivery.StdCodeNullable.Value] = totalAFin1And2.GetValueOrDefault() - totalAFin3.GetValueOrDefault();
                }
            }

            return dict;
        }

        public bool ConditionMet(int standardCode, int? totalPMRValue, IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            if (!totalPMRValue.HasValue)
            {
                return false;
            }

            var standardLearningDeliveries = learningDeliveries.Where(x => x.StdCodeNullable == standardCode).ToList();

            if (_dd17.IsTotalNegotiatedPriceMoreThanCapForStandard(standardLearningDeliveries, standardCode))
            {
                var standardTNPTotal = _learningDeliveryAppFinRecordQueryService.GetTotalTNPPriceForLatestAppFinRecordsForLearning(standardLearningDeliveries);
                var applicableDateForCap = GetApplicableDateForCapChecking(standardLearningDeliveries);

                var fundingCap = _larsDataService.GetStandardFundingForCodeOnDate(standardCode, applicableDateForCap)?.CoreGovContributionCap;
                return fundingCap.HasValue && totalPMRValue > (standardTNPTotal - fundingCap);
            }

            return false;
        }

        public DateTime GetApplicableDateForCapChecking(List<ILearningDelivery> learningDeliveries)
        {
            var earliestStartDate = learningDeliveries.Select(x =>
                    x.OrigLearnStartDateNullable.HasValue && x.OrigLearnStartDateNullable.Value < x.LearnStartDate
                        ? x.OrigLearnStartDateNullable.Value
                        : x.LearnStartDate)
                .OrderBy(x => x)
                .FirstOrDefault();

            return earliestStartDate;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnRefNumber, int standardCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnRefNumber, learnRefNumber),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, TypeOfAim.ProgrammeAim),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, TypeOfFunding.Age16To19ExcludingApprenticeships),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, standardCode),
            };
        }
    }
}
