using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Tgbis0102RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidTrustmarkUniqueMeasureReference))]
        public void Tgbis0102Rule_WithValidInput_PassesValidation(string trustmarkUniqueMeasureReference)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                TrustmarkUniqueMeasureReference = trustmarkUniqueMeasureReference
            };
            var rule = new Tgbis0102RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);
        }

        public static TheoryData<string> ValidTrustmarkUniqueMeasureReference() => new()
        {
            { "0" },
            { "a" },
            { "A" },
            { new string('0', 20) },
            { new string('a', 20) },
            { new string('A', 20) },
            { "0aA" },
            { "abcde01234ABCDE1a2b0" },
        };

        [Theory]
        [MemberData(nameof(InvalidTrustmarkUniqueMeasureReference))]
        public void Tgbis0102Rule_WithInvalidInput_FailsValidation(string trustmarkUniqueMeasureReference)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                TrustmarkUniqueMeasureReference = trustmarkUniqueMeasureReference
            };
            var rule = new Tgbis0102RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var errorFieldValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal(measureModel.TrustmarkUniqueMeasureReference, errorFieldValue);
            Assert.Equal("TGBIS0102", rule.TestNumber);
        }

        public static TheoryData<string?> InvalidTrustmarkUniqueMeasureReference() => new()
        {
            { null },
            { "" },
            { " " },
            { "\t" },
            { "*" },
            { new string('*', 20) },
            { new string(' ', 20) },
            { new string('\t', 20) },
            { new string('0', 21) },
            { new string('a', 21) },
            { new string('A', 21) },
            { new string('a', 22) },
            { new string('a', 50) },
            { "0-A" },
            { "abc00$~@{}:0a" },
            { "abc-de01234ABCDE1ab0" },
        };

    }
}