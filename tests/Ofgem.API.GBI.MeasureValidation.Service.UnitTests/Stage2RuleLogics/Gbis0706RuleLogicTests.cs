using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0706RuleLogicTests
    {
        [Theory]
        [InlineData("16")]
        [InlineData("001")]
        [InlineData("1")]
        [InlineData("21")]
        [InlineData("021")]
        [InlineData("N/A")]
        [InlineData("n/a")]

        public void Gbis0706Rule_WithValidInput_PassValidation(string innovationMeasureNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                InnovationMeasureNumber = innovationMeasureNumber
            };
            var rule = new Gbis0706RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        public void Gbis0706Rule_WithInvalidInput_FailsValidation(string innovationMeasureNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                InnovationMeasureNumber = innovationMeasureNumber
            };
            var rule = new Gbis0706RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0706", rule.TestNumber);
            Assert.Equal(measureModel.InnovationMeasureNumber, observedValue);
        }
    }
}
