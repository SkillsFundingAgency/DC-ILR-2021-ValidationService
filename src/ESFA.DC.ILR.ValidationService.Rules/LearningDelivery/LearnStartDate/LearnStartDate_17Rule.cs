using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_17Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsData;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnStartDate_17Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_17)
        {
            _larsData = larsData;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public IReadOnlyCollection<ILARSStandardValidity> GetStandardPeriodsOfValidityFor(ILearningDelivery thisDelivery) =>
            _larsData.GetStandardValiditiesFor(thisDelivery.StdCodeNullable.Value);

        public bool HasQualifyingStart(ILearningDelivery thisDelivery, IReadOnlyCollection<ILARSStandardValidity> allocations) =>
            allocations.NullSafeAny(x => _dateTimeQueryService.IsDateBetween(thisDelivery.LearnStartDate, x.StartDate, DateTime.MaxValue));

        public bool IsNotValid(ILearningDelivery thisDelivery) =>
            !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(thisDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)
            && thisDelivery.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
            && thisDelivery.AimType == AimTypes.ProgrammeAim
            && thisDelivery.StdCodeNullable.HasValue
            && !HasQualifyingStart(thisDelivery, GetStandardPeriodsOfValidityFor(thisDelivery));

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, thisDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, thisDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, thisDelivery.StdCodeNullable)
            };
        }
    }
}
