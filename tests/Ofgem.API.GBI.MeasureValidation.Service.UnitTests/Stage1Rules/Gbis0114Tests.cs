using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0114Tests
    {
        [Theory]
        [InlineData("EC1A 1BB")]
        [InlineData("W1A 0AX")]
        [InlineData("M1 1AE")]
        [InlineData("B33 8TH")]
        [InlineData("CR2 6XH")]
        [InlineData("DN55 1PT")]
        public async Task Gbis0114_WithValidInput_PassesValidation(string postCode)
        {
            var rule = new Gbis0114()
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
        [InlineData("AA AAN")]
        [InlineData("NNN NNNN")]
        [InlineData("AANN NNN")]
        [InlineData("ANA AAA")]
        [InlineData("AANA NNN")]
        [InlineData("ANN NNN")]
        [InlineData("EC1A1BB ")]
        [InlineData(" W1A0AX")]
        [InlineData(" M11AE ")]
        [InlineData("  DN551PT   ")]
        [InlineData("  97220                                                ")]     // 50 characters
        [InlineData("EC1A1BB")]

        public async Task Gbis0114_WithInvalidInput_FailsValidation(string postCode)
        {

            Gbis0114 rule = new Gbis0114()
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
            Assert.Equal("GBIS0114", error.TestNumber);
            Assert.Equal(postCode.Trim(), error.WhatWasAddedToTheNotificationTemplate);
        }
    }
}