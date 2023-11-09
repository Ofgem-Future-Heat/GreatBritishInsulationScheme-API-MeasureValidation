using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0716RuleLogicTests
    {
        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("C")]
        [InlineData("D")]
        [InlineData("E")]
        [InlineData("a")]
        [InlineData("b")]
        [InlineData("c")]
        [InlineData("d")]
        [InlineData("e")]
        [InlineData("N/A")]
        [InlineData("n/a")]
        public void Gbis0716_withValidInput_PassesValidation(string councilTaxBand)
        {
            var measureModel = new MeasureModel
            {
                CouncilTaxBand = councilTaxBand
            };

            var rule = new Gbis0716RuleLogic();
            var result = rule.FailureCondition(measureModel);
            
            Assert.False(result);
        }

        [Theory]
        [InlineData("F")]
        [InlineData("")]
        [InlineData("band a")]
        [InlineData("123")]
        public void Gbis0716RuleLogic_GivenInvalidValue_ReturnsError(string councilTaxBand)
        {
            var measureModel = new MeasureModel
            {
                CouncilTaxBand = councilTaxBand
            };
            var rule = new Gbis0716RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var actual = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0716", rule.TestNumber);
            Assert.Equal(measureModel.CouncilTaxBand, actual);

        }
    }
    
}
