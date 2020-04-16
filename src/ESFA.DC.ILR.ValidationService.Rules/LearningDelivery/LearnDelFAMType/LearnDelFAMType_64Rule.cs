using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_64Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _check;

        private readonly ILARSDataService _larsData;

        public LearnDelFAMType_64Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOps,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_64)
        {
            _check = commonOps;
            _larsData = larsDataService;
        }

        public void Validate(ILearner theLearner)
        {
            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public bool IsNotValid(ILearningDelivery theDelivery) =>
            HasQualifyingModel(theDelivery)
            && (IsProgrameAim(theDelivery)
                || (IsComponentAim(theDelivery)
                    && (HasQualifyingBasicSkillsType(theDelivery)
                        || HasQualifyingCommonComponent(theDelivery))))
            && !HasQualifyingMonitor(theDelivery);

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.ApprenticeshipsFrom1May2017);

        public bool IsProgrameAim(ILearningDelivery theDelivery) =>
            _check.InAProgramme(theDelivery);

        public bool IsComponentAim(ILearningDelivery theDelivery) =>
            _check.IsComponentOfAProgram(theDelivery);

        public bool HasQualifyingBasicSkillsType(ILearningDelivery theDelivery) =>
            _larsData
                .GetAnnualValuesFor(theDelivery.LearnAimRef)
                .Where(HasABasicSkillType)
                .Any(x => IsEnglishOrMathBasicSkill(x) && IsValueCurrent(theDelivery, x));

        public bool HasABasicSkillType(ILARSAnnualValue theValue) =>
            theValue.BasicSkillsType.HasValue;

        public bool IsEnglishOrMathBasicSkill(ILARSAnnualValue theValue) =>
            TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills.Contains(theValue.BasicSkillsType.Value);

        public bool IsValueCurrent(ILearningDelivery theDelivery, ILARSAnnualValue theValue) =>
            _larsData.IsCurrentAndNotWithdrawn(theValue, theDelivery.LearnStartDate);

        public bool HasQualifyingCommonComponent(ILearningDelivery theDelivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(theDelivery.LearnAimRef);
            return larsDelivery != null && IsBritishSignLanguage(larsDelivery);
        }

        public bool IsBritishSignLanguage(ILARSLearningDelivery theDelivery) =>
            theDelivery.FrameworkCommonComponent == TypeOfLARSCommonComponent.BritishSignLanguage;

        public bool HasQualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsApprenticeshipContract);

        public bool IsApprenticeshipContract(ILearningDeliveryFAM theMonitor) =>
            theMonitor.LearnDelFAMType.CaseInsensitiveEquals(Monitoring.Delivery.Types.ApprenticeshipContract);

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) =>
            new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, theDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.ApprenticeshipContract)
            };
    }
}
