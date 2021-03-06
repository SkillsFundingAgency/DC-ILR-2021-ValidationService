﻿using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R06RuleTests : AbstractRuleTests<R06Rule>
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("R06", result);
        }

        [Fact]
        public void ValidateWithEmptyMessage()
        {
            var testMessage = new TestMessage()
            {
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                    },
                    new TestLearner()
                    {
                    }
                }
            };
            NewRule().Validate(testMessage);
        }

        [Fact]
        public void CheckForDuplicate_LearnRefNumber_Duplicate()
        {
            var testMessage = new TestMessage()
            {
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                        LearnRefNumber = "abc1"
                    },
                    new TestLearner()
                    {
                        LearnRefNumber = "AbC1"
                    },
                    new TestLearner()
                    {
                        LearnRefNumber = "ABC1"
                    },
                    new TestLearner()
                    {
                        LearnRefNumber = "xyZ"
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testMessage);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R06, "abc1", null, null), Times.Exactly(3));
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R06, "xyZ", null, null), Times.Never);
            }
        }

        [Fact]
        public void CheckForDuplicate_LearnRefNumber_NoDuplicate()
        {
            var testMessage = new TestMessage()
            {
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                    },
                    new TestLearner()
                    {
                        LearnRefNumber = "123456"
                    },
                    new TestLearner()
                    {
                        LearnRefNumber = "1234567"
                    }
                }
            };

            NewRule().Validate(testMessage);
        }

        public R06Rule NewRule(IValidationErrorHandler errorHandler = null)
        {
            return new R06Rule(errorHandler ?? BuildValidationErrorHandlerMockForError().Object);
        }
    }
}
