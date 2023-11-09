using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0614RuleLogicTests
    {

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, PropertyTypes.House, "BGT0123456789", 1, 2)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, CommonTypesConstants.NotApplicable, 1, 2)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Flat, "ABC0123456781", 1, 2)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Bungalow, "ABC0123456781", 1, 1)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, CommonTypesConstants.NotApplicable, 1, 1)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, "BGT0123456789", 3, 3)]
        public void Gbis0614Rule_WithValidInput_PassesValidation(string eligibilityType, string property, string associatedInfillMeasure3, int? measureCategoryId, int associatedMeasure3CategoryId)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                EligibilityType = eligibilityType,
                Property = property,
                MeasureCategoryId = measureCategoryId,
                AssociatedInfillMeasure3 = associatedInfillMeasure3,
                AssociatedInfillMeasure3Details = new AssociatedMeasureModelDto
                {
                    MeasureCategoryId = associatedMeasure3CategoryId
                }
            };

            var rule = new Gbis0614RuleLogic();
            var result = rule.FailureCondition(measureModel);
            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, "BGT0123456789", 1, 2)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Bungalow, "BGT0123456789", 2, 1)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "BGT0123456789", 1, 2)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "BGT0123456789", null, 1)]
        public void Gbis0614Rule_WithInvalidInput_FailsValidation(string eligibilityType, string property, string associatedInfillMeasure3, int? measureCategoryId, int associatedMeasure3CategoryId)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                EligibilityType = eligibilityType,
                Property = property,
                MeasureCategoryId = measureCategoryId,
                AssociatedInfillMeasure3 = associatedInfillMeasure3,
                AssociatedInfillMeasure3Details = new AssociatedMeasureModelDto
                {
                    MeasureCategoryId = associatedMeasure3CategoryId
                }
            };

            var rule = new Gbis0614RuleLogic();
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0614", rule.TestNumber);
            Assert.Equal(measureModel.AssociatedInfillMeasure3, observedValue);
        }
    }
}