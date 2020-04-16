using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.FUNDLEV
{
    public class FUNDLEV_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;

        private readonly DateTime _dateCondition = new DateTime(2009, 08, 01);
        private readonly int[] _validFundLevCodes = { 10, 11, 99 };
        private readonly string[] _validLearnAimRefTypes = { "0394", "1406", "1407", "1408", "9000", "9002", "9107", "E007", "9112", "9111", "9110", "9113" };

        public FUNDLEV_02Rule(
                ILARSDataService larsDataService,
                IValidationErrorHandler validationErrorHandler)
                : base(validationErrorHandler, RuleNameConstants.FUNDLEV_02)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearnAimRef,
                    learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.LearningDeliveryHEEntity.FUNDLEV));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, string learnAimRef, ILearningDeliveryHE learningDeliveryHe)
        {
            return LearnStartDateConditionMet(learnStartDate)
                   && LARSConditionMet(learnAimRef)
                   && LearningDeliveryHeConditionMet(learningDeliveryHe);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _dateCondition;
        }

        public bool LARSConditionMet(string learnAimRef)
        {
            return _larsDataService.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, _validLearnAimRefTypes);
        }

        public bool LearningDeliveryHeConditionMet(ILearningDeliveryHE learningDeliveryHe)
        {
            return learningDeliveryHe != null
                   && !_validFundLevCodes.Contains(learningDeliveryHe.FUNDLEV);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int fundLev)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FUNDLEV, fundLev)
            };
        }
    }
}
