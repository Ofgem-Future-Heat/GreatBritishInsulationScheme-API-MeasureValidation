using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0812RuleLogicTests
    {

        [Theory]
        [InlineData("TRV", "N/A")]
        [InlineData("P&RT", "N/A")]
        public void Gbis0812RuleLogic_WithValidInput_PassesValidation(string measureType, string heatingSource)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                MeasureType = measureType,
                HeatingSource = heatingSource,
            };

            var rule = new Gbis0812RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.False(result);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(heatingSource));
        }

        [Theory]
        [InlineData("N/A", "Gas")]
        [InlineData("LI - Declared", "Gas Boiler")]
        public void Gbis0812RuleLogic_WithInvalidInput_FailsValidation(string measureType, string heatingSource)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                MeasureType = measureType,
                HeatingSource = heatingSource,
            };

            var rule = new Gbis0812RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.True(result);
            Assert.Equal("GBIS0812", rule.TestNumber);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(measureModel.HeatingSource));
        }
    }
}
