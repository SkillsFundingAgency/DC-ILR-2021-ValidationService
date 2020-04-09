using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

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

        private readonly IProvideRuleCommonOperations _check;

        public LearnAimRef_87Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonChecks)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonChecks)
                .AsGuard<ArgumentNullException>(nameof(commonChecks));

            _messageHandler = validationErrorHandler;
            _check = commonChecks;
        }

        public static DateTime FirstViableDate => new DateTime(2017, 08, 01);

        public string RuleName => Name;

        public bool HasDisqualifyingVocationalAim(ILearningDelivery delivery) =>
            DisqualifyingAims.Any(x => delivery.LearnAimRef.CaseInsensitiveStartsWith(x));

        public bool IsNotValid(ILearningDelivery delivery) =>
            _check.HasQualifyingStart(delivery, FirstViableDate)
            && HasDisqualifyingVocationalAim(delivery);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
