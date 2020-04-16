using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    /// <summary>
    /// derived data rule 05
    /// Checksum digit for Employer reference number
    /// </summary>
    public class DerivedData_05Rule :
        IDerivedData_05Rule
    {
        /// <summary>
        /// The invalid length checksum
        /// </summary>
        private const int _invalidLengthChecksum = 10;

        /// <summary>
        /// The required identifier length
        /// </summary>
        private const int _requiredIDLength = 9;

        /// <summary>
        /// The valid checksums
        /// </summary>
        private static readonly Dictionary<int, char> _validChecksums = new Dictionary<int, char>
        {
            [0] = '0',  // <= ???
            [1] = '1',
            [2] = '2',
            [3] = '3',
            [4] = '4',
            [5] = '5',
            [6] = '6',
            [7] = '7',
            [8] = '8',
            [9] = '9',
            [10] = 'X',
            [11] = '0', // <= ??? (mod by 11)
        };

        /// <summary>
        /// Gets the invalid length checksum.
        /// </summary>
        public char InvalidLengthChecksum =>
            _validChecksums[_invalidLengthChecksum];

        /// <summary>
        /// Gets the employer identifier checksum.
        /// </summary>
        /// <param name="thisEmployer">this employer.</param>
        /// <returns>
        /// the checksum character
        /// </returns>
        public char GetEmployerIDChecksum(int thisEmployer)
        {
            /*
                Calculate the checksum digit based on characters 1 to 8 using the formula
                    [11 - ((9*1st digit + 8*2nd digit + 7*3rd digit + 6*4th digit + 5*5th digit + 4*6th digit + 3*7th digit + 2*8th digit) mod 11)]
                    if result = 11 set DD05 to 0,
                    if result = 10
                        or the LearningEmploymentStatus.EmpId is not 9 characters in length
                        set DD05 to 'X',
                    otherwise set DD05 to number of the result (1-9).
             */

            var list = thisEmployer.SplitIntDigitsToList();

            // the employer id has to have a length of 9 digits
            if (list.Count != _requiredIDLength)
            {
                return InvalidLengthChecksum;
            }

            var result1 = 11 - (Sum(list) % 11);

            return _validChecksums[result1];
        }

        /// <summary>
        /// Sums the specified digits (with consideration to the positional multiples).
        /// </summary>
        /// <param name="digits">The digits.</param>
        /// <returns>the sum of the calculation</returns>
        public int Sum(IReadOnlyCollection<int> digits)
        {
            var indexComputational = 2;
            var result = 0;

            digits
                .Reverse()
                .Skip(1)
                .ForEach(x => result += Calc(x, indexComputational++));

            return result;
        }

        /// <summary>
        /// Calculates the value of the checksum element.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="multiple">The multiple.</param>
        /// <returns>the sum of the operation</returns>
        public int Calc(int item, int multiple)
        {
            return item * multiple;
        }
    }
}
