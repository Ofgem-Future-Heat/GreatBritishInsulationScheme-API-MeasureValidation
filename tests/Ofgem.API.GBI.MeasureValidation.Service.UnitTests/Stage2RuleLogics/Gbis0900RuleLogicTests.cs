using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
	public class Gbis0900RuleLogicTests
	{
		private readonly string[] _flexReferralRoutes = new string[]
		{
			"Route 2 Area Validation",
			"Route 2 Council Tax Reduction",
			"Route 2 NICE Guidance",
			"Route 2 Free School Meals"
		};

		[Theory]
		[InlineData(CommonTypesConstants.NotApplicable)]
		[InlineData("Route 2 Area Validation")]
		[InlineData("Route 2 Council Tax Reduction")]
		[InlineData("Route 2 NICE Guidance")]
		[InlineData("Route 2 Free School Meals")]
		public void Gbis0900Rule_WithValidInput_PassesValidation(string flexReferralRoute)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				FlexReferralRoute = flexReferralRoute,
				ReferenceDataDetails = new()
				{
					FlexReferralRouteList = _flexReferralRoutes
				}
			};

			var rule = new Gbis0900RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);

			// Assert
			Assert.False(result);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("Invalid Flex Referral Route")]
		public void Gbis0900Rule_WithInvalidInput_FailsValidation(string flexReferralRoute)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				FlexReferralRoute = flexReferralRoute,
				ReferenceDataDetails = new()
				{
					FlexReferralRouteList = _flexReferralRoutes
				}
			};

			var rule = new Gbis0900RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);
			var errorFieldValue = rule.FailureFieldValueFunction(measureModel);

			// Assert
			Assert.True(result);
			Assert.Equal("GBIS0900", rule.TestNumber);
			Assert.Equal(measureModel.FlexReferralRoute, errorFieldValue);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("Invalid Flex Referral Route")]
		public void Gbis0900Rule_WithNullFlexReferralRouteList_FailsValidation(string flexReferralRoute)
		{
			// Arrange
			var measureModel = new MeasureModel
			{
				FlexReferralRoute = flexReferralRoute,
				ReferenceDataDetails = new()
			};

			var rule = new Gbis0900RuleLogic();

			// Act
			var result = rule.FailureCondition(measureModel);
			var errorFieldValue = rule.FailureFieldValueFunction(measureModel);

			// Assert
			Assert.True(result);
			Assert.Equal("GBIS0900", rule.TestNumber);
			Assert.Equal(measureModel.FlexReferralRoute, errorFieldValue);
		}
	}
}
