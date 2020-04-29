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
    public class LearnDelFAMType_67Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly ILARSDataService _larsData;

        public LearnDelFAMType_67Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_67)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
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
            && IsComponentAim(theDelivery)
            && !(HasQualifyingBasicSkillsType(theDelivery)
                        || HasQualifyingCommonComponent(GetLarsAim(theDelivery)))
            && IsLearningSupportFunding(theDelivery);

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            theDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;

        public bool IsComponentAim(ILearningDelivery theDelivery) =>
            theDelivery.AimType == TypeOfAim.ComponentAimInAProgramme;

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

        public ILARSLearningDelivery GetLarsAim(ILearningDelivery theDelivery) =>
            _larsData.GetDeliveryFor(theDelivery.LearnAimRef);

        public bool HasQualifyingCommonComponent(ILARSLearningDelivery theLarsAim) =>
            theLarsAim != null && IsBritishSignLanguage(theLarsAim);

        public bool IsBritishSignLanguage(ILARSLearningDelivery theDelivery) =>
            theDelivery.FrameworkCommonComponent == TypeOfLARSCommonComponent.BritishSignLanguage;

        public bool IsLearningSupportFunding(ILearningDelivery theDelivery) =>
            _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(theDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LSF);

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersfor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.AimType, theDelivery.AimType),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.LearningSupportFunding)
        };

        private void RaiseValidationMessage(string learnRefNum, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNum, theDelivery.AimSeqNumber, BuildMessageParametersfor(theDelivery));
    }
}
