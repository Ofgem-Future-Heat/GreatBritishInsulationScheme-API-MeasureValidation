using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Tgbis0104RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidTrustmarkCompletedProjectCertIds))]
        public void Tgbis0104Rule_WithValidInput_PassesValidation(string trustmarkCompletedProjectCertIds)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                TrustMarkCompletedProjectCertID = trustmarkCompletedProjectCertIds
            };
            var rule = new Tgbis0104RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);
        }

        public static TheoryData<string?> ValidTrustmarkCompletedProjectCertIds() => new()
                {
                    { "A0000-A" },
                    { "A00000-A" },
                    { "A000000-A" },
                    { "A0000000-A" },

                    { "a0123-b" },
                    { "C45678-d" },
                    { "e901234-F" },
                    { "G5678901-H" },
                };

        [Theory]
        [MemberData(nameof(InvalidTrustmarkCompletedProjectCertIds))]
        public void Tgbis0104Rule_WithInvalidInput_FailsValidation(string trustmarkCompletedProjectCertIds)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                TrustMarkCompletedProjectCertID = trustmarkCompletedProjectCertIds
            };
            var rule = new Tgbis0104RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var errorFieldValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal(measureModel.TrustMarkCompletedProjectCertID, errorFieldValue);
            Assert.Equal("TGBIS0104", rule.TestNumber);
        }

        public static TheoryData<string?> InvalidTrustmarkCompletedProjectCertIds() => new()
                {
                    { null },
                    { "" },
                    { "\t" },
                    { "a000a" },
                    { "A000A" },
                    { "A0000A" },
                    { "A00000A" },
                    { "A000000A" },
                    { "A0000000A" },
                    { "AA000A" },
                    { "aa000a" },
                    { "A0000AA" },
                    { "AA00000AA" },
                    { "A000-A" },
                    { "AA00000A-A" },
                    { "A00000000-A" },
                    { "A000A" },
                    { "A00000A" },
                    { "A000000A" },
                    { "A0000000A" },
                };

    }
}