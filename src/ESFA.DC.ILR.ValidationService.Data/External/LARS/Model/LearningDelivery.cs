using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    public class LearningDelivery :
        ILARSLearningDelivery
    {
        private IReadOnlyCollection<ILARSLearningCategory> _categories;

        private IReadOnlyCollection<ILARSLearningDeliveryValidity> _validities;

        private IReadOnlyCollection<ILARSAnnualValue> _annualValues;

        private IReadOnlyCollection<ILARSFramework> _frameworks;

        public string LearnAimRef { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public DateTime StartDate => EffectiveFrom;

        public DateTime? EndDate => EffectiveTo;

        public string LearnAimRefType { get; set; }

        public int? EnglPrscID { get; set; }

        public string NotionalNVQLevel { get; set; }

        public string NotionalNVQLevelv2 { get; set; }

        public int? FrameworkCommonComponent { get; set; }

        public ILearnDirectClassSystemCode LearnDirectClassSystemCode1 { get; set; }

        public ILearnDirectClassSystemCode LearnDirectClassSystemCode2 { get; set; }

        public ILearnDirectClassSystemCode LearnDirectClassSystemCode3 { get; set; }

        public decimal? SectorSubjectAreaTier1 { get; set; }

        public decimal? SectorSubjectAreaTier2 { get; set; }

        public IReadOnlyCollection<ILARSLearningCategory> Categories
        {
            get => _categories ?? (_categories = Array.Empty<ILARSLearningCategory>());
            set => _categories = value;
        }

        public IReadOnlyCollection<ILARSLearningDeliveryValidity> Validities
        {
            get => _validities ?? (_validities = Array.Empty<ILARSLearningDeliveryValidity>());
            set => _validities = value;
        }

        public IReadOnlyCollection<ILARSFramework> Frameworks
        {
            get => _frameworks ?? (_frameworks = Array.Empty<ILARSFramework>());
            set => _frameworks = value;
        }

        public IReadOnlyCollection<ILARSAnnualValue> AnnualValues
        {
            get => _annualValues ?? (_annualValues = Array.Empty<ILARSAnnualValue>());
            set => _annualValues = value;
        }
    }
}
