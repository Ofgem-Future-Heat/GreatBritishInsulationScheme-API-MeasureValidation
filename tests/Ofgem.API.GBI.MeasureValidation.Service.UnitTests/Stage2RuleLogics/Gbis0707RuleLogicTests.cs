using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0707RuleLogicTests
    {
        [Theory]
        [InlineData("5")]
        [InlineData("18")]
        [InlineData("167")]
        [InlineData("1020")]
        public void Gbis0707Rule_WithValidInput_PassValidation(string floorArea)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                FloorArea = floorArea
            };
            var rule = new Gbis0707RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData("12345")]
        [InlineData("12.5")]
        [InlineData(".5")]
        [InlineData("124.325")]
        public void Gbis0707Rule_WithInvalidInput_FailsValidation(string floorArea)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                FloorArea = floorArea
            };
            var rule = new Gbis0707RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0707", rule.TestNumber);
            Assert.Equal(measureModel.FloorArea, observedValue);
        }
    }
}
