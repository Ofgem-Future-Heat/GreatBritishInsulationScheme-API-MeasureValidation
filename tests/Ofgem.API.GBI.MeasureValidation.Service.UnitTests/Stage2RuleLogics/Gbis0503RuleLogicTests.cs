using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0503RuleLogicTests
    {
        [Theory]
        [InlineData("N/A")]
        [InlineData("n/a")]
        [InlineData("1")]
        [InlineData("1001")]
        [InlineData("123456789123")]
        public void Gbis0503Rule_WithValidInput_PassValidation(string uprn)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                UniquePropertyReferenceNumber = uprn
            };
            var rule = new Gbis0503RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Test")]
        [InlineData("N-A")]
        [InlineData("1234567891234")]
        public void Gbis0503Rule_WithInvalidInput_FailsValidation(string uprn)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                UniquePropertyReferenceNumber = uprn
            };
            var rule = new Gbis0503RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0503", rule.TestNumber);
            Assert.Equal(measureModel.UniquePropertyReferenceNumber, observedValue);
        }
    }
}