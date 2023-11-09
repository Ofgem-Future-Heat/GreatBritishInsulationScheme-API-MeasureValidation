using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules;

public class Gbis0112Tests
{
    [Theory]
    [InlineData("Cambridge")]
    [InlineData("London")]
    [InlineData(null)]
    [InlineData("a")]
    [InlineData("A")]
    public async Task Gbis0112_Pass_ReturnsNilResult(string inputValueTown)
    {
        // given
        Gbis0112 rule = new()
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
    [InlineData("Llanfairpwllgwyngyllgogerychwyrndrobwllllantysiliogogogoch")]
    [InlineData("                                                   ")]    // 51 characters
    [InlineData("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n")]
    public async Task Gbis0112_Fail_ReturnsError(string inputValueTown)
    {
        // given
        Gbis0112 rule = new()
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
        Assert.Equal("GBIS0112", error.TestNumber);
        Assert.Equal(inputValueTown, error.WhatWasAddedToTheNotificationTemplate);
    }
}