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
    public class AFinDate_08Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _appFinRecordQueryService;

        public AFinDate_08Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_08)
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
                var tnp2Records =
                    _appFinRecordQueryService?.GetAppFinRecordsForTypeAndCode(
                        learningDelivery.AppFinRecords,
                        ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                        ApprenticeshipFinancialRecord.TotalNegotiatedPriceCodes.TotalAssessmentPrice);

                var tnp4Dates =
                    _appFinRecordQueryService?.GetAppFinRecordsForTypeAndCode(
                        learningDelivery.AppFinRecords,
                        ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                        ApprenticeshipFinancialRecord.TotalNegotiatedPriceCodes.ResidualAssessmentPrice)
                        .Select(af => af.AFinDate).ToList();

                if (tnp2Records.Any() && tnp4Dates.Any())
                {
                    var tnp2DateEqualToTnp4 = TNP2DateEqualToTNP4Date(tnp2Records, tnp4Dates);

                    if (tnp2DateEqualToTnp4 != null)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                tnp2DateEqualToTnp4.AFinType,
                                tnp2DateEqualToTnp4.AFinCode,
                                tnp2DateEqualToTnp4.AFinDate));
                    }
                }
            }
        }

        public IAppFinRecord TNP2DateEqualToTNP4Date(IEnumerable<IAppFinRecord> tnp2Records, List<DateTime> tnp4Dates)
        {
            return tnp2Records?.Where(af => tnp4Dates.Contains(af.AFinDate)).FirstOrDefault();
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
