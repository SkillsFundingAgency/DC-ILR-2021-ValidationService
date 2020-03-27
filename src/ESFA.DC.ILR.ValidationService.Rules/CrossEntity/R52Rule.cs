using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R52Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _learningDeliveryFAMTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            LearningDeliveryFAMTypeConstants.ACT,
            LearningDeliveryFAMTypeConstants.ALB,
            LearningDeliveryFAMTypeConstants.LSF,
        };

        public R52Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R52)
        {
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            foreach (var ld in learner.LearningDeliveries)
            {
                if (ld.LearningDeliveryFAMs != null)
                {
                    var comparer = StringComparer.OrdinalIgnoreCase;
                    Dictionary<string, Dictionary<string, int>> famTypeCount = new Dictionary<string, Dictionary<string, int>>(comparer);

                    foreach (var ldfam in ld.LearningDeliveryFAMs)
                    {
                        string famType = ldfam.LearnDelFAMType;
                        if (!_learningDeliveryFAMTypes.Contains(famType))
                        {
                            string famCode = ldfam.LearnDelFAMCode;
                            if (famTypeCount.ContainsKey(famType))
                            {
                                if (!famTypeCount[famType].ContainsKey(famCode))
                                {
                                    famTypeCount[famType].Add(famCode, 0);
                                }
                            }
                            else
                            {
                                famTypeCount.Add(famType, new Dictionary<string, int>(comparer));
                                famTypeCount[famType].Add(famCode, 0);
                            }

                            famTypeCount[famType][famCode]++;
                        }
                    }

                    foreach (var famType in famTypeCount)
                    {
                        foreach (var famCode in famType.Value)
                        {
                            if (famCode.Value > 1)
                            {
                                HandleValidationError(
                                    learner.LearnRefNumber,
                                    ld.AimSeqNumber,
                                    BuildErrorMessageParameters(famType.Key, famCode.Key));
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnDelFAMType, string learnDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFAMCode),
            };
        }
    }
}
