using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode
{
    public class LSDPostcode_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPostcodesDataService _postcodesDataService;
        private readonly DateTime _firstAugust2019 = new DateTime(2019, 08, 01);

        private readonly IEnumerable<int> _fundModels = new HashSet<int>()
        {
            TypeOfFunding.AdultSkills
        };

        public LSDPostcode_01Rule(IPostcodesDataService postcodesDataService, IValidationErrorHandler validationErrorHandler)
            :base(validationErrorHandler, RuleNameConstants.LSDPostcode_01)
        {
            _postcodesDataService = postcodesDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(learningDelivery.ProgTypeNullable, learningDelivery.FundModel, learningDelivery.LSDPostcode, learningDelivery.LearnStartDate))
                    {
                        HandleValidationError( 
                                 objectToValidate.LearnRefNumber,
                                 learningDelivery.AimSeqNumber,
                                 BuildErrorMessageParameters(learningDelivery.LearnPlanEndDate,
                                                             learningDelivery.FundModel, 
                                                             learningDelivery.LSDPostcode));
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(int? progType, int fundModel, string lsdPostcode, DateTime learnStartDate)
        {
            return ProTypeConditionMet(progType)
                && FundModelConditionMet(fundModel)
                && PostCodeNullConditionMet(lsdPostcode)
                && TemporaryPostcodeConditionMet(lsdPostcode)                
                && LearnStartDateConditionMet(learnStartDate)
                && ValidPostcodeConditionMet(lsdPostcode);
        }

        public bool ProTypeConditionMet(int? progType)
        {
            return progType.HasValue
                && progType != TypeOfLearningProgramme.Traineeship;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool PostCodeNullConditionMet(string lsdPostcode)
        {
            return !string.IsNullOrWhiteSpace(lsdPostcode);
        }

        public bool TemporaryPostcodeConditionMet(string lsdPostcode)
        {
            return lsdPostcode != ValidationConstants.TemporaryPostCode;
        }

        public bool ValidPostcodeConditionMet(string lsdPostcode)
        {
            return _postcodesDataService.PostcodeExists(lsdPostcode);
        }      

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _firstAugust2019;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate,int fundModel, string lsdPostcode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, lsdPostcode)
            };
        }

    }
}
