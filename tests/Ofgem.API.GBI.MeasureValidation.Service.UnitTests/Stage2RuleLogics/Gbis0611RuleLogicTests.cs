using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0611RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, "GBT0123456789", "0")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, "GBT0123456789", "20")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, "GBT0123456789", "64.5")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Bungalow, "GBT0123456789", "0")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Bungalow, "GBT0123456789", "20")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Bungalow, "GBT0123456789", "64.5")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "GBT0123456789", "0")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "GBT0123456789", "20")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "GBT0123456789", "64.5")]
        public void Gbis0611Rule_WithValidInput_PassValidation(string eligibilityType, string property, string associatedInFillMeasure1, string startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                EligibilityType = eligibilityType,
                Property = property,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                StartingSAPRating = startingSapRating
            };

            var rule = new Gbis0611RuleLogic();
            var result = rule.FailureCondition(measureModel);
            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, "GBT0123456789", "-10")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.House, "GBT0123456789", "69")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Bungalow, "GBT0123456789", "-10")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.Bungalow, "GBT0123456789", "69")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "GBT0123456789", "-10")]
        [InlineData(EligibilityTypes.InFill, PropertyTypes.ParkHome, "GBT0123456789", "69")]
        public void Gbis0611Rule_WithInvalidInput_FailsValidation(string eligibilityType, string property, string associatedInFillMeasure1, string startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "GBT0123456789",
                EligibilityType = eligibilityType,
                Property = property,
                AssociatedInfillMeasure1 = associatedInFillMeasure1,
                StartingSAPRating = startingSapRating
            };

            var rule = new Gbis0611RuleLogic();

            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0611", rule.TestNumber);
            Assert.Equal(measureModel.StartingSAPRating, observedValue);
        }
    }
}
