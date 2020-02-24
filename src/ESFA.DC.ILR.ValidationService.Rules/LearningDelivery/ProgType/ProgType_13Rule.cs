using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class ProgType_13Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "ProgType";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.ProgType_13;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The file data (service)
        /// </summary>
        private readonly IFileDataService _fileData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgType_13Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="fileData">The file data (service).</param>
        public ProgType_13Rule(IValidationErrorHandler validationErrorHandler, IFileDataService fileData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fileData)
                .AsGuard<ArgumentNullException>(nameof(fileData));

            _messageHandler = validationErrorHandler;
            _fileData = fileData;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

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
                .SafeWhere(x => It.IsInRange(x.ProgTypeNullable, TypeOfLearningProgramme.Traineeship) && It.IsInRange(x.AimType, TypeOfAim.ProgrammeAim))
                .ForEach(x =>
                {
                    var failedValidation = !ConditionMet(x);

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        /// <summary>
        /// Condition met.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        /// true if any any point the conditions are met
        /// </returns>
        public bool ConditionMet(ILearningDelivery thisDelivery)
        {
            return It.Has(thisDelivery) && It.IsEmpty(thisDelivery.LearnActEndDateNullable)
                ? TypeOfLearningProgramme.WithinMaxmimumOpenTrainingDuration(thisDelivery.LearnStartDate, _fileData.FilePreparationDate())
                : true;
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
