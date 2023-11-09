using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0708RuleLogicTests
    {
        [Theory]
        [InlineData("N/A")]
        [InlineData("22/10/2023")]
        [InlineData("05/08/2023")]
        [InlineData("01/01/2021")]
        public void Gbis0708Rule_WithValidInput_PassValidation(string dateOfHouseholderEligibility)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                DateOfHouseholderEligibility = dateOfHouseholderEligibility
            };
            var rule = new Gbis0708RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("na")]
        [InlineData("  ")]
        [InlineData("02/29/2024")]
        [InlineData("31/12/20")]
        [InlineData("1/2/2026")]
        [InlineData("08.05.2023")]
        [InlineData("2nd February 2026")]
        public void Gbis0708Rule_WithInvalidInput_FailsValidation(string dateOfHouseholderEligibility)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                DateOfHouseholderEligibility = dateOfHouseholderEligibility
            };
            var rule = new Gbis0708RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0708", rule.TestNumber);
            Assert.Equal(measureModel.DateOfHouseholderEligibility, observedValue);
        }
    }
}
