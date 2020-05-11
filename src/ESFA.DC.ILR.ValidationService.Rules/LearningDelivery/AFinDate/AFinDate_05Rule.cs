using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate
{
    public class AFinDate_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _appFinRecordQueryService;

        public AFinDate_05Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_05)
        {
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
                var latestTnp3Date =
               _appFinRecordQueryService?.GetLatestAppFinRecord(
                   learningDelivery.AppFinRecords,
                   ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                   ApprenticeshipFinancialRecord.TotalNegotiatedPriceCodes.ResidualTrainingPrice)
                   .AFinDate;

                if (latestTnp3Date.HasValue)
                {
                    var tnp1RecordLaterThanTnp3 = TNP1RecordLaterThanTNP3Record(learningDelivery.AppFinRecords, latestTnp3Date);

                    if (tnp1RecordLaterThanTnp3 != null)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                tnp1RecordLaterThanTnp3.AFinType,
                                tnp1RecordLaterThanTnp3.AFinCode,
                                tnp1RecordLaterThanTnp3.AFinDate));
                    }
                }
            }
        }

        public IAppFinRecord TNP1RecordLaterThanTNP3Record(IEnumerable<IAppFinRecord> appFinRecords, DateTime? latestTnp3Date)
        {
            var tnp1Records =
                _appFinRecordQueryService?.GetAppFinRecordsForTypeAndCode(
                    appFinRecords,
                    ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    ApprenticeshipFinancialRecord.TotalNegotiatedPriceCodes.TotalTrainingPrice);

            return tnp1Records?.Where(af => af.AFinDate > latestTnp3Date).FirstOrDefault();
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string aFinType, int aFinCode, DateTime aFinDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinCode, aFinCode),
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate)
            };
        }
    }
}
