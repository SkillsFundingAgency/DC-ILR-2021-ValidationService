using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    public class ProgType_13Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "ProgType";

        public const string Name = RuleNameConstants.ProgType_13;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly IFileDataService _fileData;

        public ProgType_13Rule(IValidationErrorHandler validationErrorHandler, IFileDataService fileData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fileData)
                .AsGuard<ArgumentNullException>(nameof(fileData));

            _messageHandler = validationErrorHandler;
            _fileData = fileData;
        }

        public string RuleName => Name;

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

        public bool ConditionMet(ILearningDelivery thisDelivery)
        {
            return It.Has(thisDelivery) && It.IsEmpty(thisDelivery.LearnActEndDateNullable)
                ? TypeOfLearningProgramme.WithinMaxmimumOpenTrainingDuration(thisDelivery.LearnStartDate, _fileData.FilePreparationDate())
                : true;
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
