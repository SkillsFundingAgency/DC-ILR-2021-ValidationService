using ESFA.DC.ILR.Model.Interface;
using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IProvideRuleCommonOperations : IQueryService
    {
        bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition);

        bool IsRestart(ILearningDelivery delivery);

        bool IsAdvancedLearnerLoan(ILearningDelivery delivery);

        bool IsLoansBursary(ILearningDelivery thisDelivery);

        bool IsLearnerInCustody(ILearningDelivery delivery);

        bool IsSteelWorkerRedundancyTraining(ILearningDelivery delivery);

        bool IsReleasedOnTemporaryLicence(ILearningDelivery delivery);

        bool InApprenticeship(ILearningDelivery delivery);

        bool InAProgramme(ILearningDelivery delivery);

        bool IsComponentOfAProgram(ILearningDelivery delivery);

        bool IsTraineeship(ILearningDelivery delivery);

        bool IsStandardApprenticeship(ILearningDelivery delivery);

        bool HasQualifyingFunding(ILearningDelivery delivery, params int[] desiredFundings);

        bool HasQualifyingStart(ILearningDelivery delivery, DateTime minStart, DateTime? maxStart = null);

        bool HasQualifyingStart(ILearnerEmploymentStatus employment, DateTime minStart, DateTime? maxStart = null);

        ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime? thisStartDate, IReadOnlyCollection<ILearnerEmploymentStatus> usingSources);
    }
}
