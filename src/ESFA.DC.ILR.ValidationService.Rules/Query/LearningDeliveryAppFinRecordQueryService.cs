using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearningDeliveryAppFinRecordQueryService : ILearningDeliveryAppFinRecordQueryService
    {
        public bool HasAnyLearningDeliveryAFinCodesForType(IEnumerable<IAppFinRecord> appFinRecords, string aFinType, IEnumerable<int> aFinCodes)
        {
            if (appFinRecords == null || aFinCodes == null)
            {
                return false;
            }

            return appFinRecords.Any(afr => afr.AFinType.CaseInsensitiveEquals(aFinType) && aFinCodes.Contains(afr.AFinCode));
        }

        public bool HasAnyLearningDeliveryAFinCodeForType(IEnumerable<IAppFinRecord> appFinRecords, string aFinType, int? aFinCode)
        {
            if (appFinRecords == null || aFinCode == null)
            {
                return false;
            }

            return appFinRecords.Any(afr => afr.AFinType.CaseInsensitiveEquals(aFinType) && aFinCode == afr.AFinCode);
        }

        public bool HasAnyLearningDeliveryAFinCodes(IEnumerable<IAppFinRecord> appFinRecords, IEnumerable<int> aFinCodes)
        {
            return appFinRecords != null
                   && aFinCodes != null
                   && appFinRecords.Any(afr => aFinCodes.Contains(afr.AFinCode));
        }

        public IAppFinRecord GetLatestAppFinRecord(IReadOnlyCollection<IAppFinRecord> appFinRecords, string appFinType, int appFinCode)
        {
            if (string.IsNullOrEmpty(appFinType) || appFinCode == 0)
            {
                return null;
            }

            return appFinRecords?.Where(x =>
                    x.AFinCode == appFinCode &&
                    x.AFinType.CaseInsensitiveEquals(appFinType))
                .OrderByDescending(x => x.AFinDate)
                .FirstOrDefault();
        }

        public IEnumerable<IAppFinRecord> GetAppFinRecordsForType(IEnumerable<IAppFinRecord> appFinRecords, string aFinType)
        {
            return appFinRecords?.Where(afr => afr.AFinType.CaseInsensitiveEquals(aFinType)) ?? Enumerable.Empty<IAppFinRecord>();
        }

        public int GetTotalTNPPriceForLatestAppFinRecordsForLearning(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            var total = 0;

            if (learningDeliveries != null)
            {
                foreach (var learningDelivery in learningDeliveries)
                {
                    if (learningDelivery.AppFinRecords != null)
                    {
                        var aFinCode1Value = GetLatestAppFinRecord(
                            learningDelivery.AppFinRecords,
                            ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                            TypeOfTNPAFin.TotalTrainingPrice)?.AFinAmount;

                        var aFinCode2Value = GetLatestAppFinRecord(
                            learningDelivery.AppFinRecords,
                            ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                            TypeOfTNPAFin.TotalAssessmentPrice)?.AFinAmount;

                        total += aFinCode1Value.GetValueOrDefault() + aFinCode2Value.GetValueOrDefault();
                    }
                }
            }

            return total;
        }
    }
}
