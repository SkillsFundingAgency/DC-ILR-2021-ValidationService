using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R68Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int?> _apprenticeshipProgTypes = new HashSet<int?>()
        {
            TypeOfLearningProgramme.AdvancedLevelApprenticeship,
            TypeOfLearningProgramme.IntermediateLevelApprenticeship,
            TypeOfLearningProgramme.HigherApprenticeshipLevel4,
            TypeOfLearningProgramme.HigherApprenticeshipLevel5,
            TypeOfLearningProgramme.HigherApprenticeshipLevel6,
            TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus,
            TypeOfLearningProgramme.ApprenticeshipStandard
        };

        public R68Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R68)
        {
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            var programDeliveries = learner.LearningDeliveries
                .Where(IsApprenticeshipProgrammeAim)
                .GroupBy(ld => new { ld.ProgTypeNullable, ld.StdCodeNullable, ld.FworkCodeNullable })
                .SelectMany(x => x.ToList());

            if (!programDeliveries.Any() || !programDeliveries.GroupBy(ld => ld.StdCodeNullable).Any(c => c.Count() > 1))
            {
                return;
            }

            foreach (var appFinRecords in programDeliveries
                .Where(ld => ld.AppFinRecords != null)
                .SelectMany(ld => ld.AppFinRecords)
                .GroupBy(afr => new { AFinType = afr.AFinType?.ToUpper(), afr.AFinCode, afr.AFinDate })
                .Where(apf => apf.Count() > 1)
                .ToList())
            {
                HandleValidationError(
                    learnRefNumber: learner.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(
                        appFinRecords.Key.AFinType,
                        appFinRecords.Key.AFinCode,
                        appFinRecords.Key.AFinDate));
            }
        }

        public bool IsApprenticeshipProgrammeAim(ILearningDelivery learningDelivery)
        {
            return learningDelivery.AimType == TypeOfAim.ProgrammeAim
                   && learningDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
                   && _apprenticeshipProgTypes.Contains(learningDelivery.ProgTypeNullable);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            string aFinType,
            int aFinCode,
            DateTime aFinDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinCode, aFinCode),
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate)
            };
        }
    }
}
