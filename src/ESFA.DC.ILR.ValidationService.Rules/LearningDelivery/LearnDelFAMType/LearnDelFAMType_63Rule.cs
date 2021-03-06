﻿using System;
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
    public class LearnDelFAMType_63Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _basicSkillsList = new HashSet<int>(LARSConstants.BasicSkills.EnglishAndMathsList);
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly ILARSDataService _larsData;

        public LearnDelFAMType_63Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_63)
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
            !IsExcluded(theDelivery)
                && IsApprenticeshipContract(theDelivery);

        public bool IsExcluded(ILearningDelivery theDelivery) =>
            HasQualifyingModel(theDelivery)
            && (IsProgrameAim(theDelivery)
                || (IsComponentAim(theDelivery)
                && (HasQualifyingBasicSkillsType(theDelivery)
                    || HasQualifyingCommonComponent(theDelivery))));

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            theDelivery.FundModel == FundModels.ApprenticeshipsFrom1May2017;

        public bool IsProgrameAim(ILearningDelivery theDelivery) =>
            theDelivery.AimType == AimTypes.ProgrammeAim;

        public bool IsComponentAim(ILearningDelivery theDelivery) =>
            theDelivery.AimType == AimTypes.ComponentAimInAProgramme;

        public bool HasQualifyingBasicSkillsType(ILearningDelivery theDelivery) =>
            _larsData
                .GetAnnualValuesFor(theDelivery.LearnAimRef)
                .Where(HasABasicSkillType)
                .Any(x => IsEnglishOrMathBasicSkill(x) && IsValueCurrent(theDelivery, x));

        public bool HasABasicSkillType(ILARSAnnualValue theValue) =>
            theValue.BasicSkillsType.HasValue;

        public bool IsEnglishOrMathBasicSkill(ILARSAnnualValue theValue) =>
            _basicSkillsList.Contains(theValue.BasicSkillsType.Value);

        public bool IsValueCurrent(ILearningDelivery theDelivery, ILARSAnnualValue theValue) =>
            _larsData.IsCurrentAndNotWithdrawn(theValue, theDelivery.LearnStartDate);

        public bool HasQualifyingCommonComponent(ILearningDelivery theDelivery)
        {
            var larsDelivery = _larsData.GetDeliveryFor(theDelivery.LearnAimRef);
            return larsDelivery != null && IsBritishSignLanguage(larsDelivery);
        }

        public bool IsBritishSignLanguage(ILARSLearningDelivery theDelivery) =>
            theDelivery.FrameworkCommonComponent == LARSConstants.CommonComponents.BritishSignLanguage;

        public bool IsApprenticeshipContract(ILearningDelivery theDelivery) =>
           _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(theDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT);

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        public IReadOnlyCollection<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.AimType, theDelivery.AimType),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.ApprenticeshipContract)
        };
    }
}
