using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0717RuleLogicTests
    {
        [Theory]
        [InlineData("Yes")]
        [InlineData("yes")]
        [InlineData("N/A")]
        [InlineData("n/a")]
        public void Gbis0717_withValidInput_PassesValidation(string prsSapBandException)
        {
            var measureModel = new MeasureModel
            {
                PrsSapBandException = prsSapBandException
            };

            var rule = new Gbis0717RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("No")]
        [InlineData("")]
        [InlineData("N/A with other text")]
        [InlineData("123")]
        public void Gbis0717RuleLogic_GivenInvalidValue_ReturnsError(string prsSapBandException)
        {
            var measureModel = new MeasureModel
            {
                PrsSapBandException = prsSapBandException
            };

            var rule = new Gbis0717RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var actual = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0717", rule.TestNumber);
            Assert.Equal(measureModel.PrsSapBandException, actual);

        }
    }
    
}
