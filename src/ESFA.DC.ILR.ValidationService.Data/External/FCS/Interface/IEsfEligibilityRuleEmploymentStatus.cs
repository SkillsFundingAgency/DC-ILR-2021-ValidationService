namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF Eligibility Rule Employment Status definition
    /// </summary>
    /// <seealso cref="IEsfEligibilityRuleReferences" />
    /// <seealso cref="Interface.IEsfEligibilityRuleCode{int}" />
    public interface IEsfEligibilityRuleEmploymentStatus :
        IEsfEligibilityRuleReferences,
        IEsfEligibilityRuleCode<int>
    {
    }
}
