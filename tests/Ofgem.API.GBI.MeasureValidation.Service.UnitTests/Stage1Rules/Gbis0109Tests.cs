using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules;

public class Gbis0109Tests
{
    [Theory]
    [InlineData("Westminster Abbey")]
    [InlineData("Tower of London")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("     ")]
    [InlineData("                                                  ")]     // 50 characters
    [InlineData("                                                   ")]    // 51 characters
    [InlineData(null)]
    [InlineData("N/A")]
    [InlineData("n/a")]
    [InlineData("n/a ")]
    [InlineData(" n/a     ")]
    [InlineData("n/A")]
    [InlineData("Na")]
    [InlineData("NA")]
    [InlineData("Bababadalgharaghtakamminarronnkonnbronntonnerronntuonnthunntrovarrhounawnskawntoohoohoordenenthurnu")]    // length 99
    [InlineData("Bababadalgharaghtakamminarronnkonnbronntonnerronntuonnthunntrovarrhounawnskawntoohoohoordenenthurnuk")]    // length 100
    public async Task Gbis0109_Pass_ReturnsNilResult(string buildingName)
    {
        // given
        Gbis0109 rule = new()
        {
            Model = new MeasureModel()
            {
                MeasureReferenceNumber = "BGT1234567",
                BuildingName = buildingName
            }
        };
        // when
        var result = await rule.InvokeAsync();
        // then
        Assert.Null(result);
    }

    [Theory]
    [InlineData("BababadalgharaghtakamminarronnkonnbronntonnerronntuonnthunntrovarrhounawnskawntoohoohoordenenthurnukA")]    // length 101
    [InlineData("BababadalgharaghtakamminarronnkonnbronntonnerronntuonnthunntrovarrhounawnskawntoohoohoordenenthurnukAB")]    // length 102
    [InlineData("Lorem Ipsum\r\n\"Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit...\"\r\n\"There is no one who loves pain itself")]
    public async Task Gbis0109_Fail_ReturnsError(string buildingName)
    {
        // given
        Gbis0109 rule = new()
        {
            Model = new MeasureModel()
            {
                MeasureReferenceNumber = "BGT1234567",
                BuildingName = buildingName
            }
        };
        // when
        var result = await rule.InvokeAsync();
        var error = ((IEnumerable<StageValidationError>)result.Result).First();
        // then
        Assert.NotNull(error);
        Assert.Equal("BGT1234567", error.MeasureReferenceNumber);
        Assert.Equal("GBIS0109", error.TestNumber);
        Assert.Equal(buildingName, error.WhatWasAddedToTheNotificationTemplate);
    }
}


