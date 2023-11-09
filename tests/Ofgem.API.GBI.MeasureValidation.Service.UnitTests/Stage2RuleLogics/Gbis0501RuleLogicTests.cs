using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0501RuleLogicTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("N/A")]
        [InlineData("n/a")]
        [InlineData("1")]
        [InlineData("1001")]
        [InlineData("Flat A1")]
        public void Gbis0501Rule_WithValidInput_PassValidation(string flatNameNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                FlatNameNumber = flatNameNumber
            };
            var rule = new Gbis0501RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("c1wMoXZTPyutWZyGd9hYWvvK3iV9lcosRPeDEf4X2XSfoQWKeu9")]
        public void Gbis0501Rule_WithInvalidInput_FailsValidation(string flatNameNumber)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                FlatNameNumber = flatNameNumber
            };
            var rule = new Gbis0501RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0501", rule.TestNumber);
            Assert.Equal(measureModel.FlatNameNumber, observedValue);
        }
    }
}