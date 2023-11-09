using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Tgbis0103RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidTrustmarkProjectReferenceNumber))]
        public void Tgbis0103Rule_WithValidInput_PassesValidation(string trustmarkProjectReferenceNumber)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                TrustmarkProjectReferenceNumber = trustmarkProjectReferenceNumber
            };
            var rule = new Tgbis0103RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);
        }

        public static TheoryData<string?> ValidTrustmarkProjectReferenceNumber() => new()
        {
            { "A0000" },
            { "A00000" },
            { "A000000" },
            { "A0000000" },
            { "a0123" },
            { "C45678" },
            { "e901234" },
            { "G5678901" },
        };

        [Theory]
        [MemberData(nameof(InvalidTrustmarkProjectReferenceNumber))]
        public void Tgbis0103Rule_WithInvalidInput_FailsValidation(string trustmarkProjectReferenceNumber)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                TrustmarkProjectReferenceNumber = trustmarkProjectReferenceNumber
            };
            var rule = new Tgbis0103RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var errorFieldValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal(measureModel.TrustmarkProjectReferenceNumber, errorFieldValue);
            Assert.Equal("TGBIS0103", rule.TestNumber);
        }

        public static TheoryData<string?> InvalidTrustmarkProjectReferenceNumber() => new()
        {
            { null },
            { "" },
            { "\t" },
            { "a000a" },
            { "A0000-A" },
            { "A0000--A" },
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
            { "A0000-0A" },
            { "A00000-A" },
            { "A0000000A" },
        };
    }
}