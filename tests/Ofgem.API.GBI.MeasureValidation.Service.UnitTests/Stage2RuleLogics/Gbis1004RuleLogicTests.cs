using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
	public class Gbis1004RuleLogicTests
	{
		[Theory]
		[InlineData(PurposeOfNotificationConstants.AutomaticLateExtension, false)]
		[InlineData(PurposeOfNotificationConstants.EditedNotification, true)]
		[InlineData(null, false)]
		public void Gbis1004Rule_WithValidInput_PassesValidation(string? purposeOfNotification, bool isExistingMeasure)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				PurposeOfNotification = purposeOfNotification,
				IsExistingMeasure = isExistingMeasure
			};

			var rule = new Gbis1004RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData(PurposeOfNotificationConstants.EditedNotification, false)]
		public void Gbis1004Rule_WithInalidInput_FailsValidation(string? purposeOfNotification, bool isExistingMeasure)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				PurposeOfNotification = purposeOfNotification,
				IsExistingMeasure = isExistingMeasure
			};

			var rule = new Gbis1004RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);
			var errorFieldValue = rule.FailureFieldValueFunction(measureModel);

			// Assert
			Assert.True(result);
			Assert.Equal("GBIS1004", rule.TestNumber);
			Assert.Equal(measureModel.MeasureReferenceNumber, errorFieldValue);
		}
	}
}
