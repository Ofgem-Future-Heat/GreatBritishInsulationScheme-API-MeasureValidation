using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0805RuleLogicTests
	{
		private const string MeasureReferenceNumber = "ABC0123456781";

		[Theory]
		[InlineData(CommonTypesConstants.NotApplicable, "CWI_0.027")]
		[InlineData(MeasureReferenceNumber, MeasureTypes.Trv)]
		[InlineData(MeasureReferenceNumber, MeasureTypes.PAndRt)]
		public void Gbis0805Rule_WithValidInput_PassesValidation(string associatedInsulationMrnForHeatingMeasures, string measureType)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				MeasureType = measureType,
				AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingMeasures
			};

			var rule = new Gbis0805RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData(MeasureReferenceNumber, "CWI_0.027")]
		[InlineData(MeasureReferenceNumber, "EWI_cavity_0.45_0.21")]
		public void Gbis0805Rule_WithInvalidInput_FailsValidation(string associatedInsulationMrnForHeatingMeasures, string measureType)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				MeasureType = measureType,
				AssociatedInsulationMrnForHeatingMeasures = associatedInsulationMrnForHeatingMeasures
			};

			var rule = new Gbis0805RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);
			var errorFieldValue = rule.FailureFieldValueFunction(measureModel);

			// Assert
			Assert.True(result);
			Assert.Equal("GBIS0805", rule.TestNumber);
			Assert.Equal(measureModel.AssociatedInsulationMrnForHeatingMeasures, errorFieldValue);
		}
	}
}
