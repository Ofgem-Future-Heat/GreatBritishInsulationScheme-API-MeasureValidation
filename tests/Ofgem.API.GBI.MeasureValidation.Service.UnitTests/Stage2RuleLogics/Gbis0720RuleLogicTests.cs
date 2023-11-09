using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0720RuleLogicTests
	{
		[Theory]
		[InlineData(CommonTypesConstants.NotApplicable)]
		[InlineData("123")]
		public void Gbis0720_WithValidInput_PassesValidation(string innovationMeasureNumber)
		{
			// Arrange
			string measureTypeName = "EWI_cavity_0.45_0.21";

			var measureModel = new MeasureModel
			{
				InnovationMeasureNumber = innovationMeasureNumber,
				MeasureType = measureTypeName,
				ReferenceDataDetails = new()
				{
					InnovationMeasureList = new List<InnovationMeasureDto>
					{
						new()
						{
							InnovationMeasureNumber = innovationMeasureNumber,
							MeasureTypeName = measureTypeName
						}
					}
				}
			};

			var rule = new Gbis0720RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void Gbis0720_WithInvalidInnovationMeasureNumber_FailsValidation()
		{
			// Arrange
			string measureTypeName = "EWI_cavity_0.45_0.21";

			var measureModel = new MeasureModel
			{
				InnovationMeasureNumber = "123",
				MeasureType = measureTypeName,
				ReferenceDataDetails = new()
				{
					InnovationMeasureList = new List<InnovationMeasureDto>
					{
						new()
						{
							InnovationMeasureNumber = "234",
							MeasureTypeName = measureTypeName
						}
					}
				}
			};

			var rule = new Gbis0720RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);
			var errorFieldValue = rule.FailureFieldValueFunction(measureModel);

			// Assert
			Assert.True(result);
			Assert.Equal("GBIS0720", rule.TestNumber);
			Assert.Equal(measureModel.InnovationMeasureNumber, errorFieldValue);
		}

		[Fact]
		public void Gbis0720_WithInvalidMeasureType_FailsValidation()
		{
			// Arrange
			string innovationMeasureNumber = "123";

			var measureModel = new MeasureModel
			{
				InnovationMeasureNumber = innovationMeasureNumber,
				MeasureType = "EWI_cavity_0.45_0.21",
				ReferenceDataDetails = new()
				{
					InnovationMeasureList = new List<InnovationMeasureDto>
					{
						new()
						{
							InnovationMeasureNumber = innovationMeasureNumber,
							MeasureTypeName = "EWI_cavity_0.6_0.24"
						}
					}
				}
			};

			var rule = new Gbis0720RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.True(result);
		}
	}
}
