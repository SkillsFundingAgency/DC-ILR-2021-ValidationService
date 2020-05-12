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
    public class AFinDate_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;

        public AFinDate_07Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_07)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                var tnp1Records =
                    _learningDeliveryAppFinRecordQueryService?.GetAppFinRecordsForTypeAndCode(
                        learningDelivery.AppFinRecords,
                        ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                        ApprenticeshipFinancialRecord.TotalNegotiatedPriceCodes.TotalTrainingPrice);

                var tnp3Dates =
                    _learningDeliveryAppFinRecordQueryService?.GetAppFinRecordsForTypeAndCode(
                        learningDelivery.AppFinRecords,
                        ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                        ApprenticeshipFinancialRecord.TotalNegotiatedPriceCodes.ResidualTrainingPrice)
                        .Select(af => af.AFinDate).ToList();

                if (tnp1Records.Any() && tnp3Dates.Any())
                {
                    var tnp1DateEqualToTnp3 = TNP1DateEqualToTNP3Date(tnp1Records, tnp3Dates);
                    if (tnp1DateEqualToTnp3 != null)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                tnp1DateEqualToTnp3.AFinType,
                                tnp1DateEqualToTnp3.AFinCode,
                                tnp1DateEqualToTnp3.AFinDate));
                    }
                }
            }
        }

        public IAppFinRecord TNP1DateEqualToTNP3Date(IEnumerable<IAppFinRecord> tnp1Records, List<DateTime> tnp3Dates)
        {
            return tnp1Records?.Where(af => tnp3Dates.Contains(af.AFinDate)).FirstOrDefault();
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
