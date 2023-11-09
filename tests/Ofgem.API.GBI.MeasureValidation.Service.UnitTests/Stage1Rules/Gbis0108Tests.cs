using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules;

public class Gbis0108Tests
{
    private static readonly string BuildingNumberLongerThanMaxLengthAllowed = new('0', 51);

    [Theory]
    [MemberData(nameof(ValidBuildingNumbers))]
    public async Task Gbis0108_Pass_ReturnsNil(string buildingNumber)
    {
        // Arrange
        var rule = new Gbis0108
        {
            Model = new()
            {
                MeasureReferenceNumber = "BGT1234567",
                BuildingNumber = buildingNumber,
            },
        };

        // Act
        var result = await rule.InvokeAsync();

        // Assert
        Assert.Null(result);
    }

    public static TheoryData<string?> ValidBuildingNumbers()
    {
        return new TheoryData<string?>
        {
                { string.Empty },
                { " " },
                { "0" },
                { "1" },
                { "2A" },
                { "3B" },
                { "10½" },
                { "11L" },
                { "Flat A" },
                { "Room A" },
                { "1-15" },
                { "Apartment 9D" },
                { new string('0', 49) },
                { new string('0', 50) },

                { new string(' ', 2) },
                { new string(' ', 49) },
                { new string(' ', 50) },
                { new string(' ', 51) },
                { null },
                { "\n" },
                { "\t" },

                { "n\\a" },
                { "N\\A" },
                { "N/A" },
                { "n/a" },
                { "n/a " },
                { " n/a     " },
                { "n/a\n" },
                { "n/a\r" },
                { "na" },
                { "nA" },
                { "Na" },
                { "NA" },
            };
    }

    [Theory]
    [MemberData(nameof(InvalidBuildingNumbers))]
    public async Task Gbis0108_Fail_ReturnsError(string buildingNumber)
    {
        // Arrange
        var rule = new Gbis0108
        {
            Model = new() { BuildingNumber = buildingNumber },
        };

        // Act
        var result = await rule.InvokeAsync();

        // Assert
        Assert.NotNull(result);
    }

    public static TheoryData<string> InvalidBuildingNumbers()
    {
        return new TheoryData<string>
            {
                { BuildingNumberLongerThanMaxLengthAllowed },
                { new string ('0', 52) },
                { new string ('0', 1000) },
            };
    }

    [Fact]
    public async Task GivenInvalidBuildingNumberValue_WhenRuleRun_ThenErrorTestReferenceNumberIsAsExpected()
    {
        // Arrange
        var rule = new Gbis0108()
        {
            Model = new() { BuildingNumber = BuildingNumberLongerThanMaxLengthAllowed },
        };

        // Act
        var result = await rule.InvokeAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<StageValidationError>>(result.Result);
        var measureCoreValidationErrors = (result.Result as IEnumerable<StageValidationError>)!;
        var stageValidationErrors = measureCoreValidationErrors.ToList();
        Assert.Single(stageValidationErrors);
        var error = stageValidationErrors.First();
        Assert.Equal("GBIS0108", error.TestNumber);
    }

    [Fact]
    public async Task GivenInvalidBuildingNumberValue_WhenRuleRun_ThenErrorMeasureReferenceNumberValueMatchesWithTheModel()
    {
        // Arrange
        var measureReferenceNumber = "BGT1234567";
        var rule = new Gbis0108()
        {
            Model = new()
            {
                BuildingNumber = BuildingNumberLongerThanMaxLengthAllowed,
                MeasureReferenceNumber = measureReferenceNumber
            },
        };

        // Act
        var result = await rule.InvokeAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<StageValidationError>>(result.Result);
        var measureCoreValidationErrors = (result.Result as IEnumerable<StageValidationError>)!;
        var stageValidationErrors = measureCoreValidationErrors.ToList();
        Assert.Single(stageValidationErrors);
        var error = stageValidationErrors.First();
        Assert.Equal(measureReferenceNumber, error.MeasureReferenceNumber);
    }

    [Fact]
    public async Task GivenInvalidBuildingNumberValue_WhenRuleRun_ThenErrorWhatWasAddedToTheNotificationTemplateValueMatchesWithTheModel()
    {
        // Arrange
        string buildingNumber = BuildingNumberLongerThanMaxLengthAllowed;
        var rule = new Gbis0108()
        {
            Model = new()
            {
                BuildingNumber = buildingNumber,
            },
        };

        // Act
        var result = await rule.InvokeAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<StageValidationError>>(result.Result);
        var measureCoreValidationErrors = (result.Result as IEnumerable<StageValidationError>)!;
        var stageValidationErrors = measureCoreValidationErrors.ToList();
        Assert.Single(stageValidationErrors);
        var error = stageValidationErrors.First();
        Assert.Equal(buildingNumber, error.WhatWasAddedToTheNotificationTemplate);
    }
}
