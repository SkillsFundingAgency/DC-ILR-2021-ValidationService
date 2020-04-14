using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_67Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _check;

        private readonly ILARSDataService _larsData;

        public LearnDelFAMType_67Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOps,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_67)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));
            It.IsNull(larsDataService)
                .AsGuard<ArgumentNullException>(nameof(larsDataService));

            _check = commonOps;
            _larsData = larsDataService;
        }

        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public bool IsNotValid(ILearningDelivery theDelivery) =>
            HasQualifyingModel(theDelivery)
            && IsComponentAim(theDelivery)
            && !(HasQualifyingBasicSkillsType(theDelivery)
                        || HasQualifyingCommonComponent(GetLarsAim(theDelivery)))
            && HasDisqualifyingMonitor(theDelivery);

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.ApprenticeshipsFrom1May2017);

        public bool IsComponentAim(ILearningDelivery theDelivery) =>
            _check.IsComponentOfAProgram(theDelivery);

        public bool HasQualifyingBasicSkillsType(ILearningDelivery theDelivery) =>
            _larsData
                .GetAnnualValuesFor(theDelivery.LearnAimRef)
                .Where(HasABasicSkillType)
                .Any(x => IsEnglishOrMathBasicSkill(x) && IsValueCurrent(theDelivery, x));

        public bool HasABasicSkillType(ILARSAnnualValue theValue) =>
            It.Has(theValue.BasicSkillsType);

        public bool IsEnglishOrMathBasicSkill(ILARSAnnualValue theValue) =>
            TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills.Contains((int)theValue.BasicSkillsType);

        public bool IsValueCurrent(ILearningDelivery theDelivery, ILARSAnnualValue theValue) =>
            theValue.IsCurrent(theDelivery.LearnStartDate);

        public ILARSLearningDelivery GetLarsAim(ILearningDelivery theDelivery) =>
            _larsData.GetDeliveryFor(theDelivery.LearnAimRef);

        public bool HasQualifyingCommonComponent(ILARSLearningDelivery theLarsAim) =>
            It.Has(theLarsAim) && IsBritishSignLanguage(theLarsAim);

        public bool IsBritishSignLanguage(ILARSLearningDelivery theDelivery) =>
            theDelivery.FrameworkCommonComponent == TypeOfLARSCommonComponent.BritishSignLanguage;

        public bool HasDisqualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsLearningSupportFunding);

        public bool IsLearningSupportFunding(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange(theMonitor.LearnDelFAMType, Monitoring.Delivery.Types.LearningSupportFunding);

        private void RaiseValidationMessage(string learnRefNum, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNum, theDelivery.AimSeqNumber, BuildMessageParametersfor(theDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersfor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.AimType, theDelivery.AimType),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.LearningSupportFunding)
        };
    }
}
