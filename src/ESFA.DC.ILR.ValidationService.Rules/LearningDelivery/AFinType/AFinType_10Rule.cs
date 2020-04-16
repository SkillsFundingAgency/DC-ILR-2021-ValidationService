using System;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_10Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = RuleNameConstants.AFinType_10;

        public const string Name = "AFinType_10";

        private readonly IValidationErrorHandler _messageHandler;

        public AFinType_10Rule(IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsFunded(ILearningDelivery delivery) =>
            TypeOfFunding.TypeOfFundingCollection.Contains(delivery.FundModel);

        public bool IsTargetApprenticeship(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard;

        public bool IsInAProgramme(ILearningDelivery delivery) =>
            delivery.AimType == TypeOfAim.ProgrammeAim;

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => IsFunded(x) && IsTargetApprenticeship(x) && IsInAProgramme(x))
                .ForEach(x =>
                {
                    var failedValidation = !x.AppFinRecords.NullSafeAny(y => ConditionMet(y));

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        public bool ConditionMet(IAppFinRecord financialRecord)
        {
            return financialRecord == null
                   || $"{financialRecord.AFinType}{financialRecord.AFinCode}".CaseInsensitiveEquals(ApprenticeshipFinancialRecord.TotalAssessmentPrice)
                   || $"{financialRecord.AFinType}{financialRecord.AFinCode}".CaseInsensitiveEquals(ApprenticeshipFinancialRecord.ResidualAssessmentPrice);
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Array.Empty<IErrorMessageParameter>();

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
