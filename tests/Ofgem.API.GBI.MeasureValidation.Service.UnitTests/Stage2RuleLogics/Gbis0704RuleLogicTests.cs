using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0704RuleLogicTests
    {
        [Theory]
        [InlineData("Bungalow")]
        [InlineData("House")]
        [InlineData("Flat")]
        [InlineData("Maisonette")]
        [InlineData("Park Home")]

        public void Gbis0704Rule_WithValidInput_PassValidation(string property)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                Property = property
            };
            var rule = new Gbis0704RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("N/A")]
        [InlineData("n/a")]
        [InlineData("Flat/House")]
        [InlineData("Appartement")]
        [InlineData("Maisonete")]
        [InlineData("Chalet")]
        [InlineData("B")]
        public void Gbis0704Rule_WithInvalidInput_FailsValidation(string property)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                Property = property
            };
            var rule = new Gbis0704RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0704", rule.TestNumber);
            Assert.Equal(measureModel.Property, observedValue);
        }
    }
}
