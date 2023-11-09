using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0301RuleLogicTests
    {

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.OwnerOccupied)]
        [InlineData(EligibilityTypes.GeneralGroup, TenureTypes.SocialHousing)]
        public void Gbis0301RuleLogic_WithValidInput_PassesValidation(string eligibilityType, string tenureType)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType
            };

            var rule = new Gbis0301RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);

        }

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, TenureTypes.SocialHousing)]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, "social housing (rsl)")]
        [InlineData("li - help to heat group", TenureTypes.SocialHousing)]
        [InlineData("li - help to heat group", "social housing (rsl)")]
        public void Gbis0301RuleLogic_WithValidInput_FailsValidation(string eligibilityType, string tenureType)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType
            };
            var rule = new Gbis0301RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal("GBIS0301", rule.TestNumber);
            Assert.Equal(measureModel.TenureType, observedValue);

        }

    }
}
