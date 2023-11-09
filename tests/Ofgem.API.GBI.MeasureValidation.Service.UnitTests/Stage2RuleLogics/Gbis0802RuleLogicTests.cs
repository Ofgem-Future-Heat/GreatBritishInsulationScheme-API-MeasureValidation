using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0802RuleLogicTests
    {
        [Theory]
        [InlineData("BGT0123456789", MeasureTypes.Trv)]
        [InlineData("BGT0123456789", MeasureTypes.PAndRt)]
        [InlineData(CommonTypesConstants.NotApplicable, "CWI_0.027")]
        [InlineData("BGT0123456789", "EWI_cavity_0.45_0.21")]
        public void Gbis0802Rule_WithValidInput_PassValidation(string associatedInsulationMrnForHeatingType, string measureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT0123456789",
                AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingType,
                MeasureType = measureType
            };

            var rule = new Gbis0802RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(CommonTypesConstants.NotApplicable, MeasureTypes.PAndRt)]
        [InlineData("N/A", MeasureTypes.Trv)]
        public void Gbis0802Rule_WithInvalidInput_FailsValidation(string associatedInsulationMrnForHeatingType, string measureType)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT0123456789",
                AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingType,
                MeasureType = measureType
            };

            var rule = new Gbis0802RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0802", rule.TestNumber);
        }
    }
}
