using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0806RuleLogicTests
	{
		[Theory]
		[InlineData(CommonTypesConstants.NotApplicable)]
		[InlineData("ABC0123456781")]
		public void Gbis0806Rule_WithValidInput_PassesValidation(string associatedInsulationMrnForHeatingMeasures)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingMeasures,
				AssociatedInsulationMeasureForHeatingMeasureDetails = new()
				{
					MeasureReferenceNumber = associatedInsulationMrnForHeatingMeasures
				}
			};

			var rule = new Gbis0806RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData("ABC0123456781")]
		public void Gbis0806Rule_WithInvalidInput_FailsValidation(string associatedInsulationMrnForHeatingMeasures)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingMeasures
			};

			var rule = new Gbis0806RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);
			var errorFieldValue = rule.FailureFieldValueFunction(measureModel);

			// Assert
			Assert.True(result);
			Assert.Equal("GBIS0806", rule.TestNumber);
			Assert.Equal(measureModel.AssociatedInsulationMrnForHeatingMeasures, errorFieldValue);
		}
	}
}
