using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
	public class Gbis0814RuleLogicTests
	{
		private const string MeasureReferenceNumber = "ABC0123456781";

		[Theory]
		[InlineData(CommonTypesConstants.NotApplicable, MeasureTypes.Trv)]
		[InlineData(MeasureReferenceNumber, null)]
		[InlineData(MeasureReferenceNumber, "CWI_0.027")]
		public void Gbis0814Rule_WithValidInput_PassesValidation(string associatedInsulationMrnForHeatingMeasures, string measureType)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingMeasures,
				AssociatedInsulationMeasureForHeatingMeasureDetails = new AssociatedMeasureModelDto
				{
					MeasureType = measureType
				}
			};

			var rule = new Gbis0814RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData(MeasureReferenceNumber, MeasureTypes.Trv)]
		[InlineData(MeasureReferenceNumber, MeasureTypes.PAndRt)]
		public void Gbis0814Rule_WithInvalidInput_FailsValidation(string associatedInsulationMrnForHeatingMeasures, string measureType)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingMeasures,
				AssociatedInsulationMeasureForHeatingMeasureDetails = new AssociatedMeasureModelDto
				{
					MeasureType = measureType
				}
			};

			var rule = new Gbis0814RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);
			var errorFieldValue = rule.FailureFieldValueFunction(measureModel);

			// Assert
			Assert.True(result);
			Assert.Equal("GBIS0814", rule.TestNumber);
			Assert.Equal(measureModel.AssociatedInsulationMrnForHeatingMeasures, errorFieldValue);
		}
	}
}
