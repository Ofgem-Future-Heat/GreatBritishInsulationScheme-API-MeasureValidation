using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0608RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, "FOX0123456789", "FOX0123456781", "FOX0123456782")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Bungalow, "FOX0123456789", "FOX0123456781", "FOX0123456782")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "FOX0123456789", "FOX0123456781", "FOX0123456782")]
        public void Gbis0608Rule_WithValidInput_PassValidation(string eligibilityType, string property, string associatedInFillMeasure1, string associatedInFillMeasure2, string associatedInFillMeasure3)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                Property = property,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                AssociatedInfillMeasure2 = associatedInFillMeasure2,
                AssociatedInfillMeasure3 = associatedInFillMeasure3
            };
            var rule = new Gbis0608RuleLogic();
            var result = rule.FailureCondition(measureModel);
            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, CommonTypesConstants.NotApplicable, "FOX0123456781", "FOX0123456782")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, "FOX0123456781", CommonTypesConstants.NotApplicable, "FOX0123456782")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, "FOX0123456781", "FOX0123456782", CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Bungalow, "FOX0123456781", "FOX0123456782", CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "FOX0123456781", "FOX0123456782", CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Bungalow, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "n/a", "FOX0123456781", "FOX0123456782")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "N/a", "FOX0123456789", "FOX0123456789")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable, "FOX0123456789")]
        public void Gbis0608Rule_WithInvalidInput_FailsValidation(string eligibilityType, string property, string associatedInFillMeasure1, string associatedInFillMeasure2, string associatedInFillMeasure3)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                Property = property,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                AssociatedInfillMeasure2 = associatedInFillMeasure2,
                AssociatedInfillMeasure3 = associatedInFillMeasure3
            };
            var rule = new Gbis0608RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0608", rule.TestNumber);
            Assert.Equal($"{measureModel.AssociatedInfillMeasure1} | {measureModel.AssociatedInfillMeasure2} | {measureModel.AssociatedInfillMeasure3}", observedValue);
        }

    }
}