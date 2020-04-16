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
    public class FUNDLEV_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;

        private readonly DateTime _dateCondition = new DateTime(2009, 08, 01);
        private readonly int[] _validFundLevCodes = { 20, 21, 30, 31, 99 };
        private readonly string[] _validLearnAimRefTypes = { "0393", "1410", "2001", "9100", "9101", "9109", "E008", "1411", "1412", "9103" };

    public FUNDLEV_03Rule(
            ILARSDataService larsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FUNDLEV_03)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
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
