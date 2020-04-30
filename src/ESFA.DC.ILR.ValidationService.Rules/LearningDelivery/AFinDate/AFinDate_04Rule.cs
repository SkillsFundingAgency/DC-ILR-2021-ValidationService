using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate
{
    public class AFinDate_04Rule : AbstractRule, IRule<ILearner>
    {
        private const string _aFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice;
        private readonly IDerivedData_07Rule _dd07;

        public AFinDate_04Rule(IDerivedData_07Rule dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_04)
        {
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            var learningDeliveries = objectToValidate.LearningDeliveries.Where(Filter) ?? Enumerable.Empty<ILearningDelivery>();

            foreach (var learningDelivery in learningDeliveries)
            {
                var tnpRecord = TNPRecordAfterLearnActEndDate(learningDelivery.LearnActEndDateNullable, learningDelivery.AppFinRecords);

                if (tnpRecord != null)
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearnActEndDateNullable,
                            _aFinType,
                            tnpRecord.AFinDate));
                }
            }
        }

        public bool Filter(ILearningDelivery learningDelivery) =>
               !Exclusion(learningDelivery.ProgTypeNullable)
            && ApprenticeshipFrameworkProgrammeFilter(learningDelivery.ProgTypeNullable, learningDelivery.AimType)
            && LearnActEndDateIsKnown(learningDelivery.LearnActEndDateNullable);

        public bool Exclusion(int? progType) => progType == TypeOfLearningProgramme.ApprenticeshipStandard;

        public bool ApprenticeshipFrameworkProgrammeFilter(int? progType, int aimType) => _dd07.IsApprenticeship(progType) && aimType == AimTypes.ProgrammeAim;

        public bool LearnActEndDateIsKnown(DateTime? learnActEndDate) => learnActEndDate.HasValue;

        public IAppFinRecord TNPRecordAfterLearnActEndDate(DateTime? learnActEndDate, IEnumerable<IAppFinRecord> appFinRecords)
        {
            return
                appFinRecords?
                .FirstOrDefault(f =>
                    f.AFinType.CaseInsensitiveEquals(_aFinType)
                && f.AFinDate > learnActEndDate.Value);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnActEndDate, string aFinType, DateTime aFinDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate)
            };
        }
    }
}
