using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0901RuleLogicTests
    {
        [Theory]
        [MemberData(nameof(ValidEligibilityTypeAndFlexReferralRouteTestData))]
        public void Gbis0901Rule_WithValidInput_PassesValidation(string eligibilityType, string flexReferralRoute)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                FlexReferralRoute = flexReferralRoute
            };
            var rule = new Gbis0901RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            // Assert
            Assert.False(result);
        }

        public static TheoryData<string?, string?> ValidEligibilityTypeAndFlexReferralRouteTestData() => new()
        {
                { null, null },
                { "", "" },
                { EligibilityTypes.LISupplierEvidence, "notified" },
                { CommonTypesConstants.NotApplicable, CommonTypesConstants.NotApplicable },
        };

        [Theory]
        [MemberData(nameof(InvalidTestData))]
        public void Gbis0901Rule_WithInvalidInput_FailsValidation(string eligibilityType, string flexReferralRoute)
        {
            // Arrange
            var measureModel = new MeasureModel
            {
                EligibilityType = eligibilityType,
                FlexReferralRoute = flexReferralRoute
            };
            var rule = new Gbis0901RuleLogic();
            // Act
            var result = rule.FailureCondition(measureModel);
            var errorFieldValue = rule.FailureFieldValueFunction(measureModel);
            // Assert
            Assert.True(result);
            Assert.Equal(measureModel.FlexReferralRoute, errorFieldValue);
            Assert.Equal("GBIS0901", rule.TestNumber);
        }

        public static TheoryData<string, string> InvalidTestData() => new()
        {
                { EligibilityTypes.LISupplierEvidence, CommonTypesConstants.NotApplicable },
                { "li - supplier evidence", "n/a" },
        };

    }
}