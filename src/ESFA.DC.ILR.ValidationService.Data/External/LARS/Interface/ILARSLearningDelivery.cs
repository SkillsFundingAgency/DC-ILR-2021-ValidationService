using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    public interface ILARSLearningDelivery
    {
        string LearnAimRef { get; }

        DateTime EffectiveFrom { get; }

        DateTime? EffectiveTo { get; }

        string LearnAimRefType { get; }

        int? EnglPrscID { get; }

        string NotionalNVQLevel { get; }

        string NotionalNVQLevelv2 { get; }

        int? FrameworkCommonComponent { get; }

        ILearnDirectClassSystemCode LearnDirectClassSystemCode1 { get; }

        ILearnDirectClassSystemCode LearnDirectClassSystemCode2 { get; }

        ILearnDirectClassSystemCode LearnDirectClassSystemCode3 { get; }

        decimal? SectorSubjectAreaTier1 { get; }

        decimal? SectorSubjectAreaTier2 { get; }

        IReadOnlyCollection<ILARSLearningCategory> Categories { get; }

        IReadOnlyCollection<ILARSFramework> Frameworks { get; }

        IReadOnlyCollection<ILARSAnnualValue> AnnualValues { get; }

        IReadOnlyCollection<ILARSLearningDeliveryValidity> Validities { get; }
    }
}
