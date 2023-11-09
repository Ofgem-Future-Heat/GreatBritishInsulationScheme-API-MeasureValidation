using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics;

public class Tgbis0101RuleLogicTests
{
    [Theory]
    [InlineData("A1234567-1")]
    [InlineData("A123456-1")]
    [InlineData("A12345-1")]
    [InlineData("A1234-1")]
    public void Tgbis0101Rule_WithValidInput_PassesValidation(string trustmarkLodgedCertificateID)
    {
        // Arrange
        var measureModel = new MeasureModel
        {
            TrustmarkLodgedCertificateID = trustmarkLodgedCertificateID
        };
        var rule = new Tgbis0101RuleLogic();
        // Act
        var result = rule.FailureCondition(measureModel);
        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("A67-1")]
    [InlineData("167-1")]
    public void Tgbis0101Rule_WithInvalidInput_FailsValidation(string trustmarkLodgedCertificateID)
    {
        // Arrange
        var measureModel = new MeasureModel
        {
            TrustmarkLodgedCertificateID = trustmarkLodgedCertificateID
        };
        var rule = new Tgbis0101RuleLogic();
        // Act
        var result = rule.FailureCondition(measureModel);
        var errorFieldValue = rule.FailureFieldValueFunction(measureModel);
        // Assert
        Assert.True(result);
        Assert.Equal(measureModel.TrustmarkLodgedCertificateID, errorFieldValue);
        Assert.Equal("TGBIS0101", rule.TestNumber);
    }
}