using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0607RuleLogicTests
	{
		[Theory]
		[InlineData(EligibilityTypes.GeneralGroup, PropertyTypes.Maisonette, "AAA0123456789")]
		[InlineData(EligibilityTypes.InFill, PropertyTypes.House, null)]
		[InlineData(EligibilityTypes.InFill, PropertyTypes.Flat, CommonTypesConstants.NotApplicable)]
		public void Gbis0607RuleLogic_WithValidPropertyType_PassesValidation(string eligibilityType, string propertyType, string measureReferenceNumber)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				EligibilityType = eligibilityType,
				Property = propertyType,
				AssociatedInfillMeasure2 = measureReferenceNumber,
				AssociatedInfillMeasure3 = CommonTypesConstants.NotApplicable,
			};

			var rule = new Gbis0607RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData(EligibilityTypes.InFill, PropertyTypes.Maisonette, null)]
		[InlineData(EligibilityTypes.InFill, PropertyTypes.Maisonette, "AAA0123456789")]
		[InlineData(EligibilityTypes.InFill, PropertyTypes.Flat, null)]
		[InlineData(EligibilityTypes.InFill, PropertyTypes.Flat, "AAA0123456789")]
		public void Gbis0607RuleLogic_WithInvalidPropertyType_FailsValidation(string eligibilityType, string propertyType, string measureReferenceNumber)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				EligibilityType = eligibilityType,
				Property = propertyType,
				AssociatedInfillMeasure2 = CommonTypesConstants.NotApplicable,
				AssociatedInfillMeasure3 = measureReferenceNumber
			};

			var rule = new Gbis0607RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.True(result);
		}
	}
}
