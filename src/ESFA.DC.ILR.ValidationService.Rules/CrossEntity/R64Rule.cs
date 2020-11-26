using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R64Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsData;

        public R64Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.R64)
        {
            _larsData = larsDataService;
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            var learningDeliveries = learner.LearningDeliveries.Where(Filter);

            var groupedLearningDeliveries = ApplyGroupingCondition(learningDeliveries);

            foreach (var group in groupedLearningDeliveries)
            {
                foreach (var learningDelivery in group)
                {
                    RaiseValidationMessage(learner.LearnRefNumber, learningDelivery);
                }
            }
        }

        public bool Filter(ILearningDelivery learningDelivery) =>
                  !Exclusion(learningDelivery.ProgTypeNullable)
                && CompStatusFilter(learningDelivery.CompStatus)
                && OutcomeFilter(learningDelivery.OutcomeNullable)
                && AimTypeFilter(learningDelivery.AimType)
                && FundModelFilter(learningDelivery.FundModel)
                && FrameworkComponentTypeFilter(learningDelivery.LearnAimRef);

        public IEnumerable<IEnumerable<ILearningDelivery>> ApplyGroupingCondition(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries
                .GroupBy(ld =>
                    new
                    {
                        ld.ProgTypeNullable,
                        ld.FworkCodeNullable,
                        ld.PwayCodeNullable,
                        ld.FundModel,
                    })
                .Where(g => g.Count() > 1);
        }

        public bool Exclusion(int? progType) => progType == ProgTypes.ApprenticeshipStandard;

        public bool FundModelFilter(int fundModel) => fundModel == FundModels.AdultSkills || fundModel == FundModels.ApprenticeshipsFrom1May2017;

        public bool AimTypeFilter(int aimType) => aimType == AimTypes.ComponentAimInAProgramme;

        public bool CompStatusFilter(int compStatus) => compStatus == CompletionState.HasCompleted;

        public bool OutcomeFilter(int? outcome) => outcome == OutcomeConstants.Achieved;

        public bool FrameworkComponentTypeFilter(string learnAimRef) =>
             _larsData.GetFrameworkAimsFor(learnAimRef)?.Any(a =>
                    a.FrameworkComponentType == LARSConstants.FrameworkComponentTypes.CompetencyElement
                    || a.FrameworkComponentType == LARSConstants.FrameworkComponentTypes.MainAimOrTechnicalCertificate)
                ?? false;

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.AimType, theDelivery.AimType),
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.ProgType, theDelivery.ProgTypeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.FworkCode, theDelivery.FworkCodeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.PwayCode, theDelivery.PwayCodeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.CompStatus, theDelivery.CompStatus),
            BuildErrorMessageParameter(PropertyNameConstants.Outcome, theDelivery.OutcomeNullable),
        };
    }
}
