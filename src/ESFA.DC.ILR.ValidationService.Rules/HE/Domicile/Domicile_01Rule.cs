using System;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.DOMICILE
{
    public class DOMICILE_01Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "DOMICILE";

        public const string Name = RuleNameConstants.Domicile_01;

        private readonly IValidationErrorHandler _messageHandler;

        public DOMICILE_01Rule(
            IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public DateTime LastInviableDate => new DateTime(2013, 07, 31);

        public bool IsQualifyingStartDate(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        public bool HasHigherEd(ILearningDelivery delivery) =>
            It.Has(delivery.LearningDeliveryHEEntity);

        public bool HasDomicile(ILearningDeliveryHE he) =>
            It.Has(he.DOMICILE);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => IsQualifyingStartDate(x) && HasHigherEd(x) && !HasDomicile(x.LearningDeliveryHEEntity))
                .ForEach(x =>
                {
                    RaiseValidationMessage(learnRefNumber, x);
                });
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}