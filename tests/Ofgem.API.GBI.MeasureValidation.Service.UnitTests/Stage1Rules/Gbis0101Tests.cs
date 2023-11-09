using DotNetRuleEngine.Interface;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0101Tests
    {
        [Theory]
        [InlineData("01/01/2021")]
        [InlineData("31/12/2026")]
        [InlineData("29/02/2024")]
        public async Task Gbis0101_Pass(string dateOfCompletedInstallation)
        {
            IRuleResult result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            Assert.Null(result);
        }

        public static TheoryData<string> FailureCaseDates =>
            new()
            {
                { " " },
                { string.Empty },
                { "02/29/2024" },
                { "31/12/20" },
                { "30/02/2024" },
                { "1/2/2026" },
                { "2nd February 2026" },
            };


        [Theory]
        [MemberData(nameof(FailureCaseDates))]
        public async Task Gbis0101_Fail_MeasureReferenceNumber(string dateOfCompletedInstallation)
        {
            IRuleResult result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal("BGT1234567", error.MeasureReferenceNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseDates))]
        public async Task Gbis0101_Fail_TestReferenceNumber(string dateOfCompletedInstallation)
        {
            IRuleResult result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal("GBIS0101", error.TestNumber);
        }

        [Theory]
        [MemberData(nameof(FailureCaseDates))]
        public async Task Gbis0101_Fail_WhatWasAddedToTheNotificationTemplate(string dateOfCompletedInstallation)
        {
            IRuleResult result = await CreateAndInvokeRule(dateOfCompletedInstallation);
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.NotNull(error);
            Assert.Equal(dateOfCompletedInstallation, error.WhatWasAddedToTheNotificationTemplate);
        }

        private static async Task<IRuleResult> CreateAndInvokeRule(string dateOfCompletedInstallation)
        {
            var rule = new Gbis0101
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    DateOfCompletedInstallation = dateOfCompletedInstallation
                }
            };

            var result = await rule.InvokeAsync();
            return result;
        }
    }
}
