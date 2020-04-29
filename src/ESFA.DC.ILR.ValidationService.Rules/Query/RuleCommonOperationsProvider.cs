using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class RuleCommonOperationsProvider : IProvideRuleCommonOperations
    {
        public bool HasQualifyingFunding(ILearningDelivery delivery, params int[] desiredFundings) =>
           desiredFundings.Contains(delivery.FundModel);
    }
}
