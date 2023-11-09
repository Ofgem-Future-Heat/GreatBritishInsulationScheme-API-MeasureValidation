using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
	public class Gbis0813RuleLogicTests
	{
		[Theory]
		[InlineData("ABC0123456782", CommonTypesConstants.NotApplicable)]
		[InlineData("ABC0123456782", "ABC0123456781")]
		public void Gbis0813Rule_WithValidInput_PassesValidation(string measureReferenceNumber, string associatedInsulationMrnForHeatingMeasures)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				MeasureReferenceNumber = measureReferenceNumber,
				AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingMeasures
			};

			var rule = new Gbis0813RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData("ABC0123456782", "ABC0123456782")]
		public void Gbis0813Rule_WithInvalidInput_FailsValidation(string measureReferenceNumber, string associatedInsulationMrnForHeatingMeasures)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				MeasureReferenceNumber = measureReferenceNumber,
				AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingMeasures
			};

			var rule = new Gbis0813RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);
			var errorFieldValue = rule.FailureFieldValueFunction(measureModel);

			// Assert
			Assert.True(result);
			Assert.Equal("GBIS0813", rule.TestNumber);
			Assert.Equal(measureModel.AssociatedInsulationMrnForHeatingMeasures, errorFieldValue);
		}
	}
}
