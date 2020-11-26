using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    public class Framework :
        ILARSFramework
    {
        private IEnumerable<ILARSFrameworkCommonComponent> _frameworkCommonComponents;

        public int FworkCode { get; set; }

        public int ProgType { get; set; }

        public int PwayCode { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public ILARSFrameworkAim FrameworkAim { get; set; }

        public IEnumerable<ILARSFrameworkCommonComponent> FrameworkCommonComponents
        {
            get => _frameworkCommonComponents ?? (_frameworkCommonComponents = Enumerable.Empty<ILARSFrameworkCommonComponent>());
            set => _frameworkCommonComponents = value;
        }
    }
}
