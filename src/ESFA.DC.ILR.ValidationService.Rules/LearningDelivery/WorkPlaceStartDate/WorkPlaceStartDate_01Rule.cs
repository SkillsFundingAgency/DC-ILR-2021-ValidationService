using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceStartDate
{
    public class WorkPlaceStartDate_01Rule :
        AbstractRule, IRule<ILearner>
    {
        public WorkPlaceStartDate_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WorkPlaceStartDate_01)
        {
        }

        public DateTime LastInviableDate => new DateTime(2014, 07, 31);

        public bool IsViableStart(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        public bool IsWorkPlacement(ILearningDelivery delivery) =>
            It.IsInRange(delivery.LearnAimRef, TypeOfAim.References.AsWorkPlacementCodes);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => IsViableStart(x) && IsWorkPlacement(x))
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
            return It.Has(thisDelivery)
                ? It.HasValues(thisDelivery.LearningDeliveryWorkPlacements)
                : true;
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, thisDelivery.LearnAimRef.ToString())
            };
        }
    }
}
