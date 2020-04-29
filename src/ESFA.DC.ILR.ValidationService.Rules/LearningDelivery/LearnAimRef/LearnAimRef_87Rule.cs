using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_87Rule : IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnAimRef_87;
        private readonly HashSet<string> DisqualifyingAims = new HashSet<string>
        {
            TypeOfAim.Branches.VocationalStudiesNotLeadingToARecognisedQualification,
            TypeOfAim.Branches.NonExternallyCertificatedFEOtherProvision,
            TypeOfAim.Branches.UnitsOfApprovedNQFProvision
        };

        private readonly IValidationErrorHandler _messageHandler;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnAimRef_87Rule(IValidationErrorHandler validationErrorHandler, IDateTimeQueryService dateTimeQueryService)
        {
            _messageHandler = validationErrorHandler;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public static DateTime FirstViableDate => new DateTime(2017, 08, 01);

        public string RuleName => Name;

        public bool HasDisqualifyingVocationalAim(ILearningDelivery delivery) =>
            DisqualifyingAims.Any(x => delivery.LearnAimRef.CaseInsensitiveStartsWith(x));

        public bool IsNotValid(ILearningDelivery delivery) =>
            _dateTimeQueryService.IsDateBetween(delivery.LearnStartDate, FirstViableDate, DateTime.MaxValue)
            && HasDisqualifyingVocationalAim(delivery);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
