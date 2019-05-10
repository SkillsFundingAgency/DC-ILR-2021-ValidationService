using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// the lars framework type definition
    /// </summary>
    public interface ILARSFramework
    {
        /// <summary>
        /// Gets the framework code.
        /// </summary>
        int FworkCode { get; }

        /// <summary>
        /// Gets the programme type.
        /// </summary>
        int ProgType { get; }

        /// <summary>
        /// Gets the pathway code.
        /// </summary>
        int PwayCode { get; }

        /// <summary>
        /// Gets the effective from date.
        /// </summary>
        DateTime? EffectiveFrom { get; }

        /// <summary>
        /// Gets the effective to date.
        /// </summary>
        DateTime? EffectiveTo { get; }

        /// <summary>
        /// Gets the framework aims.
        /// </summary>
        ILARSFrameworkAim FrameworkAim { get; }

        /// <summary>
        /// Gets the framework common components.
        /// </summary>
        IEnumerable<ILARSFrameworkCommonComponent> FrameworkCommonComponents { get; }
    }
}
