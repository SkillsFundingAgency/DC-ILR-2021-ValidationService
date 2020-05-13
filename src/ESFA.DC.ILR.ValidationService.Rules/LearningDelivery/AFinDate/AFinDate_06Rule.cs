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
    public class AFinDate_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _appFinRecordQueryService;

        public AFinDate_06Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_06)
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
                var latestTnp4Date =
                    _appFinRecordQueryService?.GetLatestAppFinRecord(
                        learningDelivery.AppFinRecords,
                        ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                        ApprenticeshipFinancialRecord.TotalNegotiatedPriceCodes.ResidualAssessmentPrice);

                if (latestTnp4Date != null)
                {
                    var tnp2RecordLaterThanTnp4 = TNP2RecordLaterThanTNP4Record(learningDelivery.AppFinRecords, latestTnp4Date.AFinDate);

                    if (tnp2RecordLaterThanTnp4 != null)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                tnp2RecordLaterThanTnp4.AFinType,
                                tnp2RecordLaterThanTnp4.AFinCode,
                                tnp2RecordLaterThanTnp4.AFinDate));
                    }
                }
            }
        }

        public IAppFinRecord TNP2RecordLaterThanTNP4Record(IEnumerable<IAppFinRecord> appFinRecords, DateTime? latestTnp4Date)
        {
            var tnp2Records =
              _appFinRecordQueryService?.GetAppFinRecordsForTypeAndCode(
                  appFinRecords,
                  ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                  ApprenticeshipFinancialRecord.TotalNegotiatedPriceCodes.TotalAssessmentPrice);

            return tnp2Records?.Where(af => af.AFinDate > latestTnp4Date).FirstOrDefault();
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
