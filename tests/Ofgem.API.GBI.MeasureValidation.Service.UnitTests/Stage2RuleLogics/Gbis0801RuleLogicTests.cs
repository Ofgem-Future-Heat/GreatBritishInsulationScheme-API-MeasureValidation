using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0801RuleLogicTests
    {
        [Theory]
        [InlineData(CommonTypesConstants.NotApplicable)]
        [InlineData("BGT0123456789")]
        [InlineData("ABC0123456789")]
        public void Gbis0801Rule_WithValidInput_PassValidation(string associatedInsulationMrnForHeatingType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT0123456789",
                AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingType
            };

            var rule = new Gbis0801RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("a")]
        [InlineData("BGT789")]
        [InlineData("1230123456789")]
        [InlineData("BGT1236883G")]
        [InlineData("BGT012345678987654321")]
        public void Gbis0801Rule_WithInvalidInput_FailsValidation(string associatedInsulationMrnForHeatingType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT0123456789",
                AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingType
            };

            var rule = new Gbis0801RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0801", rule.TestNumber);
        }
    }
}
