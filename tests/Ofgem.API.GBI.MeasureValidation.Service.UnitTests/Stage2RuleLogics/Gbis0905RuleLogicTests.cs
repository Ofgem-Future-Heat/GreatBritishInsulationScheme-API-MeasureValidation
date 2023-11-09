using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics;

public class Gbis0905RuleLogicTests
{
    [Theory]
    [InlineData("LI - LA Declaration", "11/10/2023")]
    [InlineData("LI - Supplier Evidence", "11/10/2023")]
    [InlineData("General Group", "N/A")]
    public void Gbis0905Rule_WithValidInput_PassValidation(string eligibilityType, string dateOfHouseholderEligibility)
    {
        var measureModel = new MeasureModel
        {
            MeasureReferenceNumber = "BGT1234567",
            EligibilityType = eligibilityType,
            DateOfHouseholderEligibility = dateOfHouseholderEligibility
        };
        var rule = new Gbis0905RuleLogic();
        var result = rule.FailureCondition(measureModel);

        Assert.False(result);
    }

    [Theory]
    [InlineData("LI - LA Declaration", "N/A")]
    [InlineData("LI - Supplier Evidence", "N/A")]
    public void Gbis0905Rule_WithInvalidInput_FailsValidation(string eligibilityType, string dateOfHouseholderEligibility)
    {
        var measureModel = new MeasureModel
        {
            MeasureReferenceNumber = "BGT1234567",
            EligibilityType = eligibilityType,
            DateOfHouseholderEligibility = dateOfHouseholderEligibility
        };
        var rule = new Gbis0905RuleLogic();

        var result = rule.FailureCondition(measureModel);
        var observedValue = rule.FailureFieldValueFunction(measureModel);

        Assert.True(result);
        Assert.Equal("GBIS0905", rule.TestNumber);
        Assert.Equal(measureModel.DateOfHouseholderEligibility, observedValue);
    }
}