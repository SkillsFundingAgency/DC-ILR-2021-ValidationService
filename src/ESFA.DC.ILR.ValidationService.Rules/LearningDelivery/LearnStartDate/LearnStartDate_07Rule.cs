using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_07Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly HashSet<int> _fwkCommonComponents = new HashSet<int>(TypeOfLARSCommonComponent.CommonComponents);
        private readonly IDerivedData_04Rule _derivedData04;
        private readonly ILARSDataService _larsData;
        private readonly IProvideRuleCommonOperations _check;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnStartDate_07Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_04Rule derivedData04,
            ILARSDataService larsData,
            IProvideRuleCommonOperations commonOperations,
            IDateTimeQueryService dateTimeQueryService)
                : base(validationErrorHandler, RuleNameConstants.LearnStartDate_07)
        {
            _derivedData04 = derivedData04;
            _larsData = larsData;
            _check = commonOperations;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner theLearner)
        {
            var learnrefNumber = theLearner.LearnRefNumber;
            var deliveries = theLearner.LearningDeliveries.ToReadOnlyCollection();

            deliveries.ForAny(
                x => IsNotValid(x, GetEarliestStartDateFor(x, deliveries)),
                x => RaiseValidationMessage(learnrefNumber, x));
        }

        public DateTime? GetEarliestStartDateFor(ILearningDelivery theDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData04.GetEarliesStartDateFor(theDelivery, usingSources);

        public bool IsNotValid(ILearningDelivery theDelivery, DateTime? earliestStart) =>
            !IsExcluded(theDelivery)
                && IsComponentAim(theDelivery)
                && IsApprenticeship(theDelivery)
                && HasEarliestStart(earliestStart)
                && !HasQualifyingFrameworkAim(
                    FilteredFrameworkAimsFor(
                    theDelivery,
                    GetQualifyingFrameworksFor(theDelivery)),
                    x => IsCurrent(x, earliestStart.Value));

        public bool IsExcluded(ILearningDelivery theDelivery) =>
            IsStandardApprenticeship(theDelivery)
                || IsRestart(theDelivery)
                || IsCommonComponent(GetLARSLearningDeliveryFor(theDelivery));

        public bool IsStandardApprenticeship(ILearningDelivery theDelivery) =>
            theDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard;

        public bool IsRestart(ILearningDelivery theDelivery) =>
            _check.IsRestart(theDelivery);

        public ILARSLearningDelivery GetLARSLearningDeliveryFor(ILearningDelivery theDelivery) =>
            _larsData.GetDeliveryFor(theDelivery.LearnAimRef);

        public bool IsCommonComponent(ILARSLearningDelivery larsDelivery) =>
            larsDelivery?.FrameworkCommonComponent != null
            && larsDelivery.FrameworkCommonComponent.HasValue
            && _fwkCommonComponents.Contains(larsDelivery.FrameworkCommonComponent.Value);

        public bool IsComponentAim(ILearningDelivery theDelivery) =>
            theDelivery.AimType == TypeOfAim.ComponentAimInAProgramme;

        public bool IsApprenticeship(ILearningDelivery theDelivery) =>
            _check.InApprenticeship(theDelivery);

        public bool HasEarliestStart(DateTime? earliestStart) =>
            earliestStart.HasValue;

        public IReadOnlyCollection<ILARSFrameworkAim> GetQualifyingFrameworksFor(ILearningDelivery theDelivery) =>
            _larsData.GetFrameworkAimsFor(theDelivery.LearnAimRef);

        public IReadOnlyCollection<ILARSFrameworkAim> FilteredFrameworkAimsFor(ILearningDelivery theDelivery, IReadOnlyCollection<ILARSFrameworkAim> usingTheseAims) =>
            usingTheseAims
                .NullSafeWhere(fa => fa.ProgType == theDelivery.ProgTypeNullable
                    && fa.FworkCode == theDelivery.FworkCodeNullable
                    && fa.PwayCode == theDelivery.PwayCodeNullable)
                .ToReadOnlyCollection();

        public bool HasQualifyingFrameworkAim(IReadOnlyCollection<ILARSFrameworkAim> frameworkAims, Func<IReadOnlyCollection<ILARSFrameworkAim>, bool> isCurrent) =>
            IsOutOfScope(frameworkAims) || isCurrent(frameworkAims);

        public bool IsOutOfScope(IReadOnlyCollection<ILARSFrameworkAim> frameworkAims) =>
            frameworkAims.IsNullOrEmpty();

        public bool IsCurrent(IReadOnlyCollection<ILARSFrameworkAim> frameworkAims, DateTime earliestStart) =>
            frameworkAims.Any(fa => IsCurrent(fa, earliestStart));

        public bool IsCurrent(ILARSFrameworkAim frameworkAim, DateTime candidateStart) =>
            _dateTimeQueryService.IsDateBetween(candidateStart, DateTime.MinValue, frameworkAim.EndDate ?? DateTime.MaxValue, true);

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery) =>
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
            BuildErrorMessageParameter(PropertyNameConstants.PwayCode, thisDelivery.PwayCodeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.ProgType, thisDelivery.ProgTypeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.FworkCode, thisDelivery.FworkCodeNullable),
        };
    }
}
