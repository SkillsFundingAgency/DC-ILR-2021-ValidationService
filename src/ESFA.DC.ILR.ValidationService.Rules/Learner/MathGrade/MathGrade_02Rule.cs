﻿using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade
{
    public class MathGrade_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _lookupProviderDetails;

        public MathGrade_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails provideLookupDetails)
            : base(validationErrorHandler, RuleNameConstants.MathGrade_02)
        {
            _lookupProviderDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null || !MathGradeSuppliedAndNotNone(objectToValidate?.MathGrade))
            {
                return;
            }

            if (MathGradeConditionMet(objectToValidate.MathGrade))
            {
                HandleValidationError(
                    learnRefNumber: objectToValidate.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(objectToValidate.MathGrade));
            }
        }

        public bool MathGradeConditionMet(string mathGrade) => !_lookupProviderDetails.Contains(TypeOfStringCodedLookup.GCSEGrade, mathGrade);

        public bool MathGradeSuppliedAndNotNone(string mathGrade) => !string.IsNullOrEmpty(mathGrade)
                && !mathGrade.CaseInsensitiveEquals(Grades.NONE);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string mathGrade)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.MathGrade, mathGrade)
            };
        }
    }
}
