using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules;

public class Gbis0111Tests
{
    [Theory]
    [InlineData("Cambridge")]
    [InlineData("London")]
    [InlineData("a")]
    [InlineData("A")]
    public async Task Gbis0111_Pass_ReturnsNilResult(string inputValueTown)
    {
        // given
        Gbis0111 rule = new()
        {
            Model = new MeasureModel()
            {
                MeasureReferenceNumber = "BGT1234567",
                Town = inputValueTown
            }
        };
        // when
        var result = await rule.InvokeAsync();
        // then
        Assert.Null(result);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("     ")]
    [InlineData("                                                   ")]    // 51 characters
    [InlineData(null)]
    [InlineData("N/A")]
    [InlineData("n/a")]
    [InlineData("N/a ")]
    [InlineData(" n/a     ")]
    public async Task Gbis0111_Fail_ReturnsError(string inputValueTown)
    {
        // given
        Gbis0111 rule = new()
        {
            Model = new MeasureModel()
            {
                MeasureReferenceNumber = "BGT1234567",
                Town = inputValueTown
            }
        };
        // when
        var result = await rule.InvokeAsync();
        var error = ((IEnumerable<StageValidationError>)result.Result).First();
        // then
        Assert.NotNull(error);
        Assert.Equal("BGT1234567", error.MeasureReferenceNumber);
        Assert.Equal("GBIS0111", error.TestNumber);
        Assert.Equal(inputValueTown, error.WhatWasAddedToTheNotificationTemplate);
    }
}