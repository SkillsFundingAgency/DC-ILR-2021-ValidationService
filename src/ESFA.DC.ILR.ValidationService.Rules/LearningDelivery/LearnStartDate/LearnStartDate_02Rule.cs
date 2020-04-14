using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_02Rule : AbstractRule, IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnStartDate_02;

        public const int OldestLearningSubmissionOffset = -10;

        private readonly IAcademicYearDataService _academicYearService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LearnStartDate_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService academicYearService,
            IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, Name)
        {
            _academicYearService = academicYearService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public bool IsOutsideValidSubmissionPeriod(ILearningDelivery delivery) =>
            delivery.LearnStartDate < _dateTimeQueryService.AddYearsToDate(_academicYearService.Start(), OldestLearningSubmissionOffset);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsOutsideValidSubmissionPeriod)
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
                BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate)
            };
        }
    }
}
