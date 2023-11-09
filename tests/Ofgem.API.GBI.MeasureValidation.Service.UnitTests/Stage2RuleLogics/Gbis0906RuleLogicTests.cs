using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics;

public class Gbis0906RuleLogicTests
{
    [Theory]
    [InlineData("LI - LA Declaration", "Route 2 Area Validation")]
    [InlineData("LI - Supplier Evidence", "Route 2 NICE Guidance")]
    [InlineData("General Group", "N/A")]
    public void Gbis0906Rule_WithValidInput_PassValidation(string eligibilityType, string flexReferralRoute)
    {
        var measureModel = new MeasureModel
        {
            MeasureReferenceNumber = "BGT1234567",
            EligibilityType = eligibilityType,
            FlexReferralRoute = flexReferralRoute
        };
        var rule = new Gbis0906RuleLogic();
        var result = rule.FailureCondition(measureModel);

        Assert.False(result);
    }

    [Theory]
    [InlineData("LI - Supplier Evidence", "N/A")]
    public void Gbis0906Rule_WithInvalidInput_FailsValidation(string eligibilityType, string flexReferralRoute)
    {
        var measureModel = new MeasureModel
        {
            MeasureReferenceNumber = "BGT1234567",
            EligibilityType = eligibilityType,
            FlexReferralRoute = flexReferralRoute
        };
        var rule = new Gbis0906RuleLogic();

        var result = rule.FailureCondition(measureModel);
        var observedValue = rule.FailureFieldValueFunction(measureModel);

        Assert.True(result);
        Assert.Equal("GBIS0906", rule.TestNumber);
        Assert.Equal(measureModel.FlexReferralRoute, observedValue);
    }
}