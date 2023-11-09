using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0113Tests
    {
        [Theory]
        [InlineData("EC1A 1BB")]
        public async Task Gbis0113_WithValidInput_PassesValidation(string postCode)
        {
            var rule = new Gbis0113()
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    PostCode = postCode
                }
            };

            var result = await rule.InvokeAsync();
            Assert.Null(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("N/A")]
        [InlineData("n/a")]
        [InlineData("n/a\n")]
        [InlineData("n/a\r")]

        public async Task Gbis0113_WithInvalidInput_FailsValidation(string postCode)
        {
            Gbis0113 rule = new Gbis0113()
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    PostCode = postCode
                }
            };

            var result = await rule.InvokeAsync();
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal("BGT1234567", error.MeasureReferenceNumber);
            Assert.Equal("GBIS0113", error.TestNumber);
            Assert.Equal(postCode, error.WhatWasAddedToTheNotificationTemplate);
        }
    }
}