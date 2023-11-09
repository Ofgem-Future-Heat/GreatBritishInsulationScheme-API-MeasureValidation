using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0626RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.InFill, "ABC0123456781", CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.LISupplierEvidence, "ABC0123456781", CommonTypesConstants.NotApplicable)]
        [InlineData(EligibilityTypes.InFill, "ABC0123456781", "CBS0123456781")]
        public void Gbis0626Rule_WithValidInput_PassesValidation(string eligibilityType, string primaryMrn, string associatedInfillMeasure1)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                MeasureReferenceNumber = primaryMrn,
                AssociatedInfillMeasure1 = associatedInfillMeasure1,
            };

            var rule = new Gbis0626RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.InFill, "ABC0123456781", "ABC0123456781")]
        public void Gbis0626Rule_WithInvalidInput_FailsValidation(string eligibilityType, string primaryMrn, string associatedInfillMeasure1)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                MeasureReferenceNumber = primaryMrn,
                AssociatedInfillMeasure1 = associatedInfillMeasure1,
            };

            var rule = new Gbis0626RuleLogic();

            // Act
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.Equal("GBIS0626", rule.TestNumber);
            Assert.Equal(measureModel.AssociatedInfillMeasure1, observedValue);

            // Assert
            Assert.True(result);
        }
    }
}
