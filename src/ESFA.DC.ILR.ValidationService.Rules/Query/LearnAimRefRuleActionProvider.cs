using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnAimRefRuleActionProvider :
        IProvideLearnAimRefRuleActions
    {
        /// <summary>
        /// The branch actions
        /// </summary>
        private readonly IDictionary<int, IReadOnlyCollection<Func<ILearningDelivery, ILearner, BranchResult>>> _branchActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRefRuleActionProvider " /> class.
        /// </summary>
        /// <param name="commonOperations">The common operations.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="derivedData11">The derived data 11 (rule).</param>
        public LearnAimRefRuleActionProvider(
            IProvideRuleCommonOperations commonOperations,
            IDerivedData_11Rule derivedData11)
        {
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));
            It.IsNull(derivedData11)
                .AsGuard<ArgumentNullException>(nameof(derivedData11));

            Check = commonOperations;
            DerivedData11 = derivedData11;

            _branchActions = new Dictionary<int, IReadOnlyCollection<Func<ILearningDelivery, ILearner, BranchResult>>>
            {
                [TypeOfFunding.AdultSkills] = PackageRoutines(IsQualifyingCategoryOLASS, IsQualifyingCategoryUnemployed, IsQualifyingCategoryApprenticeship, IsQualifyingCategoryAdultSkills),
                [TypeOfFunding.ApprenticeshipsFrom1May2017] = PackageRoutines(IsQualifyingCategoryApprenticeship, IsQualifyingCategoryApprencticeshipAny),
                [TypeOfFunding.OtherAdult] = PackageRoutines(IsQualifyingCategoryOtherFundingAny),
                [TypeOfFunding.NotFundedByESFA] = PackageRoutines(IsQualifyingCategoryAdvancedLearnerLoan, IsQualifyingCategoryOtherFundingAny),
                [TypeOfFunding.Age16To19ExcludingApprenticeships] = PackageRoutines(IsQualifyingCategory16To19EFA),
                [TypeOfFunding.Other16To19] = PackageRoutines(IsQualifyingCategory16To19EFA),
                [TypeOfFunding.EuropeanSocialFund] = PackageRoutines(IsQualifyingCategoryESF),
                [TypeOfFunding.CommunityLearning] = PackageRoutines(IsQualifyingCategoryCommunityLearning),
            };
        }

        /// <summary>
        /// Gets the apprenticeship minimum start.
        /// </summary>
        public static DateTime ApprenticeshipMinimumStart => new DateTime(2011, 08, 01);

        /// <summary>
        /// Gets the unemployed maximum start.
        /// </summary>
        public static DateTime UnemployedMaximumStart => new DateTime(2016, 08, 01);

        /// <summary>
        /// Gets the (common operations provider) check.
        /// </summary>
        protected IProvideRuleCommonOperations Check { get; }

        /// <summary>
        /// Gets the derived data 11 (rule)
        /// </summary>
        protected IDerivedData_11Rule DerivedData11 { get; }

        /// <summary>
        /// Packages the routines.
        /// </summary>
        /// <param name="routines">The routines.</param>
        /// <returns>a packaged collection of (funding model) routines</returns>
        public IReadOnlyCollection<Func<ILearningDelivery, ILearner, BranchResult>> PackageRoutines(params Func<ILearningDelivery, ILearner, BranchResult>[] routines) =>
            routines.AsSafeReadOnlyList();

        /// <summary>
        /// Gets the routines.
        /// </summary>
        /// <param name="forFundingModel">For funding model.</param>
        /// <returns>a collection of routines for the funding model</returns>
        public IReadOnlyCollection<Func<ILearningDelivery, ILearner, BranchResult>> GetRoutines(int forFundingModel) =>
            _branchActions.ContainsKey(forFundingModel)
                ? _branchActions[forFundingModel]
                : PackageRoutines(null);

        /// <summary>
        /// Determines whether [in receipt of benefits at start] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="employments">The employments.</param>
        /// <returns>
        ///   <c>true</c> if [in receipt of benefits at start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InReceiptOfBenefitsAtStart(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> employments) =>
            DerivedData11.IsAdultFundedOnBenefitsAtStartOfAim(delivery, employments);

        /// <summary>
        /// Determines whether [is valid olass (category)] [the specified delivery].
        /// this is an FM35 routine
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid olass (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryOLASS(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                    !Check.IsRestart(delivery)
                    && Check.IsLearnerInCustody(delivery),
                TypeOfLARSValidity.OLASSAdult);

        /// <summary>
        /// Determines whether [is valid unemployed (category)] [the specified delivery].
        /// this is an FM35 routine
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid unemployed (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryUnemployed(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                    !Check.IsRestart(delivery)
                    && !Check.IsLearnerInCustody(delivery)
                    && !Check.InApprenticeship(delivery)
                    && Check.HasQualifyingStart(delivery, DateTime.MinValue, UnemployedMaximumStart)
                    && InReceiptOfBenefitsAtStart(delivery, learner?.LearnerEmploymentStatuses.AsSafeReadOnlyList()),
                TypeOfLARSValidity.Unemployed);

        /// <summary>
        /// Determines whether [is valid adult skills (category)] [the specified delivery].
        /// this is an FM35 routine
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid adult skills (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryAdultSkills(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                    !Check.IsRestart(delivery)
                    && !Check.IsLearnerInCustody(delivery)
                    && !Check.InApprenticeship(delivery),
                TypeOfLARSValidity.AdultSkills);

        /// <summary>
        /// Determines whether [is valid apprenticeship (category)] [the specified delivery].
        /// this is an FM35 and FM36 routine
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid apprenticeship (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryApprenticeship(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                    !Check.IsRestart(delivery)
                    && !Check.IsStandardApprencticeship(delivery)
                    && Check.InApprenticeship(delivery)
                    && Check.IsComponentOfAProgram(delivery)
                    && Check.HasQualifyingStart(delivery, ApprenticeshipMinimumStart, DateTime.MaxValue),
                TypeOfLARSValidity.Apprenticeships);

        public BranchResult IsQualifyingCategoryApprencticeshipAny(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                !Check.IsRestart(delivery)
                    && !Check.IsAdvancedLearnerLoan(delivery)
                    && Check.IsStandardApprencticeship(delivery),
                TypeOfLARSValidity.Any);

        public BranchResult IsQualifyingCategoryOtherFundingAny(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                    !Check.IsRestart(delivery)
                    && !Check.IsAdvancedLearnerLoan(delivery),
                TypeOfLARSValidity.Any);

        /// <summary>
        /// Determines whether [is valid advanced learner loan (category)] [the specified delivery].
        /// this is an FM99 routine
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid advanced learner loan] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryAdvancedLearnerLoan(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                    !Check.IsRestart(delivery)
                    && Check.IsAdvancedLearnerLoan(delivery),
                TypeOfLARSValidity.AdvancedLearnerLoan);

        /// <summary>
        /// Determines whether [is valid 16 to 19 efa (category)] [the specified delivery].
        /// this is an FM25 and FM82 routine
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid 16 to 19 efa (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategory16To19EFA(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                    !Check.IsRestart(delivery),
                TypeOfLARSValidity.EFA16To19);

        /// <summary>
        /// Determines whether [is valid community learning (category)] [the specified delivery].
        /// this is an FM10 routine
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid community learning (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryCommunityLearning(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                    !Check.IsRestart(delivery),
                TypeOfLARSValidity.CommunityLearning);

        /// <summary>
        /// Determines whether [is valid esf (category)] [the specified delivery].
        /// this is an FM70 routine
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid esf (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryESF(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                    !Check.IsRestart(delivery),
                TypeOfLARSValidity.EuropeanSocialFund);

        /// <summary>
        /// Gets the branching result for (this delivery and learner)
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="andLearner">and learner.</param>
        /// <returns>
        /// a branching result
        /// </returns>
        public IBranchResult GetBranchingResultFor(ILearningDelivery thisDelivery, ILearner andLearner)
        {
            It.IsNull(thisDelivery)
                .AsGuard<ArgumentNullException>(nameof(thisDelivery));
            It.IsNull(andLearner)
                .AsGuard<ArgumentNullException>(nameof(andLearner));

            foreach (var doActionFor in GetRoutines(thisDelivery.FundModel))
            {
                var branch = doActionFor(thisDelivery, andLearner);
                if (branch.Passed)
                {
                    return branch;
                }
            }

            return BranchResult.Create(false, string.Empty);
        }

        public class BranchResult : IBranchResult
        {
            public BranchResult(bool result, string category)
            {
                Passed = result;
                Category = category;
            }

            /// <summary>
            /// Gets a value indicating whether [out of scope].
            /// </summary>
            public bool OutOfScope => !Passed && It.IsEmpty(Category);

            /// <summary>
            /// Gets the category.
            /// </summary>
            public string Category { get; }

            /// <summary>
            /// Gets a value indicating whether this <see cref="BranchResult"/> is passed.
            /// </summary>
            public bool Passed { get; }

            /// <summary>
            /// Creates the specified result.
            /// </summary>
            /// <param name="filterResult">if set to <c>true</c> [filter result].</param>
            /// <param name="category">The category.</param>
            /// <returns>a new branch result</returns>
            public static BranchResult Create(bool filterResult, string category)
                => new BranchResult(filterResult, category);
        }
    }
}
