using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0207RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, "validMeasureType")]
        public void Gbis0207Rule_WithValidInput_PassValidation(string eligibilityType, string measureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                MeasureType = measureType,
            };
            var rule = new Gbis0207RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.GeneralGroup, MeasureTypes.Trv)]
        [InlineData(EligibilityTypes.GeneralGroup, MeasureTypes.PAndRt)]
        [InlineData("general group", "trv")]
        [InlineData("general group", "p&rt")]
        [InlineData("general group", "P&RT")]
        [InlineData("General Group", "trv")]
        public void Gbis0207Rule_WithInvalidInput_FailsValidation(string eligibilityType, string measureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                MeasureType = measureType,
            };
            var rule = new Gbis0207RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0207", rule.TestNumber);
            Assert.Equal(measureModel.MeasureType, observedValue);
        }
    }
}