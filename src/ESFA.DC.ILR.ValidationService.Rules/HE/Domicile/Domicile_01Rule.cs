using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

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
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public DateTime LastInviableDate => new DateTime(2013, 07, 31);

        public bool IsQualifyingStartDate(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        public bool HasHigherEd(ILearningDelivery delivery) =>
            delivery.LearningDeliveryHEEntity != null;

        public bool HasDomicile(ILearningDeliveryHE he) =>
            !string.IsNullOrWhiteSpace(he.DOMICILE);

        public void Validate(ILearner objectToValidate)
        {
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