
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
    /// <summary>
    /// learning delivery funding and monitoring type rule 63
    /// this rule is the inverse of learning delivery funding and monitoring type rule 64
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class LearnDelFAMType_63Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check(er, rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDelFAMType_63Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        public LearnDelFAMType_63Rule(
            IValidationErrorHandler validationErrorHandler,
             IProvideRuleCommonOperations commonOps,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_63)
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
        /// Validates the specified learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery theDelivery) =>
            !IsExcluded(theDelivery)
                && HasQualifyingMonitor(theDelivery);

        /// <summary>
        /// Determines whether the specified delivery is excluded.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery theDelivery) =>
            HasQualifyingModel(theDelivery)
            && (IsProgrameAim(theDelivery)
                || (IsComponentAim(theDelivery)
                && (HasQualifyingBasicSkillsType(theDelivery)
                    || HasQualifyingCommonComponent(theDelivery))));

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
        /// Determines whether [is programe aim] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is programe aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProgrameAim(ILearningDelivery theDelivery) =>
            _check.InAProgramme(theDelivery);

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
            It.IsInRange(theValue.BasicSkillsType, TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills);

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
        public bool HasQualifyingCommonComponent(ILearningDelivery theDelivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(theDelivery.LearnAimRef);
            return It.Has(larsDelivery) && IsBritishSignLanguage(larsDelivery);
        }

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
        /// Determines whether [has qualifying source of funding] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying source of funding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsApprenticeshipContract);

        /// <summary>
        /// Determines whether [is apprenticeship funded] [the specified monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is apprenticeship funded] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeshipContract(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange(theMonitor.LearnDelFAMType, Monitoring.Delivery.Types.ApprenticeshipContract);

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="theDelivery">The delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns></returns>
        public IReadOnlyCollection<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.AimType, theDelivery.AimType),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.ApprenticeshipContract)
        };
    }
}
