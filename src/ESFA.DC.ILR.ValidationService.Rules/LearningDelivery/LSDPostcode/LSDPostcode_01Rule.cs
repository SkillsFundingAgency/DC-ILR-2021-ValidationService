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
            var t6 = ProTypeConditionMet(progType);
            var t1 = FundModelConditionMet(fundModel);
            var t2 = PostCodeNullConditionMet(lsdPostcode);
            var t3 = TemporaryPostcodeConditionMet(lsdPostcode);  
            var t4 = ValidPostcodeConditionMet(lsdPostcode);
            var t5 = LearnStartDateConditionMet(learnStartDate);
            

            return t6 
                && t1
                && t2
                && t3
                && t4
                && t5;
        }

        public bool ProTypeConditionMet(int? progType)
        {
            var res = progType.HasValue
                && progType != TypeOfLearningProgramme.Traineeship;
            return res;
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
            var res = _postcodesDataService.PostcodeExists(lsdPostcode);
            return res;
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
