using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0305RuleLogicTests
    {

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied, MeasureTypes.Trv)]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.PrivateRentedSector, MeasureTypes.Trv)]
        public void Gbis0305RuleLogic_WithValidInput_PassesValidation(string eligibilityType, string tenureType, string measureType)
        {
            // Arrange 
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                MeasureType = measureType
            };
            var rule = new Gbis0305RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);

        }

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, MeasureTypes.Trv)]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.PrivateRentedSector, MeasureTypes.PAndRt)]
        [InlineData("li - help to heat group", "private rented sector", "trv")]
        [InlineData("li - help to heat group", "private rented sector", "p&rt")]
        public void Gbis0305RuleLogic_WithValidInput_FailsValidation(string eligibilityType, string tenureType, string measureType)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                MeasureType = measureType
            };
            var rule = new Gbis0305RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal("GBIS0305", rule.TestNumber);
            Assert.Equal(measureModel.MeasureType, observedValue);

        }

    }
}