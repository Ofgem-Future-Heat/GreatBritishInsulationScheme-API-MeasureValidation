using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0718RuleLogicTests
    {
        [Theory]
        [InlineData("12345")]
        [InlineData("ABCDE")]
        [InlineData("N/A")]
        [InlineData("n/a")]
        public void Gbis0718_withValidInput_PassesValidation(string innovationMeasureNumber)
        {
            var measureModel = new MeasureModel
            {
                InnovationMeasureNumber = innovationMeasureNumber
            };

            var rule = new Gbis0718RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Gbis0718RuleLogic_GivenInvalidValue_ReturnsError(string innovationMeasureNumber)
        {
            var measureModel = new MeasureModel
            {
                InnovationMeasureNumber = innovationMeasureNumber
            };

            var rule = new Gbis0718RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var actual = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0718", rule.TestNumber);
            Assert.Equal(measureModel.InnovationMeasureNumber, actual);

        }
    }
    
}
