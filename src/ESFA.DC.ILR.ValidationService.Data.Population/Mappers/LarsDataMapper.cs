using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using LARSValidity = ESFA.DC.ILR.ValidationService.Data.External.LARS.Model.LARSValidity;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class LarsDataMapper : ILarsDataMapper
    {
        public IReadOnlyCollection<ILARSStandard> MapLarsStandards(IReadOnlyCollection<ReferenceDataService.Model.LARS.LARSStandard> larsStandards)
        {
            return larsStandards?.Select(ls => new Data.External.LARS.Model.LARSStandard
            {
                StandardCode = ls.StandardCode,
                EffectiveFrom = ls.EffectiveFrom,
                EffectiveTo = ls.EffectiveTo,
                NotionalEndLevel = ls.NotionalEndLevel,
                StandardSectorCode = ls.StandardSectorCode,
                StandardsFunding = ls.LARSStandardFundings?
                .Select(lsf => new Data.External.LARS.Model.LARSStandardFunding
                {
                    CoreGovContributionCap = lsf.CoreGovContributionCap,
                    EffectiveFrom = lsf.EffectiveFrom,
                    EffectiveTo = lsf.EffectiveTo
                }).ToList()
            }).ToList();
        }

        public IReadOnlyCollection<ILARSStandardValidity> MapLarsStandardValidities(IReadOnlyCollection<ReferenceDataService.Model.LARS.LARSStandard> larsStandards)
        {
            return
                larsStandards?
                .Where(ls => ls.LARSStandardValidities != null)
                .SelectMany(ls => ls.LARSStandardValidities?
                .Select(lsv => new Data.External.LARS.Model.LARSStandardValidity
                {
                    StandardCode = ls.StandardCode,
                    EndDate = lsv.EndDate,
                    StartDate = lsv.StartDate,
                    LastNewStartDate = lsv.LastNewStartDate,
                    ValidityCategory = lsv.ValidityCategory
                })).ToList();
        }

        public IReadOnlyDictionary<string, LearningDelivery> MapLarsLearningDeliveries(IReadOnlyCollection<LARSLearningDelivery> larsLearningDeliveries)
        {
            return
                larsLearningDeliveries?
                .ToDictionary(
                ld => ld.LearnAimRef,
                ld => new LearningDelivery
                {
                    FrameworkCommonComponent = ld.FrameworkCommonComponent,
                    LearnAimRef = ld.LearnAimRef,
                    EffectiveFrom = ld.EffectiveFrom,
                    EffectiveTo = ld.EffectiveTo,
                    LearnAimRefType = ld.LearnAimRefType,
                    EnglPrscID = ld.EnglPrscID,
                    NotionalNVQLevel = ld.NotionalNVQLevel,
                    NotionalNVQLevelv2 = ld.NotionalNVQLevelv2,
                    LearnDirectClassSystemCode1 = new LearnDirectClassSystemCode(ld.LearnDirectClassSystemCode1),
                    LearnDirectClassSystemCode2 = new LearnDirectClassSystemCode(ld.LearnDirectClassSystemCode2),
                    LearnDirectClassSystemCode3 = new LearnDirectClassSystemCode(ld.LearnDirectClassSystemCode3),
                    SectorSubjectAreaTier1 = ld.SectorSubjectAreaTier1,
                    SectorSubjectAreaTier2 = ld.SectorSubjectAreaTier2,
                    AnnualValues = ld.LARSAnnualValues?
                        .Select(av => new AnnualValue()
                        {
                            LearnAimRef = ld.LearnAimRef,
                            BasicSkills = av.BasicSkills,
                            BasicSkillsType = av.BasicSkillsType,
                            FullLevel2EntitlementCategory = av.FullLevel2EntitlementCategory,
                            FullLevel3EntitlementCategory = av.FullLevel3EntitlementCategory,
                            FullLevel3Percent = av.FullLevel3Percent,
                            EffectiveFrom = av.EffectiveFrom,
                            EffectiveTo = av.EffectiveTo,
                        }).ToList(),
                    Categories = ld.LARSLearningDeliveryCategories?
                        .Select(ldc => new LearningDeliveryCategory()
                        {
                            LearnAimRef = ld.LearnAimRef,
                            CategoryRef = ldc.CategoryRef,
                            EffectiveFrom = ldc.EffectiveFrom,
                            EffectiveTo = ldc.EffectiveTo,
                        }).ToList(),
                    Frameworks = ld.LARSFrameworks?
                    .Select(lf => new Framework
                    {
                        FworkCode = lf.FworkCode,
                        ProgType = lf.ProgType,
                        PwayCode = lf.PwayCode,
                        EffectiveFrom = lf.EffectiveFromNullable,
                        EffectiveTo = lf.EffectiveTo,
                        FrameworkAim = new FrameworkAim
                        {
                            FworkCode = lf.FworkCode,
                            ProgType = lf.ProgType,
                            PwayCode = lf.PwayCode,
                            LearnAimRef = ld.LearnAimRef,
                            FrameworkComponentType = lf.LARSFrameworkAim.FrameworkComponentType,
                            EffectiveFrom = lf.LARSFrameworkAim.EffectiveFrom,
                            EffectiveTo = lf.LARSFrameworkAim.EffectiveTo
                        },
                        FrameworkCommonComponents = lf.LARSFrameworkCommonComponents?
                        .Select(lfc => new FrameworkCommonComponent
                        {
                            FworkCode = lf.FworkCode,
                            ProgType = lf.ProgType,
                            PwayCode = lf.PwayCode,
                            CommonComponent = lfc.CommonComponent,
                            EffectiveFrom = lfc.EffectiveFrom,
                            EffectiveTo = lfc.EffectiveTo
                        })
                    }).ToList(),
                    Validities = ld.LARSValidities?
                        .Select(ldc => new LARSValidity
                        {
                            LearnAimRef = ld.LearnAimRef,
                            ValidityCategory = ldc.ValidityCategory,
                            StartDate = ldc.StartDate,
                            EndDate = ldc.EndDate,
                            LastNewStartDate = ldc.LastNewStartDate
                        }).ToList()
                }, StringComparer.OrdinalIgnoreCase);
        }
    }
}
