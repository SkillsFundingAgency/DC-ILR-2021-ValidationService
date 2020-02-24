using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEmpId
{
    public class WorkPlaceEmpId_03Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The temporary employer identifier
        /// </summary>
        public const int TemporaryEmpID = 999999999;

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.WorkPlaceEmpId_03;

        /// <summary>
        /// The file data service
        /// </summary>
        private readonly IFileDataService _fileDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkPlaceEmpId_03Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="fileDataService">The file data service.</param>
        public WorkPlaceEmpId_03Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService)
            : base(validationErrorHandler, Name)
        {
            // this check should be in the base class
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fileDataService)
                .AsGuard<ArgumentNullException>(nameof(fileDataService));

            _fileDataService = fileDataService;
        }

        /// <summary>
        /// Gets sixty days.
        /// </summary>
        public TimeSpan SixtyDays => new TimeSpan(60, 0, 0, 0); // 60 days

        /// <summary>
        /// Determines whether [is qualifying programme] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying programme] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingProgramme(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship);

        /// <summary>
        /// Determines whether [is inside the registration period] [the specified placement].
        /// </summary>
        /// <param name="placement">The placement.</param>
        /// <returns>
        ///   <c>true</c> if [is inside the registration period] [the specified placement]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInsideTheRegistrationPeriod(ILearningDeliveryWorkPlacement placement) =>
            (_fileDataService.FilePreparationDate() - placement.WorkPlaceStartDate) <= SixtyDays;

        /// <summary>
        /// Requires employer registration.
        /// </summary>
        /// <param name="placement">The placement.</param>
        /// <returns>true if the employer id is temporary</returns>
        public bool RequiresEmployerRegistration(ILearningDeliveryWorkPlacement placement) =>
            It.IsInRange(placement.WorkPlaceEmpIdNullable, TemporaryEmpID);

        /// <summary>
        /// Determines whether [is not valid] [the specified placement].
        /// </summary>
        /// <param name="placement">The placement.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified placement]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDeliveryWorkPlacement placement) =>
            RequiresEmployerRegistration(placement) && IsInsideTheRegistrationPeriod(placement);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery) =>
            IsQualifyingProgramme(delivery)
                && delivery.LearningDeliveryWorkPlacements.SafeAny(IsNotValid);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildErrorMessageParameters());
        }

        /// <summary>
        /// Builds the error message parameters.
        /// </summary>
        /// <returns>returns a list of mesasge parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters()
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceEmpId, TemporaryEmpID)
            };
        }
    }
}
