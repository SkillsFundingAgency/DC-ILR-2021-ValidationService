using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_14Rule : IDerivedData_14Rule
    {
        private const int _invalidLengthChecksum = 10;

        private const int _requiredIDLength = 9;

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

        public char InvalidLengthChecksum => _validChecksums[_invalidLengthChecksum];

        public char GetWorkPlaceEmpIdChecksum(int workPlaceEmpId)
        {
            /*
             Calculate the checksum digit based on characters 1 to 8 using the formula
                [11 - ((9 * 1st digit + 8 * 2nd digit + 7 * 3rd digit + 6 * 4th digit + 5 * 5th digit + 4 * 6th digit + 3 * 7th digit + 2 * 8th digit) mod 11)]
             if result = 11 set DD14 to 0,
             if result = 10
                    or the LearningDeliveryWorkPlacement.WorkPlaceEmpId is not 9 characters in length
                    set DD14 to 'X',
                otherwise set DD14 to number of the result(1 - 9)
            */

            var list = workPlaceEmpId.SplitIntDigitsToList();

            if (list.Count != _requiredIDLength)
            {
                return InvalidLengthChecksum;
            }

            var result = 11 - (Sum(list) % 11);

            return _validChecksums[result];
        }

       public int Sum(IReadOnlyCollection<int> digits)
        {
            var indexComputational = 2;
            var result = 0;
            var reversedDigits = digits.Reverse().Skip(1);

            foreach (var digit in reversedDigits)
            {
                result += Calc(digit, indexComputational++);
            }

            return result;
        }

        public int Calc(int item, int multiple)
        {
            return item * multiple;
        }
    }
}
