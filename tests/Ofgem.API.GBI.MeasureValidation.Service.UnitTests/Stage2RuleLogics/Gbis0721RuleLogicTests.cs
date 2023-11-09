using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0721RuleLogicTests
    {
        [Theory]
        [InlineData("Yes")]
        [InlineData("yes")]
        [InlineData("No")]
        [InlineData("no")]
        public void Gbis0721_withValidInput_PassesValidation(string offGas)
        {
            var measureModel = new MeasureModel
            {
                OffGas = offGas
            };

            var rule = new Gbis0721RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("ABC")]
        public void Gbis0721RuleLogic_GivenInvalidValue_ReturnsError(string offGas)
        {
            var measureModel = new MeasureModel
            {
                OffGas = offGas
            };

            var rule = new Gbis0721RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var actual = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0721", rule.TestNumber);
            Assert.Equal(measureModel.OffGas, actual);

        }
    }
    
}
