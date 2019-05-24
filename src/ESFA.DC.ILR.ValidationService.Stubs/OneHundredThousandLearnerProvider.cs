using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class OneHundredThousandLearnerProvider : LearnerProviderService, IValidationItemProviderService<IEnumerable<ILearner>>
    {
        private const int LearnerCount = 10;

        public OneHundredThousandLearnerProvider(ICache<IMessage> messageCache)
            : base(messageCache)
        {
        }

        public new async Task<IEnumerable<ILearner>> ProvideAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            var learnerList = (await base.ProvideAsync(validationContext, cancellationToken)).ToList();

            while (learnerList.Count < LearnerCount)
            {
                var temp = learnerList;
                learnerList.AddRange(temp);
            }

            return learnerList.Take(LearnerCount);
        }
    }
}
