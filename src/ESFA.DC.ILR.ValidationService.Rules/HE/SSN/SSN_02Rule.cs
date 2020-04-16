using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.SSN
{
    public class SSN_02Rule : AbstractRule, IRule<ILearner>
    {
        private const string _regexString = @"^[A-HJ-NPR-Y]{4}[0-9]{8}[A-HJ-NPR-Z]{1}$";
        private const int _startingCalculatedValue = 0;
        private const int _startingWeightFactor = 13;
        private const int _checkSumConstant = 23;

        private static readonly Dictionary<string, int> ValidLettersForPositions1To4 = new Dictionary<string, int>
        {
            ["A"] = 0,
            ["B"] = 1,
            ["C"] = 2,
            ["D"] = 3,
            ["E"] = 4,
            ["F"] = 5,
            ["G"] = 6,
            ["H"] = 7,
            ["J"] = 8,
            ["K"] = 9,
            ["L"] = 10,
            ["M"] = 11,
            ["N"] = 12,
            ["P"] = 13,
            ["R"] = 14,
            ["S"] = 15,
            ["T"] = 16,
            ["U"] = 17,
            ["V"] = 18,
            ["W"] = 19,
            ["X"] = 20,
            ["Y"] = 21
        };

        private static readonly Dictionary<int, string> ValidLettersForLastPosition = new Dictionary<int, string>
        {
            [1] = "A",
            [2] = "B",
            [3] = "C",
            [4] = "D",
            [5] = "E",
            [6] = "F",
            [7] = "G",
            [8] = "H",
            [9] = "J",
            [10] = "K",
            [11] = "L",
            [12] = "M",
            [13] = "N",
            [14] = "P",
            [15] = "R",
            [16] = "S",
            [17] = "T",
            [18] = "U",
            [19] = "V",
            [20] = "W",
            [21] = "X",
            [22] = "Y",
            [23] = "Z"
        };

        private readonly Regex _regex = new Regex(_regexString, RegexOptions.Compiled);

        public SSN_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.SSN_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (string.IsNullOrWhiteSpace(learningDelivery.LearningDeliveryHEEntity?.SSN))
                {
                    continue;
                }

                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity.SSN.ToUpper()))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.SSN));
                }
            }
        }

        public bool ConditionMet(string ssn)
        {
            return !RegexIsValid(ssn) || !CheckSumMatchesExpected(ssn);
        }

        public bool RegexIsValid(string ssn)
        {
            return _regex.IsMatch(ssn);
        }

        public bool CheckSumMatchesExpected(string ssn)
        {
            var expectedCheckSum = ssn.Substring(ssn.Length - 1);

            var checkSum = GetLetter(GetCalculatedCheckSumValue(ssn));

            return !string.IsNullOrEmpty(checkSum) && checkSum.Equals(expectedCheckSum);
        }

        public int GetCalculatedCheckSumValue(string ssn)
        {
            /* Check sum algorithm break down can be found here:
               https://www.hesa.ac.uk/collection/c18051/a/ssn */

            var ssn1To4 = ssn.Substring(0, 4);
            var ssn5To12 = ssn.Substring(4, 8).Select(c => Convert.ToInt32(c.ToString()));

            var calculatedValues = _startingCalculatedValue;
            var weightingFactor = _startingWeightFactor;

            foreach (var letter in ssn1To4)
            {
                var letterValue = GetLetterValue(letter.ToString());
                var calculatedValue = letterValue * weightingFactor;

                calculatedValues += calculatedValue;
                weightingFactor -= 1;
            }

            foreach (var number in ssn5To12)
            {
                var calculatedValue = number * weightingFactor;

                calculatedValues += calculatedValue;
                weightingFactor -= 1;
            }

            var remainder = calculatedValues % _checkSumConstant;

            var checkSumValue = _checkSumConstant - remainder;

            return checkSumValue;
        }

        public string GetLetter(int value)
        {
            return ValidLettersForLastPosition.ContainsKey(value) ? ValidLettersForLastPosition[value] : null;
        }

        public int GetLetterValue(string letter)
        {
            return ValidLettersForPositions1To4[letter];
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string ssn)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.SSN, ssn)
            };
        }
    }
}
