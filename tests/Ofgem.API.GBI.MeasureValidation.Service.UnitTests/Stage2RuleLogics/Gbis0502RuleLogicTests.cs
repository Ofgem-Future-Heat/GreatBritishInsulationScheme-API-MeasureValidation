using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0502RuleLogicTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("N/A")]
        [InlineData("n/a")]
        [InlineData("1")]
        [InlineData("1001")]
        [InlineData("Street Test")]
        public void Gbis0502Rule_WithValidInput_PassValidation(string streetName)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                StreetName = streetName
            };
            var rule = new Gbis0502RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("c1wMoXZTPyutWZyGd9hYWvvK3iV9lcosRPeDEf4X2XSfoQWKeu9")]
        public void Gbis0502Rule_WithInvalidInput_FailsValidation(string streetName)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                StreetName = streetName
            };
            var rule = new Gbis0502RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0502", rule.TestNumber);
            Assert.Equal(measureModel.StreetName, observedValue);
        }
    }
}