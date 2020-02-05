using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R68Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int?> _apprenticeshipProgTypes = new HashSet<int?>()
        {
            TypeOfLearningProgramme.AdvancedLevelApprenticeship,
            TypeOfLearningProgramme.IntermediateLevelApprenticeship,
            TypeOfLearningProgramme.HigherApprenticeshipLevel4,
            TypeOfLearningProgramme.HigherApprenticeshipLevel5,
            TypeOfLearningProgramme.HigherApprenticeshipLevel6,
            TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus,
            TypeOfLearningProgramme.ApprenticeshipStandard
        };

        public R68Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R68)
        {
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            var standardProgrammeAimsToValidate = GetStandardAppFinRecordsToValidate(learner.LearningDeliveries);
            var frameworkProgrammeAimsToValidate = GetFrameworkAppFinRecordsToValidate(learner.LearningDeliveries);

            if (!standardProgrammeAimsToValidate.Any() && !frameworkProgrammeAimsToValidate.Any())
            {
                return;
            }

            foreach (var standard in standardProgrammeAimsToValidate)           
            {
                var appFinRecords = standard.Value.SelectMany(ld => ld.AppFinRecords).ToList();

                var matchingAppFinRecords = CompareAgainstOtherAppFinRecords(appFinRecords, ConditionMet);

                foreach (var appFinRecord in matchingAppFinRecords)
                {
                    RaiseValidationMessage(learner.LearnRefNumber, null, standard.Key.Value, appFinRecord);
                }
            }

            foreach (var framework in frameworkProgrammeAimsToValidate)
            {
                var appFinRecords = framework.Value.SelectMany(ld => ld.AppFinRecords).ToList();

                var matchingAppFinRecords = CompareAgainstOtherAppFinRecords(appFinRecords, ConditionMet);

                foreach (var appFinRecord in matchingAppFinRecords)
                {
                    RaiseValidationMessage(learner.LearnRefNumber, framework.Key.Value, null, appFinRecord);
                }
            }
        }

        public bool ConditionMet(IAppFinRecord appFinRecord, IAppFinRecord comparisonAppFinRecord)
        {
            return appFinRecord.AFinType == comparisonAppFinRecord.AFinType
                && appFinRecord.AFinCode == comparisonAppFinRecord.AFinCode
                && appFinRecord.AFinDate == comparisonAppFinRecord.AFinDate;
        }


        public bool IsApprenticeshipProgrammeAim(ILearningDelivery learningDelivery)
        {
            return learningDelivery.AimType == TypeOfAim.ProgrammeAim
                   && learningDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
                   && _apprenticeshipProgTypes.Contains(learningDelivery.ProgTypeNullable);
        }

        public void RaiseValidationMessage(string learnRefNumber, int? fworkCode, int? stdCode, IAppFinRecord appFinRecord) =>
            HandleValidationError(learnRefNumber, null, BuildErrorMessageParameters(fworkCode, stdCode, appFinRecord));

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            int? fworkCode,
            int? stdCode,
            IAppFinRecord appFinRecord)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, fworkCode),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, stdCode),
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, appFinRecord.AFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinCode, appFinRecord.AFinCode),
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, appFinRecord.AFinDate)
            };
        }

        private IEnumerable<IAppFinRecord> CompareAgainstOtherAppFinRecords(IEnumerable<IAppFinRecord> appfinRecords, Func<IAppFinRecord, IAppFinRecord, bool> predicate)
        {
            var appFinRecordsList = appfinRecords.ToList();
          
            var collectionSize = appFinRecordsList.Count;

            for (var i = 0; i < collectionSize; i++)
            {
                for (var j = 0; j < collectionSize; j++)
                {
                    if (i != j)
                    {
                        var appFinRecordOne = appFinRecordsList[i];
                        var appFinRecordTwo = appFinRecordsList[j];

                        if (predicate(appFinRecordOne, appFinRecordTwo))
                        {
                            yield return appFinRecordOne;
                            break;
                        }
                    }
                }
            }
        }

        private IDictionary<int?, IEnumerable<ILearningDelivery>> GetStandardAppFinRecordsToValidate(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries.Where(IsApprenticeshipProgrammeAim)
                .GroupBy(ld => ld.StdCodeNullable)
                .Where(x => x.Any(s => s.StdCodeNullable.HasValue))
                .ToDictionary(x => x.Key, v => v.Select(ld => ld));
        }

        private IDictionary<int?, IEnumerable<ILearningDelivery>> GetFrameworkAppFinRecordsToValidate(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries.Where(IsApprenticeshipProgrammeAim)
              .GroupBy(ld => ld.FworkCodeNullable)
              .Where(x => x.Any(f => f.FworkCodeNullable.HasValue))
              .ToDictionary(x => x.Key, v => v.Select(ld => ld));
        }
    }
}
