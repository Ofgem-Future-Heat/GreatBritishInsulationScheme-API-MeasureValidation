using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0606RuleLogicTests
	{
		[Theory]
		[InlineData(EligibilityTypes.GeneralGroup, null)]
		[InlineData(EligibilityTypes.InFill, "AAA0123456789")]
		public void Gbis0606RuleLogic_WithValidEligibilityType_PassesValidation(string eligibilityType, string measureReferenceNumber)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				EligibilityType = eligibilityType,
				AssociatedInfillMeasure1 = measureReferenceNumber
			};

			var rule = new Gbis0606RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData(EligibilityTypes.InFill, null)]
		[InlineData(EligibilityTypes.InFill, CommonTypesConstants.NotApplicable)]
		public void Gbis0606RuleLogic_WithInvalidInput_FailsValidation(string eligibilityType, string measureReferenceNumber)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				EligibilityType = eligibilityType,
				AssociatedInfillMeasure1 = measureReferenceNumber
			};

			var rule = new Gbis0606RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.True(result);
		}
	}
}
