using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0807RuleLogicTests
    {

        [Theory]
        [MemberData(nameof(ValidHeatingMeasures))]
        public void Gbis0807Rule_WithValidInput_PassesValidation(string measureType, string assoicatedMrn, AssociatedMeasureModelDto assoicatedMeasure)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureType = measureType,
                AssociatedInsulationMrnForHeatingMeasures = assoicatedMrn,
                AssociatedInsulationMeasureForHeatingMeasureDetails = assoicatedMeasure
            };

            var rule = new Gbis0807RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.False(result);
        }

        public static TheoryData<string, string, AssociatedMeasureModelDto?> ValidHeatingMeasures()
        {
            var validHeatingMeasures = new TheoryData<string, string, AssociatedMeasureModelDto?>
            {
                { MeasureTypes.Trv, "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = MeasureCategories.CavityWallInsulation }},
                { MeasureTypes.PAndRt, "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = MeasureCategories.ExternalInternalWallInsulation }},
                { MeasureTypes.Trv, "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = MeasureCategories.LoftInsulation}},
                { MeasureTypes.PAndRt, "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = MeasureCategories.OtherInsulation}},
                { MeasureTypes.PAndRt, "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = "Other Insulation"}},
                { "other", "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = MeasureCategories.CavityWallInsulation}},
                { "other", "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = "other"}},
                { "other", "N/A", new AssociatedMeasureModelDto() { MeasureCategory = "other" }},
                { MeasureTypes.PAndRt, CommonTypesConstants.NotApplicable, new AssociatedMeasureModelDto() { MeasureCategory = "other" }},
                { MeasureTypes.Trv, CommonTypesConstants.NotApplicable, new AssociatedMeasureModelDto() { MeasureCategory = "other" }},
                { MeasureTypes.Trv, "N/A", new AssociatedMeasureModelDto() { MeasureCategory = null }},
                { MeasureTypes.PAndRt, "N/A", null },
                { MeasureTypes.Trv, "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = null }},
                { MeasureTypes.Trv, "FOX0123456789", null },
            };
            return validHeatingMeasures;
        }

        [Theory]
        [MemberData(nameof(InvalidHeatingMeasures))]
        public void Gbis0807Rule_WithInvalidInput_PassesValidation(string measureType, string assoicatedMrn, AssociatedMeasureModelDto assoicatedMeasure)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureType = measureType,
                AssociatedInsulationMrnForHeatingMeasures = assoicatedMrn,
                AssociatedInsulationMeasureForHeatingMeasureDetails = assoicatedMeasure
            };
            var rule = new Gbis0807RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);
            var errorFieldValue = rule.FailureFieldValueFunction(measureModel);

            // Assert
            Assert.True(result);
            Assert.Equal("GBIS0807", rule.TestNumber);
            Assert.Equal(measureModel.AssociatedInsulationMrnForHeatingMeasures, errorFieldValue);
        }

        public static TheoryData<string, string, AssociatedMeasureModelDto> InvalidHeatingMeasures()
        {
            var validHeatingMeasures = new TheoryData<string, string, AssociatedMeasureModelDto>
            {
                { MeasureTypes.Trv, "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = "other" }},
                { MeasureTypes.PAndRt, "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = "other" }},
                { MeasureTypes.Trv, "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = ""}},
                { MeasureTypes.PAndRt, "FOX0123456789", new AssociatedMeasureModelDto() { MeasureCategory = ""}},
            };
            return validHeatingMeasures;
        }

    }
}
