using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0722RuleLogicTests
    {
        [Theory]
        [InlineData("Yes")]
        [InlineData("yes")]
        [InlineData("No")]
        [InlineData("no")]
        public void Gbis0722_withValidInput_PassesValidation(string rural)
        {
            var measureModel = new MeasureModel
            {
                Rural = rural
            };

            var rule = new Gbis0722RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("ABC")]
        public void Gbis0722RuleLogic_GivenInvalidValue_ReturnsError(string rural)
        {
            var measureModel = new MeasureModel
            {
                Rural = rural
            };

            var rule = new Gbis0722RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var actual = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0722", rule.TestNumber);
            Assert.Equal(measureModel.Rural, actual);

        }
    }
    
}
