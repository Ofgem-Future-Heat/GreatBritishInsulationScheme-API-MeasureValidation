using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0811RuleLogicTests
    {

        [Theory]
        [InlineData("TRV", "Not NA")]
        [InlineData("P&RT", "Not NA")]
        [InlineData("LI - Declared", "N/A")]
        public void Gbis0811RuleLogic_WithValidInput_PassesValidation(string measureType, string heatingSource)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                MeasureType = measureType,
                HeatingSource = heatingSource,
            };

            var rule = new Gbis0811RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.False(result);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(heatingSource));
        }

        [Theory]
        [InlineData("TRV", "N/A")]
        [InlineData("P&RT", "N/A")]
        public void Gbis0811RuleLogic_WithInvalidInput_FailsValidation(string measureType, string heatingSource)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                MeasureType = measureType,
                HeatingSource = heatingSource,
            };

            var rule = new Gbis0811RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.True(result);
            Assert.Equal("GBIS0811", rule.TestNumber);
            var actual = rule.FailureFieldValueFunction(measureModel);
            Assert.True(actual!.Equals(measureModel.HeatingSource));
        }
    }
}
