using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R66Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _funModel = FundModels.NotFundedByESFA;

        public R66Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R66)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            var learningDeliveriesProgrammingAimType = objectToValidate.LearningDeliveries
                .Where(p => p.AimType == TypeOfAim.ProgrammeAim).ToList();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.AimType,
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.StdCodeNullable,
                    learningDeliveriesProgrammingAimType))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.AimType,
                            learningDelivery.FundModel,
                            learningDelivery.ProgTypeNullable,
                            learningDelivery.FworkCodeNullable,
                            learningDelivery.PwayCodeNullable,
                            learningDelivery.StdCodeNullable));
                }
            }
        }

        public bool ConditionMet(
            int aimType,
            int fundModel,
            int? progTypeNullable,
            int? fworkCodeNullable,
            int? pwayCodeNullable,
            int? stdCodeNullable,
            IEnumerable<ILearningDelivery> learningDeliveriesProgrammingAimType)
        {
            return FundModelConditionMet(fundModel)
                && AimTypeConditionMet(
                    aimType,
                    fundModel,
                    progTypeNullable,
                    fworkCodeNullable,
                    pwayCodeNullable,
                    stdCodeNullable,
                    learningDeliveriesProgrammingAimType);
        }

        public bool AimTypeConditionMet(
            int aimType,
            int fundModel,
            int? progTypeNullable,
            int? fworkCodeNullable,
            int? pwayCodeNullable,
            int? stdCodeNullable,
            IEnumerable<ILearningDelivery> learningDeliveriesProgrammingAimType)
        {
            return aimType == TypeOfAim.ComponentAimInAProgramme
                && (learningDeliveriesProgrammingAimType == null
                    || !learningDeliveriesProgrammingAimType.Any(
                        p => p.FundModel == fundModel
                        && p.ProgTypeNullable == progTypeNullable
                        && p.FworkCodeNullable == fworkCodeNullable
                        && p.PwayCodeNullable == pwayCodeNullable
                        && p.StdCodeNullable == stdCodeNullable));
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel != _funModel;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            int aimType,
            int fundModel,
            int? progTypeNullable,
            int? fworkCodeNullable,
            int? pwayCodeNullable,
            int? stdCodeNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, fworkCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, pwayCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, stdCodeNullable)
            };
        }
    }
}
