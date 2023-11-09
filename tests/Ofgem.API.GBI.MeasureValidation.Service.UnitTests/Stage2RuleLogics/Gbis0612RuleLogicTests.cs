using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0612RuleLogicTests
	{
		[Theory]
		[InlineData(EligibilityTypes.GeneralGroup, CommonTypesConstants.NotApplicable, 1, 2)]
		[InlineData(EligibilityTypes.InFill, CommonTypesConstants.NotApplicable, 1, 2)]
		[InlineData(EligibilityTypes.InFill, "ABC0123456781", 1, 1)]
		[InlineData(EligibilityTypes.InFill, "ABC0123456781", null, 1)]
		public void Gbis0612Rule_WithValidInput_PassesValidation(string eligibilityType, string associatedInfillMeasure1, int? measureCategoryId, int associatedMeasureCategoryId)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				EligibilityType = eligibilityType,
				MeasureCategoryId = measureCategoryId,
				AssociatedInfillMeasure1 = associatedInfillMeasure1,
				AssociatedInfillMeasure1Details = new AssociatedMeasureModelDto
				{
					MeasureCategoryId = associatedMeasureCategoryId
				}
			};

			var rule = new Gbis0612RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData(EligibilityTypes.InFill, "ABC0123456781", 1, 2)]
		public void Gbis0612Rule_WithInvalidInput_FailsValidation(string eligibilityType, string associatedInfillMeasure1, int measureCategoryId, int associatedMeasureCategoryId)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				EligibilityType = eligibilityType,
				MeasureCategoryId = measureCategoryId,
				AssociatedInfillMeasure1 = associatedInfillMeasure1,
				AssociatedInfillMeasure1Details = new AssociatedMeasureModelDto
				{
					MeasureCategoryId = associatedMeasureCategoryId
				}
			};

			var rule = new Gbis0612RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.True(result);
		}
	}
}
