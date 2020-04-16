using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly IDerivedData_06Rule _dd06;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LLDDHealthProb_06Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IDerivedData_06Rule dd06,
            IDateTimeQueryService dateTimeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LLDDHealthProb_06)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
            _dd06 = dd06;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var dd06Date = _dd06.Derive(objectToValidate.LearningDeliveries);

            if (ConditionMet(
                objectToValidate.LLDDHealthProb,
                objectToValidate.LLDDAndHealthProblems,
                objectToValidate.LearningDeliveries,
                objectToValidate.DateOfBirthNullable,
                dd06Date))
            {
                HandleValidationError(objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(int llddHealthProb, IEnumerable<ILLDDAndHealthProblem> llddAndHealthProblems, IEnumerable<ILearningDelivery> learningDeliveries, DateTime? dateOfBirth, DateTime dd06Date)
        {
            return LLDDHealthProbConditionMet(llddHealthProb)
                   && LLDDRecordConditionMet(llddAndHealthProblems)
                   && !Excluded(learningDeliveries, dateOfBirth, dd06Date);
        }

        public bool LLDDHealthProbConditionMet(int llddHealthProb)
        {
            return llddHealthProb == 1;
        }

        public bool LLDDRecordConditionMet(IEnumerable<ILLDDAndHealthProblem> llddAndHealthProblems)
        {
            return llddAndHealthProblems == null
                   || !llddAndHealthProblems.Any();
        }

        public bool Excluded(IEnumerable<ILearningDelivery> learningDeliveries, DateTime? dateOfBirth, DateTime dd06Date)
        {
            return ExcludedFundModelConditionMet(learningDeliveries)
                   || ExcludedDeliveryFAMConditionMet(learningDeliveries)
                   || ExcludedDOBConditionMet(dateOfBirth, dd06Date);
        }

        public bool ExcludedFundModelConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries != null
                   && learningDeliveries.Any(ld => ld.FundModel == 10);
        }

        public bool ExcludedDeliveryFAMConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries != null
                   && learningDeliveries.Any(learningDelivery => learningDelivery.FundModel == 99 && _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "108"));
        }

        public bool ExcludedDOBConditionMet(DateTime? dateOfBirth, DateTime dd06Date)
        {
            return dateOfBirth.HasValue
                   && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, dd06Date) >= 25;
        }
    }
}
