using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_23Rule :
        IDerivedData_23Rule
    {
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public DerivedData_23Rule(IDateTimeQueryService dateTimeQueryService)
        {
            _dateTimeQueryService = dateTimeQueryService;
        }

        public int? GetLearnersAgeAtStartOfESFContract(
            ILearner learner,
            string conRefNumber)
        {
            if (learner?.DateOfBirthNullable == null)
            {
                return null;
            }

            var delivery = learner.LearningDeliveries
                    ?.OrderByDescending(x => x.LearnStartDate)
                    .FirstOrDefault(ld => ld.LearnAimRef.CaseInsensitiveEquals(TypeOfAim.References.ESFLearnerStartandAssessment)
                             && ld.CompStatus == CompletionState.HasCompleted
                             && ld.ConRefNumber.CaseInsensitiveEquals(conRefNumber));

            if (delivery == null)
            {
                return null;
            }

            return _dateTimeQueryService.AgeAtGivenDate(
                learner.DateOfBirthNullable.Value,
                delivery.LearnStartDate);
        }
    }
}