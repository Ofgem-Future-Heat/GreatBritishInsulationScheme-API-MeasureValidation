using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0300RuleLogicTests
    {

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, "Yes")]
        [InlineData(EligibilityTypes.GeneralGroup, "No")]
        public void Gbis0300RuleLogic_WithValidInput_PassesValidation(string eligibilityType, string privateDomesticPremises)
        {
            // Arrange 
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                PrivateDomesticPremises = privateDomesticPremises
            };
            var rule = new Gbis0300RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);

        }

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, "No")]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, "no")]
        [InlineData("li - help to heat group", "no")]
        [InlineData("LI - help to heat group", "NO")]
        public void Gbis0300RuleLogic_WithValidInput_FailsValidation(string eligibilityType, string privateDomesticPremises)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                PrivateDomesticPremises = privateDomesticPremises
            };
            var rule = new Gbis0300RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal("GBIS0300", rule.TestNumber);
            Assert.Equal(measureModel.PrivateDomesticPremises, observedValue);

        }

    }
}