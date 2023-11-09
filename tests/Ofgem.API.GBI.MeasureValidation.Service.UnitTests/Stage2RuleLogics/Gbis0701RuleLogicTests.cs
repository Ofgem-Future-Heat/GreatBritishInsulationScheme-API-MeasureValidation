using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0701RuleLogicTests
    {
        [Theory]
        [InlineData("5")]
        [InlineData("50")]
        [InlineData("3.8")]
        [InlineData("38.9")]
        public void Gbis0701Rule_WithValidInput_PassValidation(string startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                StartingSAPRating = startingSapRating,
            };
            var rule = new Gbis0701RuleLogic();
            var result = rule.FailureCondition(measureModel);
            
            Assert.False(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(".1")]
        [InlineData("0.300")]
        [InlineData("680")]
        [InlineData("68.35")]
        [InlineData("678.354")]
        public void Gbis0701Rule_WithInvalidIput_FailsValidation(string startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                StartingSAPRating = startingSapRating,
            };
            var rule = new Gbis0701RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0701", rule.TestNumber);
            Assert.Equal(measureModel.StartingSAPRating, observedValue);

        }

    }
}
