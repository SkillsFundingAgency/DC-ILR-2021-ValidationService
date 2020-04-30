using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R30Rule : AbstractRule, IRule<ILearner>
    {
        public R30Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R30)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var groups = objectToValidate.LearningDeliveries
                .GroupBy(ld => new { ld.ProgTypeNullable, ld.FworkCodeNullable, ld.PwayCodeNullable })
                .Where(grp => grp.Key.ProgTypeNullable != TypeOfLearningProgramme.ApprenticeshipStandard);

            foreach (var group in groups)
            {
                if (group.Any(d => d.AimType == AimTypes.ComponentAimInAProgramme) && group.All(d => d.AimType != AimTypes.ProgrammeAim))
                {
                    var aimSequenceNumber = group.First(x => x.AimType == AimTypes.ComponentAimInAProgramme).AimSeqNumber;
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        aimSequenceNumber,
                        BuildErrorMessageParameters(group.Key.ProgTypeNullable, group.Key.FworkCodeNullable, group.Key.PwayCodeNullable));
                }
            }
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? progType, int? fworkCode, int? pwayCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType.ToString()),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, fworkCode.ToString()),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, pwayCode.ToString()),
            };
        }
    }
}