using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.PCTLDCS
{
    public class PCTLDCS_01Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly ILARSDataService _larsData;

        private readonly IProvideRuleCommonOperations _check;

        public PCTLDCS_01Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IProvideRuleCommonOperations commonChecks)
            : base(validationErrorHandler, RuleNameConstants.PCTLDCS_01)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));
            It.IsNull(commonChecks)
                .AsGuard<ArgumentNullException>(nameof(commonChecks));

            _larsData = larsData;
            _check = commonChecks;
        }

        public static DateTime FirstViableDate => new DateTime(2009, 08, 01);

        public bool HasKnownLDCSCode(ILearningDelivery delivery) =>
            _larsData.HasKnownLearnDirectClassSystemCode3For(delivery.LearnAimRef);

        public bool HasQualifyingPCTLDCSNull(ILearningDeliveryHE deliveryHE) =>
            deliveryHE != null && deliveryHE.PCTLDCSNullable == null;

        public bool IsNotValid(ILearningDelivery delivery) =>
            _check.HasQualifyingStart(delivery, FirstViableDate)
                && HasKnownLDCSCode(delivery)
                && HasQualifyingPCTLDCSNull(delivery.LearningDeliveryHEEntity);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

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
