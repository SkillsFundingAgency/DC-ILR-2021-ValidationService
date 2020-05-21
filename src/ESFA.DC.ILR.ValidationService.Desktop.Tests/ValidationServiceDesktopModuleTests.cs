using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Desktop.Modules;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutCollDate;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutEndDate;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutStartDate;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutType;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutULN;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using ESFA.DC.ILR.ValidationService.Rules.HE;
using ESFA.DC.ILR.ValidationService.Rules.HE.DOMICILE;
using ESFA.DC.ILR.ValidationService.Rules.HE.ELQ;
using ESFA.DC.ILR.ValidationService.Rules.HE.FinancialSupport.FINTYPE;
using ESFA.DC.ILR.ValidationService.Rules.HE.FUNDCOMP;
using ESFA.DC.ILR.ValidationService.Rules.HE.FUNDLEV;
using ESFA.DC.ILR.ValidationService.Rules.HE.GROSSFEE;
using ESFA.DC.ILR.ValidationService.Rules.HE.HEPostcode;
using ESFA.DC.ILR.ValidationService.Rules.HE.LearningDeliveryHE;
using ESFA.DC.ILR.ValidationService.Rules.HE.MODESTUD;
using ESFA.DC.ILR.ValidationService.Rules.HE.MSTUFEE;
using ESFA.DC.ILR.ValidationService.Rules.HE.NETFEE;
using ESFA.DC.ILR.ValidationService.Rules.HE.NUMHUS;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCFLDCS;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCOLAB;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCSLDCS;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCTLDCS;
using ESFA.DC.ILR.ValidationService.Rules.HE.QUALENT3;
using ESFA.DC.ILR.ValidationService.Rules.HE.SEC;
using ESFA.DC.ILR.ValidationService.Rules.HE.SOC2000;
using ESFA.DC.ILR.ValidationService.Rules.HE.SPECFEE;
using ESFA.DC.ILR.ValidationService.Rules.HE.SSN;
using ESFA.DC.ILR.ValidationService.Rules.HE.STULOAD;
using ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM;
using ESFA.DC.ILR.ValidationService.Rules.HE.TYPEYR;
using ESFA.DC.ILR.ValidationService.Rules.HE.UCASAPPID;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Accom;
using ESFA.DC.ILR.ValidationService.Rules.Learner.AddLine1;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ALSCost;
using ESFA.DC.ILR.ValidationService.Rules.Learner.CampId;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Ethnicity;
using ESFA.DC.ILR.ValidationService.Rules.Learner.FamilyName;
using ESFA.DC.ILR.ValidationService.Rules.Learner.GivenNames;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDCat;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb;
using ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade;
using ESFA.DC.ILR.ValidationService.Rules.Learner.NiNumber;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanEEPHours;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PMUKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Postcode;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PostcodePrior;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PrevUKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ProvSpecLearnMonOccur;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Sex;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimSeqNumber;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ConRefNumber;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EmpOutcome;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EPAOrgID;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FworkCode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnActEndDate;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateFrom;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateTo;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnPlanEndDate;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OtherFundAdj;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PartnerUKPRN;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PHours;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PriorLearnFundAdj;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProvSpecDelMonOccur;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PwayCode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.StdCode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.SWSupAimId;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEmpId;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEndDate;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceMode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Entity;
using ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Header;
using ESFA.DC.ILR.ValidationService.Rules.Message.UKPRN;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Desktop.Tests
{
    public class ValidationServiceDesktopModuleTests
    {
        [Fact]
        public void MessageValidation_Resolve()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<ValidationServiceDesktopModule>();

            containerBuilder.RegisterCommonServiceStubs();

            var container = containerBuilder.Build();

            var rulesetOrchestrationService = container.Resolve<IPreValidationOrchestrationService>();

            rulesetOrchestrationService.Should().NotBeNull();
        }

        [Fact]
        public void MessageRules_Resolve()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<ValidationServiceDesktopModule>();

            containerBuilder.RegisterCommonServiceStubs();

            var container = containerBuilder.Build();

            var messageRules = container.Resolve<IEnumerable<IRule<IMessage>>>();

            messageRules.Should().NotBeNull();

            var rules = new[]
            {
                typeof(Entity_1Rule),
                typeof(Header_2Rule),
                typeof(Header_3Rule),
                typeof(R06Rule),
                typeof(R59Rule),
                typeof(R69Rule),
                typeof(R71Rule),
                typeof(R85Rule),
                typeof(R107Rule),
                typeof(R108Rule),
                typeof(UKPRN_03Rule)
            };

            foreach (var rule in rules)
            {
                messageRules.Should().Contain(x => x.GetType() == rule);
            }

            messageRules.Should().HaveCount(rules.Length);
        }

        [Fact]
        public void LearnerRules_Resolve()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<ValidationServiceDesktopModule>();

            containerBuilder.RegisterCommonServiceStubs();

            var container = containerBuilder.Build();

            var messageRules = container.Resolve<IEnumerable<IRule<ILearner>>>();

            messageRules.Should().NotBeNull();

            var rules = new[]
            {
                typeof(Accom_01Rule),
                typeof(AchDate_02Rule),
                typeof(AchDate_03Rule),
                typeof(AchDate_04Rule),
                typeof(AchDate_05Rule),
                typeof(AchDate_07Rule),
                typeof(AchDate_08Rule),
                typeof(AchDate_09Rule),
                typeof(AchDate_10Rule),
                typeof(AchDate_11Rule),
                typeof(AddHours_01Rule),
                typeof(AddHours_02Rule),
                typeof(AddHours_03Rule),
                typeof(AddHours_04Rule),
                typeof(AddHours_05Rule),
                typeof(AddHours_06Rule),
                typeof(AddLine1_03Rule),
                typeof(AFinDate_03Rule),
                typeof(AFinDate_04Rule),
                typeof(AFinDate_05Rule),
                typeof(AFinDate_06Rule),
                typeof(AFinDate_07Rule),
                typeof(AFinDate_08Rule),
                typeof(AFinDate_09Rule),
                typeof(AFinDate_12Rule),
                typeof(AFinDate_13Rule),
                typeof(AFinType_01Rule),
                typeof(AFinType_02Rule),
                typeof(AFinType_04Rule),
                typeof(AFinType_07Rule),
                typeof(AFinType_08Rule),
                typeof(AFinType_10Rule),
                typeof(AFinType_11Rule),
                typeof(AFinType_12Rule),
                typeof(AFinType_13Rule),
                typeof(AFinType_14Rule),
                typeof(AimSeqNumber_02Rule),
                typeof(AimType_01Rule),
                typeof(AimType_05Rule),
                typeof(AimType_07Rule),
                typeof(ALSCost_02Rule),
                typeof(CampId_01Rule),
                typeof(CampId_02Rule),
                typeof(CompStatus_01Rule),
                typeof(CompStatus_02Rule),
                typeof(CompStatus_03Rule),
                typeof(CompStatus_04Rule),
                typeof(CompStatus_05Rule),
                typeof(CompStatus_06Rule),
                typeof(CompStatus_07Rule),
                typeof(ConRefNumber_01Rule),
                typeof(ConRefNumber_03Rule),
                typeof(ContPrefType_01Rule),
                typeof(ContPrefType_02Rule),
                typeof(ContPrefType_03Rule),
                typeof(ContPrefType_04Rule),
                typeof(ContPrefType_05Rule),
                typeof(ContPrefType_06Rule),
                typeof(ContPrefType_07Rule),
                typeof(DateEmpStatApp_01Rule),
                typeof(DateEmpStatApp_02Rule),
                typeof(DateOfBirth_01Rule),
                typeof(DateOfBirth_02Rule),
                typeof(DateOfBirth_04Rule),
                typeof(DateOfBirth_05Rule),
                typeof(DateOfBirth_06Rule),
                typeof(DateOfBirth_07Rule),
                typeof(DateOfBirth_12Rule),
                typeof(DateOfBirth_13Rule),
                typeof(DateOfBirth_14Rule),
                typeof(DateOfBirth_20Rule),
                typeof(DateOfBirth_23Rule),
                typeof(DateOfBirth_24Rule),
                typeof(DateOfBirth_25Rule),
                typeof(DateOfBirth_26Rule),
                typeof(DateOfBirth_27Rule),
                typeof(DateOfBirth_30Rule),
                typeof(DateOfBirth_32Rule),
                typeof(DateOfBirth_34Rule),
                typeof(DateOfBirth_35Rule),
                typeof(DateOfBirth_36Rule),
                typeof(DateOfBirth_37Rule),
                typeof(DateOfBirth_38Rule),
                typeof(DateOfBirth_40Rule),
                typeof(DateOfBirth_41Rule),
                typeof(DateOfBirth_43Rule),
                typeof(DateOfBirth_44Rule),
                typeof(DateOfBirth_46Rule),
                typeof(DateOfBirth_47Rule),
                typeof(DateOfBirth_48Rule),
                typeof(DateOfBirth_49Rule),
                typeof(DateOfBirth_50Rule),
                typeof(DateOfBirth_51Rule),
                typeof(DateOfBirth_52Rule),
                typeof(DateOfBirth_53Rule),
                typeof(DateOfBirth_55Rule),
                typeof(DelLocPostCode_03Rule),
                typeof(DelLocPostCode_11Rule),
                typeof(DelLocPostCode_14Rule),
                typeof(DelLocPostCode_15Rule),
                typeof(DelLocPostCode_16Rule),
                typeof(DelLocPostCode_17Rule),
                typeof(DelLocPostCode_18Rule),
                typeof(DOMICILE_01Rule),
                typeof(DOMICILE_02Rule),
                typeof(Ethnicity_01Rule),
                typeof(ELQ_01Rule),
                typeof(ELQ_02Rule),
                typeof(EngGrade_01Rule),
                typeof(EngGrade_02Rule),
                typeof(EngGrade_03Rule),
                typeof(EngGrade_04Rule),
                typeof(EmpId_01Rule),
                typeof(EmpId_02Rule),
                typeof(EmpId_10Rule),
                typeof(EmpId_13Rule),
                typeof(EmpId_14Rule),
                typeof(EmpOutcome_01Rule),
                typeof(EmpOutcome_02Rule),
                typeof(EmpOutcome_03Rule),
                typeof(EmpStat_01Rule),
                typeof(EmpStat_02Rule),
                typeof(EmpStat_04Rule),
                typeof(EmpStat_05Rule),
                typeof(EmpStat_06Rule),
                typeof(EmpStat_07Rule),
                typeof(EmpStat_08Rule),
                typeof(EmpStat_09Rule),
                typeof(EmpStat_10Rule),
                typeof(EmpStat_11Rule),
                typeof(EmpStat_12Rule),
                typeof(EmpStat_14Rule),
                typeof(EmpStat_15Rule),
                typeof(EmpStat_17Rule),
                typeof(EmpStat_18Rule),
                typeof(EmpStat_19Rule),
                typeof(EmpStat_20Rule),
                typeof(EPAOrgID_01Rule),
                typeof(EPAOrgID_02Rule),
                typeof(ESMType_01Rule),
                typeof(ESMType_02Rule),
                typeof(ESMType_05Rule),
                typeof(ESMType_07Rule),
                typeof(ESMType_08Rule),
                typeof(ESMType_09Rule),
                typeof(ESMType_10Rule),
                typeof(ESMType_11Rule),
                typeof(ESMType_12Rule),
                typeof(ESMType_13Rule),
                typeof(ESMType_14Rule),
                typeof(ESMType_15Rule),
                typeof(EPAOrgID_03Rule),
                typeof(FamilyName_01Rule),
                typeof(FamilyName_02Rule),
                typeof(FamilyName_04Rule),
                typeof(FINTYPE_01Rule),
                typeof(FINTYPE_02Rule),
                typeof(FUNDLEV_01Rule),
                typeof(FUNDLEV_02Rule),
                typeof(FUNDLEV_03Rule),
                typeof(FUNDCOMP_01Rule),
                typeof(FundModel_01Rule),
                typeof(FundModel_03Rule),
                typeof(FundModel_04Rule),
                typeof(FundModel_05Rule),
                typeof(FundModel_06Rule),
                typeof(FundModel_07Rule),
                typeof(FundModel_08Rule),
                typeof(FundModel_09Rule),
                typeof(FundModel_10Rule),
                typeof(FworkCode_01Rule),
                typeof(FworkCode_02Rule),
                typeof(FworkCode_05Rule),
                typeof(GivenNames_01Rule),
                typeof(GivenNames_02Rule),
                typeof(GivenNames_04Rule),
                typeof(GROSSFEE_01Rule),
                typeof(GROSSFEE_02Rule),
                typeof(GROSSFEE_03Rule),
                typeof(HEPostCode_01Rule),
                typeof(HEPostCode_02Rule),
                typeof(LearnActEndDate_01Rule),
                typeof(LearnActEndDate_04Rule),
                typeof(LearnAimRef_01Rule),
                typeof(LearnAimRef_29Rule),
                typeof(LearnAimRef_30Rule),
                typeof(LearnAimRef_55Rule),
                typeof(LearnAimRef_56Rule),
                typeof(LearnAimRef_57Rule),
                typeof(LearnAimRef_71Rule),
                typeof(LearnAimRef_72Rule),
                typeof(LearnAimRef_73Rule),
                typeof(LearnAimRef_78Rule),
                typeof(LearnAimRef_79Rule),
                typeof(LearnAimRef_80Rule),
                typeof(LearnAimRef_81Rule),
                typeof(LearnAimRef_84Rule),
                typeof(LearnAimRef_85Rule),
                typeof(LearnAimRef_86Rule),
                typeof(LearnAimRef_87Rule),
                typeof(LearnAimRef_88Rule),
                typeof(LearnAimRef_89Rule),
                typeof(LearnDelFAMDateFrom_01Rule),
                typeof(LearnDelFAMDateFrom_02Rule),
                typeof(LearnDelFAMDateFrom_03Rule),
                typeof(LearnDelFAMDateFrom_04Rule),
                typeof(LearnDelFAMDateTo_01Rule),
                typeof(LearnDelFAMDateTo_02Rule),
                typeof(LearnDelFAMDateTo_03Rule),
                typeof(LearnDelFAMDateTo_04Rule),
                typeof(LearnDelFAMDateTo_05Rule),
                typeof(LearnDelFAMType_01Rule),
                typeof(LearnDelFAMType_02Rule),
                typeof(LearnDelFAMType_03Rule),
                typeof(LearnDelFAMType_04Rule),
                typeof(LearnDelFAMType_06Rule),
                typeof(LearnDelFAMType_07Rule),
                typeof(LearnDelFAMType_08Rule),
                typeof(LearnDelFAMType_09Rule),
                typeof(LearnDelFAMType_14Rule),
                typeof(LearnDelFAMType_15Rule),
                typeof(LearnDelFAMType_16Rule),
                typeof(LearnDelFAMType_18Rule),
                typeof(LearnDelFAMType_22Rule),
                typeof(LearnDelFAMType_27Rule),
                typeof(LearnDelFAMType_31Rule),
                typeof(LearnDelFAMType_35Rule),
                typeof(LearnDelFAMType_36Rule),
                typeof(LearnDelFAMType_38Rule),
                typeof(LearnDelFAMType_39Rule),
                typeof(LearnDelFAMType_40Rule),
                typeof(LearnDelFAMType_44Rule),
                typeof(LearnDelFAMType_45Rule),
                typeof(LearnDelFAMType_46Rule),
                typeof(LearnDelFAMType_48Rule),
                typeof(LearnDelFAMType_50Rule),
                typeof(LearnDelFAMType_53Rule),
                typeof(LearnDelFAMType_54Rule),
                typeof(LearnDelFAMType_56Rule),
                typeof(LearnDelFAMType_57Rule),
                typeof(LearnDelFAMType_59Rule),
                typeof(LearnDelFAMType_60Rule),
                typeof(LearnDelFAMType_61Rule),
                typeof(LearnDelFAMType_63Rule),
                typeof(LearnDelFAMType_64Rule),
                typeof(LearnDelFAMType_65Rule),
                typeof(LearnDelFAMType_66Rule),
                typeof(LearnDelFAMType_67Rule),
                typeof(LearnDelFAMType_68Rule),
                typeof(LearnDelFAMType_69Rule),
                typeof(LearnDelFAMType_70Rule),
                typeof(LearnDelFAMType_71Rule),
                typeof(LearnDelFAMType_72Rule),
                typeof(LearnDelFAMType_73Rule),
                typeof(LearnDelFAMType_74Rule),
                typeof(LearnDelFAMType_75Rule),
                typeof(LearnDelFAMType_76Rule),
                typeof(LearnDelFAMType_77Rule),
                typeof(LearnDelFAMType_78Rule),
                typeof(LearnDelFAMType_79Rule),
                typeof(LearnDelFAMType_80Rule),
                typeof(LearnDelFAMType_81Rule),
                typeof(LearningDeliveryHE_02Rule),
                typeof(LearningDeliveryHE_03Rule),
                typeof(LearningDeliveryHE_06Rule),
                typeof(LearningDeliveryHE_07Rule),
                typeof(LearningDeliveryHE_08Rule),
                typeof(LearnerHE_02Rule),
                typeof(LearnFAMType_01Rule),
                typeof(LearnFAMType_03Rule),
                typeof(LearnFAMType_09Rule),
                typeof(LearnFAMType_10Rule),
                typeof(LearnFAMType_11Rule),
                typeof(LearnFAMType_14Rule),
                typeof(LearnFAMType_15Rule),
                typeof(LearnFAMType_16Rule),
                typeof(LearnStartDate_02Rule),
                typeof(LearnStartDate_03Rule),
                typeof(LearnStartDate_05Rule),
                typeof(LearnStartDate_06Rule),
                typeof(LearnStartDate_07Rule),
                typeof(LearnStartDate_12Rule),
                typeof(LearnStartDate_13Rule),
                typeof(LearnStartDate_14Rule),
                typeof(LearnStartDate_15Rule),
                typeof(LearnStartDate_16Rule),
                typeof(LearnStartDate_17Rule),
                typeof(LearnPlanEndDate_02Rule),
                typeof(LearnPlanEndDate_03Rule),
                typeof(LSDPostcode_01Rule),
                typeof(LSDPostcode_02Rule),
                typeof(LSDPostcode_03Rule),
                typeof(LLDDCat_01Rule),
                typeof(LLDDCat_02Rule),
                typeof(LLDDHealthProb_01Rule),
                typeof(LLDDHealthProb_04Rule),
                typeof(LLDDHealthProb_06Rule),
                typeof(LLDDHealthProb_07Rule),
                typeof(MathGrade_01Rule),
                typeof(MathGrade_02Rule),
                typeof(MathGrade_03Rule),
                typeof(MathGrade_04Rule),
                typeof(MODESTUD_01Rule),
                typeof(MODESTUD_03Rule),
                typeof(MSTUFEE_01Rule),
                typeof(MSTUFEE_02Rule),
                typeof(MSTUFEE_03Rule),
                typeof(MSTUFEE_04Rule),
                typeof(MSTUFEE_05Rule),
                typeof(NETFEE_01Rule),
                typeof(NETFEE_02Rule),
                typeof(NINumber_01Rule),
                typeof(NINumber_02Rule),
                typeof(NUMHUS_01Rule),
                typeof(OrigLearnStartDate_01Rule),
                typeof(OrigLearnStartDate_02Rule),
                typeof(OrigLearnStartDate_03Rule),
                typeof(OrigLearnStartDate_04Rule),
                typeof(OrigLearnStartDate_05Rule),
                typeof(OrigLearnStartDate_06Rule),
                typeof(OrigLearnStartDate_07Rule),
                typeof(OrigLearnStartDate_08Rule),
                typeof(OrigLearnStartDate_09Rule),
                typeof(OtherFundAdj_01Rule),
                typeof(Outcome_01Rule),
                typeof(Outcome_04Rule),
                typeof(Outcome_05Rule),
                typeof(Outcome_07Rule),
                typeof(Outcome_08Rule),
                typeof(Outcome_09Rule),
                typeof(OutGrade_01Rule),
                typeof(OutGrade_04Rule),
                typeof(OutGrade_05Rule),
                typeof(OutGrade_06Rule),
                typeof(OutGrade_07Rule),
                typeof(PartnerUKPRN_01Rule),
                typeof(PartnerUKPRN_02Rule),
                typeof(PartnerUKPRN_03Rule),
                typeof(PCFLDCS_01Rule),
                typeof(PCFLDCS_02Rule),
                typeof(PCFLDCS_03Rule),
                typeof(PCSLDCS_01Rule),
                typeof(PCSLDCS_02Rule),
                typeof(PCTLDCS_01Rule),
                typeof(PCTLDCS_02Rule),
                typeof(PCOLAB_01Rule),
                typeof(PHours_01Rule),
                typeof(PlanEEPHours_01Rule),
                typeof(PlanLearnHours_01Rule),
                typeof(PlanLearnHours_02Rule),
                typeof(PlanLearnHours_03Rule),
                typeof(PlanLearnHours_04Rule),
                typeof(PlanLearnHours_05Rule),
                typeof(PMUKPRN_01Rule),
                typeof(Postcode_14Rule),
                typeof(Postcode_15Rule),
                typeof(PostcodePrior_01Rule),
                typeof(PostcodePrior_02Rule),
                typeof(PrevUKPRN_01Rule),
                typeof(PrimaryLLDD_01Rule),
                typeof(PrimaryLLDD_02Rule),
                typeof(PrimaryLLDD_03Rule),
                typeof(PrimaryLLDD_04Rule),
                typeof(PriorAttain_01Rule),
                typeof(PriorAttain_03Rule),
                typeof(PriorAttain_04Rule),
                typeof(PriorAttain_06Rule),
                typeof(PriorAttain_07Rule),
                typeof(PriorLearnFundAdj_01Rule),
                typeof(ProgType_01Rule),
                typeof(ProgType_02Rule),
                typeof(ProgType_03Rule),
                typeof(ProgType_06Rule),
                typeof(ProgType_07Rule),
                typeof(ProgType_08Rule),
                typeof(ProgType_12Rule),
                typeof(ProgType_13Rule),
                typeof(ProgType_14Rule),
                typeof(ProvSpecDelMonOccur_01Rule),
                typeof(PwayCode_02Rule),
                typeof(PwayCode_03Rule),
                typeof(ProvSpecLearnMonOccur_01Rule),
                typeof(QUALENT3_01Rule),
                typeof(QUALENT3_02Rule),
                typeof(QUALENT3_03Rule),
                typeof(R07Rule),
                typeof(R20Rule),
                typeof(R30Rule),
                typeof(R31Rule),
                typeof(R43Rule),
                typeof(R45Rule),
                typeof(R47Rule),
                typeof(R49Rule),
                typeof(R50Rule),
                typeof(R51Rule),
                typeof(R52Rule),
                typeof(R56Rule),
                typeof(R58Rule),
                typeof(R61Rule),
                typeof(R63Rule),
                typeof(R64Rule),
                typeof(R66Rule),
                typeof(R67Rule),
                typeof(R68Rule),
                typeof(R72Rule),
                typeof(R73Rule),
                typeof(R89Rule),
                typeof(R90Rule),
                typeof(R91Rule),
                typeof(R92Rule),
                typeof(R96Rule),
                typeof(R97Rule),
                typeof(R99Rule),
                typeof(R100Rule),
                typeof(R101Rule),
                typeof(R102Rule),
                typeof(R104Rule),
                typeof(R105Rule),
                typeof(R106Rule),
                typeof(R110Rule),
                typeof(R112Rule),
                typeof(R113Rule),
                typeof(R114Rule),
                typeof(R117Rule),
                typeof(R118Rule),
                typeof(R119Rule),
                typeof(R121Rule),
                typeof(R122Rule),
                typeof(R123Rule),
                typeof(R124Rule),
                typeof(Sex_01Rule),
                typeof(SEC_01Rule),
                typeof(SEC_02Rule),
                typeof(SPECFEE_01Rule),
                typeof(SPECFEE_02Rule),
                typeof(SSN_02Rule),
                typeof(SOC2000_02Rule),
                typeof(SOC2000_03Rule),
                typeof(SOC2000_04Rule),
                typeof(SWSupAimId_01Rule),
                typeof(StdCode_01Rule),
                typeof(StdCode_02Rule),
                typeof(StdCode_03Rule),
                typeof(STULOAD_04Rule),
                typeof(TTACCOM_01Rule),
                typeof(TTACCOM_02Rule),
                typeof(TTACCOM_04Rule),
                typeof(TYPEYR_01Rule),
                typeof(TYPEYR_02Rule),
                typeof(UCASAPPID_01Rule),
                typeof(UKPRN_05Rule),
                typeof(UKPRN_06Rule),
                typeof(UKPRN_08Rule),
                typeof(UKPRN_09Rule),
                typeof(UKPRN_10Rule),
                typeof(UKPRN_11Rule),
                typeof(UKPRN_12Rule),
                typeof(UKPRN_13Rule),
                typeof(UKPRN_14Rule),
                typeof(UKPRN_15Rule),
                typeof(UKPRN_16Rule),
                typeof(UKPRN_17Rule),
                typeof(UKPRN_18Rule),
                typeof(UKPRN_19Rule),
                typeof(UKPRN_20Rule),
                typeof(ULN_02Rule),
                typeof(ULN_03Rule),
                typeof(ULN_04Rule),
                typeof(ULN_05Rule),
                typeof(ULN_06Rule),
                typeof(ULN_07Rule),
                typeof(ULN_09Rule),
                typeof(ULN_10Rule),
                typeof(ULN_11Rule),
                typeof(ULN_12Rule),
                typeof(WithdrawReason_02Rule),
                typeof(WithdrawReason_03Rule),
                typeof(WithdrawReason_04Rule),
                typeof(WithdrawReason_05Rule),
                typeof(WithdrawReason_06Rule),
                typeof(WorkPlaceEndDate_01Rule),
                typeof(WorkPlaceEndDate_02Rule),
                typeof(WorkPlaceStartDate_01Rule),
                typeof(WorkPlaceStartDate_02Rule),
                typeof(WorkPlaceStartDate_03Rule),
                typeof(WorkPlaceStartDate_04Rule),
                typeof(WorkPlaceMode_01Rule),
                typeof(WorkPlaceEmpId_01Rule),
                typeof(WorkPlaceEmpId_02Rule),
                typeof(WorkPlaceEmpId_03Rule),
                typeof(WorkPlaceEmpId_04Rule),
                typeof(WorkPlaceEmpId_05Rule),
            };

            foreach (var rule in rules)
            {
                messageRules.Should().Contain(x => x.GetType() == rule);
            }

            messageRules.Should().HaveCount(rules.Length);
        }

        [Fact]
        public void LearnerDestinationAndProgressionRules_Resolve()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<ValidationServiceDesktopModule>();

            containerBuilder.RegisterCommonServiceStubs();

            var container = containerBuilder.Build();

            var messageRules = container.Resolve<IEnumerable<IRule<ILearnerDestinationAndProgression>>>();

            messageRules.Should().NotBeNull();

            var rules = new[]
            {
                typeof(OutCollDate_01Rule),
                typeof(OutCollDate_02Rule),
                typeof(OutEndDate_01Rule),
                typeof(OutStartDate_01Rule),
                typeof(OutStartDate_02Rule),
                typeof(OutType_01Rule),
                typeof(OutType_02Rule),
                typeof(OutType_03Rule),
                typeof(OutType_04Rule),
                typeof(OutType_05Rule),
                typeof(OutULN_01Rule),
                typeof(OutULN_02Rule)
            };

            foreach (var rule in rules)
            {
                messageRules.Should().Contain(x => x.GetType() == rule);
            }

            messageRules.Should().HaveCount(rules.Length);
        }
    }
}
