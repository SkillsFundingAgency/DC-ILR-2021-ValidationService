using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_20Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataService _fileDataService;
        private readonly IFCSDataService _fcsDataService;

        public UKPRN_20Rule(
            IFileDataService fileDataService,
            IFCSDataService fcsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_20)
        {
            _fileDataService = fileDataService;
            _fcsDataService = fcsDataService;
        }

        public UKPRN_20Rule()
           : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            throw new NotImplementedException();
        }
    }
}
