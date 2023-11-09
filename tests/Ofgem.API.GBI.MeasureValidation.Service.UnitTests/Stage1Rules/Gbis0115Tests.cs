using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0115Tests
    {
        [Theory]
        [InlineData("1", "")]
        [InlineData("", "A")]
        [InlineData("1", "A")]
        [InlineData("1", "Kensington Palace")]
        [InlineData("", "Kensington Palace")]
        [InlineData("00", "")]
        [InlineData("1", "n/a")]
        [InlineData("n/a", "10")]
        public async Task Gbis0115_Pass_ReturnsNil(string buildingNumber, string buildingName)
        {
            // given
            Gbis0115 rule = new()
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    BuildingNumber = buildingName,
                    BuildingName = buildingNumber
                }
            };
            // when
            var result = await rule.InvokeAsync();
            // then
            Assert.Null(result);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData("", "\n")]
        [InlineData("\n", "\t")]
        [InlineData("    ", "")]
        [InlineData("", "    ")]
        [InlineData("n/a", "n/a")]
        [InlineData("N/A", null)]
        [InlineData("N/A", "")]
        [InlineData("N/A", "N/A")]
        [InlineData(" ", "n/a")]
        public async Task Gbis0115_Fail_ReturnsError(string buildingNumber, string buildingName)
        {
            // given
            Gbis0115 rule = new()
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    BuildingNumber = buildingNumber,
                    BuildingName = buildingName
                }
            };
            // when
            var result = await rule.InvokeAsync();
            var error = ((IEnumerable<StageValidationError>)result.Result).First();
            // then
            Assert.NotNull(error);
            Assert.Equal("BGT1234567", error.MeasureReferenceNumber);
            Assert.Equal("GBIS0115", error.TestNumber);
            Assert.Equal(buildingNumber + ", " + buildingName, error.WhatWasAddedToTheNotificationTemplate);
        }
    }
}


