using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_81Rule :
        IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnAimRef_81;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly ILARSDataService _larsData;

        private readonly IProvideRuleCommonOperations _check;

        public LearnAimRef_81Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IProvideRuleCommonOperations commonChecks)
        {
            _messageHandler = validationErrorHandler;
            _larsData = larsData;
            _check = commonChecks;
        }

        public static DateTime FirstViableDate => new DateTime(2016, 08, 01);

        public string RuleName => Name;

        public bool HasDisqualifyingLearningCategory(ILARSLearningCategory category) =>
            category.CategoryRef == TypeOfLARSCategory.LicenseToPractice;

        public bool HasDisqualifyingLearningCategory(ILearningDelivery delivery)
        {
            var categories = _larsData.GetCategoriesFor(delivery.LearnAimRef).ToReadOnlyCollection();

            return categories.Any(HasDisqualifyingLearningCategory);
        }

        public bool InReceiptOfAnotherStateBenefit(ILearningDelivery delivery, ILearner learner)
        {
            var candidate = _check.GetEmploymentStatusOn(delivery.LearnStartDate, learner.LearnerEmploymentStatuses);

            var esms = candidate?.EmploymentStatusMonitorings.ToReadOnlyCollection();
            return esms.NullSafeAny(InReceiptOfAnotherStateBenefit);
        }

        public bool InReceiptOfAnotherStateBenefit(IEmploymentStatusMonitoring monitor) =>
             Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit.CaseInsensitiveEquals($"{monitor.ESMType}{monitor.ESMCode}");

        public bool IsExcluded(ILearningDelivery delivery) =>
            _check.IsSteelWorkerRedundancyTraining(delivery);

        public bool IsNotValid(ILearningDelivery delivery, ILearner learner) =>
            !IsExcluded(delivery)
            && _check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills, TypeOfFunding.OtherAdult, TypeOfFunding.EuropeanSocialFund)
            && _check.HasQualifyingStart(delivery, FirstViableDate)
            && InReceiptOfAnotherStateBenefit(delivery, learner)
            && HasDisqualifyingLearningCategory(delivery);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(x => IsNotValid(x, objectToValidate))
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(IEmploymentStatusMonitoring.ESMType), Monitoring.EmploymentStatus.Types.BenefitStatusIndicator),
                _messageHandler.BuildErrorMessageParameter(nameof(IEmploymentStatusMonitoring.ESMCode), 3),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate),
                _messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.FundModel), thisDelivery.FundModel)
        };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
