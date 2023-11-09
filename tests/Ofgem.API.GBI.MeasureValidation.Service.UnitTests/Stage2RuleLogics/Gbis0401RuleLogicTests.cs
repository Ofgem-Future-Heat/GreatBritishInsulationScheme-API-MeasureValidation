using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0401RuleLogicTests
    {

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, "1.2")]
        [InlineData(EligibilityTypes.LISupplierEvidence, "68.3")]
        public void Gbis0401RuleLogic_WithValidInput_PassesValidation(string eligibilityType, string startingSapRating)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                StartingSAPRating = startingSapRating
            };

            var rule = new Gbis0401RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);

        }

        [Theory]
        [InlineData(EligibilityTypes.LIHelpToHeatGroup, "-0.1")]
        [InlineData(EligibilityTypes.LISupplierEvidence, "70")]
        public void Gbis0401RuleLogic_WithValidInput_FailsValidation(string eligibilityType, string startingSapRating)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "FOX0123456789",
                EligibilityType = eligibilityType,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0401RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal("GBIS0401", rule.TestNumber);
            Assert.Equal(measureModel.StartingSAPRating, observedValue);

        }


    }
}
