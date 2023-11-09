using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0403RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.LISocialHousing, "validMeasureType")]
        public void Gbis0403Rule_WithValidInput_PassValidation(string eligibilityType, string measureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                MeasureType = measureType,
            };
            var rule = new Gbis0403RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.LISocialHousing, MeasureTypes.Trv)]
        [InlineData(EligibilityTypes.LISocialHousing, MeasureTypes.PAndRt)]
        [InlineData("LI - Social Housing", "trv")]
        [InlineData("LI - Social Housing", "p&rt")]
        [InlineData("LI - Social Housing", "P&RT")]
        [InlineData("li - social housing", "trv")]
        public void Gbis0403Rule_WithInvalidInput_FailsValidation(string eligibilityType, string measureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                MeasureType = measureType,
            };
            var rule = new Gbis0403RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0403", rule.TestNumber);
            Assert.Equal(measureModel.MeasureType, observedValue);
        }
    }
}
