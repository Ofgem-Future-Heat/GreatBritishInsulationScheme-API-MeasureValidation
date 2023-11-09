using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0627RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.InFill, "ABC0123456781", CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.LISupplierEvidence, "ABC0123456781", CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, "ABC0123456781", "CBS0123456781")]
        public void Gbis0627Rule_WithValidInput_PassesValidation(string eligibilityType, string primaryMrn, string associatedInfillMeasure2)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                MeasureReferenceNumber = primaryMrn,
                AssociatedInfillMeasure2 = associatedInfillMeasure2,
            };

            var rule = new Gbis0627RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, "ABC0123456781", "ABC0123456781")]
        public void Gbis0627Rule_WithInvalidInput_FailsValidation(string eligibilityType, string primaryMrn, string associatedInfillMeasure2)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                MeasureReferenceNumber = primaryMrn,
                AssociatedInfillMeasure2 = associatedInfillMeasure2,
            };

            var rule = new Gbis0627RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.Equal("GBIS0627", rule.TestNumber);
            Assert.Equal(measureModel.AssociatedInfillMeasure2, observedValue);

            // Assert
            Assert.True(result);
        }
    }
}
