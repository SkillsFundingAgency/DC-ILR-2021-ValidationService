namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF eligibility rule enterprise partnership
    /// </summary>
    /// <seealso cref="IEsfEligibilityRuleReferences" />
    /// <seealso cref="Interface.IEsfEligibilityRuleCode{string}" />
    public interface IEsfEligibilityRuleLocalEnterprisePartnership :
        IEsfEligibilityRuleReferences,
        IEsfEligibilityRuleCode<string>
    {
    }
}
