using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R56Rule : AbstractRule, IRule<ILearner>
    {
        public R56Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R56)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var nonFundedOpenMainAims =
                objectToValidate.LearningDeliveries
                    .Where(ld => ld.FundModel == FundModels.NotFundedByESFA &&
                                 ld.AimType == AimTypes.ProgrammeAim &&
                                 !ld.LearnActEndDateNullable.HasValue).ToList();

            foreach (var nonFundedMainAim in nonFundedOpenMainAims)
            {
                var openFundedComponentAims = objectToValidate.LearningDeliveries.Where(x =>
                                  x.AimType == AimTypes.ComponentAimInAProgramme &&
                                  x.FundModel != FundModels.NotFundedByESFA &&
                                  x.ProgTypeNullable == nonFundedMainAim.ProgTypeNullable &&
                                  x.FworkCodeNullable == nonFundedMainAim.FworkCodeNullable &&
                                  x.PwayCodeNullable == nonFundedMainAim.PwayCodeNullable &&
                                   x.StdCodeNullable == nonFundedMainAim.StdCodeNullable);

                foreach (var openFundedComponentAim in openFundedComponentAims)
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        openFundedComponentAim.AimSeqNumber,
                        BuildErrorMessageParameters(openFundedComponentAim));
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, learningDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, learningDelivery.FworkCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, learningDelivery.PwayCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, learningDelivery.StdCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable)
            };
        }
    }
}
