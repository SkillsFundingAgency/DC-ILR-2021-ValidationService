using ESFA.DC.ILR.Model.Interface;
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
        /// <summary>
        /// The check(er, rule common operations provider)
        /// </summary>
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

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="theLearner">The object to validate.</param>
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

        /// <summary>
        /// Determines whether [has qualifying model] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.ApprenticeshipsFrom1May2017);

        /// <summary>
        /// Determines whether [is component aim] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is component aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComponentAim(ILearningDelivery theDelivery) =>
            _check.IsComponentOfAProgram(theDelivery);

        /// <summary>
        /// Determines whether [has qualifying basic skills type] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying basic skills type] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingBasicSkillsType(ILearningDelivery theDelivery) =>
            _larsData
                .GetAnnualValuesFor(theDelivery.LearnAimRef)
                .Where(HasABasicSkillType)
                .Any(x => IsEnglishOrMathBasicSkill(x) && IsValueCurrent(theDelivery, x));

        /// <summary>
        /// Determines whether [has a basic skill type] [the specified value].
        /// </summary>
        /// <param name="theValue">The value.</param>
        /// <returns>
        ///   <c>true</c> if [has a basic skill type] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasABasicSkillType(ILARSAnnualValue theValue) =>
            It.Has(theValue.BasicSkillsType);

        /// <summary>
        /// Determines whether [is english or math basic skill] [the specified delivery].
        /// </summary>
        /// <param name="theValue">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is english or math basic skill] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnglishOrMathBasicSkill(ILARSAnnualValue theValue) =>
            TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills.Contains((int)theValue.BasicSkillsType);

        /// <summary>
        /// Determines whether the specified the delivery is current.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="theValue">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery is current; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValueCurrent(ILearningDelivery theDelivery, ILARSAnnualValue theValue) =>
            theValue.IsCurrent(theDelivery.LearnStartDate);

        /// <summary>
        /// Determines whether [has qualifying common component] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying common component] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public ILARSLearningDelivery GetLarsAim(ILearningDelivery theDelivery) =>
            _larsData.GetDeliveryFor(theDelivery.LearnAimRef);

        /// <summary>
        /// Determines whether [has qualifying common component] [the specified lars aim].
        /// </summary>
        /// <param name="theLarsAim">The lars aim.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying common component] [the specified lars aim]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingCommonComponent(ILARSLearningDelivery theLarsAim) =>
            It.Has(theLarsAim) && IsBritishSignLanguage(theLarsAim);

        /// <summary>
        /// Determines whether [is british sign language] [the specified lars framework].
        /// </summary>
        /// <param name="larsFramework">The lars framework.</param>
        /// <returns>
        ///   <c>true</c> if [is british sign language] [the specified lars framework]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBritishSignLanguage(ILARSLearningDelivery theDelivery) =>
            theDelivery.FrameworkCommonComponent == TypeOfLARSCommonComponent.BritishSignLanguage;

        /// <summary>
        /// Determines whether [has disqualifying monitor] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying monitor] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsLearningSupportFunding);

        /// <summary>
        /// Determines whether [is learning support funding] [the specified monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is learning support funding] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLearningSupportFunding(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange(theMonitor.LearnDelFAMType, Monitoring.Delivery.Types.LearningSupportFunding);

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNum">The learn reference number.</param>
        /// <param name="theDelivery">The delivery.</param>
        private void RaiseValidationMessage(string learnRefNum, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNum, theDelivery.AimSeqNumber, BuildMessageParametersfor(theDelivery));

        /// <summary>
        /// Builds the message parametersfor.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns></returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersfor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.AimType, theDelivery.AimType),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.LearningSupportFunding)
        };
    }
}
