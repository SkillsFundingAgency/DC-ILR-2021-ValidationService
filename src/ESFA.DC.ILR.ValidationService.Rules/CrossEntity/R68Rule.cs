using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Structs;

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

            var apprenticeshipLearningDeliveries = learner.LearningDeliveries.Where(IsApprenticeshipProgrammeAim).ToList();

            if (!apprenticeshipLearningDeliveries.Any())
            {
                return;
            }

            var standardProgrammeAimsToValidate = GetGroupedAppFinRecordsToValidate(apprenticeshipLearningDeliveries, ld => ld.StdCodeNullable);
            var frameworkProgrammeAimsToValidate = GetGroupedAppFinRecordsToValidate(apprenticeshipLearningDeliveries, ld => ld.FworkCodeNullable);

            ValidateAims(learner.LearnRefNumber, standardProgrammeAimsToValidate);
            ValidateAims(learner.LearnRefNumber, frameworkProgrammeAimsToValidate);
        }

        public void ValidateAims(string learnRefNumber, IDictionary<int?, IEnumerable<R68AppFinRecord>> aims)
        {
            if (aims != null)
            {
                foreach (var key in aims)
                {
                    var matchingAppFinRecords = CompareAgainstOtherAppFinRecords(key.Value, ConditionMet).Distinct();

                    foreach (var appFinRecord in matchingAppFinRecords)
                    {
                        RaiseValidationMessage(learnRefNumber, appFinRecord.AimSeqNumber, appFinRecord);
                    }
                }
            }
        }

        public bool ConditionMet(R68AppFinRecord appFinRecord, R68AppFinRecord comparisonAppFinRecord)
        {
            return appFinRecord.AFinType == comparisonAppFinRecord.AFinType
                && appFinRecord.AFinCode == comparisonAppFinRecord.AFinCode
                && appFinRecord.AFinDate == comparisonAppFinRecord.AFinDate;
        }

        public bool IsApprenticeshipProgrammeAim(ILearningDelivery learningDelivery)
        {
            return learningDelivery.AimType == TypeOfAim.ProgrammeAim
                   && learningDelivery.FundModel == FundModels.ApprenticeshipsFrom1May2017
                   && _apprenticeshipProgTypes.Contains(learningDelivery.ProgTypeNullable);
        }

        public void RaiseValidationMessage(string learnRefNumber, int aimSeqNumber, R68AppFinRecord appFinRecord) =>
         HandleValidationError(learnRefNumber, aimSeqNumber, BuildErrorMessageParameters(appFinRecord.FworkCode, appFinRecord.StdCode, appFinRecord.AFinType, appFinRecord.AFinCode, appFinRecord.AFinDate));

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            int? fworkCode,
            int? stdCode,
            string aFinType,
            int aFinCode,
            DateTime aFinDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, fworkCode),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, stdCode),
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinCode, aFinCode),
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate)
            };
        }

        private IEnumerable<R68AppFinRecord> CompareAgainstOtherAppFinRecords(IEnumerable<R68AppFinRecord> appfinRecords, Func<R68AppFinRecord, R68AppFinRecord, bool> predicate)
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

        private IDictionary<int?, IEnumerable<R68AppFinRecord>> GetGroupedAppFinRecordsToValidate(IEnumerable<ILearningDelivery> apprenticeshipLearningDeliveries, Func<ILearningDelivery, int?> groupBy)
        {
            return apprenticeshipLearningDeliveries
              .GroupBy(groupBy)
              .Where(x => x.Any(f => groupBy(f).HasValue))
              .ToDictionary(
                x => x.Key,
                v => v.Where(af => af.AppFinRecords != null)
                .SelectMany(ld => ld.AppFinRecords?
                    .Select(
                        af => new R68AppFinRecord(ld.AimSeqNumber, ld.FworkCodeNullable, ld.StdCodeNullable, af.AFinType, af.AFinCode, af.AFinDate))));
        }
    }
}
