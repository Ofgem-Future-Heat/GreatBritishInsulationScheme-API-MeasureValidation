using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0700RuleLogicTests
    {
        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        [InlineData("yes")]
        [InlineData("no")]

        public void Gbis0700Rule_WithValidInput_PassValidation(string privateDomesticPremises)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                PrivateDomesticPremises = privateDomesticPremises
            };
            var rule = new Gbis0700RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("N/A")]
        [InlineData("n/a")]
        [InlineData("1")]
        [InlineData("0")]
        [InlineData("Y")]
        [InlineData("N")]
        [InlineData("Maybe")]
        public void Gbis0700Rule_WithInvalidInput_FailsValidation(string privateDomesticPremises)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                PrivateDomesticPremises = privateDomesticPremises
            };
            var rule = new Gbis0700RuleLogic();

            var result = rule.FailureCondition(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0700", rule.TestNumber);
        }
    }
}
