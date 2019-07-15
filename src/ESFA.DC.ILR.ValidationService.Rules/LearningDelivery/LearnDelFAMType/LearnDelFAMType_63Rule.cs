using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_63Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "LearnDelFAMType";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "LearnDelFAMType_63";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

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
            ILARSDataService larsData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));

            _messageHandler = validationErrorHandler;
            _larsData = larsData;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Determines whether [is basic skills learner] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is basic skills learner OR exists within CommonComponents for FunctionalSkillsEnglish] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBasicSkillsLearner(ILearningDelivery delivery)
        {
            var validities = _larsData.GetValiditiesFor(delivery.LearnAimRef);
            var annualValues = _larsData.GetAnnualValuesFor(delivery.LearnAimRef);
            var larsFramework = _larsData.GetDeliveryFor(delivery.LearnAimRef).Frameworks;

            return validities.Any(x => x.IsCurrent(delivery.LearnStartDate))
                && (annualValues.Any(IsBasicSkillsLearner) || larsFramework.Any(IsCommonComponent));
        }  

        public bool IsCommonComponent(ILARSFramework larsFramework )
        {
            return larsFramework
                        .FrameworkCommonComponents
                        .Any(x => x.CommonComponent.Equals(TypeOfLARSBasicSkill.FunctionalSkillsEnglish));
        }

        /// <summary>
        /// Determines whether [is basic skills learner] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is basic skills learner] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBasicSkillsLearner(ILARSAnnualValue monitor) =>
            It.IsInRange(monitor.BasicSkillsType, TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills);

        /// <summary>
        /// Determines whether the specified delivery is apprenticeship.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is apprenticeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeship(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.ApprenticeshipsFrom1May2017);

        /// <summary>
        /// Determines whether [is programme aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is programme aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProgrammeAim(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ProgrammeAim);

        /// <summary>
        /// Determines whether [is component aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is component aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComponentAim(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ComponentAimInAProgramme);

        /// <summary>
        /// Determines whether [is core part of a programme] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is core part of a programme] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCorePartOfAProgramme(ILearningDelivery delivery) =>
            IsComponentAim(delivery)
                && IsBasicSkillsLearner(delivery);

        /// <summary>
        /// Determines whether the specified delivery is excluded.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery delivery) =>
            IsApprenticeship(delivery)
                && (IsProgrammeAim(delivery) || IsCorePartOfAProgramme(delivery));

        /// <summary>
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if any of the delivery fams match the condition</returns>
        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether [is apprenticeship contract] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is apprenticeship contract] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeshipContract(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.ApprenticeshipContract);

        /// <summary>
        /// Determines whether [is apprenticeship contract] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is apprenticeship contract] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeshipContract(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsApprenticeshipContract);

        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsExcluded(delivery)
                && IsApprenticeshipContract(delivery);

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
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.AimType, thisDelivery.AimType));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.ApprenticeshipContract));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
