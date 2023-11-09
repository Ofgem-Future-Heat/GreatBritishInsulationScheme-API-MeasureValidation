using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0702RuleLogicTests
    {
        [Theory]
        [InlineData("Company ABC")]
        [InlineData("Installation Co.")]
        [InlineData("#1 Best Installation Company")]
        [InlineData("Z")]
        public void Gbis0702Rule_WithValidInput_PassValidation(string installerName)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                InstallerName = installerName,
            };
            var rule = new Gbis0702RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData("n/a")]
        [InlineData("N/A")]
        public void Gbis0702Rule_WithInvalidIput_FailsValidation(string installerName)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                InstallerName = installerName,
            };
            var rule = new Gbis0702RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0702", rule.TestNumber);
            Assert.Equal(measureModel.InstallerName, observedValue);
        }
    }
}
