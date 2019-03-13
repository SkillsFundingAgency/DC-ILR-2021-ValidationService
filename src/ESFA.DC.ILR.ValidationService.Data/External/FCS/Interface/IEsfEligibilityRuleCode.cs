namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF eligibility rule enterprise partnership
    /// </summary>
    /// <typeparam name="TCode">The type of code.</typeparam>
    public interface IEsfEligibilityRuleCode<TCode>
    {
        TCode Code { get; }
    }
}
