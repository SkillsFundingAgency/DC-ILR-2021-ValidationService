using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.PCTLDCS
{
    public class PCTLDCS_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsData;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public PCTLDCS_01Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.PCTLDCS_01)
        {
            _larsData = larsData;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public static DateTime FirstViableDate => new DateTime(2009, 08, 01);

        public bool HasKnownLDCSCode(ILearningDelivery delivery) =>
            _larsData.HasKnownLearnDirectClassSystemCode3For(delivery.LearnAimRef);

        public bool HasQualifyingPCTLDCSNull(ILearningDeliveryHE deliveryHE) =>
            deliveryHE != null && deliveryHE.PCTLDCSNullable == null;

        public bool IsNotValid(ILearningDelivery delivery) =>
            _dateTimeQueryService.IsDateBetween(delivery.LearnStartDate, FirstViableDate, DateTime.MaxValue)
            && HasKnownLDCSCode(delivery)
            && HasQualifyingPCTLDCSNull(delivery.LearningDeliveryHEEntity);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, thisDelivery.LearnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel)
            };
        }
    }
}
