using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface
{
    public interface IPostcodesDataService : IDataService
    {
        bool PostcodeExists(string postcode);

        IReadOnlyCollection<IONSPostcode> GetONSPostcodes(string fromPostcode);
        
        IReadOnlyCollection<IDevolvedPostcode> GetDevolvedPostcodes(string fromPostcode);
    }
}
