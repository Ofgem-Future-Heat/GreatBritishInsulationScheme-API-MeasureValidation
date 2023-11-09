using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Tgbis0100RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidTrustmarkBusinessLicenceNumberInputs))]
        public void Tgbis0100Rule_WithValidInput_PassesValidation(string trustmarkBusinessLicenceNumber)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                TrustmarkBusinessLicenceNumber = trustmarkBusinessLicenceNumber
            };
            var rule = new Tgbis0100RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);
        }

        public static TheoryData<string?> ValidTrustmarkBusinessLicenceNumberInputs() => new()
        {
            { new string('0', 12) },
            { "012345678912" },
        };

        [Theory]
        [MemberData(nameof(InvalidTrustmarkBusinessLicenceNumberInputs))]
        public void Tgbis0100Rule_WithInvalidInput_FailsValidation(string trustmarkBusinessLicenceNumber)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                TrustmarkBusinessLicenceNumber = trustmarkBusinessLicenceNumber
            };
            var rule = new Tgbis0100RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var errorFieldValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal(measureModel.TrustmarkBusinessLicenceNumber, errorFieldValue);
            Assert.Equal("TGBIS0100", rule.TestNumber);
        }

        public static TheoryData<string?> InvalidTrustmarkBusinessLicenceNumberInputs() => new()
        {
            { null },
            { "" },
            { new string('0', 1) },
            { new string('0', 11) },
            { new string('0', 13) },
            { new string('0', 20) },
            { new string(' ', 12) },
            { new string('A', 11) },
            { new string('A', 12) },
            { new string('A', 13) },
            { "abcdefghijkl" },
        };

    }
}