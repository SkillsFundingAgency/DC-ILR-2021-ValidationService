namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF eligibility rule local authority definition
    /// </summary>
    /// <seealso cref="IEsfEligibilityRuleReferences" />
    /// <seealso cref="Interface.IEsfEligibilityRuleCode{string}" />
    public interface IEsfEligibilityRuleLocalAuthority :
        IEsfEligibilityRuleReferences,
        IEsfEligibilityRuleCode<string>
    {
    }
}
