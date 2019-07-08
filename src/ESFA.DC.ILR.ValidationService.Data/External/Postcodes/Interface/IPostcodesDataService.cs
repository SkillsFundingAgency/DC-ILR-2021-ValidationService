using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface
{
    /// <summary>
    /// the postcodes service definition
    /// </summary>
    public interface IPostcodesDataService : IDataService
    {
        /// <summary>
        /// Postcode exists.
        /// </summary>
        /// <param name="postcode">The postcode.</param>
        /// <returns>returns true if the postcode is found</returns>
        bool PostcodeExists(string postcode);

        /// <summary>
        /// Gets the ons postcode.
        /// </summary>
        /// <param name="fromPostcode">From postcode.</param>
        /// <returns>an ons postcodes (if found)</returns>
        IReadOnlyCollection<IONSPostcode> GetONSPostcodes(string fromPostcode);
        
        /// <summary>
        /// Gets the McaglaSOF postcode.
        /// </summary>
        /// <param name="fromPostcode">From postcode.</param>
        /// <returns>McaglaSOF postcodes (if found)</returns>
        IReadOnlyCollection<IMcaglaSOFPostcode> GetMcaglaSOFPostcodes(string fromPostcode);
    }
}
