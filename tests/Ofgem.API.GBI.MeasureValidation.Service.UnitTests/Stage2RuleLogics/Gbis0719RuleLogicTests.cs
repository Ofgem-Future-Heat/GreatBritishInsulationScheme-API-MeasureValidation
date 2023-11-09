using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0719RuleLogicTests
	{
		[Theory]
		[InlineData(CommonTypesConstants.NotApplicable)]
		[InlineData("123")]
		public void Gbis0719_WithValidInput_PassesValidation(string innovationMeasureNumber)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				InnovationMeasureNumber = innovationMeasureNumber
			};

			var rule = new Gbis0719RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("12")]
		[InlineData("1234")]
		[InlineData("ABC")]
		public void Gbis0719_WithIvnalidInput_FailsValidation(string innovationMeasureNumber)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				InnovationMeasureNumber = innovationMeasureNumber
			};

			var rule = new Gbis0719RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);
			var actual = rule.FailureFieldValueFunction(measureModel);

			// Assert
			Assert.True(result);
			Assert.Equal("GBIS0719", rule.TestNumber);
			Assert.Equal(measureModel.InnovationMeasureNumber, actual);
		}
	}
}
