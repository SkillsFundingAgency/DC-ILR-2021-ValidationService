using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    /// <summary>
    /// the lars learning delivery
    /// </summary>
    /// <seealso cref="ILARSLearningDelivery" />
    public class LearningDelivery :
        ILARSLearningDelivery
    {
        /// <summary>
        /// The learning categories
        /// </summary>
        private IReadOnlyCollection<ILARSLearningCategory> _categories;

        /// <summary>
        /// The learning delivery validities
        /// </summary>
        private IReadOnlyCollection<ILARSLearningDeliveryValidity> _validities;

        /// <summary>
        /// The annual values
        /// </summary>
        private IReadOnlyCollection<ILARSAnnualValue> _annualValues;

        /// <summary>
        /// The frameworks
        /// </summary>
        private IReadOnlyCollection<ILARSFramework> _frameworks;

        /// <summary>
        /// Gets or sets the learn aim reference.
        /// </summary>
        public string LearnAimRef { get; set; }

        /// <summary>
        /// Gets or sets the effective from date.
        /// </summary>
        public DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the effective to date.
        /// </summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>
        /// Gets the effective from (date).
        /// </summary>
        public DateTime StartDate => EffectiveFrom;

        /// <summary>
        /// Gets the effective to (date).
        /// </summary>
        public DateTime? EndDate => EffectiveTo;

        /// <summary>
        /// Gets or sets the learn aim reference type.
        /// </summary>
        public string LearnAimRefType { get; set; }

        /// <summary>
        /// Gets or sets the english prescribed Ids.
        /// </summary>
        public int? EnglPrscID { get; set; }

        /// <summary>
        /// Gets or sets the notional NVQ level.
        /// </summary>
        public string NotionalNVQLevel { get; set; }

        /// <summary>
        /// Gets or sets the notional NVQ level v2.
        /// </summary>
        public string NotionalNVQLevelv2 { get; set; }

        /// <summary>
        /// Gets or sets the framework common component.
        /// </summary>
        public int? FrameworkCommonComponent { get; set; }

        /// <summary>
        /// Gets or sets the learn direct class system code 1.
        /// </summary>
        public ILearnDirectClassSystemCode LearnDirectClassSystemCode1 { get; set; }

        /// <summary>
        ///  Gets or sets the learn direct class system code 2
        /// </summary>
        public ILearnDirectClassSystemCode LearnDirectClassSystemCode2 { get; set; }

        /// <summary>
        /// Gets or sets the learn direct class system code 3.
        /// </summary>
        public ILearnDirectClassSystemCode LearnDirectClassSystemCode3 { get; set; }

        /// <summary>
        ///  Gets or sets the Sector Subject Area Tier 1
        /// </summary>
        public decimal? SectorSubjectAreaTier1 { get; set; }

        /// <summary>
        ///  Gets or sets the Sector Subject Area Tier 2
        /// </summary>
        public decimal? SectorSubjectAreaTier2 { get; set; }

        /// <summary>
        /// Gets or sets the learning delivery categories.
        /// </summary>
        public IReadOnlyCollection<ILARSLearningCategory> Categories
        {
            get => _categories ?? (_categories = Collection.EmptyAndReadOnly<ILARSLearningCategory>());
            set => _categories = value;
        }

        /// <summary>
        /// Gets or sets the learning delivery periods of validity.
        /// </summary>
        public IReadOnlyCollection<ILARSLearningDeliveryValidity> Validities
        {
            get => _validities ?? (_validities = Collection.EmptyAndReadOnly<ILARSLearningDeliveryValidity>());
            set => _validities = value;
        }

        /// <summary>
        /// Gets or sets the framework aims.
        /// </summary>
        public IReadOnlyCollection<ILARSFramework> Frameworks
        {
            get => _frameworks ?? (_frameworks = Collection.EmptyAndReadOnly<ILARSFramework>());
            set => _frameworks = value;
        }

        /// <summary>
        /// Gets or sets the annual values.
        /// </summary>
        public IReadOnlyCollection<ILARSAnnualValue> AnnualValues
        {
            get => _annualValues ?? (_annualValues = Collection.EmptyAndReadOnly<ILARSAnnualValue>());
            set => _annualValues = value;
        }
    }
}
