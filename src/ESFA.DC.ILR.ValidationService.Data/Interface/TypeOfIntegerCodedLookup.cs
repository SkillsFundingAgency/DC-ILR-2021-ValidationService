namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    /// <summary>
    /// lookup simple keys
    /// these are NOT time restricted integer based value domains
    /// </summary>
    public enum TypeOfIntegerCodedLookup
    {
        Accom,
        AimType,
        CompStatus,
        ContType,
        ELQ,
        EmpStat,
        Ethnicity,
        FinType,
        FundLev,
        FundModel,
        LLDDHealthProb,
        LocType,
        ModeStud,
        MSTuFee,
        Outcome,
        PriorAttain,
        SEC,
        SOC2000,
        SOC2010,
        SpecFee,
        TypeYr,
        WithdrawReason,
        WorkPlaceMode,

        // prog types are not time restricted
        ProgType,
    }
}
